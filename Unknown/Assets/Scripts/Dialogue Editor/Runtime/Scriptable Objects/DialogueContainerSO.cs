using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Dialogue Container", menuName = "Dialogue/Dialogue Graph", order = 0)]
[System.Serializable]
public class DialogueContainerSO : ScriptableObject {

}

[System.Serializable]
public class LanguageGeneric<T> {
    public LanguageType LanguageType;
    public T LanguageGenericType;
}

[System.Serializable]
public class DialogueNodePort {
    public string InputGrid;
    public string OutputGrid;
    public Port myPort;
    public TextField TextField;
    public List<LanguageGeneric<string>> TextLanguages = new List<LanguageGeneric<string>>();
}