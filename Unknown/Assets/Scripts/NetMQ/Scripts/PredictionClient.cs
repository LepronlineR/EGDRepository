using System;
using UnityEngine;
using TMPro;

public class PredictionClient : MonoBehaviour
{
    private PredictionRequester predictionRequester;

    [SerializeField] TMP_Text emotion_text;

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
        predictionRequester = new PredictionRequester(HandleMessage);
        // predictionRequester.Start();
        // NetEventManager.Instance.onSendRequest.AddListener(OnClientRequest);
    }

    public void Predict(byte[] bytes)
    {
        // send message

        // request messa
        // NetEventManager.Instance.onClientBusy.Invoke();
        predictionRequester.RequestMessage(bytes);
        // NetEventManager.Instance.onClientFree.Invoke();
    }

    private string setMessage = "";
    bool setOnce = false;

    private void HandleMessage(string message){
        //Debug.Log(message);
        //emotion_text.text = message;
        setMessage = message;
        setOnce = true;
    }

    // Update is called once per frame
    void Update () {
        if(setOnce){
            emotion_text.text = setMessage;
            setOnce = false;
        }
    }

    public bool SetEmotion(){
        return setOnce;
    }

    public string GetEmotion(){
        return setMessage;
    }

    private void OnDestroy()
    {
        // predictionRequester.Stop();
    }
}