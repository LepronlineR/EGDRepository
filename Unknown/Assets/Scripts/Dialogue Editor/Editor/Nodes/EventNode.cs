using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class EventNode : BaseNode
{

    private EventData eventData = new EventData();
    public EventData EventData { get => eventData; set => eventData = value; }

    public EventNode() {}

    public EventNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView){
        
        editorWindow = _editorWindow;
        graphView = _graphView;

        StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Nodes/EventNodeStyleSheet");
        styleSheets.Add(styleSheet);
        
        title = "Event";
        SetPosition(new Rect(_position, defaultNodeSize));
        nodeGuid = Guid.NewGuid().ToString();

        AddInputPort("Input", Port.Capacity.Multi);
        AddOutputPort("Output", Port.Capacity.Single);

        TopButton();
    }

    private void TopButton(){
        ToolbarMenu menu = new ToolbarMenu();
        menu.text = "Add Event";
        menu.menu.AppendAction("String Event Modifier", new Action<DropdownMenuAction>(x => AddStringEvent()));
        menu.menu.AppendAction("Scriptable Object", new Action<DropdownMenuAction>(x => AddScriptableEvent()));
        titleContainer.Add(menu);
    }

    public void AddStringEvent(EventDataStringModifier stringEvent = null){
        AddStringModifierEventBuild(eventData.eventDataStringModifiers, stringEvent);
    }

    public void AddScriptableEvent(ContainerDialogueEventSO container = null) {
        ContainerDialogueEventSO temp = new ContainerDialogueEventSO();
        if(container != null){
            temp.dialogueEventSO = container.dialogueEventSO;
        }
        eventData.containerDialogueEventSOs.Add(temp);

        // Container of all objects
        Box boxContainer = new Box();
        boxContainer.AddToClassList("EventBox");

        // Scriptable Object Event
        ObjectField objectField = GetNewObjectFieldDialogueEvent(temp, "EventObject");
        
        // Remove
        Button btn = GetNewButton("X", "RemoveButton");
        btn.clicked += () => {
            DeleteBox(boxContainer);
            eventData.containerDialogueEventSOs.Remove(temp);
        };

        // Add to box
        boxContainer.Add(objectField);
        boxContainer.Add(btn);

        mainContainer.Add(boxContainer);
        RefreshExpandedState();
    }
}

