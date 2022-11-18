using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Dialogue Container", menuName = "Dialogue/Dialogue Graph", order = 0)]
[System.Serializable]
public class DialogueContainerSO : ScriptableObject {
    public List<NodeLinkData> nodeLinkDatas = new List<NodeLinkData>();
    public List<DialogueNodeData> dialogueNodeDatas = new List<DialogueNodeData>();
    public List<EndNodeData> endNodeDatas = new List<EndNodeData>();
    public List<StartNodeData> startNodeDatas = new List<StartNodeData>();
    public List<EventNodeData> eventNodeDatas = new List<EventNodeData>();

    public List<BaseNodeData> allNodes {
        get {
            List<BaseNodeData> temp = new List<BaseNodeData>();
            temp.AddRange(dialogueNodeDatas);
            temp.AddRange(endNodeDatas);
            temp.AddRange(startNodeDatas);
            temp.AddRange(eventNodeDatas);
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

[System.Serializable]
public class BaseNodeData {
    public string nodeGuid;
    public Vector2 position;
}

[System.Serializable]
public class DialogueNodeData : BaseNodeData {
    public List<DialogueNodePort> dialogueNodePorts;
    public Sprite sprite;
    public DialogueEmotionType dialogueEmotionType;
    public List<LanguageGeneric<AudioClip>> audioClips;
    public string name;
    public List<LanguageGeneric<string>> textType;
}

[System.Serializable]
public class EndNodeData : BaseNodeData {
    public EndNodeType endNodeType;
}

[System.Serializable]
public class StartNodeData : BaseNodeData {

}

#region Event Node
[System.Serializable]
public class EventNodeData : BaseNodeData {
    public List<EventStringIDData> eventStringIDDatas;
    public List<EventScriptableObjectData> eventScriptableObjectDatas;
}

[System.Serializable]
public class EventStringIDData {
    public string stringEvent;
    public int ID;
}

[System.Serializable]
public class EventScriptableObjectData {
    public DialogueEventSO dialogueEventSO;
}

#endregion

[System.Serializable]
public class LanguageGeneric<T> {
    public LanguageType languageType;
    public T languageGenericType;
}

[System.Serializable]
public class DialogueNodePort {
    public string inputGuid;
    public string outputGuid;
    public Port myPort;
    public TextField textField;
    public List<LanguageGeneric<string>> textLanguages = new List<LanguageGeneric<string>>();
}