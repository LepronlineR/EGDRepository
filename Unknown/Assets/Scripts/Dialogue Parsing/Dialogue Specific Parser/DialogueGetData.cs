using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueGetData : MonoBehaviour
{
    [SerializeField] protected List<DialogueContainerSO> dialogueContainers;
    protected DialogueContainerSO currentDialogueContainer;
    
    protected BaseData currentData;

    protected void DetermineDialogueContainer(){
        foreach(DialogueContainerSO container in dialogueContainers){
            StartData nodeData = container.startDatas[0];
            if(MainSystem.Instance.PlayerWord.ToLower().Equals(
                nodeData.text.Find(language => language.languageType == LanguageController.Instance.Language).languageGenericType.ToLower())){
                    currentDialogueContainer = container;
                    currentData = GetNextNode(currentDialogueContainer.startDatas[0]);
            }
        }
    }

    public List<string> GetAllStartNodeTexts(){
        List<string> result = new List<string>();
        foreach(DialogueContainerSO container in dialogueContainers){
            StartData nodeData = container.startDatas[0];
            result.Add(nodeData.text.Find(language => language.languageType == LanguageController.Instance.Language).languageGenericType);
        }
        return result;
    }
    
    protected BaseData GetNodeByGuid(string targetNodeGuid){
        return currentDialogueContainer.allDatas.Find(node => node.nodeGuid == targetNodeGuid);
    }

    protected BaseData GetNodeByNodePort(DialogueDataPort port){
        return currentDialogueContainer.allDatas.Find(node => node.nodeGuid == port.inputGuid);
    }

    protected BaseData GetNextNode(BaseData baseNodeData){
        NodeLinkData nld = currentDialogueContainer.nodeLinkDatas.Find(edge => edge.baseNodeGuid == baseNodeData.nodeGuid);
        return GetNodeByGuid(nld.targetNodeGuid);
    }
}
