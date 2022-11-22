using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[System.Serializable]
public class ObjectChoiceData : BaseData {
    #if UNITY_EDITOR
    public ObjectField objectField;
    #endif
    public ContainerObject choiceObject = new ContainerObject();
}
