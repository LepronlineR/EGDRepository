using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private GameObject dialogueUI;

    [Header("Text")]
    [SerializeField] private TMP_Text textName;
    [SerializeField] private TMP_Text textBox;
    [SerializeField] [Range(0.0f, 1.0f)] float interval;

    [Header("Image")]
    [SerializeField] private Image image;
    [SerializeField] private GameObject imageGO;

    // IN THIS EXACT ORDER        neutral, angry, sad, happy, fear
    [SerializeField] List<Sprite> characterSprites;

    // flags
    private bool interactable;
    private bool speaking;
    private bool skip; 

    void Awake() {
        ShowDialogue(false);
    }

    void Update() {
        if(interactable && Input.GetMouseButtonDown(0)){ // process data
            if(!speaking){

            } else {
                skip = true;
            }
        }
    }

    public void ShowDialogue(bool _show){
        dialogueUI.SetActive(_show);
    }

    public void SetText(string _name, string _text){
        textName.text = _name;
        StartCoroutine(Type(_text));
    }

    public IEnumerator Type(string word){
        textBox.text = word;
        textBox.ForceMeshUpdate();
        speaking = true;
        //audios.Play();
        //audios.loop = true;
        int totalCharacters = textBox.textInfo.characterCount;
        int count = 0;
        skip = false;
        while(count < totalCharacters+1){
            textBox.maxVisibleCharacters = count % (totalCharacters+1);
            yield return new WaitForSeconds(interval);
            count++;
            if(skip){ // skip the dialogue
                skip = false;
                textBox.maxVisibleCharacters = totalCharacters;
                break;
            }
        }
        //if(audios == null) yield break; 
        //audios.loop = false;
        speaking = false;
        yield return new WaitForSeconds(0.6f);
    }

    public void SetImage(Sprite _image, DialogueEmotionType dialogueEmotionType){
        if(_image != null){
            switch(dialogueEmotionType){
                case DialogueEmotionType.Neutral:
                    image.sprite = characterSprites[0];
                    break;
                case DialogueEmotionType.Angry:
                    image.sprite = characterSprites[1];
                    break;
                case DialogueEmotionType.Sad:
                    image.sprite = characterSprites[2];
                    break;
                case DialogueEmotionType.Happy:
                    image.sprite = characterSprites[3];
                    break;
                case DialogueEmotionType.Fear:
                    image.sprite = characterSprites[4];
                    break;
                default:
                    break;
            }
        }
    }
    
    public void SetActions(List<string> texts, List<UnityAction> actions){

    }
}
