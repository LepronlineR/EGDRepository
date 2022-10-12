using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;

public class SoundRequester : RunAbleThread {

    public string message = null;

    protected override void Run() {
        ForceDotNet.Force(); // this line is needed to prevent unity freeze after one use, not sure why yet
        using (RequestSocket client = new RequestSocket()) {
            client.Connect("tcp://localhost:5555");

            // send client file


            // recieve server response
            string temp = null;
            bool gotMessage = false;
            while (Running){
                gotMessage = client.TryReceiveFrameString(out temp); // this returns true if it's successful
                if (gotMessage) break;
            }

            if (gotMessage) {
                message = temp;
            }
        }

        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use, not sure why yet
    }
}