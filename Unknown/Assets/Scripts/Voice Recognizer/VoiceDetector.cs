using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using TMPro;

public class VoiceDetector : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;
    private List<string> words = new List<string>();

    void Start() {
        keywordRecognizer = new KeywordRecognizer(words.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();
    }

    void RecognizedSpeech(PhraseRecognizedEventArgs speech){
        Debug.Log(speech.text);
    }
}
