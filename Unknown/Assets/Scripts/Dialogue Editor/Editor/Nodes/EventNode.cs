using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class EventNode : BaseNode
{

    private List<EventStringIDData> eventStringIDDatas = new List<EventStringIDData>();
    private List<EventScriptableObjectData> eventScriptableObjectDatas = new List<EventScriptableObjectData>();

    public List<EventStringIDData> EventStringIDDatas { get => eventStringIDDatas; set => eventStringIDDatas = value; }
    public List<EventScriptableObjectData> EventScriptableObjectDatas { get => eventScriptableObjectDatas; set => eventScriptableObjectDatas = value; }

    public EventNode() {}

    public EventNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView){
        
        editorWindow = _editorWindow;
        graphView = _graphView;
        
        title = "Event";
        SetPosition(new Rect(_position, defaultNodeSize));
        nodeGuid = Guid.NewGuid().ToString();

        AddInputPort("Input", Port.Capacity.Multi);
        AddOutputPort("Output", Port.Capacity.Multi);

        TopButton();

        RefreshExpandedState();
        RefreshPorts();
    }

    public override void LoadValueIntoField() {

    }

    private void TopButton(){
        ToolbarMenu menu = new ToolbarMenu();
        menu.text = "Add Event";
        menu.menu.AppendAction("String ID", new Action<DropdownMenuAction>(x => AddStringEvent()));
        menu.menu.AppendAction("Scriptable Object", new Action<DropdownMenuAction>(x => AddScriptableEvent()));
        titleContainer.Add(menu);
    }

    public void AddStringEvent(EventStringIDData data = null) {

        EventStringIDData temp = new EventStringIDData();
        if(data != null){
            temp.stringEvent = data.stringEvent;
            temp.ID = data.ID;
        }
        eventStringIDDatas.Add(temp);

        Box boxContainer = new Box();
        boxContainer.AddToClassList("EventBox");

        // Text
        TextField textField = new TextField();
        textField.AddToClassList("EventText");
        boxContainer.Add(textField);
        textField.RegisterValueChangedCallback(value => {
            temp.stringEvent = value.newValue;
        });
        textField.SetValueWithoutNotify(temp.stringEvent);

        // ID 
        IntegerField intField = new IntegerField();
        intField.AddToClassList("EventInt");
        boxContainer.Add(intField);
        intField.RegisterValueChangedCallback(value => {
            temp.ID = value.newValue;
        });
        intField.SetValueWithoutNotify(temp.ID);
        
        // Remove
        Button btn = new Button() {
            text = "X",
        };
        btn.clicked += () => {
            DeleteBox(boxContainer);
            eventStringIDDatas.Remove(temp);
        };
        btn.AddToClassList("EventButton");
        boxContainer.Add(btn);

        mainContainer.Add(boxContainer);
        RefreshExpandedState();
    }

    public void AddScriptableEvent(EventScriptableObjectData data = null) {
        EventScriptableObjectData temp = new EventScriptableObjectData();
        if(data != null){
            temp.dialogueEventSO = data.dialogueEventSO;
        }
        eventScriptableObjectDatas.Add(temp);

        Box boxContainer = new Box();
        boxContainer.AddToClassList("EventBox");

        // Scriptable Object Event
        ObjectField objField = new ObjectField() {
            objectType = typeof(DialogueEventSO),
            allowSceneObjects = false,
            value = null,
        };
        objField.AddToClassList("EventObject");
        boxContainer.Add(objField);
        objField.RegisterValueChangedCallback(value => {
            temp.dialogueEventSO = value.newValue as DialogueEventSO;
        });
        objField.SetValueWithoutNotify(temp.dialogueEventSO);
        
        // Remove
        Button btn = new Button() {
            text = "X",
        };
        btn.clicked += () => {
            DeleteBox(boxContainer);
            eventScriptableObjectDatas.Remove(temp);
        };
        btn.AddToClassList("EventButton");
        boxContainer.Add(btn);

        mainContainer.Add(boxContainer);
        RefreshExpandedState();
    }

    private void DeleteBox(Box boxContainer) {
        mainContainer.Remove(boxContainer);
        RefreshExpandedState();
    }

}

