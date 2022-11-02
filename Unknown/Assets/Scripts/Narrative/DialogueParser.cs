using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueParser : MonoBehaviour {

    [SerializeField] private DialogueContainer dialogue;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private VoiceDetector detector;
    [SerializeField] Transform speechBubbleLoc;
    [SerializeField] GameObject prompts;

    [Header("Dialogue Customization")]
    [SerializeField] float interval;

    // flags
    private bool interactable;
    private bool speaking;
    private bool skip; 

    NodeLinkData narrativeData;

    private void Start() {
        interactable = false;
        speaking = false;
        narrativeData = dialogue.NodeLinks.First(); //Entrypoint node
        //ProceedToNarrative(narrativeData.TargetNodeGUID);
        //StartCoroutine(Script());
    }

    public void MakeInteractable(){
        interactable = true;
    }

    public void MakeUninteractable(){
        interactable = false;
    }

    bool progress = false;
    bool said = false;
    bool picture = false;

    string r1 = "Hello, how are you doing?";
    string r2 = "Today seems like a good day";
    string r3 = "Can you say tree?";
    string r4 = "That is wrong";
    string r5 = "Good";
    string r6 = "Can you give me a picture of a white paint bucket?";
    string r7 = "That's not it";
    string r8 = "Cool paint.";

    int r = 1;

    
    void Update(){
        if(interactable && Input.GetMouseButtonDown(0)){ // process data
            if(!speaking){
                Debug.Log("Clicked on " + this.gameObject.name);
                Debug.Log(r);
                if(said)
                    DetermineSpeech();
                if(picture)
                    DetermineEvidence();
                switch(r){
                    case 1:
                        StartCoroutine(Type(r1));
                        r++;
                        break;
                    case 2:
                        StartCoroutine(Type(r2));
                        r++;
                        break;
                    case 3:
                        StartCoroutine(Type(r3));
                        said = true;
                        break;
                    case 4:
                        StartCoroutine(Type(r4));
                        said = false;
                        r--;
                        break;
                    case 5:
                        StartCoroutine(Type(r5));
                        said = false;
                        r++;
                        break;
                    case 6:
                        StartCoroutine(Type(r6));
                        picture = true;
                        break;
                    case 7:
                        StartCoroutine(Type(r7));
                        picture = false;
                        r--;
                        break;
                    case 8:
                        StartCoroutine(Type(r8));
                        r++;
                        break;
                }
            }
            //ProceedToNarrative(narrativeData.TargetNodeGUID);
        }
    }

    void DetermineSpeech(){
        // get sentence from speech
        if(DictationEngine.Instance.GetSentence().ToLower().Equals("tree")){ // success
            said = false;
            r = 5;
        } else if(DictationEngine.Instance.GetSentence().Equals("")){
            r = 4;
        }
    }

    public GameObject evidence;

    void DetermineEvidence(){
        // get selected image
        if(MainSystem.Instance.ImageContainsEvidence(evidence)){ // success
            picture = false;
            r = 8;
        } else {
            r = 7;
        }
    }

    /*

    private void DetermineNarrative(string narrativeDataGUID) {
        IEnumerable<NodeLinkData> choices = dialogue.NodeLinks.Where(x => x.BaseNodeGUID == narrativeDataGUID);
        if(choices.Count() > 1){ // process different choices (word)

        } else if(choices.Count() > 1) {// process different choices (evidence)

        } else { // default proceed
            ProceedToNarrative(narrativeDataGUID);
        }
    }

    private void ProceedToNarrative(string narrativeDataGUID) {
        var text = dialogue.DialogueNodeData.Find(x => x.NodeGUID == narrativeDataGUID).DialogueText;
        IEnumerable<NodeLinkData> choices = dialogue.NodeLinks.Where(x => x.BaseNodeGUID == narrativeDataGUID);
        StartCoroutine(Type(ProcessProperties(text)));
        dialogueText.text = ProcessProperties(text);
        foreach (NodeLinkData choice in choices){
            narrativeData = choice;
        }
    }

    private string ProcessProperties(string text){
        foreach (var exposedProperty in dialogue.ExposedProperties){
            text = text.Replace($"[{exposedProperty.PropertyName}]", exposedProperty.PropertyValue);
        }
        return text;
    }*/

    public IEnumerator Type(string word){
        dialogueText.text = word;
        dialogueText.ForceMeshUpdate();
        speaking = true;
        //audios.Play();
        //audios.loop = true;
        int totalCharacters = dialogueText.textInfo.characterCount;
        int count = 0;
        skip = false;
        while(count < totalCharacters+1){
            dialogueText.maxVisibleCharacters = count % (totalCharacters+1);
            yield return new WaitForSeconds(interval);
            count++;
            if(skip){ // skip the dialogue
                skip = false;
                dialogueText.maxVisibleCharacters = totalCharacters;
                break;
            }
        }
        //if(audios == null) yield break; 
        //audios.loop = false;
        speaking = false;
        yield return new WaitForSeconds(0.6f);
    }


}
