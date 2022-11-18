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

        AddInputPort("Input", Port.Capacity.Single);

        MakeMainContainer();
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
