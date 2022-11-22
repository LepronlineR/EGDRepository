using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EndData : BaseData
{
    public List<ContainerDialogueContainerSO> endDialogueContainers = new List<ContainerDialogueContainerSO>();
    public ContainerEndNodeType endNodeType = new ContainerEndNodeType();
}