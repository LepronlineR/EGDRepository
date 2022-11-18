using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Dialogue Container", menuName = "Dialogue/Dialogue Graph", order = 0)]
[System.Serializable]
public class DialogueContainerSO : ScriptableObject {
    
    public List<NodeLinkData> nodeLinkDatas = new List<NodeLinkData>();
    public List<DialogueData> dialogueDatas = new List<DialogueData>();
    public List<EndData> endDatas = new List<EndData>();
    public List<StartData> startDatas = new List<StartData>();
    public List<EventData> eventDatas = new List<EventData>();
    public List<BranchData> branchDatas = new List<BranchData>();
    public List<ChoiceData> choiceDatas = new List<ChoiceData>();

    public List<BaseData> allDatas {
        get {
            List<BaseData> temp = new List<BaseData>();
            temp.AddRange(dialogueDatas);
            temp.AddRange(endDatas);
            temp.AddRange(startDatas);
            temp.AddRange(eventDatas);
            temp.AddRange(branchDatas);
            temp.AddRange(choiceDatas);
            return temp;
        }
    }
}

[System.Serializable]
public class NodeLinkData {
    public string baseNodeGuid;
    public string basePortName;
    public string targetNodeGuid;
    public string targetPortName;
}