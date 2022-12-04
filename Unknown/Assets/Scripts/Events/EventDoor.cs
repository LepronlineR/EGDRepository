using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDoor : MonoBehaviour
{
    public int ID;
    public DoorTrigger door;

    void Start() {
        door = this.GetComponent<DoorTrigger>();
        GameEvents.Instance.DoorAction += Door;
    }

    //private void OnEnable() {
    //    GameEvents.Instance.DoorAction += Door;
    //}

    private void OnDestroy() {
        GameEvents.Instance.DoorAction -= Door; 
    }

    //private void OnDisable() {
    //    GameEvents.Instance.DoorAction -= Door; 
    //}

    public void Door(int ID, bool open){
        if(this.ID == ID){
            if(open){
                door.ForceOpen();
            } else {
                door.ForceClose();
            }
        }
    }
}
