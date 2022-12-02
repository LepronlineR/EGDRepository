using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] Door door;

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player"){
            if(!door.isOpen){
                door.Open(other.transform.position);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.tag == "Player"){
            if(door.isOpen){
                door.Close();
            }
        }
    }
}
