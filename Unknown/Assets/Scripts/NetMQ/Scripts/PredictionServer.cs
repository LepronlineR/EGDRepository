using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictionServer : MonoBehaviour
{
    private PredictionReceiver predictionReceiver;
    public static PredictionServer Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start() => Initialize();

    public void Initialize()
    {
        predictionReceiver = new PredictionReceiver();
    }
}
