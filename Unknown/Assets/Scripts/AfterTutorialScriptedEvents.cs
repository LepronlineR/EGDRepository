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
            StartCoroutine("EndTutorialEvents");
        }
    }

    public void EndTutorial() { }

    IEnumerable EndTutorialEvents()
    {
        Debug.LogWarning("Start events");
        // lock auditorium door
        door.LockDoor();
        Debug.LogWarning("locked door");
        door.ForceClose();
        Debug.LogWarning("forced close");

        yield return new WaitForSeconds(1.0f);

        Debug.LogWarning("triggered");
        tutorialPerson.SetActive(false);

        // play crash sound
        aud.Play();

        // turn off/on evidences
        EvidenceContainer.Instance.TurnOnEvidences();
        EvidenceContainer.Instance.TurnOffEvidences();

        foreach (GameObject people in peoples)
        {
            // show people
            people.SetActive(true);
        }
    }
}
