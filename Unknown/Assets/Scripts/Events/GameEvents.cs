using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class GameEvents : MonoBehaviour
{

    private event Action<int> defaultAction;
    protected UseStringEventCondition useStringEventCondition = new UseStringEventCondition();
    protected UseStringEventModifier useStringEventModifier = new UseStringEventModifier();

    public static GameEvents Instance { get; private set; }

    public Action<int> DefaultAction { get => defaultAction; set => defaultAction = value; }

    private void Awake() { 
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) { 
            Destroy(this);
        } else { 
            Instance = this; 
        } 
    }

    public void CallDefaultAction(int number){
        defaultAction?.Invoke(number);
    }

    public virtual void DialogueModifierEvents(string stringEvent, StringEventModifierType stringEventModifierType, float value = 0){

    }

    public virtual bool DialogueConditionEvents(string stringEvent, StringEventConditionType stringEventConditionType, float value = 0){
        return false;
    }

}
