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

    [SerializeField] GameObject InventoryCanvas;
    [SerializeField] GameObject CameraCanvas;
    [SerializeField] PlayerAiming playerAim;

    void Start(){
        inventoryOn = false;
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Q)){
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
