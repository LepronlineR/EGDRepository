using UnityEngine;
using UnityEngine.Windows.Speech;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;

public class DictationEngine : MonoBehaviour
{

    public TMP_Text word_text;

    protected DictationRecognizer dictationRecognizer;

    bool started = false;

    bool isRecording = true;
    private AudioSource audioSource;
 
    //temporary audio vector we write to every second while recording is enabled..
    List<float> tempRecording = new List<float>();
 
    //list of recorded clips...
    // List<float[]> recordedClips = new List<float[]>();

    void Start(){
        StartDictationEngine();
        audioSource = GetComponent<AudioSource>();
        //set up recording to last a max of 1 seconds and loop over and over
        audioSource.clip = Microphone.Start(null, true, 1, 44100);
        audioSource.Play();
    }

    string prediction = "";
    
    
    void Update(){
        
        if(Input.GetMouseButton(0)){
            if(!started){
                started = true;
                dictationRecognizer.Start();
                // =============== begin recording ===============
                
                audioSource.Stop();
                tempRecording.Clear();
                Microphone.End(null);
                audioSource.clip = Microphone.Start(null, true, 1, 44100);
                Invoke("ResizeRecording", 1);
            }
        } else {
            if(started){
                started = false;
                CloseDictationEngine();
                // =============== end recording ===============

                //stop recording, get length, create a new array of samples
                int length = Microphone.GetPosition(null);
             
                Microphone.End(null);
                float[] clipData = new float[length];
                audioSource.clip.GetData(clipData, 0);

                float[] fullClip = new float[clipData.Length + tempRecording.Count];
                for (int i = 0; i < fullClip.Length; i++)
                {
                    //write data all recorded data to fullCLip vector
                    if (i < tempRecording.Count)
                        fullClip[i] = tempRecording[i];
                    else
                        fullClip[i] = clipData[i - tempRecording.Count];
                }

                PredictionClient.Instance.Predict(fullClip, output => {
                    var outputMax = output.Max();
                    var maxIndex = Array.IndexOf(output, outputMax);
                    prediction = "Prediction: " + Convert.ToChar(64 + maxIndex);
                }, error => {
                    // hope no error exists
                });

                Debug.Log(prediction);
               
                //recordedClips.Add(fullClip);
                //audioSource.clip = AudioClip.Create("recorded samples", fullClip.Length, 1, 44100, false);
                //audioSource.clip.SetData(fullClip, 0);
            }
        }
    }

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
                //StartDictationEngine();
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