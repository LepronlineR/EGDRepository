using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider), typeof(DialogueController))]
public class CharacterBehavior : MonoBehaviour {

    [SerializeField] Transform player;

    DialogueController parser;

    void Start(){
        parser = GetComponent<DialogueController>();
    }

    void Update(){
        if(player != null){
            Vector3 rot = Quaternion.LookRotation(player.position - transform.position).eulerAngles;
            rot.x = 0;
            transform.rotation = Quaternion.Euler(rot);
        }
    }

    
    private void OnMouseEnter() {
        // Debug.Log("The cursor entered Mouse.");
        parser.MakeInteractable(true);
        if(parser.HasNotBegan()){
            List<string> allTexts = parser.GetAllStartNodeTexts();
            // TODO: make this appear as UI elements in front of the player
            foreach(string texts in allTexts){
                MainSystem.Instance.GenerateBubbleText(texts);
            }
        }
    }

    private void OnMouseExit() {
        // Debug.Log("The cursor exited Mouse.");
        parser.MakeInteractable(false);
        if(parser.HasNotBegan()){
            // TODO: fade this away and delete
            MainSystem.Instance.RemoveAllBubbles();
        }
    }
}
