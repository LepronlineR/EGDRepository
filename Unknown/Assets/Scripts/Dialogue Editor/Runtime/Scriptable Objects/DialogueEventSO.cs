using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue Event", menuName = "Dialogue Event/Dialogue Event", order = 0)]

[System.Serializable]
public class DialogueEventSO : ScriptableObject {

    public virtual void RunEvent() {
        Debug.Log("Run Event");
    }
}
