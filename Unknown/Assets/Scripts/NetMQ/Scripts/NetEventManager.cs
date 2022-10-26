using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NetEventManager : MonoBehaviour
{
    public static NetEventManager Instance;

    private void Awake(){
        if (Instance == null){
            Instance = this;
            
            onSendRequest = new UnityEvent();
            onClientBusy = new UnityEvent();
            onClientFree = new UnityEvent();
        } else
            Destroy(this);
    }

    public UnityEvent onSendRequest;
    public UnityEvent onClientBusy;
    public UnityEvent onClientFree;
}
