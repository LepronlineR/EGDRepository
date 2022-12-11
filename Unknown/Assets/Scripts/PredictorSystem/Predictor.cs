using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Python.Runtime;
using System.Text;
using Unity.Collections;
using System.Threading;
using System.Threading.Tasks;

public struct PredictorJob : IJobParallelFor
{
    public NativeArray<byte> result;
    public void Execute(int index)
    {
        Debug.Log("begin executing");
        byte[] resArr = result.ToArray();
        var path = Encoding.ASCII.GetString(resArr);
        var res = Predictor.Instance.predict_py(path);
        MainSystem.Instance.SetPlayerEmotion(res);
        Debug.Log("done executing");
    }

}

public class Predictor : MonoBehaviour
{
    public static Predictor Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    #region Jobs (Multithreading)

    //PredictorJob job;
    //JobHandle jobHandle;
    Task predictTask;

    public void predict(string path)
    {
        /*
        byte[] bytes = Encoding.ASCII.GetBytes(path);
        NativeArray<byte> result_ = new NativeArray<byte>(bytes, Allocator.Persistent);
        job = new PredictorJob
        {
            result = result_,
        };
        job.Schedule(1, 1, jobHandle);
        jobHandle.Complete();
        result_.Dispose();
        */
        string result = predict_py(path);
        MainSystem.Instance.SetPlayerEmotion(result);
    }

    #endregion

    #region Predictor (Python)
    [SerializeField] string pythonDLLPath = "/StreamingAssets/embedded-python/python310.dll";

    [SerializeField] string modelPath = "/StreamingAssets/model";

    // libraries
    dynamic np;
    dynamic os;
    dynamic tf;
    dynamic librosa;
    dynamic display;
    dynamic zmq;
    dynamic io;
    dynamic wave;
    dynamic sf;
    dynamic scipy;

    // private vars
    dynamic list_labels;
    dynamic model;

    // Start is called before the first frame update
    void Start()
    {
        Runtime.PythonDLL = Application.dataPath + pythonDLLPath;
        PythonEngine.Initialize();

        // initialize libraries
        np = PyModule.Import("numpy");
        tf = PyModule.Import("tensorflow");
        os = PyModule.Import("os");
        librosa = PyModule.Import("librosa");
        display = PyModule.Import("librosa.display");
        zmq = PyModule.Import("zmq");
        io = PyModule.Import("io");
        wave = PyModule.Import("scipy.io.wavfile");
        sf = PyModule.Import("soundfile");
        scipy = PyModule.Import("scipy");

        // initialize private vars
        list_labels = np.array(new List<string> { "angry", "fear", "happy", "neutral", "sad" });
        model = tf.keras.models.load_model(Application.dataPath + modelPath + "/results.h5");
    }
    public dynamic extract_features(dynamic data, dynamic sample_rate)
    {
        // ZCR
        dynamic result = np.array(new List<float>());
        dynamic zcr = np.mean(np.transpose(librosa.feature.zero_crossing_rate(data)), axis: 0);
        dynamic result1 = np.hstack(np.array(new List<dynamic> { result, zcr }));

        // STFT
        dynamic stft = np.abs(librosa.stft(data));
        dynamic chroma_stft = np.mean(np.transpose(librosa.feature.chroma_stft(stft, sample_rate)[0]), axis: 0);
        dynamic result2 = np.hstack(np.array(new List<dynamic> { result1, chroma_stft }));

        // MFCC
        dynamic mfcc = np.mean(np.transpose(librosa.feature.mfcc(data, sample_rate)), axis: 0);
        dynamic result3 = np.hstack(np.array(new List<dynamic> { result2, mfcc }));

        // Root Mean Square Value
        dynamic rmsv = np.mean(np.transpose(librosa.feature.rms(data)), axis: 0);
        dynamic result4 = np.hstack(np.array(new List<dynamic> { result3, rmsv }));

        // MelSpectogram
        dynamic mel = np.mean(np.transpose(librosa.feature.melspectrogram(data, sample_rate)), axis: 0);
        dynamic result5 = np.hstack(np.array(new List<dynamic> { result4, mel }));

        return result5;
    }
    public string predict_py(dynamic path)
    {
        // obtain the data
        dynamic data = librosa.load(path);
        // data[0] = data, data[1] = sample_rate
        dynamic X = extract_features(data[0], data[1]);
        // predict
        dynamic X_ = tf.expand_dims(X, axis: 0);
        dynamic pred = model.predict(X_);
        dynamic pred_class = list_labels[tf.argmax(pred[0])];

        return pred_class.ToString();
    }
    public void test()
    {
        dynamic list_labels = np.array(new List<string> { "angry", "fear", "happy", "neutral", "sad" });

        dynamic model = tf.keras.models.load_model(Application.dataPath + modelPath + "/results.h5");

        dynamic path = Application.dataPath + modelPath + "/hi.mp3";
        dynamic data = librosa.load(path);
        // data[0] = data, data[1] = sample_rate

        dynamic X = extract_features(data[0], data[1]);

        dynamic X_ = tf.expand_dims(X, axis: 0);
        dynamic pred = model.predict(X_);
        dynamic pred_class = list_labels[tf.argmax(pred[0])];
        Debug.Log(pred_class);
    }
    public void OnApplicationQuit()
    {
        if (PythonEngine.IsInitialized)
        {
            Debug.Log("End Python");
            PythonEngine.Shutdown();
        }
    }

    #endregion

}
