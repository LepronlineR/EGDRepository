using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue Door Event", menuName = "Dialogue Event/Dialogue Door Event", order = 0)]
public class DialogueEventDoorSO : DialogueEventSO
{
    [SerializeField] int ID;
    [SerializeField] bool openDoor;

    public override void RunEvent() {
        GameEvents.Instance.Door(ID, openDoor);
    }
}