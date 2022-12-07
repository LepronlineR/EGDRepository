using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] List<Door> doors;
    [SerializeField] GameObject player;

    public bool lockState = false;

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player" && !lockState){
            foreach(Door door in doors){
                if(!door.isOpen){
                    door.Open(other.transform.position);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.tag == "Player" && !lockState){
            foreach(Door door in doors){
                if(door.isOpen){
                    door.Close();
                }
            }
        }
    }

    public void ForceOpen() {
        foreach(Door door in doors){
            if(!door.isOpen){
                door.Open(player.transform.position);
            }
        }
    }

    public void ForceClose() {
        foreach(Door door in doors){
            if(door.isOpen){
                door.Close();
            }
        }
    }

    public void LockDoor() { lockState = true; }
    public void UnlockDoor() { lockState = false; }
}
