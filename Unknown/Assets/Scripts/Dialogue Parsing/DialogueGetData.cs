using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueGetData : MonoBehaviour
{
    [SerializeField] public DialogueContainerSO dialogueContainerSO;
    /*
    protected BaseNodeData GetNodeByGuid(string _targetNodeGuid){
        return dialogueContainerSO.allNodes.Find(node => node.nodeGuid == _targetNodeGuid);
    }

    protected BaseNodeData GetNodeByNodePort(DialogueNodePort port){
        return dialogueContainerSO.allNodes.Find(node => node.nodeGuid == port.inputGuid);
    }

    protected BaseNodeData GetNextNode(BaseNodeData baseNodeData){
        NodeLinkData nld = dialogueContainerSO.nodeLinkDatas.Find(edge => edge.baseNodeGuid == baseNodeData.nodeGuid);
        return GetNodeByGuid(nld.targetNodeGuid);
    }
    */
}
