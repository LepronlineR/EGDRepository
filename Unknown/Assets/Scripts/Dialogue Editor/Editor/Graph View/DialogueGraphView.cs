using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Linq;

public class DialogueGraphView : GraphView
{
    private string styleSheetsName = "USS/GraphView/GraphViewStyleSheet";
    private DialogueEditorWindow editorWindow;
    private NodeSearchWindow searchWindow;

    public DialogueGraphView(DialogueEditorWindow editorWindow){
        this.editorWindow = editorWindow;

        // zoom exist
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        StyleSheet temp = Resources.Load<StyleSheet>(styleSheetsName);
        styleSheets.Add(temp);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(new FreehandSelector());

        GridBackground grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        // canPasteSerializedData += AllowPaste;
        // unserializeAndPaste += OnPaste;

        AddSearchWindow();
    }

    // private bool AllowPaste(string data){
    //     return true;
    // }

    // private void OnPaste(string a, string b) {
        //
    // }

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

    public void ReloadLanguage() {
        List<BaseNode> allNodes = nodes.ToList().Where(node => node is BaseNode).Cast<BaseNode>().ToList();
        foreach(BaseNode node in allNodes){
            node.ReloadLanguage();
        }
    }

    public StartNode CreateStartNode(Vector2 pos) {
        return new StartNode(pos, editorWindow, this);
    }

    public EndNode CreateEndNode(Vector2 pos) {
        return new EndNode(pos, editorWindow, this);
    }

    public DialogueNode CreateDialogueNode(Vector2 pos) {
        return new DialogueNode(pos, editorWindow, this);
    }

    public EventNode CreateEventNode(Vector2 pos) {
        return new EventNode(pos, editorWindow, this);
    }

    public BranchNode CreateBranchNode(Vector2 pos) {
        return new BranchNode(pos, editorWindow, this);
    }

    public ChoiceNode CreateChoiceNode(Vector2 pos){
        return new ChoiceNode(pos, editorWindow, this);
    }

    public EmotionChoiceNode CreateEmotionChoiceNode(Vector2 pos){
        return new EmotionChoiceNode(pos, editorWindow, this);
    }

    public ObjectChoiceNode CreateObjectChoiceNode(Vector2 pos){
        return new ObjectChoiceNode(pos, editorWindow, this);
    }
}
