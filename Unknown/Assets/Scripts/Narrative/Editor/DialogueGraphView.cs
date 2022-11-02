using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{

    public readonly Vector2 defaultNodeSize = new Vector2(150, 200);
    public readonly Vector2 DefaultCommentBlockSize = new Vector2(300, 200);
    public DialogueNode EntryPointNode;
    public Blackboard Blackboard = new Blackboard();
    public List<ExposedProperty> ExposedProperties { get; private set; } = new List<ExposedProperty>();
    private NodeSearchWindow _searchWindow;

    public DialogueGraphView(DialogueGraph editorWindow){
        // styleSheets.Add(Resources.Load<StyleSheet>("Dialogue Tree"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(new FreehandSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        AddElement(GenerateEntryPointNode());
        AddSearchWindow(editorWindow);
    }

    private void AddSearchWindow(DialogueGraph editorWindow){
        _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
        _searchWindow.Configure(editorWindow, this);
        nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
    }

    public void ClearBlackBoardAndExposedProperties(){
        ExposedProperties.Clear();
        Blackboard.Clear();
    }

    public Group CreateCommentBlock(Rect rect, CommentBlockData commentBlockData = null){
        if(commentBlockData==null)
            commentBlockData = new CommentBlockData();
        var group = new Group {
            autoUpdateGeometry = true,
            title = commentBlockData.Title
        };
        AddElement(group);
        group.SetPosition(rect);
        return group;
    }

    public void AddPropertyToBlackBoard(ExposedProperty property, bool loadMode = false) {
        var localPropertyName = property.PropertyName;
        var localPropertyValue = property.PropertyValue;
        if (!loadMode){
            while (ExposedProperties.Any(x => x.PropertyName == localPropertyName)) 
                localPropertyName = $"{localPropertyName}(1)";
        }

        var item = ExposedProperty.CreateInstance();
        item.PropertyName = localPropertyName;
        item.PropertyValue = localPropertyValue;
        ExposedProperties.Add(item);

        var container = new VisualElement();
        var field = new BlackboardField {text = localPropertyName, typeText = "string"};
        container.Add(field);

        var propertyValueTextField = new TextField("Value:"){
            value = localPropertyValue
        };
        propertyValueTextField.RegisterValueChangedCallback(evt => {
            var index = ExposedProperties.FindIndex(x => x.PropertyName == item.PropertyName);
            ExposedProperties[index].PropertyValue = evt.newValue;
        });
        var sa = new BlackboardRow(field, propertyValueTextField);
        container.Add(sa);
        Blackboard.Add(container);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter){
        var compatiblePorts = new List<Port>();
        var startPortView = startPort;

        ports.ForEach((port) => {
            var portView = port;
            if (startPortView != portView && startPortView.node != portView.node)
                compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }

    private Port GeneratePort(DialogueNode target, Direction portDir, Port.Capacity capacity = Port.Capacity.Single){
        return target.InstantiatePort(Orientation.Horizontal, portDir, capacity, typeof(float)); // arbitrary type
    }

    private DialogueNode GenerateEntryPointNode(){
        var nodeCache = new DialogueNodeText(){
            title = "Begin",
            GUID = Guid.NewGuid().ToString(),
            DialogueText = "N/A",
            EntryPoint = true
        };

        var generatedPort = GeneratePort(nodeCache, Direction.Output);
        generatedPort.portName = "Next";
        nodeCache.outputContainer.Add(generatedPort);

        nodeCache.capabilities &= ~Capabilities.Movable;
        nodeCache.capabilities &= ~Capabilities.Deletable;

        nodeCache.RefreshExpandedState();
        nodeCache.RefreshPorts();
        nodeCache.SetPosition(new Rect(100, 200, 100, 150));
        return nodeCache;
    }

    public void CreateNode(string nodeName, Vector2 position){
        AddElement(CreateDialogueNode(nodeName, position));
    }

    public DialogueNode CreateDialogueNode(string nodeName, Vector2 position){
        var dialogueNode = new DialogueNodeText {
            title = nodeName,
            GUID = Guid.NewGuid().ToString(),
            DialogueText = nodeName,
            EntryPoint = false
        };

        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);

        var responsePort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Single);
        responsePort.portName = "Response";
        dialogueNode.inputContainer.Add(responsePort);

        var evidencePort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Single);
        evidencePort.portName = "Evidence";
        dialogueNode.inputContainer.Add(evidencePort);

        var button = new UnityEngine.UIElements.Button(()=> {
            AddChoicePort(dialogueNode);
        });
        button.text = "Add";
        dialogueNode.titleContainer.Add(button);

        var textField = new TextField(string.Empty);
        textField.RegisterValueChangedCallback(evt => {
            dialogueNode.DialogueText = evt.newValue;
            dialogueNode.title = evt.newValue;
        });
        textField.SetValueWithoutNotify(dialogueNode.title);
        dialogueNode.mainContainer.Add(textField);

        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(position, defaultNodeSize));
        
        return dialogueNode;
    }

    

    //public void AddResponsePort(DialogueNode nodeCahce, string overridenPortName = ""){
    //    
    //}

    public void AddChoicePort(DialogueNode nodeCache, string overriddenPortName = ""){
        var generatedPort = GeneratePort(nodeCache, Direction.Output);
        var portLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(portLabel);

        var outputPortCount = nodeCache.outputContainer.Query("connector").ToList().Count();
         var outputPortName = string.IsNullOrEmpty(overriddenPortName)
            ? $"Option {outputPortCount + 1}" : overriddenPortName;


        var textField = new TextField() {
            name = string.Empty,
            value = outputPortName
        };
        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        generatedPort.contentContainer.Add(new Label("  "));
        generatedPort.contentContainer.Add(textField);
        var deleteButton = new UnityEngine.UIElements.Button(() => RemovePort(nodeCache, generatedPort)){
            text = "X"
        };
        generatedPort.contentContainer.Add(deleteButton);
        generatedPort.portName = outputPortName;
        nodeCache.outputContainer.Add(generatedPort);
        nodeCache.RefreshPorts();
        nodeCache.RefreshExpandedState();
    }

    private void RemovePort(Node node, Port socket) {
        var targetEdge = edges.ToList().Where(x => x.output.portName == socket.portName && x.output.node == socket.node);
        if (targetEdge.Any()){
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }

        node.outputContainer.Remove(socket);
        node.RefreshPorts();
        node.RefreshExpandedState();
    }

    void OnDragUpdatedEvent(DragUpdatedEvent e){
        if (DragAndDrop.GetGenericData("DragSelection") is List<ISelectable> selection && (selection.OfType<BlackboardField>().Count() >= 0)){
            DragAndDrop.visualMode = e.actionKey ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Move;
        }
    }
    
    void OnDragPerformEvent(DragPerformEvent e){
        var selection = DragAndDrop.GetGenericData("DragSelection") as List<ISelectable>;
        IEnumerable<BlackboardField> fields = selection.OfType<BlackboardField>();
        foreach (BlackboardField field in fields){
            // Create a node
        }
    }
}
