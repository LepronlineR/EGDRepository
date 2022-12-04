using System;
using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;
using System.Threading;

public class PredictionRequester {

    private readonly Thread receiveThread;
    private bool running;

    private readonly Action<string> _messageCallback;
    private RequestSocket client;

    public PredictionRequester(Action<string> messageCallback){
        _messageCallback = messageCallback;
        // receiveThread = new Thread(() => RequestMessage());
    }
    
    public void RequestMessage(byte[] bytes) {
        Debug.Log("Request Message");
        ForceDotNet.Force(); // this line is needed to prevent unity freeze after one use, not sure why yet
        using (RequestSocket client = new RequestSocket()) {
            this.client = client;
            client.Connect("tcp://localhost:5555");

            var output = "";
            bool gotMessage = false;
            if(!client.TrySendFrame(bytes)){
                Debug.Log("Sending failed");
            }

            while (true){
                try {
                    gotMessage = client.TryReceiveFrameString(out output); // this returns true if it's successful
                    if (gotMessage) break;
                }
                catch (Exception e){
                    Debug.LogWarning(e);
                    break;
                }
            }
            if (gotMessage) {
                _messageCallback(output);
            }
        }

        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use, not sure why yet
    }
}