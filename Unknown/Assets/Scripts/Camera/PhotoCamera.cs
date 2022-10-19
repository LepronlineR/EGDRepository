using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoCamera : MonoBehaviour {
    
    [Header("Photo Taker")]
    [SerializeField] UnityEngine.UI.Image photoDisplay;
    [SerializeField] GameObject frame;
    [SerializeField] GameObject player;
    [SerializeField] LayerMask PlayerLayerMask;

    [Header("Flash Effect")]
    [SerializeField] GameObject flash;
    [SerializeField] float flashDuration;
    
    [Header("Photo Fade Effect")]
    [SerializeField] Animation fadeAnimation;

    [Header("Camera Sound Effect")]
    [SerializeField] AudioSource photoSound;

    [Header("Camera UI")]
    [SerializeField] GameObject cameraUI;
    [SerializeField] UnityEngine.UI.Image photoDisplayUI;

    [Header("Inventory Stuff")]
    [SerializeField] GameObject imagePrefab;
    [SerializeField] Transform imageHolder;

    private bool view;

    void Start(){
        frame.SetActive(false);
    }

    void Update(){
        if(Input.GetMouseButtonDown(0)){
            if(view)
                StartCoroutine(RemovePicture());
            else
                StartCoroutine(TakePicture());
        }
    }



    private bool IsInView(GameObject origin, GameObject toCheck){
        Vector3 pointOnScreen = Camera.main.WorldToScreenPoint(toCheck.GetComponentInChildren<Renderer>().bounds.center);
 
        //Is in front
        if (pointOnScreen.z < 0){
            Debug.Log("Behind: " + toCheck.name);
            return false;
        }
 
        //Is in the image FOV?
        if ((pointOnScreen.x < 0) || (pointOnScreen.x > Screen.width) || 
            (pointOnScreen.y < 0) || (pointOnScreen.y > Screen.height)){
            Debug.Log("OutOfBounds: " + toCheck.name);
            return false;
        }
 
        RaycastHit hit;
        Vector3 heading = toCheck.transform.position - origin.transform.position;
        Vector3 direction = heading.normalized;// / heading.magnitude;
        
        if (Physics.Linecast(Camera.main.transform.position, toCheck.GetComponentInChildren<Renderer>().bounds.center, out hit, ~PlayerLayerMask)){
            if (hit.transform.name != toCheck.name){
                Debug.Log(toCheck.name + " occluded by " + hit.transform.name);
                return false;
            }
        }
        return true;
    }

    IEnumerator RemovePicture(){
        view = false;
        frame.SetActive(false);
        cameraUI.SetActive(true);
        yield return null;
    }

    IEnumerator CameraFlashEffect(){
        // play audio
        flash.SetActive(true);
        yield return new WaitForSeconds(flashDuration);
        flash.SetActive(false);
    }

    IEnumerator TakePicture(){
        view = true;

        // remove camera UI
        cameraUI.SetActive(false);

        yield return new WaitForEndOfFrame();

        Texture2D screencap = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
        // get image
        screencap.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
        screencap.Apply();

        // image to sprite
        Sprite photoSprite = Sprite.Create(screencap, new Rect(0.0f, 0.0f, screencap.width, screencap.height), new Vector2(0.5f, 0.5f), 100.0f);
        photoDisplay.sprite = photoSprite;
        photoDisplayUI.sprite = photoSprite;

        // is the evidence in view?
        List<GameObject> evidences = EvidenceContainer.Instance.evidences;
        foreach(GameObject evidence in evidences){
            Debug.Log(IsInView(player, evidence));
        }

        // flash effect
        StartCoroutine(CameraFlashEffect());
        fadeAnimation.Play("PhotoFadeIn");

        // play sound
        photoSound.Play();

        frame.SetActive(true);

        // add image to the inventory
        GameObject imageGO = (GameObject) Instantiate(imagePrefab, Vector3.zero, Quaternion.identity);
        imageGO.GetComponent<InventoryImage>().SetImage(photoSprite);
        imageGO.transform.SetParent(imageHolder);
        imageGO.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        

    }
}
