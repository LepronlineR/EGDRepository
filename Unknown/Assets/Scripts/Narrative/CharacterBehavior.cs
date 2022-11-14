using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(DialogueParser), typeof(BoxCollider2D))]
public class CharacterBehavior : MonoBehaviour {

    [SerializeField] Transform player;

    DialogueParser parser;

    void Start(){
        parser = GetComponent<DialogueParser>();
    }

    void Update(){
        if(player != null){
            Vector3 rot = Quaternion.LookRotation(player.position - transform.position).eulerAngles;
            rot.x = 0;
            transform.rotation = Quaternion.Euler(rot);
        }
    }

    
    private void OnMouseEnter() {
        Debug.Log("The cursor entered Mouse.");
        // parser.MakeInteractable();
    }

    private void OnMouseExit() {
        Debug.Log("The cursor exited Mouse.");
        // parser.MakeUninteractable();
    }
}
