using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Python.Runtime;
using System;
using System.Threading;

public class PredictionReceiver
{
    [SerializeField] string pythonDLLPath = "/StreamingAssets/embedded-python/python310.dll";

    [SerializeField] string modelPath = "/StreamingAssets/model";

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

    bool _threadRunning;
    Thread _thread;

    public PredictionReceiver()
    {
        Runtime.PythonDLL = Application.dataPath + pythonDLLPath;
        PythonEngine.Initialize();
        try
        {
            // initialize
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

            // run through loop

        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log(e.StackTrace);
        }
        Debug.Log("Start server");
        _thread = new Thread(() => server());
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
    public dynamic predict(dynamic list_labels, dynamic model, dynamic bytes_wav, dynamic file_name)
    {
        dynamic path = Application.dataPath + modelPath + "/hi.mp3";

        if (os.path.exists(file_name))
        {
            os.remove(file_name);
        }

        dynamic f = os.open(file_name, mode: "bx");
        f.write(bytes_wav);

        // obtain the data
        dynamic data = librosa.load(path);
        // data[0] = data, data[1] = sample_rate
        dynamic X = extract_features(data[0], data[1]);

        // predict
        dynamic X_ = tf.expand_dims(X, axis: 0);
        dynamic pred = model.predict(X_);
        dynamic pred_class = list_labels[tf.argmax(pred[0])];

        return pred_class;
    }
    public void server()
    {
        _threadRunning = true;

        dynamic list_labels = np.array(new List<string> { "angry", "fear", "happy", "neutral", "sad" });

        dynamic model = tf.keras.models.load_model(Application.dataPath + modelPath + "/results.h5");

        dynamic context = zmq.Context();
        dynamic socket = context.socket(zmq.REP);
        socket.bind("tcp://*:5555");

        Debug.Log("binded to tcp://*:5555");

        dynamic message;
        dynamic pred;

        dynamic path = Application.dataPath + modelPath + "/output.wav";

        while (_threadRunning)
        {
            message = socket.recv();

            pred = predict(list_labels, model, message, path);

            socket.send_string(pred);
        }
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

    private void OnDisable()
    {
        if (_threadRunning)
        {
            _threadRunning = false;
            _thread.Join();
        }
    }

    public void OnApplicationQuit()
    {
        if (PythonEngine.IsInitialized)
        {
            Debug.Log("End Python");
            PythonEngine.Shutdown();
        }
    }

}
