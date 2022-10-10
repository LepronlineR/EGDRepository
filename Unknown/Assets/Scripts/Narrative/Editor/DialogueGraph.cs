using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraph : EditorWindow
{
    private DialogueGraphView _graphView;
    private string _fileName = "Dialogue Tree";
    private DialogueContainer _dialogueContainer;


    [MenuItem("Dialogue/Dialogue Graph")]
    public static void OpenDialogueGraph() {
        var window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }

    private void OnEnable() {
        ConstructGraph();
        GenerateToolbar();
        GenerateMiniMap();
        GenerateBlackBoard();
    }

    private void OnDisable() {
        rootVisualElement.Remove(_graphView);
    }

    private void GenerateToolbar(){
        var toolbar = new Toolbar();

        var fileNameTextField = new TextField("File Name:");
        fileNameTextField.SetValueWithoutNotify("Dialogue Tree");
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
        toolbar.Add(fileNameTextField);

        toolbar.Add(new Button(()=> RequestOperation(true)){text = "Save"});
        toolbar.Add(new Button(()=> RequestOperation(false)){text = "Load"});

        var nodeCreateButton = new Button(()=> {
            _graphView.CreateNode("Dialogue Node", Vector2.zero);
        });
        //nodeCreateButton.text = "Create Node";
        //toolbar.Add(nodeCreateButton);

        rootVisualElement.Add(toolbar);
    }

    private void RequestOperation(bool save){
        if(string.IsNullOrEmpty(_fileName)){
            EditorUtility.DisplayDialog("Invalid file name.", "Enter a valid file name.", "Ok");
            return;
        }
        var saveUtil = GraphSaveUtil.GetInstance(_graphView);
        if(save)
            saveUtil.Save(_fileName);
        else
            saveUtil.Load(_fileName);
    }

    private void ConstructGraph(){
        _graphView = new DialogueGraphView(this) {
            name = "Dialogue Graph"
        };
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    //  noit working
    private void GenerateMiniMap() {
        var miniMap = new MiniMap {anchored = true};
        var cords = _graphView.contentViewContainer.WorldToLocal(new Vector2(this.maxSize.x - 10, 30));
        miniMap.SetPosition(new Rect(cords.x, cords.y, 200, 140));
        _graphView.Add(miniMap);
    }

    private void GenerateBlackBoard() {
        var blackboard = new Blackboard(_graphView);
        blackboard.Add(new BlackboardSection {title = "Exposed Variables"});
        blackboard.addItemRequested = _blackboard => {
            _graphView.AddPropertyToBlackBoard(ExposedProperty.CreateInstance(), false);
        };
        blackboard.editTextRequested = (_blackboard, element, newValue) => {
            var oldPropertyName = ((BlackboardField) element).text;
            if (_graphView.ExposedProperties.Any(x => x.PropertyName == newValue)) {
                EditorUtility.DisplayDialog("Error", "This property name already exists, please chose another one.", "OK");
                return;
            }

            var targetIndex = _graphView.ExposedProperties.FindIndex(x => x.PropertyName == oldPropertyName);
            _graphView.ExposedProperties[targetIndex].PropertyName = newValue;
            ((BlackboardField) element).text = newValue;
        };
        blackboard.SetPosition(new Rect(10,30,200,300));
        _graphView.Add(blackboard);
        _graphView.Blackboard = blackboard;
    }
}
