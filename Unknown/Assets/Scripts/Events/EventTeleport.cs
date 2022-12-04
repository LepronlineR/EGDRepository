using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTeleport : MonoBehaviour
{
    public Transform where;
    public int ID;

    void Start() {
        GameEvents.Instance.TeleportAction += Teleport;
    }

    //private void OnEnable() {
    //    GameEvents.Instance.Teleport += Teleport;
    //}

    private void OnDestroy() {
        GameEvents.Instance.TeleportAction -= Teleport; 
    }

    //private void OnDisable() {
    //    GameEvents.Instance.Teleport -= Teleport; 
    //}

    public void Teleport(int ID, PersonName person){
        if(this.ID == ID && this.GetComponent<CharacterBehavior>().Person == person){
            this.transform.position = where.position;
        }
    }
}
