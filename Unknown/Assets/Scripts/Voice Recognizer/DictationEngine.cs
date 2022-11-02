using UnityEngine;
using UnityEngine.Windows.Speech;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

public class DictationEngine : MonoBehaviour
{

    [SerializeField] TMP_Text word_text;
    [SerializeField] TMP_Text emotion_text;

    [SerializeField] GameObject AudioObject;

    protected DictationRecognizer dictationRecognizer;

    bool started = false;

    bool isRecording = true;
    private AudioSource audioSource;

    /*
    public void SendEmotionPrediction(string pred){
        emotion_text.text = pred;
    } 

    private void OnEnable() {
        PredictionRequester.onEmotionPrediction += SendEmotionPrediction;
    }

    private void OnDisable() {
        PredictionRequester.onEmotionPrediction -= SendEmotionPrediction;
    }*/

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
        audioSource = GetComponent<AudioSource>();
        emotion_text.text = "";
        AudioObject.SetActive(false);
    }
    
    void Update(){
        
        if(Input.GetMouseButton(1)){
            if(!started){
                // Debug.Log("started recording");

                started = true;
                dictationRecognizer.Start();
                AudioObject.SetActive(true);
                emotion_text.text = "";
                word_text.text = "";
                // =============== begin recording ===============
                
                audioSource.Stop();
                Microphone.End(null);
                audioSource.clip = StartRecording();
                // Invoke("ResizeRecording", 1);
            }
        } else {
            if(started){
                // Debug.Log("ended recording");

                started = false;
                dictationRecognizer.Stop();
                AudioObject.SetActive(false);
                // =============== end recording ===============
                AudioClip clip = StopRecording(audioSource, null);
                //if(!SavWav.Save("output.wav", clip)){
                //    Debug.Log("failed");
                //}

                byte[] bytes = SavWav.GetByteFromClip(clip);

                PredictionClient.Instance.Predict(bytes);
            }
        }
    }

    string result = "";

    public bool Started(){
        return started;
    }

    public string GetSentence(){
        return result;
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
        Debug.Log("Dictation hypothesis: " + text);
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
        Debug.Log("Dictation result: " + text);
        word_text.text = text;
        result = text;
        // write to file
    }

    private void DictationRecognizer_OnDictationError(string error, int hresult){
        // Debug.Log("Dictation error: " + error);
        // word_text.text   = error;
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