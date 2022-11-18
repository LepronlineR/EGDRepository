using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class BranchNode : BaseNode
{
    private BranchData branchData = new BranchData();
    public BranchData BranchData { get => branchData; set => branchData = value; }

    public BranchNode() { }

    public BranchNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView){
        this.editorWindow = _editorWindow;
        this.graphView = _graphView;

        StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Nodes/BranchNodeStyleSheet");
        styleSheets.Add(styleSheet);

        title = "Branch";
        SetPosition(new Rect(_position, defaultNodeSize));
        nodeGuid = Guid.NewGuid().ToString();

        AddInputPort("Input", Port.Capacity.Multi);
        AddOutputPort("True", Port.Capacity.Single);
        AddOutputPort("False", Port.Capacity.Single);

        TopButton();
    }

    private void TopButton() {
        ToolbarMenu menu = new ToolbarMenu();
        menu.text = "Add Condition";
        
        menu.menu.AppendAction("String Event Condition", new Action<DropdownMenuAction>(x => AddCondition()));

        titleButtonContainer.Add(menu);
    }

    public void AddCondition(EventDataStringCondition stringEvent = null){
        AddStringConditionEventBuild(branchData.eventDataStringConditions, stringEvent);
    }
}
