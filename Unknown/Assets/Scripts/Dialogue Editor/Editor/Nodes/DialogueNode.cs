using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Linq;

public class DialogueNode : BaseNode
{
    private List<LanguageGeneric<string>> texts = new List<LanguageGeneric<string>>();
    private List<LanguageGeneric<AudioClip>> audioClips = new List<LanguageGeneric<AudioClip>>();
    private Sprite faceImage;
    private string name = "";
    private DialogueEmotionType faceImageType;

    private List<DialogueNodePort> dialogueNodePorts = new List<DialogueNodePort>();

    public List<LanguageGeneric<string>> Texts { get => texts; set => texts = value; }
    public List<LanguageGeneric<AudioClip>> AudioClips { get => audioClips; set => audioClips = value; }
    public Sprite FaceImage { get => faceImage; set => faceImage = value; }
    public string Name { get => name; set => name = value; }
    public DialogueEmotionType FaceImageType { get => faceImageType; set => faceImageType = value; }
    public List<DialogueNodePort> DialogueNodePorts { get => dialogueNodePorts; set => dialogueNodePorts = value; }

    private TextField textsField;
    private ObjectField audioClipsField;
    private ObjectField faceImageField;
    private TextField nameField;
    private UnityEngine.UIElements.EnumField faceImageTypeField;

    public DialogueNode() { }

    public DialogueNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView){
        
        editorWindow = _editorWindow;
        graphView = _graphView;
        
        title = "Dialogue";

        SetPosition(new Rect(_position, defaultNodeSize));
        nodeGuid = Guid.NewGuid().ToString();

        AddInputPort("Input", Port.Capacity.Multi);

        foreach(LanguageType language in (LanguageType[])Enum.GetValues(typeof(LanguageType))){
            texts.Add(new LanguageGeneric<string> {
                LanguageType = language,
                LanguageGenericType = ""
            });

            audioClips.Add(new LanguageGeneric<AudioClip>{
                LanguageType = language,
                LanguageGenericType = null
            });
        }

        // FACE IMAGES
        faceImageField = new ObjectField() {
            objectType = typeof(Sprite),
            allowSceneObjects = false,
            value = faceImage
        };
        faceImageField.RegisterValueChangedCallback(value => {
            faceImage = value.newValue as Sprite;
        });
        mainContainer.Add(faceImageField);
        
        // FACE IMAGE TYPES
        faceImageTypeField = new UnityEngine.UIElements.EnumField { value = faceImageType };
        faceImageTypeField.Init(faceImageType);
        faceImageTypeField.RegisterValueChangedCallback(value => {
            faceImageType = (DialogueEmotionType) value.newValue;
        });
        mainContainer.Add(faceImageTypeField);

        // AUDIO CLIPS
        audioClipsField = new ObjectField() {
            objectType = typeof(AudioClip),
            allowSceneObjects = false,
            value = audioClips.Find(audioClip => audioClip.LanguageType == editorWindow.LanguageType).LanguageGenericType
        };
        audioClipsField.RegisterValueChangedCallback(value => {
            audioClips.Find(audioClip => audioClip.LanguageType == editorWindow.LanguageType).LanguageGenericType = value.newValue as AudioClip;
        });
        audioClipsField.SetValueWithoutNotify(audioClips.Find(audioClip => audioClip.LanguageType == editorWindow.LanguageType).LanguageGenericType);
        mainContainer.Add(audioClipsField);

        // NAME FIELD
        Label labelName = new Label("Name");
        labelName.AddToClassList("labelName");
        labelName.AddToClassList("Label");
        mainContainer.Add(labelName);

        nameField = new TextField("");
        nameField.RegisterValueChangedCallback(value => {
            name = value.newValue;
        });
        nameField.SetValueWithoutNotify(name);
        nameField.AddToClassList("TextName");
        mainContainer.Add(nameField);

        // TEXT FIELD
        Label labelText = new Label("Text Box");
        labelText.AddToClassList("labelText");
        labelText.AddToClassList("Label");
        mainContainer.Add(labelText);

        textsField = new TextField("");
        textsField.RegisterValueChangedCallback(value => {
            texts.Find(text => text.LanguageType == editorWindow.LanguageType).LanguageGenericType = value.newValue; 
        });
        textsField.SetValueWithoutNotify(texts.Find(text => text.LanguageType == editorWindow.LanguageType).LanguageGenericType);
        textsField.multiline = true;
        textsField.AddToClassList("TextBox");
        mainContainer.Add(textsField);

        

        Button button = new Button() {
            text = "Add Options"
        };
        button.clicked += () => {
            AddChoicePort(this);
        };

        titleButtonContainer.Add(button);

        RefreshExpandedState();
        RefreshPorts();
    }

    public void ReloadLanguage(){
        textsField.RegisterValueChangedCallback(value => {
            texts.Find(text => text.LanguageType == editorWindow.LanguageType).LanguageGenericType = value.newValue; 
        });
        textsField.SetValueWithoutNotify(texts.Find(text => text.LanguageType == editorWindow.LanguageType).LanguageGenericType);

        audioClipsField.RegisterValueChangedCallback(value => {
            audioClips.Find(audioClip => audioClip.LanguageType == editorWindow.LanguageType).LanguageGenericType = value.newValue as AudioClip;
        });
        audioClipsField.SetValueWithoutNotify(audioClips.Find(audioClip => audioClip.LanguageType == editorWindow.LanguageType).LanguageGenericType);

        foreach(DialogueNodePort nodePort in dialogueNodePorts){
            nodePort.TextField.RegisterValueChangedCallback(value => {
                nodePort.TextLanguages.Find(language => language.LanguageType == editorWindow.LanguageType).LanguageGenericType = value.newValue;
            });
            nodePort.TextField.SetValueWithoutNotify(nodePort.TextLanguages.Find(language => language.LanguageType == editorWindow.LanguageType).LanguageGenericType);
        }
    }

    public override void LoadValueIntoField(){
        textsField.SetValueWithoutNotify(texts.Find(language => language.LanguageType == editorWindow.LanguageType).LanguageGenericType);
        audioClipsField.SetValueWithoutNotify(audioClips.Find(language => language.LanguageType == editorWindow.LanguageType).LanguageGenericType);
        faceImageField.SetValueWithoutNotify(faceImage);
        faceImageTypeField.SetValueWithoutNotify(faceImageType);
        nameField.SetValueWithoutNotify(name);
    }

    public Port AddChoicePort(BaseNode _baseNode, DialogueNodePort _dialoguePortNode = null) {
        Port port = GetPortInstance(Direction.Output);
        int outputPortCount = _baseNode.outputContainer.Query("connector").ToList().Count;
        string outputPortName = $"Choice {outputPortCount + 1}";

        DialogueNodePort dialogueNodePort = new DialogueNodePort();

        foreach(LanguageType language in (LanguageType[]) Enum.GetValues(typeof(LanguageType))){
            dialogueNodePort.TextLanguages.Add(new LanguageGeneric<string>(){
                LanguageType = language,
                LanguageGenericType = outputPortName
            });
        }

        if(_dialoguePortNode != null){
            dialogueNodePort.InputGrid = dialogueNodePort.InputGrid;
            dialogueNodePort.OutputGrid = dialogueNodePort.OutputGrid;

            foreach(LanguageGeneric<string> languageGeneric in _dialoguePortNode.TextLanguages){
                dialogueNodePort.TextLanguages.Find(language => language.LanguageType == languageGeneric.LanguageType).LanguageGenericType = languageGeneric.LanguageGenericType;
            }
        }

        // Port text
        dialogueNodePort.TextField = new TextField();
        dialogueNodePort.TextField.RegisterValueChangedCallback(value => {
            dialogueNodePort.TextLanguages.Find(language => language.LanguageType == editorWindow.LanguageType).LanguageGenericType = value.newValue;
        });
        dialogueNodePort.TextField.SetValueWithoutNotify(dialogueNodePort.TextLanguages.Find(language => language.LanguageType == editorWindow.LanguageType).LanguageGenericType); 
        port.contentContainer.Add(dialogueNodePort.TextField);

        // Delete button
        Button delButton = new Button(() => DeletePort(_baseNode, port)) { text = "X" };
        port.contentContainer.Add(delButton);

        dialogueNodePort.myPort = port;
        port.portName = "";

        dialogueNodePorts.Add(dialogueNodePort);

        // Refresh and return
        _baseNode.outputContainer.Add(port);
        _baseNode.RefreshPorts();
        _baseNode.RefreshExpandedState();
        return port;
    }

    private void DeletePort(BaseNode node, Port port) {
        DialogueNodePort temp = dialogueNodePorts.Find(p => p.myPort == port);
        dialogueNodePorts.Remove(temp);
        
        IEnumerable<Edge> portEdges = graphView.edges.ToList().Where(e => e.output == port);

        if(portEdges.Any()){
            Edge edge = portEdges.First();
            edge.input.Disconnect(edge);
            edge.output.Disconnect(edge);
            graphView.RemoveElement(edge);
        }

        node.outputContainer.Remove(port);
        
        node.RefreshPorts();
        node.RefreshExpandedState();
    }
}
