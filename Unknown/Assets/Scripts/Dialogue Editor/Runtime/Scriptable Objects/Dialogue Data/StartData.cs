using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class StartData : BaseData
{
    #if UNITY_EDITOR
    public TextField textField { get; set; }
    #endif

    public List<LanguageGeneric<string>> text = new List<LanguageGeneric<string>>();
}
