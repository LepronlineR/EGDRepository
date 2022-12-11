using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvidenceContainer : MonoBehaviour
{
    public static EvidenceContainer Instance { get; private set; }
    private void Awake() { 
        // If there is an instance, and it's not me, delete myself.
        
        if (Instance != null && Instance != this) { 
            Destroy(this); 
        } else { 
            Instance = this; 
        } 
    }

    public List<GameObject> evidences;

    public List<GameObject> evidencesTurnOn;

    public List<GameObject> evidencesTurnOff;

    public void TurnOnEvidences()
    {
        foreach(GameObject go in evidencesTurnOn)
        {
            go.SetActive(true);
        }
    }

    public void TurnOffEvidences()
    {
        foreach (GameObject go in evidencesTurnOff)
        {
            go.SetActive(false);
        }
    }
}
