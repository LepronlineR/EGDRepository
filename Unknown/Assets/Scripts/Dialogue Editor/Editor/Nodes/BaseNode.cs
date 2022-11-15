using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

public class BaseNode : Node {
    protected string nodeGuid;
    protected DialogueGraphView graphView;
    protected DialogueEditorWindow editorWindow;
    protected Vector2 defaultNodeSize = new Vector2(200, 250);

    protected string NodeGuid { get => nodeGuid; set => nodeGuid = value; }

    public BaseNode() {
        StyleSheet styleSheet = Resources.Load<StyleSheet>("NodeStyleSheet");
        styleSheets.Add(styleSheet);
    }

    public void AddOutputPort(string name, Port.Capacity capacity = Port.Capacity.Single){
        Port outputPort = GetPortInstance(Direction.Output, capacity);
        outputPort.portName = name;
        outputContainer.Add(outputPort);
    }

    public void AddInputPort(string name, Port.Capacity capacity = Port.Capacity.Multi){
        Port inputPort = GetPortInstance(Direction.Input, capacity);
        inputPort.portName = name;
        inputContainer.Add(inputPort);
    }

    public Port GetPortInstance(Direction nodeDir, Port.Capacity capacity = Port.Capacity.Single){
        return InstantiatePort(Orientation.Horizontal, nodeDir, capacity, typeof(float));
    }

    public virtual void LoadValueIntoField() {

    }
}
