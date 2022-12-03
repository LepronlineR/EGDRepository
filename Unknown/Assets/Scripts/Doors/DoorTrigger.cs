using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] List<Door> doors;

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player"){
            foreach(Door door in doors){
                if(!door.isOpen){
                    door.Open(other.transform.position);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.tag == "Player"){
            foreach(Door door in doors){
                if(door.isOpen){
                    door.Close();
                }
            }
        }
    }
}
