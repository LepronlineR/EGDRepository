using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] GameObject InventoryCanvas;
    [SerializeField] GameObject CameraCanvas;
    [SerializeField] PlayerAiming playerAim;

    [Header("Main Canvases")]
    [SerializeField] GameObject cameraModeObject;
    [SerializeField] GameObject speechModeObject;

    void Start(){
        inventoryOn = false;
        speechMode = true;
        cameraModeObject.SetActive(!speechMode);
        speechModeObject.SetActive(speechMode);
    }

    void Update() {
        // turn on and off speech mode
        if(Input.GetKeyDown(KeyCode.E)){
            speechMode = !speechMode;
            if(speechMode){
                cameraModeObject.SetActive(!speechMode);
                speechModeObject.SetActive(speechMode);
                if(inventoryOn){ // get out of inventory
                    playerAim.on = true;
                    Cursor.visible = false; 
                    Cursor.lockState = CursorLockMode.Locked;
                }
            } else {
                cameraModeObject.SetActive(!speechMode);
                speechModeObject.SetActive(speechMode);
                if(inventoryOn){ // get in of inventory
                    playerAim.on = true;
                    Cursor.visible = true; 
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }

        // inventory
        if(!speechMode && Input.GetKeyDown(KeyCode.Q)){
            inventoryOn = !inventoryOn;
            if(inventoryOn){ // turn on inventory
                playerAim.on = false;
                InventoryCanvas.SetActive(true);
                CameraCanvas.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true; 
            } else { // turn off inventory
                playerAim.on = true;
                InventoryCanvas.SetActive(false);
                CameraCanvas.SetActive(true);
                Cursor.visible = false; 
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}
