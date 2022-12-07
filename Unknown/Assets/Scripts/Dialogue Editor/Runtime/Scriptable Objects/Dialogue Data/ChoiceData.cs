using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif

[System.Serializable]
public class ChoiceData : BaseData
{
    #if UNITY_EDITOR
    public TextField textField { get; set; }
    public ObjectField objectField { get; set; }
    #endif

    public ContainerChoiceStateType choiceStateType = new ContainerChoiceStateType();
    public List<LanguageGeneric<string>> text = new List<LanguageGeneric<string>>();
    public List<LanguageGeneric<AudioClip>> audioClips = new List<LanguageGeneric<AudioClip>>();
    public List<EventDataStringCondition> eventDataStringConditions = new List<EventDataStringCondition>();
    public List<EventDataGameObjectCondition> eventDataObjectConditions = new List<EventDataGameObjectCondition>();
}
