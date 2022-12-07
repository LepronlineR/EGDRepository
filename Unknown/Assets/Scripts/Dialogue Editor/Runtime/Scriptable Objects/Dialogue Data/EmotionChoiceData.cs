using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif

[System.Serializable]
public class EmotionChoiceData : BaseData
{
    public ContainerEmotionChoiceType choiceStateType = new ContainerEmotionChoiceType();
}
