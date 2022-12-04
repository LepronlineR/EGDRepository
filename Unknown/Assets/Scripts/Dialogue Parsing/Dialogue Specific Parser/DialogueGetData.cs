using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueGetData : MonoBehaviour
{
    [SerializeField] protected List<DialogueContainerSO> dialogueContainers;
    protected DialogueContainerSO currentDialogueContainer;
    
    protected BaseData currentData;

    protected bool DetermineDialogueContainer(){
        foreach(DialogueContainerSO container in dialogueContainers){
            StartData nodeData = container.startDatas[0];
            // if the begin node text is blank then we can just proceed
            if(nodeData.text.Find(language => language.languageType == LanguageController.Instance.Language).languageGenericType == ""){
                MainSystem.Instance.RemoveAllBubbles();
                    
                currentDialogueContainer = container;
                currentData = GetNextNode(currentDialogueContainer.startDatas[0]);
                return true;
            }
            if(MainSystem.Instance.PlayerWord.ToLower().Equals(
                nodeData.text.Find(language => language.languageType == LanguageController.Instance.Language).languageGenericType.ToLower())){
                    // Remove the thought bubbles
                    MainSystem.Instance.RemoveAllBubbles();
                    
                    currentDialogueContainer = container;
                    currentData = GetNextNode(currentDialogueContainer.startDatas[0]);
                    return true;
            }
        }
        return false;
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
