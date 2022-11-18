using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class DialogueEditorWindow : EditorWindow {

    private DialogueContainerSO currentDialogueContainer;
    private DialogueGraphView graphView;
    private DialogueSaveAndLoad saveAndLoad;

    private LanguageType languageType = LanguageType.English;
    private ToolbarMenu languagesDropdownMenu;
    private Label nameOfDialogueContainer;

    public LanguageType LanguageType { get => languageType; set => languageType = value; }

    [OnOpenAsset(1)]
    private static bool ShowWindow(int _instanceID, int line) {
        UnityEngine.Object item = EditorUtility.InstanceIDToObject(_instanceID);
        if(item is DialogueContainerSO){
            DialogueEditorWindow window = (DialogueEditorWindow) GetWindow(typeof(DialogueEditorWindow));
            window.titleContent = new GUIContent("Dialogue Editor");
            window.currentDialogueContainer = item as DialogueContainerSO;
            window.minSize = new Vector2(500, 250);
            window.Load();
        }
        return false;
    }

    private void OnEnable() {
        ConstructGraphView();
        GenerateToolbar();
        Load();
    }

    private void OnDisable() {
        rootVisualElement.Remove(graphView);
    }

    private void ConstructGraphView() {
        graphView = new DialogueGraphView(this);
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);

       //  saveAndLoad = new DialogueSaveAndLoad(graphView);
    }

    private void GenerateToolbar() {
        StyleSheet styleSheet = Resources.Load<StyleSheet>("GraphViewStyleSheet");
        rootVisualElement.styleSheets.Add(styleSheet);

        Toolbar toolbar = new Toolbar();

        // save button
        Button saveButton = new Button() { text = "Save" };
        saveButton.clicked += () => { Save(); };
        toolbar.Add(saveButton);

        // load button
        Button loadButton = new Button() { text = "Load" };
        loadButton.clicked += () => { Load(); };
        toolbar.Add(loadButton);
        
        // downdown menu
        languagesDropdownMenu = new ToolbarMenu();
        foreach(LanguageType language in (LanguageType[])Enum.GetValues(typeof(LanguageType))){
            languagesDropdownMenu.menu.AppendAction(language.ToString(), 
                new Action<DropdownMenuAction>(x => Language(language)));
        }
        toolbar.Add(languagesDropdownMenu);

        // name of current dialogue container you have open
        nameOfDialogueContainer = new Label("");
        toolbar.Add(nameOfDialogueContainer);
        nameOfDialogueContainer.AddToClassList("nameOfDialogueContainer");

        rootVisualElement.Add(toolbar);
    }

    private void Load() {
        if(currentDialogueContainer != null){
            Language(LanguageType.English);
            nameOfDialogueContainer.text = "File Name: " + currentDialogueContainer.name;
            // saveAndLoad.Load(currentDialogueContainer);
        }
    }

    private void Save() {
        //if(currentDialogueContainer != null)
            //saveAndLoad.Save(currentDialogueContainer);
    }

    private void Language(LanguageType language){
        languagesDropdownMenu.text = "Language: " + language.ToString();
        languageType = language;
        graphView.LanguageReload();
    }
}