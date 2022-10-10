using UnityEngine;

[CreateAssetMenu(fileName = "DialogueCharacter", menuName = "Dialogue/DialogueCharacter", order = 0)]
public class DialogueCharacter : ScriptableObject {
    
    public string name;
    // public AudioClip voice;
    // public Sprite portrait;

    [TextArea] public string details;
}
