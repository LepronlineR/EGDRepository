using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Linq;

public class DialogueGraphView : GraphView
{
    private string styleSheetsName = "GraphViewStyleSheet";
    private DialogueEditorWindow editorWindow;
    private NodeSearchWindow searchWindow;

    public DialogueGraphView(DialogueEditorWindow _editorWindow){
        editorWindow = _editorWindow;
        StyleSheet temp = Resources.Load<StyleSheet>(styleSheetsName);
        styleSheets.Add(temp);

        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(new FreehandSelector());

        GridBackground grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        AddSearchWindow();
    }

    private void AddSearchWindow() {
        searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
        searchWindow.Configure(editorWindow, this);
        nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter){
        List<Port> compatiblePorts = new List<Port>();
        Port startPortView = startPort;
        ports.ForEach((port) => {
            Port portView = port;
            if(startPortView != portView && startPortView.node != portView.node && startPortView.direction != port.direction){
                compatiblePorts.Add(port);
            }
        });
        return compatiblePorts;
    }

    public void LanguageReload() {
        List<DialogueNode> dialogueNodes = nodes.ToList().Where(node => node is DialogueNode).Cast<DialogueNode>().ToList();
        foreach(DialogueNode dialogueNode in dialogueNodes){
            dialogueNode.ReloadLanguage();
        }
    }

    public StartNode CreateStartNode(Vector2 pos) {
        StartNode node = new StartNode(pos, editorWindow, this);
        return node;
    }

    public EndNode CreateEndNode(Vector2 pos) {
        EndNode node = new EndNode(pos, editorWindow, this);
        return node;
    }

    public DialogueNode CreateDialogueNode(Vector2 pos) {
        DialogueNode node = new DialogueNode(pos, editorWindow, this);
        return node;
    }

    public EventNode CreateEventNode(Vector2 pos) {
        EventNode node = new EventNode(pos, editorWindow, this);
        return node;
    }
}
