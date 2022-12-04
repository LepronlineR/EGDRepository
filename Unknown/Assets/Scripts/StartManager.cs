using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    [SerializeField] SceneManagerMult sceneManager;
    public bool startGame = false;

    void Start() {
        startGame = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.S) || startGame == true){
            sceneManager.LoadRandomStartingScene();
        }
    }
}
