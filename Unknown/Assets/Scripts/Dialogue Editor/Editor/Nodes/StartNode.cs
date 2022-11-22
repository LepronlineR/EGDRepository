using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class StartNode : BaseNode {
    
    private StartData startData = new StartData();
    public StartData StartData { get => startData; set => startData = value; }

    public StartNode(){ }

    public StartNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView){
        
        editorWindow = _editorWindow;
        graphView = _graphView;

        StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Nodes/StartNodeStyleSheet");
        styleSheets.Add(styleSheet);
        
        title = "Begin";
        SetPosition(new Rect(_position, defaultNodeSize));
        nodeGuid = Guid.NewGuid().ToString();

        AddOutputPort("Output", Port.Capacity.Single);

        TextLine();

        RefreshExpandedState();
        RefreshPorts();
    }

    public void TextLine(){
        // Make Container Box
        Box boxContainer = new Box();
        boxContainer.AddToClassList("TextLineBox");

        // Text
        TextField textField = GetNewTextFieldTextLanguage(startData.text, "Text", "TextBox");
        startData.textField = textField;
        boxContainer.Add(textField);

        // Reaload the current selected language
        ReloadLanguage();

        mainContainer.Add(boxContainer);
    }

    public override void LoadValueIntoField(){
        if (startData.textField != null){
            foreach(LanguageGeneric<string> lg in startData.text){
                startData.textField.SetValueWithoutNotify(lg.languageGenericType);
            }
        }
    }
}
