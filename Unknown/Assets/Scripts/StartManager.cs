using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    [SerializeField] SceneManagerMult sceneManager;
    [SerializeField] bool startingScene = false;
    [SerializeField] bool trueEnd = false;
    public bool startGame = false;

    void Start() {
        startGame = false;
        Cursor.visible = true;
        if(!trueEnd)
            Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        if((Input.GetKey(KeyCode.S) || startGame == true) && startingScene){
            sceneManager.LoadRandomStartingScene();
        }
        if ((Input.GetKey(KeyCode.G) || startGame == true) && !startingScene)
        {
            sceneManager.LoadStart();
        }
    }
}
