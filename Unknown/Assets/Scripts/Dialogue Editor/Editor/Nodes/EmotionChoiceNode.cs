using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class EmotionChoiceNode : BaseNode {
    
    private EmotionChoiceData emotionChoiceData = new EmotionChoiceData();
    public EmotionChoiceData EmotionChoiceData { get => emotionChoiceData; set => emotionChoiceData = value; }

    public EmotionChoiceNode() { }

    public EmotionChoiceNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView){
        
        editorWindow = _editorWindow;
        graphView = _graphView;

        StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Nodes/ChoiceNodeStyleSheet");
        styleSheets.Add(styleSheet);
        
        title = "Emotion Choice";
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
        EnumField enumField = GetNewEnumFieldEmotionChoiceNodeType(emotionChoiceData.choiceStateType);

        mainContainer.Add(enumField);
    }

    public override void LoadValueIntoField(){
        if (emotionChoiceData.choiceStateType.enumField != null)
                emotionChoiceData.choiceStateType.enumField.SetValueWithoutNotify(emotionChoiceData.choiceStateType.value);
    }
}
