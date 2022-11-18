using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class BranchNode : BaseNode
{
   // private List<BranchStringIDData> branchStringIDDatas = new List<BranchStringIDData>();

    //public List<BranchStringIDData> BranchStringIDDatas { get => branchStringIDDatas; set => branchStringIDDatas = value; }

    public BranchNode() {}

    public BranchNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView){
        this.editorWindow = _editorWindow;
        this.graphView = _graphView;

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
        // menu.menu.AppendAction("String Condition", new Action<DropdownMenuAction>(x => AddCondition()));

        titleButtonContainer.Add(menu);
    }

    //public void AddCondition(BranchStringIDData branchData = null){
        
    //}

    /*

    public void AddCondition(BranchStringIDData branchData = null){
        BranchStringIDData temp = new BranchStringIDData();
        if(branchData != null){
            temp.stringEvent = branchData.stringEvent;
            temp.ID = branchData.ID;
        }
        branchStringIDDatas.Add(temp);

        Box boxContainer = new Box();
        boxContainer.AddToClassList("BranchBox");

        // Text
        TextField textField = new TextField();
        textField.AddToClassList("BranchText");
        boxContainer.Add(textField);
        textField.RegisterValueChangedCallback(value => {
            temp.stringEvent = value.newValue;
        });
        textField.SetValueWithoutNotify(temp.stringEvent);

        // ID 
        IntegerField intField = new IntegerField();
        intField.AddToClassList("BranchID");
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
            branchStringIDDatas.Remove(temp);
        };
        btn.AddToClassList("BranchButton");
        boxContainer.Add(btn);

        mainContainer.Add(boxContainer);
        RefreshExpandedState();
    }

    private void DeleteBox(Box boxContainer){
        mainContainer.Remove(boxContainer);
        RefreshExpandedState();
    }

    */
}
