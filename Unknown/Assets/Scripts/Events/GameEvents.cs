using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class GameEvents : MonoBehaviour
{

    private event Action gameEvent;

    public static GameEvents Instance { get; private set; }

    public Action GameEvent { get => gameEvent; set => gameEvent = value; }

    private void Awake(){
        Instance = this;
    }


}
