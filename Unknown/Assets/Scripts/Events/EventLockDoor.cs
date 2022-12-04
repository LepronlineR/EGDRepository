using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventLockDoor : MonoBehaviour
{
    public int ID;
    public DoorTrigger door;

    void Start() {
        door = this.GetComponent<DoorTrigger>();
        GameEvents.Instance.DoorLockAction += DoorLock;
    }

    //private void OnEnable() {
    //    GameEvents.Instance.DoorAction += Door;
    //}

    private void OnDestroy() {
        GameEvents.Instance.DoorLockAction -= DoorLock; 
    }

    //private void OnDisable() {
    //    GameEvents.Instance.DoorAction -= Door; 
    //}

    public void DoorLock(int ID, bool lockDoor){
        if(this.ID == ID){
            door.lockState = lockDoor;
        }
    }
}
