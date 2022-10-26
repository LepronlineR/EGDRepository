using System;
using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;

public class PredictionRequester {

    private RequestSocket client;

    //private Action<string> onOutputReceived;
    private Action<Exception> onFail;
    string resultOutput = "";

    private readonly Action<string> _messageCallback;

    public PredictionRequester(Action<string> messageCallback){
         _messageCallback = messageCallback;
    }

    /*
    protected override void Run() {
        ForceDotNet.Force(); // this line is needed to prevent unity freeze after one use, not sure why yet
        using (RequestSocket client = new RequestSocket()) {
            this.client = client;
            client.Connect("tcp://localhost:5555");

            var output = "";
            bool gotMessage = false;
            while (Running){
                try {
                    gotMessage = client.TryReceiveFrameString(out output); // this returns true if it's successful
                    if (gotMessage) break;
                }
                catch (Exception e){
                    
                }
            }
            if (gotMessage) {
                //onOutputReceived?.Invoke(output);
                //onEmotionPrediction?.Invoke(output);
                _messageCallback(output);
            }
        }

        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use, not sure why yet
    }
    */

    public void SendInput(){
        ForceDotNet.Force(); // this line is needed to prevent unity freeze after one use, not sure why yet
        using (RequestSocket client = new RequestSocket()) {
            this.client = client;
            client.Connect("tcp://localhost:5555");

            var output = "";
            bool gotMessage = false;
            while (true){
                try {
                    Debug.Log("sent to server");
                    gotMessage = client.TryReceiveFrameString(out output); // this returns true if it's successful
                    if (gotMessage) break;
                }
                catch (Exception e){
                    break;
                }
            }
            if (gotMessage) {
                //onOutputReceived?.Invoke(output);
                //onEmotionPrediction?.Invoke(output);
                _messageCallback(output);
            }
        }

        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use, not sure why yet
    }

    /*
    public void SetOnTextReceivedListener(Action<string> onOutputReceived, Action<Exception> fallback)
    {
        this.onOutputReceived = onOutputReceived;
        onFail = fallback;
    }
    */
}