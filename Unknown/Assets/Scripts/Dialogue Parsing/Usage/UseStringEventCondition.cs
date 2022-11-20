using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseStringEventCondition 
{
    public bool ConditionStringCheck(string currentValue, string checkValue, StringEventConditionType stringEventConditionType){
        switch(stringEventConditionType){
            case StringEventConditionType.Equals:
                return ValueEquals(currentValue, checkValue);
            default:
                Debug.LogWarning("GameEvents could not find a suitable event");
                return false;
        }
    }

    public bool ConditionFloatCheck(float currentValue, float checkValue, StringEventConditionType stringEventConditionType){
        switch(stringEventConditionType){
            case StringEventConditionType.Equals:
                return ValueEquals(currentValue, checkValue);
            case StringEventConditionType.EqualsMoreThan:
                return ValueEqualsMoreThan(currentValue, checkValue);
            case StringEventConditionType.EqualsLessThan:
                return ValueEqualsLessThan(currentValue, checkValue);
            case StringEventConditionType.MoreThan:
                return ValueMoreThan(currentValue, checkValue);
            case StringEventConditionType.LessThan:
                return ValueLessThan(currentValue, checkValue);
            default:
                Debug.LogWarning("GameEvents could not find a suitable event");
                return false;
        }
    }

    public bool ConditionBoolCheck(bool currentValue, StringEventConditionType stringEventConditionType){
        switch(stringEventConditionType){
            case StringEventConditionType.True:
                return ValueBool(currentValue, true);
            case StringEventConditionType.False:
                return ValueBool(currentValue, false);
            default:
                Debug.LogWarning("GameEvents could not find a suitable event");
                return false;
        }
    }

    private bool ValueBool(bool currentValue, bool checkValue){
        return currentValue == checkValue;
    }

    private bool ValueEquals(string currentValue, string checkValue){
        return currentValue.ToLower().Equals(checkValue.ToLower());
    }

    private bool ValueEquals(float currentValue, float checkValue){
        return currentValue == checkValue;
    }

    private bool ValueEqualsMoreThan(float currentValue, float checkValue){
        return currentValue >= checkValue;
    }

    private bool ValueEqualsLessThan(float currentValue, float checkValue){
        return currentValue <= checkValue;
    }

    private bool ValueMoreThan(float currentValue, float checkValue){
        return currentValue > checkValue;
    }

    private bool ValueLessThan(float currentValue, float checkValue){
        return currentValue < checkValue;
    }
}
