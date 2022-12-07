using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterTutorialScriptedEvents : MonoBehaviour
{

    public List<GameObject> peoples;
    public List<DialogueController> dialogueControllers;

    public void StartEvents()
    {
        StartCoroutine("Events");
    }

    public void EndTutorial() { }

    IEnumerable Events()
    {
        // lock auditorium door
        yield return new WaitForSeconds(20.0f);
        foreach(GameObject go in peoples)
        {
            go.SetActive(true);
        }

        // play crash sound

        // turn off/on evidences
        EvidenceContainer.Instance.TurnOnEvidences();
        EvidenceContainer.Instance.TurnOffEvidences();
    }
}
