using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class EndNode : BaseNode
{   

    private EndData endData = new EndData();

    public EndData EndData { get => endData; set => endData = value; }

    public EndNode() {}

    public EndNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView){
        
        editorWindow = _editorWindow;
        graphView = _graphView;

        StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Nodes/EndNodeStyleSheet");
        styleSheets.Add(styleSheet);
        
        title = "End";
        SetPosition(new Rect(_position, defaultNodeSize));
        nodeGuid = Guid.NewGuid().ToString();

        AddInputPort("Input", Port.Capacity.Multi);
        
        TopContainer();
        MakeMainContainer();
    }

    private void TopContainer(){
        AddDropdownMenu();
    }

    private void AddDropdownMenu(){
        ToolbarMenu menu = new ToolbarMenu();
        menu.text = "Add Content";

        menu.menu.AppendAction("Add More Trees to Character", new Action<DropdownMenuAction>(x => AddScriptableEvent()));

        titleButtonContainer.Add(menu);
    }

    public void AddScriptableEvent(ContainerDialogueContainerSO container = null) {
        ContainerDialogueContainerSO temp = new ContainerDialogueContainerSO();
        if(container != null){
            temp.dialogueContainerSO = container.dialogueContainerSO;
        }
        endData.endDialogueContainers.Add(temp);

        // Container of all objects
        Box boxContainer = new Box();
        boxContainer.AddToClassList("EventBox");

        // Scriptable Object Event
        ObjectField objectField = GetNewObjectFieldDialogueContainer(temp, "EventObject");
        
        // Remove
        Button btn = GetNewButton("X", "RemoveButton");
        btn.clicked += () => {
            DeleteBox(boxContainer);
            endData.endDialogueContainers.Remove(temp);
        };

        // Add to box
        boxContainer.Add(objectField);
        boxContainer.Add(btn);

        mainContainer.Add(boxContainer);
        RefreshExpandedState();
    }

    private void MakeMainContainer() {
        EnumField enumField = GetNewEnumFieldEndNodeType(endData.endNodeType);

        mainContainer.Add(enumField);
    }

    public override void LoadValueIntoField(){
        if(endData.endNodeType.enumField != null){
            endData.endNodeType.enumField.SetValueWithoutNotify(endData.endNodeType.value);
        }
    }

}
