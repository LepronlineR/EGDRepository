using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue Door Lock Event", menuName = "Dialogue Event/Dialogue Door Lock Event", order = 0)]
public class DialogueEventDoorLockSO : DialogueEventSO
{
    [SerializeField] int ID;
    [SerializeField] bool lockDoor;

    public override void RunEvent() {
        GameEvents.Instance.DoorLock(ID, lockDoor);
    }
}
 