using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class DialogueController : DialogueGetData {

    [Header("Text")]
    [SerializeField] private TMP_Text textName;
    [SerializeField] private TMP_Text textBox;
    [SerializeField] [Range(0.0f, 1.0f)] float interval;

    [Header("Image")]
    [SerializeField] private Image image;
    [SerializeField] private GameObject imageGO;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [Header("Dialogue Talker")]
    private DialogueData currentDialogueNodeData;
    private DialogueData lastDialogueNodeData;
    private List<DialogueDataBaseContainer> baseContainers;
    private int currentIndex = 0;

    // flags
    private bool interactable = false;
    private bool response = false;
    private bool speaking = false;
    private bool skip = true; 

    void Start() {
        
    }

    void Update() {
        if(interactable && Input.GetMouseButtonDown(0)){ // process data
            if(!speaking){
                // process current node
                if(response){
                    ProcessResponse();
                } else if(currentIndex > 0) {
                    ParseDialogue();
                } else {
                    ProcessCurrentNode(currentData);
                }
            } else {
                skip = true;
            }
        }
    }

    #region Dialogue Parser
    
    private void ProcessCurrentNode(BaseData baseNodeData){
        switch (baseNodeData){
            case null: // havent chosen a node yet (Run as StartData)
                DetermineDialogueContainer();
                break;
            case DialogueData nodeData:
                RunNode(nodeData);
                break;
            case EventData nodeData:
                RunNode(nodeData);
                break;
            case EndData nodeData:
                RunNode(nodeData);
                break;
            default:
                Debug.Log("Unable to parse or not implemented");
                break;
        }
    }

    private void RunNode(EventData nodeData){
        foreach (ContainerDialogueEventSO item in nodeData.containerDialogueEventSOs){
            if (item.dialogueEventSO != null){
                item.dialogueEventSO.RunEvent();
            }
        }

        foreach (EventDataStringModifier item in nodeData.eventDataStringModifiers){
            GameEvents.Instance.DialogueModifierEvents(item.stringEventText.value, item.stringEventModifierType.value, item.number.value);
        }
        currentData = GetNextNode(currentData);
        ProcessCurrentNode(currentData);
    }

    private void RunNode(EndData nodeData){
        // currently only supports one end result
        switch (nodeData.endNodeType.value) {
            case EndNodeType.End:
                // add all the other containers from the end to the list (make sure that they are not the same)
                foreach(ContainerDialogueContainerSO container in nodeData.endDialogueContainers){
                    dialogueContainers.AddItem(container.dialogueContainerSO);
                }
                // remove it from the current container
                dialogueContainers.Remove(currentDialogueContainer);
                currentDialogueContainer = null;
                break;
            default:
                break;
        }
        // reset
        currentData = null;
        textBox.text = "";
    }

    private void RunNode(DialogueData nodeData){

        currentDialogueNodeData = nodeData;

        baseContainers = new List<DialogueDataBaseContainer>();
        baseContainers.AddRange(nodeData.dialogueDataImagess);
        baseContainers.AddRange(nodeData.dialogueDataNames);
        baseContainers.AddRange(nodeData.dialogueDataTexts);
        baseContainers.AddRange(nodeData.dialogueResponseTexts);

        currentIndex = 0;

        baseContainers.Sort(delegate (DialogueDataBaseContainer x, DialogueDataBaseContainer y){
            return x.ID.value.CompareTo(y.ID.value);
        });

        ParseDialogue();
    }

    private void ParseDialogue(){
        for(int x = currentIndex; x < baseContainers.Count; x++){
            currentIndex = x + 1;

            if(baseContainers[x] is DialogueDataName){
                DialogueDataName tmp = baseContainers[x] as DialogueDataName;
                SetName(tmp.characterName.value);
            }
            if(baseContainers[x] is DialogueDataImages){
                DialogueDataImages tmp = baseContainers[x] as DialogueDataImages;
                SetImage(tmp.sprite.value);
            }

            if(baseContainers[x] is DialogueDataText){
                DialogueDataText tmp = baseContainers[x] as DialogueDataText;
                SetText(tmp.texts.Find(t => t.languageType == LanguageController.Instance.Language).languageGenericType);
                PlayAudio(tmp.audioClips.Find(ac => ac.languageType == LanguageController.Instance.Language).languageGenericType);
                break;
            }

            if(baseContainers[x] is DialogueDataResponseText){
                DialogueDataResponseText tmp = baseContainers[x] as DialogueDataResponseText;
                response = true;
                SetResponse(tmp.responseText.value);
                break;
            }
        }
    }

    private void ProcessResponse(){
        foreach (DialogueDataPort port in currentDialogueNodeData.dialogueDataPorts){
            if(ChoiceCheck(port.inputGuid)){
                ProcessCurrentNode(currentData);
                response = false;
                break;
            }
        }
    }

    private bool ChoiceCheck(string guidID){
        BaseData data = GetNodeByGuid(guidID);
        switch(data){
            case EmotionChoiceData ecd:
                return RunChoice(ecd);
            case ObjectChoiceData ocd:
                return RunChoice(ocd);
            default:
                break;
        }
        return false;
    }

    private bool RunChoice(EmotionChoiceData data){
        // compare the emotion choice to the player emotion
        if(MainSystem.Instance.PlayerEmotion.Equals(data.choiceStateType.value) || 
        data.choiceStateType.value == DialogueEmotionType.Otherwise){

            currentData = GetNextNode(data);
            return true;
        }
        return false;
    }

    private bool RunChoice(ObjectChoiceData data){
        // compare the object choice to the player current object at hand
        foreach(GameObject obj in MainSystem.Instance.PlayerEvidence){
            if(GameObject.ReferenceEquals(obj, data.choiceObject.value) ||
            data.choiceObject.value == null){
                currentData = GetNextNode(data);
            return true;
            }
        }
        return false;
    }

    #endregion

    #region Set Specifics (GO/Image/Etc)

    public void MakeInteractable(bool inter){
        interactable = inter;
    }

    public bool HasNotBegan(){
        return currentDialogueContainer == null ? true : false;
    }

    public void SetName(string name){
        textName.text = name;
    }

    public void SetText(string text){
        StartCoroutine(Type(text));
    }

    private void PlayAudio(AudioClip audioClip){
        audioSource.Stop();
        if(audioClip != null){
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }

    public void SetResponse(string text){
        audioSource.Stop();
        // TODO: make this appear as UI elements in front of the player
        Debug.Log(text);
    }

    public IEnumerator Type(string word){
        textBox.text = word;
        textBox.ForceMeshUpdate();
        speaking = true;
        //audios.Play();
        //audios.loop = true;
        int totalCharacters = textBox.textInfo.characterCount;
        int count = 0;
        skip = false;
        while(count < totalCharacters+1){
            textBox.maxVisibleCharacters = count % (totalCharacters+1);
            yield return new WaitForSeconds(interval);
            count++;
            if(skip){ // skip the dialogue
                skip = false;
                textBox.maxVisibleCharacters = totalCharacters;
                break;
            }
        }
        //if(audios == null) yield break; 
        //audios.loop = false;
        speaking = false;
        yield return new WaitForSeconds(0.6f);
    }

    public void SetImage(Sprite setImg){
        if(setImg != null){
            image.sprite = setImg;
        }
    }
    
    public void SetActions(List<string> texts, List<UnityAction> actions){

    }

    public void SetContinue(UnityAction unityAction){
        // buttonContinue.onClick = new Button.ButtonClickedEvent();
        // buttonContinue.onClick.AddListener(unityAction);
        // buttonContinue.gameObject.SetActive(true);
    }
    
    #endregion
}
