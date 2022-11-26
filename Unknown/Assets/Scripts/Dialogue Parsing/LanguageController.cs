using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageController : MonoBehaviour
{
    [SerializeField] LanguageType language;

    public static LanguageController Instance { get; private set; }

    public LanguageType Language { get => language; set => language = value; }

    private void Awake() { 
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) { 
            Destroy(this);
        } else { 
            Instance = this; 
        } 
    }
}
