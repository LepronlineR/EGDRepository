using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif

[System.Serializable]
public class ObjectChoiceData : BaseData {
    #if UNITY_EDITOR
    public ObjectField objectField;
    #endif
    public ContainerObject choiceObject = new ContainerObject();
}
