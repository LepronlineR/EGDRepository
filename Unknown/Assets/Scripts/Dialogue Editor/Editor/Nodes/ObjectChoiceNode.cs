using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class ObjectChoiceNode : BaseNode {
    
    private ObjectChoiceData objectChoiceData = new ObjectChoiceData();
    public ObjectChoiceData ObjectChoiceData { get => objectChoiceData; set => objectChoiceData = value; }

    public ObjectChoiceNode() { }

    public ObjectChoiceNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView){
        
        editorWindow = _editorWindow;
        graphView = _graphView;

        StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Nodes/ChoiceNodeStyleSheet");
        styleSheets.Add(styleSheet);
        
        title = "Object Choice";
        SetPosition(new Rect(_position, defaultNodeSize));
        nodeGuid = Guid.NewGuid().ToString();

        // Add standard ports.
        Port inputPort = AddInputPort("Input", Port.Capacity.Multi);
        AddOutputPort("Output", Port.Capacity.Single);

        // Set color of the port.
        inputPort.portColor = Color.yellow;

        MakeMainContainer();
    }

    private void MakeMainContainer() {
        objectChoiceData.objectField = GetNewObjectFieldGameObject(objectChoiceData.choiceObject);
        mainContainer.Add(objectChoiceData.objectField);
    }

    public override void LoadValueIntoField(){
        if(objectChoiceData.objectField != null)
            objectChoiceData.objectField.SetValueWithoutNotify(objectChoiceData.choiceObject.value);
    }
}
