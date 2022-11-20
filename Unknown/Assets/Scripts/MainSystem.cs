using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fragsurf.Movement;
using TMPro;

public class MainSystem : MonoBehaviour
{
    public static MainSystem Instance { get; private set; }
    private void Awake() { 
        // If there is an instance, and it's not me, delete myself.
        
        if (Instance != null && Instance != this) { 
            Destroy(this); 
        } else { 
            Instance = this; 
        } 
    }

    bool inventoryOn;
    bool speechMode;
    bool startedRecording;

    [SerializeField] GameObject InventoryCanvas;
    [SerializeField] GameObject CameraCanvas;
    [SerializeField] PlayerAiming playerAim;
    [SerializeField] SurfCharacter playerMove;

    [Header("Main Canvases")]
    [SerializeField] GameObject cameraModeObject;
    [SerializeField] GameObject speechModeObject;

    [Header("Inventory/Images")]
    private InventoryImage selectedImage;

    [Header("Recording")]
    private AudioSource audioSource;
    [SerializeField] GameObject audioImage;
    [SerializeField] TMP_Text word_text;
    [SerializeField] TMP_Text emotion_text;

    [Header("Gameplay")]
    private string playerWord;
    private List<GameObject> playerEvidence;
    private string playerEmotion;
    //
    public string PlayerWord { get => playerWord; set => playerWord = value; }
    public List<GameObject> PlayerEvidence { get => playerEvidence; set => playerEvidence = value; }
    public string PlayerEmotion { get => playerEmotion; set => playerEmotion = value; }

    public void SetCurrentImage(InventoryImage img){
        selectedImage = img;
    }

    public bool ImageContainsEvidence(GameObject evidence){
        if(evidence == null || selectedImage == null)
            return false;
        return selectedImage.ContainsEvidence(evidence);
    }

    void Start(){
        inventoryOn = false;
        speechMode = true;
        cameraModeObject.SetActive(!speechMode);
        speechModeObject.SetActive(speechMode);
        audioSource = GetComponent<AudioSource>();
    }

    public string GetPlayerWord(){
        return playerWord;
    }

    public string GetPlayerEmotion() {
        return playerEmotion;
    }

    void Update() {

        // turn on and off speech mode
        if(Input.GetKeyDown(KeyCode.Tab)){
            speechMode = !speechMode;
            if(speechMode){
                cameraModeObject.SetActive(!speechMode);
                speechModeObject.SetActive(speechMode);
                if(inventoryOn){ // get out of inventory
                    playerMove.StartMovement();
                    playerAim.on = true;
                    Cursor.visible = false; 
                    Cursor.lockState = CursorLockMode.Locked;
                }
            } else {
                cameraModeObject.SetActive(!speechMode);
                speechModeObject.SetActive(speechMode);
                if(inventoryOn){ // get in of inventory
                    playerMove.StopMovement();
                    playerAim.on = true;
                    Cursor.visible = true; 
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }

        // recording 
        if(speechMode && Input.GetKeyDown(KeyCode.E)){
            if(!startedRecording){ // Begin recording
                startedRecording = true;

                // Begin recording process
                DictationEngine.Instance.StartDictation();
                audioImage.SetActive(true);
                audioSource.Stop();
                word_text.text = "";
                emotion_text.text = "";
                audioSource.clip = DictationEngine.Instance.StartRecording();
            } else { // End recording
                startedRecording = false;

                // End recording process
                DictationEngine.Instance.EndDictation();
                audioImage.SetActive(false);

                AudioClip clip = DictationEngine.Instance.StopRecording(audioSource, null);
                byte[] bytes = SavWav.GetByteFromClip(clip);

                // Perform prediction
                PredictionClient.Instance.Predict(bytes);

                // Resulting word
                playerWord = DictationEngine.Instance.GetSentence();
            }
        }

        // inventory
        if(!speechMode && Input.GetKeyDown(KeyCode.Q)){
            inventoryOn = !inventoryOn;
            if(inventoryOn){ // turn on inventory
                playerMove.StopMovement();
                playerAim.on = false;
                InventoryCanvas.SetActive(true);
                CameraCanvas.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true; 
            } else { // turn off inventory
                playerMove.StartMovement();
                playerAim.on = true;
                InventoryCanvas.SetActive(false);
                CameraCanvas.SetActive(true);
                Cursor.visible = false; 
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}
