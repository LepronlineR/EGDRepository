using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerMult : MonoBehaviour
{

    public void LoadMainScene() {
        SceneManager.LoadScene("Main");
    }

    public void LoadRandomStartingScene() {
        int index = Random.Range(1, 3);
        SceneManager.LoadScene(index);
    }

    public void LoadStart()
    {
        SceneManager.LoadScene("Start");
    }
    
    public void Quit() {
        Application.Quit();
    }

    public void LoadEnd()
    {
        SceneManager.LoadScene("End");
    }

    public void LoadTrueEnd()
    {
        SceneManager.LoadScene("TrueEnd");
    }

}
