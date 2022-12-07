using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterTutorialScriptedEvents : MonoBehaviour
{
    public GameObject tutorialPerson;
    public List<GameObject> peoples;
    public DoorTrigger door;
    public AudioSource aud;

    public GameObject startGameSE;

    [SerializeField] float waitTime;

    bool once = true;

    public void StartEvents()
    {
        StartCoroutine("EndTutorialEvents");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (once)
        {
            once = false;
            StartCoroutine(EndTutorialEvents());
        }
    }

    public void EndTutorial() { }

    IEnumerator EndTutorialEvents()
    {
        // lock auditorium door
        door.LockDoor();
        door.ForceClose();

        yield return new WaitForSeconds(waitTime);

        tutorialPerson.SetActive(false);

        // play crash sound
        aud.Play();

        // turn off/on evidences
        EvidenceContainer.Instance.TurnOnEvidences();
        EvidenceContainer.Instance.TurnOffEvidences();

        door.UnlockDoor();
        startGameSE.SetActive(true);
    }

}
