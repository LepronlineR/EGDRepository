using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTalk : DialogueGetData
{
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private AudioSource audioSource;

    private DialogueNodeData currentNodeData;
    private DialogueNodeData lastNodeData;

    void Awake() {
        
    }

    void StartDialogue() {
        CheckNodeType(GetNextNode(dialogueContainerSO.startNodeDatas[0]));
        dialogueController.ShowDialogue(true);
    }

    private void CheckNodeType(BaseNodeData _baseNodeData){
        switch(_baseNodeData){
            case StartNodeData nodeData:
                RunNode(nodeData);
                break;
            case DialogueNodeData nodeData:
                RunNode(nodeData);
                break;
            case EventNodeData nodeData:
                RunNode(nodeData);
                break;
            case EndNodeData nodeData:
                RunNode(nodeData);
                break;
            default:
                break;
        }
    }

    private void RunNode(StartNodeData nodeData){
        CheckNodeType(GetNextNode(dialogueContainerSO.startNodeDatas[0]));
    }

    private void RunNode(DialogueNodeData nodeData){
        if(currentNodeData != nodeData){
            lastNodeData = currentNodeData;
            currentNodeData = nodeData;
        }

        dialogueController.SetText(nodeData.name, nodeData.textType.Find(text => text.languageType == LanguageController.Instance.Language).languageGenericType);
        dialogueController.SetImage(nodeData.sprite, nodeData.dialogueEmotionType);

        MakeActions(nodeData.dialogueNodePorts);

        audioSource.clip = nodeData.audioClips.Find(clip => clip.languageType == LanguageController.Instance.Language).languageGenericType;
        audioSource.Play();
    }

    private void RunNode(EventNodeData nodeData){
        foreach(var item in nodeData.eventScriptableObjectDatas){
            if(item.dialogueEventSO != null){
                item.dialogueEventSO.RunEvent();
            }
        }
        CheckNodeType(GetNextNode(nodeData));
    }

    private void RunNode(EndNodeData nodeData){
        switch(nodeData.endNodeType){
            case EndNodeType.End:
                dialogueController.ShowDialogue(false);
                break;
        }
    }

    private void MakeActions(List<DialogueNodePort> nodePorts){
        List<string> texts = new List<string>();
        List<UnityAction> unityActions = new List<UnityAction>();
        foreach(DialogueNodePort nodePort in nodePorts){
            texts.Add(nodePort.textLanguages.Find(text => text.languageType == LanguageController.Instance.Language).languageGenericType);
            UnityAction action = null;
            action += () => {
                CheckNodeType(GetNodeByGuid(nodePort.inputGuid));
                audioSource.Stop();
            };
            unityActions.Add(action);
        }
        dialogueController.SetActions(texts, unityActions);
    }
}
