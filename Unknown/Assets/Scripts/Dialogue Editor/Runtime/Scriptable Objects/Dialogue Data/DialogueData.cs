using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[System.Serializable]
public class DialogueData : BaseData
{
    public List<DialogueDataBaseContainer> dialogueBaseContainer { get; set; } = new List<DialogueDataBaseContainer>();
    public List<DialogueDataName> dialogueDataNames = new List<DialogueDataName>();
    public List<DialogueDataText> dialogueDataTexts = new List<DialogueDataText>();
    public List<DialogueDataImages> dialogueDataImagess = new List<DialogueDataImages>();
    public List<DialogueDataPort> dialogueDataPorts = new List<DialogueDataPort>();

}

[System.Serializable]
public class DialogueDataBaseContainer {
    public ContainerInt ID = new ContainerInt();
}

[System.Serializable]
public class DialogueDataName {
    public ContainerString characterName = new ContainerString();

}

[System.Serializable]
public class DialogueDataText : DialogueDataBaseContainer {
    #if UNITY_EDITOR
    public TextField textField { get; set; }
    public ObjectField objectField { get; set; }
    #endif 

    public ContainerString GuidID = new ContainerString();
    public List<LanguageGeneric<string>> texts = new List<LanguageGeneric<string>>();
    public List<LanguageGeneric<AudioClip>> audioClips = new List<LanguageGeneric<AudioClip>>();
}

[System.Serializable]
public class DialogueDataImages : DialogueDataBaseContainer {
    public ContainerSprite sprite = new ContainerSprite();
}

[System.Serializable]
public class DialogueDataPort {
    public string portGuid;
    public string inputGuid;
    public string outputGuid;
}
