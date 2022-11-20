using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueGetData : MonoBehaviour
{
    [SerializeField] protected DialogueContainerSO dialogueContainer;
    
    protected BaseData GetNodeByGuid(string targetNodeGuid){
        return dialogueContainer.allDatas.Find(node => node.nodeGuid == targetNodeGuid);
    }

    protected BaseData GetNodeByNodePort(DialogueDataPort port){
        return dialogueContainer.allDatas.Find(node => node.nodeGuid == port.inputGuid);
    }

    protected BaseData GetNextNode(BaseData baseNodeData){
        NodeLinkData nld = dialogueContainer.nodeLinkDatas.Find(edge => edge.baseNodeGuid == baseNodeData.nodeGuid);
        return GetNodeByGuid(nld.targetNodeGuid);
    }
}
