using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class ChoiceNode : BaseNode {
    
    private ChoiceData choiceData = new ChoiceData();
    public ChoiceData ChoiceData { get => choiceData; set => choiceData = value; }

    private Box choiceStateEnumBox;

    public ChoiceNode() { }

    public ChoiceNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView){
        
        editorWindow = _editorWindow;
        graphView = _graphView;

        StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Nodes/ChoiceNodeStyleSheet");
        styleSheets.Add(styleSheet);
        
        title = "Choice";
        SetPosition(new Rect(_position, defaultNodeSize));
        nodeGuid = Guid.NewGuid().ToString();

        // Add standard ports.
        Port inputPort = AddInputPort("Input", Port.Capacity.Multi);
        AddOutputPort("Output", Port.Capacity.Single);

        // Set color of the port.
        inputPort.portColor = Color.yellow;

        TopButton();

        TextLine();
        ChoiceStateEnum();
    }

    private void TopButton(){
        ToolbarMenu menu = new ToolbarMenu();
        menu.text = "Add Condition";

        menu.menu.AppendAction("String Event Condition", new Action<DropdownMenuAction>(x => AddCondition()));

        titleButtonContainer.Add(menu);
    }

    public void AddCondition(EventDataStringCondition stringEvent = null){
        AddStringConditionEventBuild(choiceData.eventDataStringConditions, stringEvent);
        ShowHideChoiceEnum();
    }

    public void TextLine(){
        // Make Container Box
        Box boxContainer = new Box();
        boxContainer.AddToClassList("TextLineBox");

        // Text
        TextField textField = GetNewTextFieldTextLanguage(choiceData.text, "Text", "TextBox");
        choiceData.textField = textField;
        boxContainer.Add(textField);

        // Audio
        ObjectField objectField = GetNewObjectFieldAudioClipLanguage(ChoiceData.audioClips, "AudioClip");
        choiceData.objectField = objectField;
        boxContainer.Add(objectField);

        // Reaload the current selected language
        ReloadLanguage();

        mainContainer.Add(boxContainer);
    }

    private void ChoiceStateEnum(){
        choiceStateEnumBox = new Box();
        choiceStateEnumBox.AddToClassList("BoxRow");
        ShowHideChoiceEnum();

        // Make fields.
        Label enumLabel = GetNewLabel("If the condition is not met", "ChoiceLabel");
        EnumField choiceStateEnumField = GetNewEnumFieldChoiceStateType(choiceData.choiceStateType, "enumHide");

        // Add fields to box.
        choiceStateEnumBox.Add(choiceStateEnumField);
        choiceStateEnumBox.Add(enumLabel);

        mainContainer.Add(choiceStateEnumBox);
    }

    protected override void DeleteBox(Box boxContainer){
        base.DeleteBox(boxContainer);
        ShowHideChoiceEnum();
    }

    private void ShowHideChoiceEnum(){
        ShowHide(choiceData.eventDataStringConditions.Count > 0, choiceStateEnumBox);
    }

    public override void ReloadLanguage(){
        base.ReloadLanguage();
    }

    public override void LoadValueIntoField(){
        if (choiceData.choiceStateType.enumField != null)
                choiceData.choiceStateType.enumField.SetValueWithoutNotify(choiceData.choiceStateType.value);
    }
}
