using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private DialogueEditorWindow editorWindow;
    private DialogueGraphView graphView;

    private Texture2D pic;

    public void Configure(DialogueEditorWindow eWindow, DialogueGraphView dGraphView){
        editorWindow = eWindow;
        graphView = dGraphView;
    
        pic = new Texture2D(1, 1);
        pic.SetPixel(0, 0, new Color(0, 0, 0, 0));
        pic.Apply();
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context){
        List<SearchTreeEntry> tree = new List<SearchTreeEntry> {
            new SearchTreeGroupEntry(new GUIContent("Dialogue Node"), 0),
            new SearchTreeGroupEntry(new GUIContent("Dialogue"), 1),

            AddNodeSearch("Start Node", new StartNode()),
            AddNodeSearch("End Node", new EndNode()),
            AddNodeSearch("Event Node", new EventNode()),
            AddNodeSearch("Dialogue Node", new DialogueNode()),
        };

        return tree;
    }

    private SearchTreeEntry AddNodeSearch(string n, BaseNode node){
        SearchTreeEntry temp = new SearchTreeEntry(new GUIContent(n, pic)) {
            level = 2,
            userData = node
        };

        return temp;
    }

    public bool OnSelectEntry(SearchTreeEntry ste, SearchWindowContext swc){
        Vector2 mousePos = editorWindow.rootVisualElement.ChangeCoordinatesTo(editorWindow.rootVisualElement.parent, swc.screenMousePosition - editorWindow.position.position);
        Vector2 graphMousePos = VisualElementExtensions.WorldToLocal(graphView.contentViewContainer, mousePos);
        return CheckForNodeType(ste, graphMousePos);
    }

    private bool CheckForNodeType(SearchTreeEntry ste, Vector2 pos){
        switch(ste.userData){
            case StartNode node:
                graphView.AddElement(graphView.CreateStartNode(pos));
                return true;
            case EndNode node:
                graphView.AddElement(graphView.CreateEndNode(pos));
                return true;
            case DialogueNode node:
                graphView.AddElement(graphView.CreateDialogueNode(pos));
                return true;
            case EventNode node:
                graphView.AddElement(graphView.CreateEventNode(pos));
                return true;
            default:
                break;
        }
        return false;
    }
}
