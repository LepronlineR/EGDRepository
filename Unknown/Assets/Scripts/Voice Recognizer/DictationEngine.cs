using UnityEngine;
using UnityEngine.Windows.Speech;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

public class DictationEngine : MonoBehaviour {

    protected DictationRecognizer dictationRecognizer;

    [SerializeField] TMP_Text word_text;

    public static DictationEngine Instance { get; private set; }
    private void Awake() { 
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) { 
            Destroy(this);
        } else { 
            Instance = this; 
        } 
    }

    void Start(){
        StartDictationEngine();
    }

    string result = "";

    public string GetSentence(){
        return result;
    }

    public void StartDictation(){
        dictationRecognizer.Start();
    }

    public void EndDictation(){
        dictationRecognizer.Stop();
    }

    public AudioClip StopRecording(AudioSource audS, string deviceName) {
        //Capture the current clip data
        AudioClip recordedClip = audS.clip;
        var position = Microphone.GetPosition(deviceName);
        var soundData = new float[recordedClip.samples * recordedClip.channels];
        recordedClip.GetData(soundData, 0);
 
        //Create shortened array for the data that was used for recording
        var newData = new float[position * recordedClip.channels];
 
        //Copy the used samples to a new array
        for (int i = 0; i < newData.Length; i++) {
            newData[i] = soundData[i];
        }
 
        // we make a new one with the appropriate length
        var newClip = AudioClip.Create(recordedClip.name, position, recordedClip.channels, recordedClip.frequency, false);
        newClip.SetData(newData, 0);        //Give it the data from the old clip
        return newClip;
    }

    
    public AudioClip StartRecording(string deviceName = null){
        var audioClip = UnityEngine.Microphone.Start(deviceName, true, 20, 44100);
        while (UnityEngine.Microphone.GetPosition(deviceName) <= 0) ;
        return audioClip;
    }

    private void DictationRecognizer_OnDictationHypothesis(string text){
        word_text.text = text;
    }

    private void DictationRecognizer_OnDictationComplete(DictationCompletionCause completionCause){
        switch (completionCause)
        {
            case DictationCompletionCause.TimeoutExceeded:
            case DictationCompletionCause.PauseLimitExceeded:
            case DictationCompletionCause.Canceled:
            case DictationCompletionCause.Complete:
                // Restart required
                CloseDictationEngine();
                StartDictationEngine();
                break;
            case DictationCompletionCause.UnknownError:
            case DictationCompletionCause.AudioQualityFailure:
            case DictationCompletionCause.MicrophoneUnavailable:
            case DictationCompletionCause.NetworkFailure:
                // Error
                CloseDictationEngine();
                break;
        }
    }

    private void DictationRecognizer_OnDictationResult(string text, ConfidenceLevel confidence){
        result = text;
        word_text.text = text;
    }

    private void DictationRecognizer_OnDictationError(string error, int hresult){

    }

    private void OnApplicationQuit(){
        CloseDictationEngine();
    }

    private void StartDictationEngine(){
        dictationRecognizer = new DictationRecognizer();
        dictationRecognizer.DictationHypothesis += DictationRecognizer_OnDictationHypothesis;
        dictationRecognizer.DictationResult += DictationRecognizer_OnDictationResult;
        dictationRecognizer.DictationComplete += DictationRecognizer_OnDictationComplete;
        dictationRecognizer.DictationError += DictationRecognizer_OnDictationError;
    }

    private void CloseDictationEngine(){
        if (dictationRecognizer != null)
        {
            dictationRecognizer.DictationHypothesis -= DictationRecognizer_OnDictationHypothesis;
            dictationRecognizer.DictationComplete -= DictationRecognizer_OnDictationComplete;
            dictationRecognizer.DictationResult -= DictationRecognizer_OnDictationResult;
            dictationRecognizer.DictationError -= DictationRecognizer_OnDictationError;
            if (dictationRecognizer.Status == SpeechSystemStatus.Running)
            {
                dictationRecognizer.Stop();
            }
            dictationRecognizer.Dispose();
        }
    }
}