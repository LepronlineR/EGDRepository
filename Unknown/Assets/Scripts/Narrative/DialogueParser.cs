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
    }

    public void MakeInteractable(){
        interactable = true;
    }

    public void MakeUninteractable(){
        interactable = false;
    }

    
    void Update(){
        if(interactable && Input.GetMouseButtonDown(0)){ // process data
            if(!speaking)
                ProceedToNarrative(narrativeData.TargetNodeGUID);
        }
    }



    private void ProceedToNarrative(string narrativeDataGUID) {
        var text = dialogue.DialogueNodeData.Find(x => x.NodeGUID == narrativeDataGUID).DialogueText;
        IEnumerable<NodeLinkData> choices = dialogue.NodeLinks.Where(x => x.BaseNodeGUID == narrativeDataGUID);
        StartCoroutine(Type(ProcessProperties(text)));
        dialogueText.text = ProcessProperties(text);
        if(choices.Count() > 1){
            foreach (NodeLinkData choice in choices){
                //var prompt = Instantiate(prompts, speechBubbleLoc);
               // prompt.GetComponent<TMP_Text>().text = ProcessProperties(choice.PortName);
            }
        } else if(choices.Count() == 1){
            foreach (NodeLinkData choice in choices){
                narrativeData = choice;
                //var prompt = Instantiate(prompts, speechBubbleLoc);
               // prompt.GetComponent<TMP_Text>().text = ProcessProperties(choice.PortName);
            }
        }
        /*
        var buttons = buttonContainer.GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++){
            Destroy(buttons[i].gameObject);
        }

        foreach (var choice in choices){
            var button = Instantiate(choicePrefab, buttonContainer);
            button.GetComponentInChildren<Text>().text = ProcessProperties(choice.PortName);
            button.onClick.AddListener(() => ProceedToNarrative(choice.TargetNodeGUID));
        }*/
    }

    private string ProcessProperties(string text){
        foreach (var exposedProperty in dialogue.ExposedProperties){
            text = text.Replace($"[{exposedProperty.PropertyName}]", exposedProperty.PropertyValue);
        }
        return text;
    }

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
