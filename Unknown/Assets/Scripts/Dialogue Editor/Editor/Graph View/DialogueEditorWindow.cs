using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogueEditorWindow : EditorWindow {

    private DialogueContrainerSO currentDialogueContainer;
    private DialogueGraphView graphView;

    [OnOpenAsset(1)]
    private static bool ShowWindow(int _instanceID, int line) {
        UnityEngine.Object item = EditorUtility.InstanceIDToObject(_instanceID);
        if(item is DialogueContrainerSO){
            DialogueEditorWindow window = (DialogueEditorWindow) GetWindow(typeof(DialogueEditorWindow));
            window.titleContent = new GUIContent("Dialogue Editor");
            window.currentDialogueContainer = item as DialogueContrainerSO;
            window.minSize = new Vector2(500, 250);
            window.Load();
        }
    }

    [MenuItem("Unknown/DialogueWindow")]
    private static void ShowWindow() {
        var window = GetWindow<DialogueEditorWindow>();
        window.titleContent = new GUIContent("DialogueWindow");
        window.Show();
    }

    private void OnEnable() {
        
    }

    private void OnDisable() {
        
    }

    private void OnGUI() {
        
    }

    private void ConstructGraphView() {

    }

    public void GenerateToolbar() {

    }

    private void Load() {
        
    }

    private void Save() {

    }

    private void Language(){

    }
}