using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class StartNode : BaseNode {
    

    public StartNode(){

    }

    public StartNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView){
        
        editorWindow = _editorWindow;
        graphView = _graphView;
        
        title = "Begin";
        SetPosition(new Rect(_position, defaultNodeSize));
        nodeGuid = Guid.NewGuid().ToString();

        AddOutputPort("Output", Port.Capacity.Single);

        RefreshExpandedState();
        RefreshPorts();
    }
}
