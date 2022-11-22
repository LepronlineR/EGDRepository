using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using UnityEngine.UIElements;

public class DialogueSaveAndLoad {

    private List<Edge> edges => graphView.edges.ToList();
    private List<BaseNode> nodes => graphView.nodes.ToList().Where(node => node is BaseNode).Cast<BaseNode>().ToList();

    private DialogueGraphView graphView;

    public DialogueSaveAndLoad(DialogueGraphView graphView){
        this.graphView = graphView;
    }

    public void Save(DialogueContainerSO dialogueContainerSO){
        SaveEdges(dialogueContainerSO);
        SaveNodes(dialogueContainerSO);

        EditorUtility.SetDirty(dialogueContainerSO);
        AssetDatabase.SaveAssets();
    }

    public void Load(DialogueContainerSO dialogueContainerSO){
        ClearGraph();
        GenerateNodes(dialogueContainerSO);
        ConnectNodes(dialogueContainerSO);
    }

    #region Save

    private void SaveEdges(DialogueContainerSO dialogueContainerSO){

        dialogueContainerSO.nodeLinkDatas.Clear();

        Edge[] connectedEdges = edges.Where(edge => edge.input.node != null).ToArray();
        for (int i = 0; i < connectedEdges.Count(); i++){
            BaseNode outputNode = (BaseNode)connectedEdges[i].output.node;
            BaseNode inputNode = connectedEdges[i].input.node as BaseNode;

            dialogueContainerSO.nodeLinkDatas.Add(new NodeLinkData {
                baseNodeGuid = outputNode.NodeGuid,
                basePortName = connectedEdges[i].output.portName,
                targetNodeGuid = inputNode.NodeGuid,
                targetPortName = connectedEdges[i].input.portName,
            });
        }
    }

    private void SaveNodes(DialogueContainerSO dialogueContainerSO) {
        dialogueContainerSO.eventDatas.Clear();
        dialogueContainerSO.endDatas.Clear();
        dialogueContainerSO.startDatas.Clear();
        dialogueContainerSO.branchDatas.Clear();
        dialogueContainerSO.dialogueDatas.Clear();
        dialogueContainerSO.choiceDatas.Clear();
        dialogueContainerSO.emotionChoiceDatas.Clear();
        dialogueContainerSO.objectChoiceDatas.Clear();

        nodes.ForEach(node => {
            switch (node) {
                case DialogueNode dialogueNode:
                    dialogueContainerSO.dialogueDatas.Add(SaveNodeData(dialogueNode));
                    break;
                case StartNode startNode:
                    dialogueContainerSO.startDatas.Add(SaveNodeData(startNode));
                    break;
                case EndNode endNode:
                    dialogueContainerSO.endDatas.Add(SaveNodeData(endNode));
                    break;
                case EventNode eventNode:
                    dialogueContainerSO.eventDatas.Add(SaveNodeData(eventNode));
                    break;
                case BranchNode branchNode:
                    dialogueContainerSO.branchDatas.Add(SaveNodeData(branchNode));
                    break;
                case ChoiceNode choiceNode:
                    dialogueContainerSO.choiceDatas.Add(SaveNodeData(choiceNode));
                    break;
                case EmotionChoiceNode emotionChoiceNode:
                    dialogueContainerSO.emotionChoiceDatas.Add(SaveNodeData(emotionChoiceNode));
                    break;
                case ObjectChoiceNode objectChoiceNode:
                    dialogueContainerSO.objectChoiceDatas.Add(SaveNodeData(objectChoiceNode));
                    break;
                default:
                    break;
            }
        });
    }

    private DialogueData SaveNodeData(DialogueNode node) {
        DialogueData dialogueData = new DialogueData {
            nodeGuid = node.NodeGuid,
            position = node.GetPosition().position,
        };

        // Set ID
        for (int i = 0; i < node.DialogueData.dialogueBaseContainers.Count; i++) {
            node.DialogueData.dialogueBaseContainers[i].ID.value = i;
        }

        foreach (DialogueDataBaseContainer baseContainer in node.DialogueData.dialogueBaseContainers){
            // Name
            if (baseContainer is DialogueDataName) {
                DialogueDataName tmp = (baseContainer as DialogueDataName);
                DialogueDataName tmpData = new DialogueDataName();

                tmpData.ID.value = tmp.ID.value;
                tmpData.characterName.value = tmp.characterName.value;

                dialogueData.dialogueDataNames.Add(tmpData);
            }

            // Text
            if (baseContainer is DialogueDataText){
                DialogueDataText tmp = (baseContainer as DialogueDataText);
                DialogueDataText tmpData = new DialogueDataText();

                tmpData.ID = tmp.ID;
                tmpData.GuidID = tmp.GuidID;
                tmpData.texts = tmp.texts;
                tmpData.audioClips = tmp.audioClips;

                dialogueData.dialogueDataTexts.Add(tmpData);
            }

            // Response Text
            if (baseContainer is DialogueDataResponseText) {
                DialogueDataResponseText tmp = (baseContainer as DialogueDataResponseText);
                DialogueDataResponseText tmpData = new DialogueDataResponseText();

                tmpData.ID.value = tmp.ID.value;
                tmpData.responseText.value = tmp.responseText.value;

                dialogueData.dialogueResponseTexts.Add(tmpData);
            }

            // Images
            if (baseContainer is DialogueDataImages){
                DialogueDataImages tmp = (baseContainer as DialogueDataImages);
                DialogueDataImages tmpData = new DialogueDataImages();

                tmpData.ID.value = tmp.ID.value;
                tmpData.sprite.value = tmp.sprite.value;

                dialogueData.dialogueDataImagess.Add(tmpData);
            }
        }

        // Port
        foreach (DialogueDataPort port in node.DialogueData.dialogueDataPorts) {
            DialogueDataPort portData = new DialogueDataPort();

            portData.outputGuid = string.Empty;
            portData.inputGuid = string.Empty;
            portData.portGuid = port.portGuid;

            foreach (Edge edge in edges){
                if (edge.output.portName == port.portGuid){
                    portData.outputGuid = (edge.output.node as BaseNode).NodeGuid;
                    portData.inputGuid = (edge.input.node as BaseNode).NodeGuid;
                }
            }

            dialogueData.dialogueDataPorts.Add(portData);
        }

        return dialogueData;
    }

    private StartData SaveNodeData(StartNode node) {
        StartData nodeData = new StartData() {
            nodeGuid = node.NodeGuid,
            position = node.GetPosition().position,
            text = node.StartData.text,
        };

        return nodeData;
    }

    private EndData SaveNodeData(EndNode node) {
        EndData nodeData = new EndData() {
            nodeGuid = node.NodeGuid,
            position = node.GetPosition().position,
            endDialogueContainers = node.EndData.endDialogueContainers
        };
        
        nodeData.endNodeType.value = node.EndData.endNodeType.value;

        return nodeData;
    }

    private EventData SaveNodeData(EventNode node) {
        EventData nodeData = new EventData() {
            nodeGuid = node.NodeGuid,
            position = node.GetPosition().position,
        };

        // Save Dialogue Event
        foreach (ContainerDialogueEventSO dialogueEvent in node.EventData.containerDialogueEventSOs) {
            nodeData.containerDialogueEventSOs.Add(dialogueEvent);
        }

        // Save String Event
        foreach (EventDataStringModifier stringEvents in node.EventData.eventDataStringModifiers){
            EventDataStringModifier tmp = new EventDataStringModifier();
            tmp.number.value = stringEvents.number.value;
            tmp.stringEventText.value = stringEvents.stringEventText.value;
            tmp.stringEventModifierType.value = stringEvents.stringEventModifierType.value;

            nodeData.eventDataStringModifiers.Add(tmp);
        }

        return nodeData;
    }

    private BranchData SaveNodeData(BranchNode node) {
        List<Edge> tmpEdges = edges.Where(x => x.output.node == node).Cast<Edge>().ToList();

        Edge trueOutput = edges.FirstOrDefault(x => x.output.node == node && x.output.portName == "True");
        Edge falseOutput = edges.FirstOrDefault(x => x.output.node == node && x.output.portName == "False");

        BranchData nodeData = new BranchData() {
            nodeGuid = node.NodeGuid,
            position = node.GetPosition().position,
            trueGuidNode = (trueOutput != null ? (trueOutput.input.node as BaseNode).NodeGuid : string.Empty),
            falseGuidNode = (falseOutput != null ? (falseOutput.input.node as BaseNode).NodeGuid : string.Empty),
        };

        foreach (EventDataStringCondition stringEvents in node.BranchData.eventDataStringConditions) {
            EventDataStringCondition tmp = new EventDataStringCondition();
            tmp.number.value = stringEvents.number.value;
            tmp.stringEventText.value = stringEvents.stringEventText.value;
            tmp.stringEventConditionType.value = stringEvents.stringEventConditionType.value;

            nodeData.eventDataStringConditions.Add(tmp);
        }

        return nodeData;
    }

    private ChoiceData SaveNodeData(ChoiceNode node) {
        ChoiceData nodeData = new ChoiceData() {
            nodeGuid = node.NodeGuid,
            position = node.GetPosition().position,
            text = node.ChoiceData.text,
            audioClips = node.ChoiceData.audioClips,
        };
        nodeData.choiceStateType.value = node.ChoiceData.choiceStateType.value;

        // Event String Condition
        foreach (EventDataStringCondition stringEvents in node.ChoiceData.eventDataStringConditions) {
            EventDataStringCondition tmp = new EventDataStringCondition();
            tmp.stringEventText.value = stringEvents.stringEventText.value;
            tmp.number.value = stringEvents.number.value;
            tmp.stringEventConditionType.value = stringEvents.stringEventConditionType.value;

            nodeData.eventDataStringConditions.Add(tmp);
        }

        // Event GameObject Condition
        foreach(EventDataGameObjectCondition objectEvents in node.ChoiceData.eventDataObjectConditions) {
            EventDataGameObjectCondition tmp = new EventDataGameObjectCondition();
            tmp.objectEvent.value = objectEvents.objectEvent.value;
            tmp.objectEventConditionType.value = objectEvents.objectEventConditionType.value;

            nodeData.eventDataObjectConditions.Add(tmp);
        }

        return nodeData;
    }

    private EmotionChoiceData SaveNodeData(EmotionChoiceNode node) {
        EmotionChoiceData nodeData = new EmotionChoiceData() {
            nodeGuid = node.NodeGuid,
            position = node.GetPosition().position,
        };
        
        nodeData.choiceStateType.value = node.EmotionChoiceData.choiceStateType.value;

        return nodeData;
    }

    private ObjectChoiceData SaveNodeData(ObjectChoiceNode node) {
        ObjectChoiceData nodeData = new ObjectChoiceData() {
            nodeGuid = node.NodeGuid,
            position = node.GetPosition().position
        };
        
        nodeData.choiceObject.value = node.ObjectChoiceData.choiceObject.value;

        return nodeData;
    }

    #endregion

    #region Load

    private void ClearGraph() {
        edges.ForEach(edge => graphView.RemoveElement(edge));

        foreach (BaseNode node in nodes){
            graphView.RemoveElement(node);
        }
    }

    private void GenerateNodes(DialogueContainerSO dialogueContainer) {
        // Start
        foreach (StartData node in dialogueContainer.startDatas){
            StartNode tempNode = graphView.CreateStartNode(node.position);
            tempNode.NodeGuid = node.nodeGuid;
            
            foreach (LanguageGeneric<string> dataText in node.text){
                foreach (LanguageGeneric<string> editorText in tempNode.StartData.text) {
                    if (editorText.languageType == dataText.languageType){
                        editorText.languageGenericType = dataText.languageGenericType;
                    }
                }
            }

            tempNode.LoadValueIntoField();
            graphView.AddElement(tempNode);
        }

        // End Node 
        foreach (EndData node in dialogueContainer.endDatas) {
            EndNode tempNode = graphView.CreateEndNode(node.position);
            tempNode.NodeGuid = node.nodeGuid;
            tempNode.EndData.endNodeType.value = node.endNodeType.value;

            foreach (ContainerDialogueContainerSO item in node.endDialogueContainers) {
                tempNode.AddScriptableEvent(item);
            }

            tempNode.LoadValueIntoField();
            graphView.AddElement(tempNode);
        }

        // Event Node
        foreach (EventData node in dialogueContainer.eventDatas) {
            EventNode tempNode = graphView.CreateEventNode(node.position);
            tempNode.NodeGuid = node.nodeGuid;

            foreach (ContainerDialogueEventSO item in node.containerDialogueEventSOs) {
                tempNode.AddScriptableEvent(item);
            }

            foreach (EventDataStringModifier item in node.eventDataStringModifiers) {
                tempNode.AddStringEvent(item);
            }

            tempNode.LoadValueIntoField();
            graphView.AddElement(tempNode);
        }

        // Branch Node
        foreach (BranchData node in dialogueContainer.branchDatas) {
            BranchNode tempNode = graphView.CreateBranchNode(node.position);
            tempNode.NodeGuid = node.nodeGuid;

            foreach (EventDataStringCondition item in node.eventDataStringConditions){
                tempNode.AddCondition(item);
            }

            tempNode.LoadValueIntoField();
            tempNode.ReloadLanguage();
            graphView.AddElement(tempNode);
        }

        // Choice Node
        foreach (ChoiceData node in dialogueContainer.choiceDatas){
            ChoiceNode tempNode = graphView.CreateChoiceNode(node.position);
            tempNode.NodeGuid = node.nodeGuid;

            tempNode.ChoiceData.choiceStateType.value = node.choiceStateType.value;

            foreach (LanguageGeneric<string> dataText in node.text){
                foreach (LanguageGeneric<string> editorText in tempNode.ChoiceData.text) {
                    if (editorText.languageType == dataText.languageType){
                        editorText.languageGenericType = dataText.languageGenericType;
                    }
                }
            }
            foreach (LanguageGeneric<AudioClip> dataAudioClip in node.audioClips) {
                foreach (LanguageGeneric<AudioClip> editorAudioClip in tempNode.ChoiceData.audioClips){
                    if (editorAudioClip.languageType == dataAudioClip.languageType) {
                        editorAudioClip.languageGenericType = dataAudioClip.languageGenericType;
                    }
                }
            }

            foreach (EventDataStringCondition item in node.eventDataStringConditions){
                tempNode.AddCondition(item);
            }

            foreach (EventDataGameObjectCondition item in node.eventDataObjectConditions){
                tempNode.AddCondition(item);
            }

            tempNode.LoadValueIntoField();
            tempNode.ReloadLanguage();
            graphView.AddElement(tempNode);
        }

        // Dialogue Node
        foreach (DialogueData node in dialogueContainer.dialogueDatas) {
            DialogueNode tempNode = graphView.CreateDialogueNode(node.position);
            tempNode.NodeGuid = node.nodeGuid;

            List<DialogueDataBaseContainer> dataBaseContainer = new List<DialogueDataBaseContainer>();

            dataBaseContainer.AddRange(node.dialogueDataImagess);
            dataBaseContainer.AddRange(node.dialogueDataTexts);
            dataBaseContainer.AddRange(node.dialogueDataNames);
            dataBaseContainer.AddRange(node.dialogueResponseTexts);

            dataBaseContainer.Sort(delegate (DialogueDataBaseContainer x, DialogueDataBaseContainer y){
                return x.ID.value.CompareTo(y.ID.value);
            });

            foreach (DialogueDataBaseContainer data in dataBaseContainer){
                switch (data) {
                    case DialogueDataName name:
                        tempNode.CharacterName(name);
                        break;
                    case DialogueDataText text:
                        tempNode.TextLine(text);
                        break;
                    case DialogueDataImages image:
                        tempNode.ImagePic(image);
                        break;
                    case DialogueDataResponseText response:
                        tempNode.ResponseText(response);
                        break;
                    default:
                        break;
                }
            }

            foreach (DialogueDataPort port in node.dialogueDataPorts){
                tempNode.AddChoicePort(tempNode, port);
            }

            tempNode.LoadValueIntoField();
            tempNode.ReloadLanguage();
            graphView.AddElement(tempNode);
        }

        // Emotion Choice Node 
        foreach (EmotionChoiceData node in dialogueContainer.emotionChoiceDatas) {
            EmotionChoiceNode tempNode = graphView.CreateEmotionChoiceNode(node.position);
            tempNode.NodeGuid = node.nodeGuid;
            tempNode.EmotionChoiceData.choiceStateType.value = node.choiceStateType.value;

            tempNode.LoadValueIntoField();
            graphView.AddElement(tempNode);
        }

        // Object Choice Node 
        foreach (ObjectChoiceData node in dialogueContainer.objectChoiceDatas) {
            ObjectChoiceNode tempNode = graphView.CreateObjectChoiceNode(node.position);
            tempNode.NodeGuid = node.nodeGuid;
            tempNode.ObjectChoiceData.choiceObject.value = node.choiceObject.value;

            tempNode.LoadValueIntoField();
            graphView.AddElement(tempNode);
        }
    }

    private void ConnectNodes(DialogueContainerSO dialogueContainer) {
        // Make connection for all node.
        for (int i = 0; i < nodes.Count; i++) {
            List<NodeLinkData> connections = dialogueContainer.nodeLinkDatas.Where(edge => edge.baseNodeGuid == nodes[i].NodeGuid).ToList();

            List<Port> allOutputPorts = nodes[i].outputContainer.Children().Where(x => x is Port).Cast<Port>().ToList();

            for (int j = 0; j < connections.Count; j++) {
                string targetNodeGuid = connections[j].targetNodeGuid;
                BaseNode targetNode = nodes.First(node => node.NodeGuid == targetNodeGuid);

                if (targetNode == null)
                    continue;

                foreach (Port item in allOutputPorts) {
                    if (item.portName == connections[j].basePortName) {
                        LinkNodesTogether(item, (Port)targetNode.inputContainer[0]);
                    }
                }
            }
        }
    }

    private void LinkNodesTogether(Port outputPort, Port inputPort) {
        Edge tempEdge = new Edge() {
            output = outputPort,
            input = inputPort
        };
        tempEdge.input.Connect(tempEdge);
        tempEdge.output.Connect(tempEdge);
        graphView.Add(tempEdge);
    }

    #endregion
}