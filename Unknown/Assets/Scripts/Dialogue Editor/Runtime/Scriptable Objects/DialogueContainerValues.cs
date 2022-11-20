using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueContainerValues {}

[System.Serializable]
public class LanguageGeneric<T> {
    public LanguageType languageType;
    public T languageGenericType;
}

[System.Serializable]
public class ContainerDialogueEventSO {
    public DialogueEventSO dialogueEventSO;
}

// ============== VALUES ======================
[System.Serializable]
public class ContainerString {
    public string value;
}

[System.Serializable]
public class ContainerInt {
    public int value;
}

[System.Serializable]
public class ContainerFloat {
    public float value;
}

[System.Serializable]
public class ContainerSprite {
    public Sprite value;
}

[System.Serializable]
public class ContainerObject {
    public GameObject value;
}

// ================ ENUMS =========================

[System.Serializable]
public class ContainerChoiceStateType {
    #if UNITY_EDITOR
    public UnityEngine.UIElements.EnumField enumField;
    #endif
    public ChoiceStateType value = ChoiceStateType.Hide;
}

[System.Serializable]
public class ContainerEndNodeType {
    #if UNITY_EDITOR
    public UnityEngine.UIElements.EnumField enumField;
    #endif
    public EndNodeType value = EndNodeType.End;
}

[System.Serializable]
public class ContainerEmotionChoiceType {
    #if UNITY_EDITOR
    public UnityEngine.UIElements.EnumField enumField;
    #endif
    public DialogueEmotionType value = DialogueEmotionType.Neutral;
}

[System.Serializable]
public class ContainerStringEventModifierType {
    #if UNITY_EDITOR
    public UnityEngine.UIElements.EnumField enumField;
    #endif
    public StringEventModifierType value = StringEventModifierType.SetTrue;
}

[System.Serializable]
public class ContainerStringEventConditionType {
    #if UNITY_EDITOR
    public UnityEngine.UIElements.EnumField enumField;
    #endif
    public StringEventConditionType value = StringEventConditionType.True;
}

[System.Serializable]
public class ContainerObjectEventConditionType {
    #if UNITY_EDITOR
    public UnityEngine.UIElements.EnumField enumField;
    #endif
    public ObjectEventConditionType value = ObjectEventConditionType.NotEquals;
}

[System.Serializable]
public class ContainerObjectEventModifierType {
    #if UNITY_EDITOR
    public UnityEngine.UIElements.EnumField enumField;
    #endif
    public ObjectEventModifierType value = ObjectEventModifierType.SetActive;
}

// ================ EVENTS =========================

[System.Serializable]
public class EventDataStringModifier {
    public ContainerString stringEventText = new ContainerString();
    public ContainerFloat number = new ContainerFloat();

    public ContainerStringEventModifierType stringEventModifierType = new ContainerStringEventModifierType();
}

[System.Serializable]
public class EventDataStringCondition {
    public ContainerString stringEventText = new ContainerString();
    public ContainerFloat number = new ContainerFloat();

    public ContainerStringEventConditionType stringEventConditionType = new ContainerStringEventConditionType();
}

[System.Serializable]
public class EventDataGameObjectCondition {
    public ContainerObject objectEvent = new ContainerObject();

    public ContainerObjectEventConditionType objectEventConditionType = new ContainerObjectEventConditionType();
}