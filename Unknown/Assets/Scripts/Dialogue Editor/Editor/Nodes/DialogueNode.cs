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

    public DialogueNode() { }

    public DialogueNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView){
        
        editorWindow = _editorWindow;
        graphView = _graphView;
        
        title = "Dialogue";
        SetPosition(new Rect(_position, defaultNodeSize));
        nodeGuid = Guid.NewGuid().ToString();

        AddInputPort("Input", Port.Capacity.Multi);
    }

    /*
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
    private Image faceImagePreview;
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
                languageType = language,
                languageGenericType = ""
            });

            audioClips.Add(new LanguageGeneric<AudioClip>{
                languageType = language,
                languageGenericType = null
            });
        }

        // FACE IMAGES
        faceImageField = new ObjectField() {
            objectType = typeof(Sprite),
            allowSceneObjects = false,
            value = faceImage
        };

        faceImagePreview = new Image();
        faceImagePreview.AddToClassList("faceImagePreview");

        faceImageField.RegisterValueChangedCallback(value => {
            Sprite tmp = value.newValue as Sprite;
            faceImage = tmp;
            faceImagePreview.image = (tmp != null ? tmp.texture : null);
        });

        mainContainer.Add(faceImagePreview);
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
            value = audioClips.Find(audioClip => audioClip.languageType == editorWindow.LanguageType).languageGenericType
        };
        audioClipsField.RegisterValueChangedCallback(value => {
            audioClips.Find(audioClip => audioClip.languageType == editorWindow.LanguageType).languageGenericType = value.newValue as AudioClip;
        });
        audioClipsField.SetValueWithoutNotify(audioClips.Find(audioClip => audioClip.languageType == editorWindow.LanguageType).languageGenericType);
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
            texts.Find(text => text.languageType == editorWindow.LanguageType).languageGenericType = value.newValue; 
        });
        textsField.SetValueWithoutNotify(texts.Find(text => text.languageType == editorWindow.LanguageType).languageGenericType);
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
            texts.Find(text => text.languageType == editorWindow.LanguageType).languageGenericType = value.newValue; 
        });
        textsField.SetValueWithoutNotify(texts.Find(text => text.languageType == editorWindow.LanguageType).languageGenericType);

        audioClipsField.RegisterValueChangedCallback(value => {
            audioClips.Find(audioClip => audioClip.languageType == editorWindow.LanguageType).languageGenericType = value.newValue as AudioClip;
        });
        audioClipsField.SetValueWithoutNotify(audioClips.Find(audioClip => audioClip.languageType == editorWindow.LanguageType).languageGenericType);

        foreach(DialogueNodePort nodePort in dialogueNodePorts){
            nodePort.textField.RegisterValueChangedCallback(value => {
                nodePort.textLanguages.Find(language => language.languageType == editorWindow.LanguageType).languageGenericType = value.newValue;
            });
            nodePort.textField.SetValueWithoutNotify(nodePort.textLanguages.Find(language => language.languageType == editorWindow.LanguageType).languageGenericType);
        }
    }

    public override void LoadValueIntoField(){
        textsField.SetValueWithoutNotify(texts.Find(language => language.languageType == editorWindow.LanguageType).languageGenericType);
        audioClipsField.SetValueWithoutNotify(audioClips.Find(language => language.languageType == editorWindow.LanguageType).languageGenericType);
        faceImageField.SetValueWithoutNotify(faceImage);
        faceImageTypeField.SetValueWithoutNotify(faceImageType);
        nameField.SetValueWithoutNotify(name);

        if(faceImage != null){
            faceImagePreview.image = ((Sprite)faceImageField.value).texture;
        }
    }

    public Port AddChoicePort(BaseNode _baseNode, DialogueNodePort _dialoguePortNode = null) {
        Port port = GetPortInstance(Direction.Output);
        int outputPortCount = _baseNode.outputContainer.Query("connector").ToList().Count;
        string outputPortName = $"Continue";

        DialogueNodePort dialogueNodePort = new DialogueNodePort();
        dialogueNodePort.portGuid = Guid.NewGuid().ToString();

        foreach(LanguageType language in (LanguageType[]) Enum.GetValues(typeof(LanguageType))){
            dialogueNodePort.textLanguages.Add(new LanguageGeneric<string>(){
                languageType = language,
                languageGenericType = outputPortName
            });
        }

        if(_dialoguePortNode != null){
            dialogueNodePort.inputGuid = _dialoguePortNode.inputGuid;
            dialogueNodePort.outputGuid = _dialoguePortNode.outputGuid;
            dialogueNodePort.portGuid = _dialoguePortNode.portGuid;

            foreach(LanguageGeneric<string> languageGeneric in _dialoguePortNode.textLanguages){
                dialogueNodePort.textLanguages.Find(language => language.languageType == languageGeneric.languageType).languageGenericType = languageGeneric.languageGenericType;
            }
        }

        // Port text
        dialogueNodePort.textField = new TextField();
        dialogueNodePort.textField.RegisterValueChangedCallback(value => {
            dialogueNodePort.textLanguages.Find(language => language.languageType == editorWindow.LanguageType).languageGenericType = value.newValue;
        });
        dialogueNodePort.textField.SetValueWithoutNotify(dialogueNodePort.textLanguages.Find(language => language.languageType == editorWindow.LanguageType).languageGenericType); 
        port.contentContainer.Add(dialogueNodePort.textField);

        // Delete button
        Button delButton = new Button(() => DeletePort(_baseNode, port)) { text = "X" };
        port.contentContainer.Add(delButton);

        // dialogueNodePort.myPort = port;
        port.portName = dialogueNodePort.portGuid;
        Label portNameLabel = port.contentContainer.Q<Label>("type");
        portNameLabel.AddToClassList("PortName");

        dialogueNodePorts.Add(dialogueNodePort);

        // Refresh and return
        _baseNode.outputContainer.Add(port);
        _baseNode.RefreshPorts();
        _baseNode.RefreshExpandedState();
        return port;
    }

    private void DeletePort(BaseNode node, Port port) {
        DialogueNodePort temp = dialogueNodePorts.Find(p => p.portGuid == port.portName);
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
    */
}
