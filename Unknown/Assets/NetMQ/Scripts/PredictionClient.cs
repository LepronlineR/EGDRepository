using System;
using UnityEngine;

public class PredictionClient : MonoBehaviour
{
    private PredictionRequester predictionRequester;

    public static PredictionClient Instance { get; private set; }
    
    private void Awake() { 
        // If there is an instance, and it's not me, delete myself.
        
        if (Instance != null && Instance != this) { 
            Destroy(this); 
        } else { 
            Instance = this; 
        } 
    }

    private void Start() => InitializeServer();

    public void InitializeServer()
    {
        predictionRequester = new PredictionRequester();
        predictionRequester.Start();
    }

    public void Predict(float[] input, Action<float[]> onOutputReceived, Action<Exception> fallback)
    {
        predictionRequester.SetOnTextReceivedListener(onOutputReceived, fallback);
        predictionRequester.SendInput(input);
    }

    private void OnDestroy()
    {
        predictionRequester.Stop();
    }
}