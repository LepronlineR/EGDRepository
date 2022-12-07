using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fragsurf.Movement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    [SerializeField] SceneManagerMult sm;

    [Header("Main Canvases")]
    [SerializeField] GameObject cameraModeObject;
    [SerializeField] GameObject speechModeObject;

    [Header("Inventory/Images")]
    private InventoryImage selectedImage;
    [SerializeField] GameObject selectedImageHolder;
    [SerializeField] TMP_Text selectedTextName;
    [SerializeField] Image selectedForImage;

    [Header("Recording")]
    private AudioSource audioSource;
    private float timePassed = 0.0f;
    [SerializeField] TMP_Text word_text;
    [SerializeField] Image recordingFillArea;
    [SerializeField] TMP_Text recordingTime;
    [SerializeField] Slider recordingSlider;
    [SerializeField] [Range(0.0f, 5.0f)] float timeToStopRecording = 1.0f;
    [SerializeField] [Range(4.0f, 10.0f)] float timeToEndRecording = 5.0f;

    [Header("Dialogue")]
    [SerializeField] GameObject leftBubble;
    [SerializeField] GameObject rightBubble;
    [SerializeField] Transform[] bubblePositions = new Transform[6];
    private bool[] bubbleTakenPositions = new bool[6];
    private List<GameObject> generatedBubbles = new List<GameObject>();

    [Header("Emotion")]
    [SerializeField] GameObject emotionDot;

    [Header("Gameplay")]
    public string playerWord;
    public List<GameObject> playerEvidence;
    public DialogueEmotionType playerEmotion;
    //
    public string PlayerWord { get => playerWord; set => playerWord = value; }
    public List<GameObject> PlayerEvidence { get => playerEvidence; set => playerEvidence = value; }
    public DialogueEmotionType PlayerEmotion { get => playerEmotion; set => playerEmotion = value; }

    [Header("Tutorial")]
    public bool beginThoughtBubbles = false;
    public GameObject ATSE;
    public List<DialogueContainerSO> horrorDialogue;
    public List<DialogueContainerSO> stickDialogue;
    public List<DialogueContainerSO> picassoDialogue;
    public bool startedGame = false;

    [Header("People")]
    public List<GameObject> peoples;

    void Start(){
        inventoryOn = false;
        speechMode = true;
        cameraModeObject.SetActive(!speechMode);
        speechModeObject.SetActive(speechMode);
        audioSource = GetComponent<AudioSource>();
        recordingSlider.maxValue = timeToEndRecording;
        selectedImageHolder.SetActive(false);

        for(int x = 0; x < bubbleTakenPositions.Length; x++){
            bubbleTakenPositions[x] = false;
        }

        ATSE.SetActive(false);
    }

    #region Tutorial

    public void StartTutorial() {
        foreach(GameObject people in peoples){
            // remove current (before tutorial) dialogue
            people.GetComponent<DialogueController>().RemoveAllDialogues();
            // remove their dialogue trees and add new ones
            switch (people.GetComponent<CharacterBehavior>().Person){
                case PersonName.Horror:
                    people.GetComponent<DialogueController>().AddDialogue(horrorDialogue);
                    break;
                case PersonName.Stick:
                    people.GetComponent<DialogueController>().AddDialogue(stickDialogue);
                    break;
                case PersonName.Picasso:
                    people.GetComponent<DialogueController>().AddDialogue(picassoDialogue);
                    break;
            }
            // hide people away
            people.SetActive(false);
        }
    }

    public void EndTutorial() {
        ATSE.SetActive(true);
    }

    public void EndGame()
    {
        sm.LoadEnd();
    }

    public void TrueEndGame()
    {
        sm.LoadTrueEnd();
    }

    #endregion

    #region Inventory Images    

    public void RemoveImagePicture(){
        selectedImageHolder.SetActive(false);
    }

    public void EnableImagePicture(){
        selectedImageHolder.SetActive(true);
    }

    public void ChangeTextForSetCurrentImage(string text){
        selectedTextName.text = text;
    }

    #endregion

    #region Emotion

    private Vector3 SadVector = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 AngryVector = new Vector3(100.0f, 0.0f, 0.0f);
    private Vector3 FearVector = new Vector3(200.0f, 0.0f, 0.0f);
    private Vector3 HappyVector = new Vector3(300.0f, 0.0f, 0.0f);
    private Vector3 NeutralVector = new Vector3(400.0f, 0.0f, 0.0f);

    public void ChangeEmotion(){
        switch(playerEmotion){
            case DialogueEmotionType.Sad:
                StartCoroutine(LerpPosition(emotionDot.transform, SadVector, 1));
                word_text.color = new Color(0.67f, 0.84f, 0.9f);
                break;
            case DialogueEmotionType.Angry:
                StartCoroutine(LerpPosition(emotionDot.transform, AngryVector, 1));
                word_text.color = Color.red;
                break;
            case DialogueEmotionType.Fear:
                StartCoroutine(LerpPosition(emotionDot.transform, FearVector, 1));
                word_text.color = new Color(0.5f, 0.0f, 0.0f);
                break;
            case DialogueEmotionType.Happy:
                StartCoroutine(LerpPosition(emotionDot.transform, HappyVector, 1));
                word_text.color = Color.green;
                break;
            case DialogueEmotionType.Neutral:
                StartCoroutine(LerpPosition(emotionDot.transform, NeutralVector, 1));
                word_text.color = Color.white;
                break;
            default:
                break;
        }
    }

    IEnumerator LerpPosition(Transform objectToMove, Vector3 targetPosition, float duration) {
        float time = 0;
        Vector3 startPosition = objectToMove.localPosition;
        while (time < duration)
        {
            objectToMove.localPosition = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        objectToMove.localPosition = targetPosition;
    }

    #endregion

    #region Voice Recording

    void BeginRecordingVoice(){
        // Reset processes
        audioSource.Stop();
        // word_text.text = "";
        recordingSlider.value = 0.0f;
        recordingFillArea.color = Color.red;
        recordingTime.text = string.Format("{0:0}:{1:00}", 0, 0);
        timePassed = 0.0f;

        // Begin recording process
        DictationEngine.Instance.StartDictation();
        audioSource.clip = DictationEngine.Instance.StartRecording();

        // 
    }

    void UpdateRecording(){
        int min = Mathf.FloorToInt(timePassed / 60.0f);
        int sec = Mathf.FloorToInt(timePassed - min * 60f);

        recordingSlider.value = timePassed;
        recordingTime.text = string.Format("{0:0}:{1:00}", min, sec);

        // if a certain time is passed swap the color from red to green
        if(timePassed >= timeToStopRecording){
            recordingFillArea.color = Color.green;
        }
    }

    void EndRecording(){
        // End recording process
        startedRecording = false;
        DictationEngine.Instance.EndDictation();

        AudioClip clip = DictationEngine.Instance.StopRecording(audioSource, null);
        byte[] bytes = SavWav.GetByteFromClip(clip);

        // Perform prediction
        PredictionClient.Instance.Predict(bytes);

        timePassed = 0.0f;
    }

    #endregion

    #region Get/Setters

    public bool CurrentImageIsSelectedImage(InventoryImage img){
        return selectedImage == img;
    }

    public void SetCurrentImage(InventoryImage img){
        // deselect previous image
        if(selectedImage != null)
            selectedImage.DelectThisImage();
        EnableImagePicture();
        // select new img
        selectedTextName.text = img.Input.text;
        selectedForImage.sprite = img.Img.sprite;
        selectedImage = img;
        // clear evidence and set that for the image
        this.playerEvidence = img.Evidences;
    }

    public bool ImageContainsEvidence(GameObject evidence){
        if(evidence == null || selectedImage == null)
            return false;
        return selectedImage.ContainsEvidence(evidence);
    }

    public string GetPlayerWord(){
        return playerWord;
    }

    public void SetPlayerEmotion(string text){
        if(text.ToLower().Equals("angry")){
            playerEmotion = DialogueEmotionType.Angry;
        } else if(text.ToLower().Equals("sad")){
            playerEmotion = DialogueEmotionType.Sad;
        } else if(text.ToLower().Equals("fear")){
            playerEmotion = DialogueEmotionType.Fear;
        } else if(text.ToLower().Equals("neutral")){
            playerEmotion = DialogueEmotionType.Neutral;
        } else if(text.ToLower().Equals("happy")){
            playerEmotion = DialogueEmotionType.Happy;
        } else {
            playerEmotion = DialogueEmotionType.Otherwise;
        }
    }

    public string GetPlayerEmotion() {
        switch(playerEmotion){
            case DialogueEmotionType.Angry:
                return "angry";
            case DialogueEmotionType.Sad:
                return "sad";
            case DialogueEmotionType.Fear:
                return "fear";
            case DialogueEmotionType.Neutral:
                return "neutral";
            case DialogueEmotionType.Happy:
                return "happy";
            case DialogueEmotionType.Otherwise:
                return "";
        }
        return "";
    }

    #endregion

    #region Thought Bubbles

    private int FindEmptyBubble(){
        for(int x = 0; x < bubbleTakenPositions.Length; x++){
            if(!bubbleTakenPositions[x])
                return x;
        }
        return -1;
    }

    public void GenerateBubbleText(string text){
        if(!beginThoughtBubbles)
            return;
        int whereBubble = FindEmptyBubble();
        if(whereBubble < 0)
            return;
        bubbleTakenPositions[whereBubble] = true;
        if(whereBubble >= 0){
            if(whereBubble % 2 != 0){
                GameObject bubble = (GameObject) Instantiate(leftBubble, bubblePositions[whereBubble].position, Quaternion.identity);
                bubble.GetComponent<ThoughtBubble>().SetText(text);
                bubble.transform.SetParent(bubblePositions[whereBubble]);
                bubble.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                generatedBubbles.Add(bubble);
            } else {
                GameObject bubble = (GameObject) Instantiate(rightBubble, bubblePositions[whereBubble].position, Quaternion.identity);
                bubble.GetComponent<ThoughtBubble>().SetText(text);
                bubble.transform.SetParent(bubblePositions[whereBubble]);
                bubble.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                generatedBubbles.Add(bubble);
            }
        }
        // error
    }

    public void RemoveAllBubbles(){
        foreach(GameObject go in generatedBubbles){
            Destroy(go);
        }
        for(int x = 0; x < bubbleTakenPositions.Length; x++){
            bubbleTakenPositions[x] = false;
        }
    }

    #endregion

    void Update() { // usually note safe to do

        // turn on and off speech mode
        if(Input.GetKeyDown(KeyCode.Tab)){
            if(startedRecording){ // turn off recording
                startedRecording = false;
                EndRecording();
            }
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
                    playerAim.on = false;
                    Cursor.visible = true; 
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }

        if(speechMode){

            if(Input.GetKeyDown(KeyCode.R)){
                if(!startedRecording){
                    startedRecording = true;
                    BeginRecordingVoice();
                } else if(timePassed >= timeToStopRecording){
                    startedRecording = false;
                    EndRecording();
                }
            }

            // Recording audio
            if(startedRecording){
                timePassed += Time.deltaTime;
                UpdateRecording();
            }

            if(timePassed >= timeToEndRecording){ // End recording
                EndRecording();
            }
        } else {
            if(Input.GetKeyDown(KeyCode.Q)){
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

        // quit game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            sm.Quit();
        }
    }
}
