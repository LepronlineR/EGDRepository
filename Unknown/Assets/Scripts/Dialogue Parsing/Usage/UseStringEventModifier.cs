using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseStringEventModifier 
{
    public bool ModifierBoolCheck(StringEventModifierType stringEventModifierType){
        switch(stringEventModifierType){
            case StringEventModifierType.SetTrue:
                return true;
            case StringEventModifierType.SetFalse:
                return false;
            default:
                Debug.LogError("GameEvents could not find a suitable event");
                return false;
        }
    }

    public float ModifierFloatCheck(float inputValue, StringEventModifierType stringEventModifierType){
        switch(stringEventModifierType){
            case StringEventModifierType.Add:
                return inputValue;
            case StringEventModifierType.Subtract:
                return -inputValue;
            default:
                return 0;
        }
    }
}
