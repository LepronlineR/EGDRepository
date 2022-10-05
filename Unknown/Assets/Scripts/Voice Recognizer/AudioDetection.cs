using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDetection : MonoBehaviour
{

    public static AudioDetection Instance { get; private set; }

    void Awake() {
        if(Instance != null && Instance != this){
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public int sampleWindow = 64;

    public float GetAudioAmp(int pos, AudioClip clip){
        int start = pos - sampleWindow;
        if(start < 0)
            return 0.0f;
        float[] data = new float[sampleWindow];
        clip.GetData(data, start);

        float total = 0.0f;
        for(int x = 0; x < sampleWindow; x++){
            total += Mathf.Abs(data[x]);
        }

        // get mean
        return total/sampleWindow;
    }
}
