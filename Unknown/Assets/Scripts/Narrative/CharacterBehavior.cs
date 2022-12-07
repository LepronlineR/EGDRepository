using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider), typeof(DialogueController))]
public class CharacterBehavior : MonoBehaviour {

    [SerializeField] Transform player;
    [SerializeField] float close = 5.0f;
    [SerializeField] PersonName personName;

    public PersonName Person { get => personName; private set => personName = value; }

    DialogueController parser;

    bool interactable = false;

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

    void BeginInteraction() {
        interactable = true;
        parser.MakeInteractable(interactable);
        if(parser.HasNotBegan()){
            List<string> allTexts = parser.GetAllStartNodeTexts();
            // TODO: make this appear as UI elements in front of the player
            foreach(string texts in allTexts){
                if(!texts.Equals(string.Empty))
                    MainSystem.Instance.GenerateBubbleText(texts);
            }
        }
    }

    void EndInteraction() {
        interactable = false;
        parser.MakeInteractable(interactable);
        if(parser.HasNotBegan()){
            // TODO: fade this away and delete
            MainSystem.Instance.RemoveAllBubbles();
        }
    }

    
    private void OnMouseEnter() {
        
    }

    private void OnMouseOver(){
        if(Vector3.Distance(this.transform.position, player.transform.position) >= close)
            return;
        // conviction
        if ((personName != PersonName.Tutorial || personName != PersonName.Player) 
            && MainSystem.Instance.startedGame
            && Input.GetKey(KeyCode.C))
        {
            MainSystem.Instance.EndGame();
        }
        if (!interactable){
            BeginInteraction();
        }
    }

    private void OnMouseExit() {
        if(interactable)
            EndInteraction();
    }
}
