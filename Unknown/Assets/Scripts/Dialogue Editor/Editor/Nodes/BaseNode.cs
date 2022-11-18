using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

public class BaseNode : Node {

    protected string nodeGuid;
    protected DialogueGraphView graphView;
    protected DialogueEditorWindow editorWindow;
    protected Vector2 defaultNodeSize = new Vector2(200, 250);

    private List<LanguageGenericHolderText> languageGenericListTexts = new List<LanguageGenericHolderText>();
    private List<LanguageGenericHolderAudioClip> languageGenericListAudioClips = new List<LanguageGenericHolderAudioClip>();

    public string NodeGuid { get => nodeGuid; set => nodeGuid = value; }

    public BaseNode() {
        StyleSheet styleSheet = Resources.Load<StyleSheet>("NodeStyleSheet");
        styleSheets.Add(styleSheet);
    }

    public Port AddOutputPort(string name, Port.Capacity capacity = Port.Capacity.Single){
        Port outputPort = GetPortInstance(Direction.Output, capacity);
        outputPort.portName = name;
        outputContainer.Add(outputPort);
        return outputPort;
    }

    public Port AddInputPort(string name, Port.Capacity capacity = Port.Capacity.Multi){
        Port inputPort = GetPortInstance(Direction.Input, capacity);
        inputPort.portName = name;
        inputContainer.Add(inputPort);
        return inputPort;
    }

    public Port GetPortInstance(Direction nodeDir, Port.Capacity capacity = Port.Capacity.Single){
        return InstantiatePort(Orientation.Horizontal, nodeDir, capacity, typeof(float));
    }

    public virtual void LoadValueIntoField() {}

    public virtual void ReloadLanguage() {
        foreach(LanguageGenericHolderText textHolder in languageGenericListTexts){
            ReloadTextLanguage(textHolder.inputText, textHolder.textField, textHolder.placeholderText);
        }
        foreach(LanguageGenericHolderAudioClip audioHolder in languageGenericListAudioClips){
            ReloadAudioClipLanguage(audioHolder.inputAudioClip, audioHolder.objectField);
        }
    }

    protected void ReloadTextLanguage(List<LanguageGeneric<string>> inputText, TextField textField, string placeholderText = "placeholder"){
        // Reload text
        textField.RegisterValueChangedCallback(value => {
            inputText.Find(text => text.languageType == editorWindow.LanguageType).languageGenericType = value.newValue;
        });
        textField.SetValueWithoutNotify(inputText.Find(text => text.languageType == editorWindow.LanguageType).languageGenericType);
        SetPlaceholderText(textField, placeholderText);
    }

    protected void ReloadAudioClipLanguage(List<LanguageGeneric<AudioClip>> inputAudioClip, ObjectField objectField){
        // Reload audio
        objectField.RegisterValueChangedCallback(value => {
            inputAudioClip.Find(audioClip => audioClip.languageType == editorWindow.LanguageType).languageGenericType = value.newValue as AudioClip;
        });
        objectField.SetValueWithoutNotify(inputAudioClip.Find(audioClip => audioClip.languageType == editorWindow.LanguageType).languageGenericType);
    }

    protected void SetPlaceholderText(TextField textField, string placeholder){
        string placeholderClass = TextField.ussClassName + "__placeholder";

        CheckForText();
        onFocusOut();
        textField.RegisterCallback<FocusInEvent>(evt => onFocusIn());
        textField.RegisterCallback<FocusOutEvent>(evt => onFocusOut());

        void onFocusIn(){
            if(textField.ClassListContains(placeholderClass)){
                textField.value = string.Empty;
                textField.RemoveFromClassList(placeholderClass);
            }
        }

        void onFocusOut(){
            if(string.IsNullOrEmpty(textField.text)){
                textField.SetValueWithoutNotify(placeholder);
                textField.AddToClassList(placeholderClass);
            }
        }

        void CheckForText() {
            if(!string.IsNullOrEmpty(textField.text)){
                textField.RemoveFromClassList(placeholderClass);
            }
        }
    }

    #region Language Generic Holder Class

    class LanguageGenericHolderText {

        public List<LanguageGeneric<string>> inputText;
        public TextField textField;
        public string placeholderText;

        public LanguageGenericHolderText(List<LanguageGeneric<string>> inputText, TextField textField, string placeholderText = "placeholder"){
            this.inputText = inputText;
            this.textField = textField;
            this.placeholderText = placeholderText;
        }
        
    }

    class LanguageGenericHolderAudioClip {

        public List<LanguageGeneric<AudioClip>> inputAudioClip;
        public ObjectField objectField;

        public LanguageGenericHolderAudioClip(List<LanguageGeneric<AudioClip>> inputAudioClip, ObjectField objectField){
            this.inputAudioClip = inputAudioClip;
            this.objectField = objectField;
        }

    }

    #endregion
}
