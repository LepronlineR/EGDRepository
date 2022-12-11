using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Python.Runtime;

public class Predictor : MonoBehaviour
{
    #region Predictor (Python)
    string pythonDLLPath = "/python/python310.dll";
    string modelPath = "/model";

    // libraries
    dynamic np;
    dynamic os;
    dynamic tf;
    dynamic librosa;
    dynamic io;
    // dynamic scipy;

    // private vars
    dynamic list_labels;
    dynamic model;

    // Start is called before the first frame update
    // I was able to import the modules by adding Lib/site-packages to the PYTHONPATH variable (rather than the PATH)
    void Start()
    {
        Runtime.PythonDLL = Application.streamingAssetsPath + pythonDLLPath;

        Debug.Log(Application.streamingAssetsPath + pythonDLLPath);

        PythonEngine.Initialize();
        try
        {
            // initialize libraries
            np = PyModule.Import("numpy");
            tf = PyModule.Import("tensorflow");
            os = PyModule.Import("os");
            librosa = PyModule.Import("librosa");
            io = PyModule.Import("io");
            // scipy = PyModule.Import("scipy");

            // initialize private vars
            list_labels = np.array(new List<string> { "angry", "fear", "happy", "neutral", "sad" });
            model = tf.keras.models.load_model(Application.streamingAssetsPath + modelPath + "/results.h5");
        } catch(Exception E)
        {
            print(E);
            print(E.StackTrace);
        }
    }
    
    public void predict(string path)
    {
        string result = predict_py(path);
        MainSystem.Instance.SetPlayerEmotion(result);
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
        try
        {
            // obtain the data
            dynamic data = librosa.load(path);
            // data[0] = data, data[1] = sample_rate
            dynamic X = extract_features(data[0], data[1]);
            // predict
            dynamic X_ = tf.expand_dims(X, axis: 0);
            print(X_.shape);

            dynamic pred = model.predict(X_);
            print(pred);
            dynamic pred_class = list_labels[tf.argmax(pred[0])];
            return pred_class.ToString();
        }
        catch (Exception e)
        {
            print(e);
            print(e.StackTrace);
        }

        return "";
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
