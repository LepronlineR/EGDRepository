using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class StartNode : BaseNode {
    

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

        RefreshExpandedState();
        RefreshPorts();
    }
}
