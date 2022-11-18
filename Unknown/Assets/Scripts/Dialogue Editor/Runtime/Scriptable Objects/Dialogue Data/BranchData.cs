using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BranchData : BaseData
{
    public string trueGuidNode;
    public string falseGuidNode;
    public List<EventDataStringCondition> eventDataStringConditions = new List<EventDataStringCondition>();
}
