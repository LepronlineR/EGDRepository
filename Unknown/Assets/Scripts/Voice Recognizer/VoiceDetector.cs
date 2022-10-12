using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using TMPro;

public class VoiceDetector : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;
    public List<string> words = new List<string>();
    public TMP_Text word_text;

    void Start() {
        keywordRecognizer = new KeywordRecognizer(words.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();
    }

    void RecognizedSpeech(PhraseRecognizedEventArgs speech){
        word_text.text = speech.text;
    }
}
