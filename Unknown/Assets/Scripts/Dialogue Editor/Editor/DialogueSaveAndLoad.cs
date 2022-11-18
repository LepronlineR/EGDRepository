using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using UnityEngine.UIElements;

public class DialogueSaveAndLoad {
    /*
    private DialogueGraphView graphView;
    private List<Edge> edges => graphView.edges.ToList();
    private List<BaseNode> nodes => graphView.nodes.ToList().Where(node => node is BaseNode).Cast<BaseNode>().ToList();

    public DialogueSaveAndLoad(DialogueGraphView dgv){
        graphView = dgv;
    }

    public void Save(DialogueContainerSO container){
        SaveEdges(container);
        SaveNodes(container);

        EditorUtility.SetDirty(container);
        AssetDatabase.SaveAssets();
    }

    public void Load(DialogueContainerSO container){
        ClearGraph();
        GenerateNodes(container);
        ConnectNodes(container);
    }

    #region Save Scripts
    private void SaveEdges(DialogueContainerSO container){
        container.nodeLinkDatas.Clear();
        Edge[] connectedEdges = edges.Where(edge => edge.input.node != null).ToArray();
        for(int x = 0; x < connectedEdges.Count(); x++){
            BaseNode outputNode = (BaseNode) connectedEdges[x].output.node;
            BaseNode inputNode = connectedEdges[x].input.node as BaseNode;

            container.nodeLinkDatas.Add(new NodeLinkData {
                baseNodeGuid = outputNode.NodeGuid,
                basePortName = connectedEdges[x].output.portName,
                targetNodeGuid = inputNode.NodeGuid,
                targetPortName = connectedEdges[x].input.portName
            });
        }
    }

    private void SaveNodes(DialogueContainerSO container){
        container.dialogueNodeDatas.Clear();
        container.eventNodeDatas.Clear();
        container.endNodeDatas.Clear();
        container.startNodeDatas.Clear();
        container.branchNodeDatas.Clear();
        nodes.ForEach(node => {
            switch(node){
                case DialogueNode dialogueNode:
                    container.dialogueNodeDatas.Add(SaveNodeData(dialogueNode));
                    break;
                case StartNode startNode:
                    container.startNodeDatas.Add(SaveNodeData(startNode));
                    break;
                case EndNode endNode:
                    container.endNodeDatas.Add(SaveNodeData(endNode));
                    break;
                case EventNode eventNode:
                    container.eventNodeDatas.Add(SaveNodeData(eventNode));
                    break;
                case BranchNode branchNode:
                    container.branchNodeDatas.Add(SaveNodeData(branchNode));
                    break;
                default:
                    break;
            }
        });
    }

    private DialogueNodeData SaveNodeData(DialogueNode node){
        DialogueNodeData dialogueNodeData = new DialogueNodeData {
            nodeGuid = node.NodeGuid,
            position = node.GetPosition().position,
        };

        return dialogueNodeData;
    }

    private StartNodeData SaveNodeData(StartNode node){
        StartNodeData nodeData =  new StartNodeData(){
            nodeGuid = node.NodeGuid,
            position = node.GetPosition().position
        };

        return nodeData;
    }

    private EndNodeData SaveNodeData(EndNode node){
        EndNodeData nodeData =  new EndNodeData(){
            nodeGuid = node.NodeGuid,
            position = node.GetPosition().position, 
        };

        return nodeData;
    }

    private EventNodeData SaveNodeData(EventNode node){
        EventNodeData nodeData =  new EventNodeData(){
            nodeGuid = node.NodeGuid,
            position = node.GetPosition().position,
        };

        return nodeData;
    }

    private BranchNodeData SaveNodeData(BranchNode node){
        List<Edge> tempEdges = edges.Where(x => x.output.node == node).Cast<Edge>().ToList();
        Edge trueOutput = edges.FirstOrDefault(x => x.output.node == node && x.output.portName == "True");
        Edge falseOutput = edges.FirstOrDefault(x => x.output.node == node && x.output.portName == "False");

        BranchNodeData nodeData =  new BranchNodeData(){
            nodeGuid = node.NodeGuid,
            position = node.GetPosition().position,
        };

        return nodeData;
    }

    #endregion

    #region Load Scripts

    private void ClearGraph(){
        edges.ForEach(edge => graphView.RemoveElement(edge));
        foreach(BaseNode node in nodes){
            graphView.RemoveElement(node);
        }
    }

    private void GenerateNodes(DialogueContainerSO container){

        // Start Node
        foreach(StartNodeData node in container.startNodeDatas){
            StartNode temp = graphView.CreateStartNode(node.position);
            temp.NodeGuid = node.nodeGuid;

            graphView.AddElement(temp);
        }

        // End Node
        foreach(EndNodeData node in container.endNodeDatas){
            EndNode temp = graphView.CreateEndNode(node.position);
            temp.NodeGuid = node.nodeGuid;

            temp.LoadValueIntoField();
            graphView.AddElement(temp);
        }

        // Event Node
        foreach(EventNodeData node in container.eventNodeDatas){
            EventNode temp = graphView.CreateEventNode(node.position);
            temp.NodeGuid = node.nodeGuid;

            temp.LoadValueIntoField();
            graphView.AddElement(temp);
        }

        foreach(BranchNodeData node in container.branchNodeDatas){
            BranchNode temp = graphView.CreateBranchNode(node.position);
            temp.NodeGuid = node.nodeGuid;
            foreach(BranchStringIDData item in node.branchStringIDDatas){
                temp.AddCondition(item);
            }
            temp.LoadValueIntoField();
            graphView.AddElement(temp);
        }

        foreach(DialogueNodeData node in container.dialogueNodeDatas){
            DialogueNode temp = graphView.CreateDialogueNode(node.position);
            temp.NodeGuid = node.nodeGuid;
            
            temp.LoadValueIntoField();
            graphView.AddElement(temp);
        }
    }

    private void ConnectNodes(DialogueContainerSO container){
        for(int x = 0; x < nodes.Count(); x++){
            List<NodeLinkData> connections = container.nodeLinkDatas.Where(edge => edge.baseNodeGuid == nodes[x].NodeGuid).ToList();
            
            List<Port> allOutputPorts = nodes[x].outputContainer.Children().Where(x => x is Port).Cast<Port>().ToList();

            foreach(NodeLinkData connection in connections){
                string targetNodeGuid = connection.targetNodeGuid;
                BaseNode targetNode = nodes.First(n => n.NodeGuid == targetNodeGuid);

                if(targetNode == null){
                    continue;
                }

                foreach(Port item in allOutputPorts){
                    if(item.portName == connection.basePortName){
                        LinkNodesTogether(item, (Port) targetNode.inputContainer[0]);
                    }
                }
            }
        }
    }

    private void LinkNodesTogether(Port outputPort, Port inputPort) {
        Edge temp = new Edge() {
            output = outputPort,
            input = inputPort
        };
        temp.input.Connect(temp);
        temp.output.Connect(temp);
        graphView.Add(temp);
    }

    #endregion

    private void PrintAllPorts(){
        for(int x = 0; x < nodes.Count(); x++){
            List<Port> allOutputPorts = nodes[x].outputContainer.Children().Where(x => x is Port).Cast<Port>().ToList();
            foreach(Port item in allOutputPorts){
                Debug.Log(item);
            }
        }
    }
    */
}
