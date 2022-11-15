using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class EndNode : BaseNode
{
    private EndNodeType endNodeType = EndNodeType.End;
    private UnityEngine.UIElements.EnumField enumField;

    public EndNodeType EndNodeType { get => endNodeType; set => endNodeType = value; }

    public EndNode() {}

    public EndNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView){
        
        editorWindow = _editorWindow;
        graphView = _graphView;
        
        title = "End";
        SetPosition(new Rect(_position, defaultNodeSize));
        nodeGuid = Guid.NewGuid().ToString();

        AddInputPort("Input", Port.Capacity.Single);

        enumField = new UnityEngine.UIElements.EnumField() {
            value = endNodeType
        };

        enumField.Init(endNodeType);
        enumField.RegisterValueChangedCallback((value) => {
            endNodeType = (EndNodeType) value.newValue;
        });
        enumField.SetValueWithoutNotify(endNodeType);

        mainContainer.Add(enumField);

        RefreshExpandedState();
        RefreshPorts();
    }

    public override void LoadValueIntoField() {
        enumField.SetValueWithoutNotify(endNodeType);
    }
}
