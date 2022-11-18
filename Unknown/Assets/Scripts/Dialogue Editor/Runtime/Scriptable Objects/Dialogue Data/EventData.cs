using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EventData : BaseData
{
    public List<EventDataStringModifier> eventDataStringModifiers = new List<EventDataStringModifier>();
    public List<ContainerDialogueEventSO> containerDialogueEventSOs = new List<ContainerDialogueEventSO>();
}
