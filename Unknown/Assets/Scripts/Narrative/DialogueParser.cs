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
    [SerializeField] Transform lookAtPlayer;

    private bool interactable;

    private void Start() {
        interactable = false;

        var narrativeData = dialogue.NodeLinks.First(); //Entrypoint node
        //ProceedToNarrative(narrativeData.TargetNodeGUID);
    }

    public void MakeInteractable(){
        interactable = true;
    }

    public void MakeUninteractable(){
        interactable = false;
    }

    
    void Update(){
        if(Input.GetMouseButtonDown(0)){ // process data

        }
    }



    private void ProceedToNarrative(string narrativeDataGUID) {
        var text = dialogue.DialogueNodeData.Find(x => x.NodeGUID == narrativeDataGUID).DialogueText;
        IEnumerable<NodeLinkData> choices = dialogue.NodeLinks.Where(x => x.BaseNodeGUID == narrativeDataGUID);
        dialogueText.text = ProcessProperties(text);
        if(choices.Count() > 1){
            foreach (NodeLinkData choice in choices){
                var prompt = Instantiate(prompts, speechBubbleLoc);
                prompt.GetComponent<TMP_Text>().text = ProcessProperties(choice.PortName);
            }
        } else if(choices.Count() == 1){
            //ProceedToNarrative = choices[0].PortName;
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
}
