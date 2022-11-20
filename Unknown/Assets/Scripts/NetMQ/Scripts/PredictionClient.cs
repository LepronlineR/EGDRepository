using System;
using UnityEngine;
using TMPro;

public class PredictionClient : MonoBehaviour
{

    private PredictionRequester predictionRequester;
    public static PredictionClient Instance { get; private set; }

    [SerializeField] TMP_Text emotion_text;
    
    private void Awake() { 
        if (Instance != null && Instance != this) { 
            Destroy(this); 
        } else { 
            Instance = this; 
        } 
    }

    private void Start() => InitializeServer();

    public void InitializeServer()
    {
        predictionRequester = new PredictionRequester(HandleMessage);
        // predictionRequester.Start();
        // NetEventManager.Instance.onSendRequest.AddListener(OnClientRequest);
    }

    public void Predict(byte[] bytes)
    {
        // request message
        predictionRequester.RequestMessage(bytes);
    }

    private string setMessage = "";

    private void HandleMessage(string message){
        setMessage = message;
        emotion_text.text = message;
        // set this in the main system
        MainSystem.Instance.PlayerEmotion = message;
    }

    public string GetEmotion(){
        return setMessage;
    }

    private void OnDestroy()
    {
        // predictionRequester.Stop();
    }
}