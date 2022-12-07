using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameScriptedEvents : MonoBehaviour
{
    public GameObject canvas;
    public List<GameObject> peoples;
    public Animation anim;

    [SerializeField] float waitTime;

    bool once = true;

    private void OnTriggerEnter(Collider other)
    {
        if (once)
        {
            once = false;
            StartCoroutine(StartGameEvents());
        }
    }

    public void EndTutorial() { }

    IEnumerator StartGameEvents()
    {
        yield return new WaitForSeconds(waitTime);

        canvas.SetActive(true);
        anim.Play();

        foreach (GameObject people in peoples)
        {
            // show people
            people.SetActive(true);
        }


        MainSystem.Instance.startedGame = true;
        yield return null;
    }
}
