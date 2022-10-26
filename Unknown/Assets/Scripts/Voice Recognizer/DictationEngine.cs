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
 
    //temporary audio vector we write to every second while recording is enabled..
    List<float> tempRecording = new List<float>();
 
    //list of recorded clips...
    // List<float[]> recordedClips = new List<float[]>();

    void Start(){
        StartDictationEngine();
        audioSource = GetComponent<AudioSource>();
        //set up recording to last a max of 1 seconds and loop over and over
        //audioSource.clip = Microphone.Start(null, true, 1, 44100);
        //audioSource.Play();
        emotion_text.text = "";
    }
    
    void Update(){
        
        if(Input.GetMouseButton(1)){
            if(!started){
                Debug.Log("started recording");

                started = true;
                dictationRecognizer.Start();
                emotion_text.text = "";
                word_text.text = "";
                // =============== begin recording ===============
                
                audioSource.Stop();
                tempRecording.Clear();
                Microphone.End(null);
                audioSource.clip = StartRecording();
                // Invoke("ResizeRecording", 1);
            }
        } else {
            if(started){
                Debug.Log("ended recording");

                started = false;
                dictationRecognizer.Stop();
                // =============== end recording ===============
                StopRecording();
                if(!SavWav.Save("output.wav", audioSource.clip)){
                    Debug.Log("failed");
                }

                PredictionClient.Instance.Predict(null);
            }
        }
    }

    public AudioClip StartRecording(string deviceName = null){
        var audioClip = UnityEngine.Microphone.Start(deviceName, true, 10, 44100);
        while (UnityEngine.Microphone.GetPosition(deviceName) <= 0) ;
        return audioClip;
    }



    public void StopRecording(string deviceName = null){
        UnityEngine.Microphone.End(deviceName);
    }

    /*
    void ResizeRecording(){
        if (started){
            //add the next second of recorded audio to temp vector
            int length = 44100;
            float[] clipData = new float[length];
            audioSource.clip.GetData(clipData, 0);
            tempRecording.AddRange(clipData);
            Invoke("ResizeRecording", 1);
        }
    }

*/
    public byte[] returnByteArrayForCurrentRecording(AudioClip audioClip) {
        var samples = new float[audioClip.samples];
        audioClip.GetData(samples, 0);
        Int16[] intData = new Int16[samples.Length];
        Byte[] bytesData = new Byte[samples.Length * 2];
        int rescaleFactor = 32767;
        for (int i = 0; i < samples.Length; i++){
            intData[i] = (short)(samples[i] * rescaleFactor);
            Byte[] byteArr = new Byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }

        return bytesData;
    }
    

    private void DictationRecognizer_OnDictationHypothesis(string text){
        // Debug.Log("Dictation hypothesis: " + text);
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
        // Debug.Log("Dictation result: " + text);
        word_text.text = text;
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