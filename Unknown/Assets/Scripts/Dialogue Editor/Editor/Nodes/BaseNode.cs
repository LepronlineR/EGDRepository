using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class BaseNode : Node {

    protected string nodeGuid;
    protected DialogueGraphView graphView;
    protected DialogueEditorWindow editorWindow;
    protected Vector2 defaultNodeSize = new Vector2(200, 250);

    private List<LanguageGenericHolderText> languageGenericListTexts = new List<LanguageGenericHolderText>();
    private List<LanguageGenericHolderAudioClip> languageGenericListAudioClips = new List<LanguageGenericHolderAudioClip>();

    public string NodeGuid { get => nodeGuid; set => nodeGuid = value; }

    public BaseNode() { 
        StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Nodes/NodeStyleSheet");
        styleSheets.Add(styleSheet);
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

    #region Add to Build

    protected void AddStringModifierEventBuild(List<EventDataStringModifier> stringEventModifier, EventDataStringModifier stringEvent = null){
        EventDataStringModifier temp = new EventDataStringModifier();
        if(stringEvent != null){
            temp.stringEventText.value = stringEvent.stringEventText.value;
            temp.number.value = stringEvent.number.value;
            temp.stringEventModifierType.value = stringEvent.stringEventModifierType.value;
        }

        stringEventModifier.Add(temp);

        Box boxContainer = new Box();
        Box boxFloatField = new Box();
        boxContainer.AddToClassList("StringEventBox");
        boxFloatField.AddToClassList("StringEventBoxFloatField");

        // Text
        TextField textField = GetNewTextField(temp.stringEventText, "String Event", "StringEventText");

        // ID
        UnityEngine.UIElements.FloatField floatField = GetNewFloatField(temp.number, "StringEventInt");

        UnityEngine.UIElements.EnumField enumField = null;

        // String Event Modifier
        Action tmp = () => ShowHideStringEventModifierType(temp.stringEventModifierType.value, boxFloatField);
        // EnumField String Event Modifier
        enumField = GetNewEnumFieldStringEventModifierType(temp.stringEventModifierType, tmp, "StringEventEnum");
        // Run the show and hide.
        ShowHideStringEventModifierType(temp.stringEventModifierType.value, boxFloatField);

        // Remove button.
        Button btn = GetNewButton("X", "RemoveButton");
        btn.clicked += () => {
            stringEventModifier.Remove(temp);
            DeleteBox(boxContainer);
        };

        // Add it to the box
        boxContainer.Add(textField);
        boxContainer.Add(enumField);
        boxFloatField.Add(floatField);
        boxContainer.Add(boxFloatField);
        boxContainer.Add(btn);

        mainContainer.Add(boxContainer);
        RefreshExpandedState();
    }

    protected void AddStringConditionEventBuild(List<EventDataStringCondition> stringEventCondition, EventDataStringCondition stringEvent = null){
        EventDataStringCondition temp = new EventDataStringCondition();
        if(stringEvent != null){
            temp.stringEventText.value = stringEvent.stringEventText.value;
            temp.number.value = stringEvent.number.value;
            temp.stringEventConditionType.value = stringEvent.stringEventConditionType.value;
        }

        stringEventCondition.Add(temp);

        Box boxContainer = new Box();
        Box boxFloatField = new Box();
        boxContainer.AddToClassList("StringEventBox");
        boxFloatField.AddToClassList("StringEventBoxFloatField");

        // Text
        TextField textField = GetNewTextField(temp.stringEventText, "String Event", "StringEventText");

        // ID
        UnityEngine.UIElements.FloatField floatField = GetNewFloatField(temp.number, "StringEventInt");

        UnityEngine.UIElements.EnumField enumField = null;

        // String Event Modifier
        Action tmp = () => ShowHideStringEventConditionType(temp.stringEventConditionType.value, boxFloatField);
        // EnumField String Event Modifier
        enumField = GetNewEnumFieldStringEventConditionType(temp.stringEventConditionType, tmp, "StringEventEnum");
        // Run the show and hide.
        ShowHideStringEventConditionType(temp.stringEventConditionType.value, boxFloatField);

        // Remove button.
        Button btn = GetNewButton("X", "RemoveButton");
        btn.clicked += () => {
            stringEventCondition.Remove(temp);
            DeleteBox(boxContainer);
        };

        // Add it to the box
        boxContainer.Add(textField);
        boxContainer.Add(enumField);
        boxFloatField.Add(floatField);
        boxContainer.Add(boxFloatField);
        boxContainer.Add(btn);

        mainContainer.Add(boxContainer);
        RefreshExpandedState();
    }

    #endregion

    #region Ports

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

    #endregion

    #region Button/Label Support

    protected Label GetNewLabel(string labelName, string USS01 = "", string USS02 = ""){
        Label labels = new Label(labelName);
        labels.AddToClassList(USS01);
        labels.AddToClassList(USS02);
        return labels;
    }

    protected Button GetNewButton(string btnName, string USS01 = "", string USS02 = ""){
        Button btn = new Button() {
            text = btnName,
        };
        btn.AddToClassList(USS01);
        btn.AddToClassList(USS02);
        return btn;
    }

    #endregion

    #region Field Support

    protected UnityEngine.UIElements.IntegerField GetNewIntegerField(ContainerInt inputValue, string USS01 = "", string USS02 = ""){
        UnityEngine.UIElements.IntegerField integerField = new UnityEngine.UIElements.IntegerField();

        integerField.RegisterValueChangedCallback(value => {
            inputValue.value = value.newValue;
        });
        integerField.SetValueWithoutNotify(inputValue.value);

        integerField.AddToClassList(USS01);
        integerField.AddToClassList(USS02);

        return integerField;
    }

    protected UnityEngine.UIElements.FloatField GetNewFloatField(ContainerFloat inputValue, string USS01 = "", string USS02 = ""){
        UnityEngine.UIElements.FloatField floatField = new UnityEngine.UIElements.FloatField();

        floatField.RegisterValueChangedCallback(value => {
            inputValue.value = value.newValue;
        });
        floatField.SetValueWithoutNotify(inputValue.value);

        floatField.AddToClassList(USS01);
        floatField.AddToClassList(USS02);

        return floatField;
    }

    protected TextField GetNewTextField(ContainerString inputValue, string placeholderText, string USS01 = "", string USS02 = ""){
        TextField textField = new TextField();

        textField.RegisterValueChangedCallback(value => {
            inputValue.value = value.newValue;
        });
        textField.SetValueWithoutNotify(inputValue.value);

        textField.AddToClassList(USS01);
        textField.AddToClassList(USS02);

        SetPlaceholderText(textField, placeholderText);

        return textField;
    }

    protected Image GetNewImage(string USS01 = "", string USS02 = ""){
        Image imagePreview = new Image();

        imagePreview.AddToClassList(USS01);
        imagePreview.AddToClassList(USS02);

        return imagePreview;
    }

    protected ObjectField GetNewObjectFieldSprite(ContainerSprite inputValue, Image imagePreview, string USS01 = "", string USS02 = ""){
        ObjectField objectField = new ObjectField() {
            objectType = typeof(Sprite),
            allowSceneObjects = false,
            value = inputValue.value
        };

        objectField.RegisterValueChangedCallback(value => {
            inputValue.value = value.newValue as Sprite;
            imagePreview.image = (inputValue.value != null ? inputValue.value.texture : null);
        });
        imagePreview.image = (inputValue.value != null ? inputValue.value.texture : null);

        objectField.AddToClassList(USS01);
        objectField.AddToClassList(USS02);

        return objectField;
    }

    protected ObjectField GetNewObjectFieldDialogueEvent(ContainerDialogueEventSO inputValue, string USS01 = "", string USS02 = ""){
        ObjectField objectField = new ObjectField() {
            objectType = typeof(DialogueEventSO),
            allowSceneObjects = false,
            value = inputValue.dialogueEventSO
        };

        objectField.RegisterValueChangedCallback(value => {
            inputValue.dialogueEventSO = value.newValue as DialogueEventSO;
        });
        objectField.SetValueWithoutNotify(inputValue.dialogueEventSO);

        objectField.AddToClassList(USS01);
        objectField.AddToClassList(USS02);

        return objectField;
    }

    protected UnityEngine.UIElements.EnumField GetNewEnumFieldChoiceStateType(ContainerChoiceStateType inputValue, string USS01 = "", string USS02 = ""){
        UnityEngine.UIElements.EnumField enumField = new UnityEngine.UIElements.EnumField() {
            value = inputValue.value
        };
        enumField.Init(enumField.value);

        enumField.RegisterValueChangedCallback(value => {
            inputValue.value = (ChoiceStateType) value.newValue;
        });
        enumField.SetValueWithoutNotify(inputValue.value);

        enumField.AddToClassList(USS01);
        enumField.AddToClassList(USS02);

        inputValue.enumField = enumField;
        return enumField;
    }

    protected UnityEngine.UIElements.EnumField GetNewEnumFieldEndNodeType(ContainerEndNodeType inputValue, string USS01 = "", string USS02 = ""){
        UnityEngine.UIElements.EnumField enumField = new UnityEngine.UIElements.EnumField() {
            value = inputValue.value
        };
        enumField.Init(enumField.value);

        enumField.RegisterValueChangedCallback(value => {
            inputValue.value = (EndNodeType) value.newValue;
        });
        enumField.SetValueWithoutNotify(inputValue.value);

        enumField.AddToClassList(USS01);
        enumField.AddToClassList(USS02);

        inputValue.enumField = enumField;
        return enumField;
    }

    protected UnityEngine.UIElements.EnumField GetNewEnumFieldStringEventModifierType(ContainerStringEventModifierType inputValue, Action action, string USS01 = "", string USS02 = ""){
        UnityEngine.UIElements.EnumField enumField = new UnityEngine.UIElements.EnumField() {
            value = inputValue.value
        };
        enumField.Init(enumField.value);

        enumField.RegisterValueChangedCallback(value => {
            inputValue.value = (StringEventModifierType) value.newValue;
            action?.Invoke();
        });
        enumField.SetValueWithoutNotify(inputValue.value);

        enumField.AddToClassList(USS01);
        enumField.AddToClassList(USS02);

        inputValue.enumField = enumField;
        return enumField;
    }

    protected UnityEngine.UIElements.EnumField GetNewEnumFieldStringEventConditionType(ContainerStringEventConditionType inputValue, Action action, string USS01 = "", string USS02 = ""){
        UnityEngine.UIElements.EnumField enumField = new UnityEngine.UIElements.EnumField() {
            value = inputValue.value
        };
        enumField.Init(enumField.value);

        enumField.RegisterValueChangedCallback(value => {
            inputValue.value = (StringEventConditionType) value.newValue;
            action?.Invoke();
        });
        enumField.SetValueWithoutNotify(inputValue.value);

        enumField.AddToClassList(USS01);
        enumField.AddToClassList(USS02);

        inputValue.enumField = enumField;
        return enumField;
    }

    #endregion

    #region Custom Field Support

    protected TextField GetNewTextFieldTextLanguage(List<LanguageGeneric<string>> text, string placeholderText = "", string USS01 = "", string USS02 = ""){
        // Add languages
        foreach(LanguageType language in (LanguageType[])Enum.GetValues(typeof(LanguageType))){
            text.Add(new LanguageGeneric<string>{
                languageType = language,
                languageGenericType = ""
            });
        }

        // Make textfield
        TextField textField = new TextField("");

        // Add it to the language list
        languageGenericListTexts.Add(new LanguageGenericHolderText(text, textField, placeholderText));

        textField.RegisterValueChangedCallback(value => {
            text.Find(t => t.languageType == editorWindow.LanguageType).languageGenericType = value.newValue;
        });
        textField.SetValueWithoutNotify(text.Find(t => t.languageType == editorWindow.LanguageType).languageGenericType);

        textField.multiline = true;

        textField.AddToClassList(USS01);
        textField.AddToClassList(USS02);

        return textField;
    }

    protected ObjectField GetNewObjectFieldAudioClipLanguage(List<LanguageGeneric<AudioClip>> audioClips, string USS01 = "", string USS02 = ""){
        // Add languages
        foreach(LanguageType language in (LanguageType[])Enum.GetValues(typeof(LanguageType))){
            audioClips.Add(new LanguageGeneric<AudioClip>{
                languageType = language,
                languageGenericType = null
            });
        }

        // Make objectfield
        ObjectField objectField = new ObjectField() {
            objectType = typeof(AudioClip),
            allowSceneObjects = false,
            value = audioClips.Find(ac => ac.languageType == editorWindow.LanguageType).languageGenericType,
        };

        // Add it to the language list
        languageGenericListAudioClips.Add(new LanguageGenericHolderAudioClip(audioClips, objectField));

        objectField.RegisterValueChangedCallback(value => {
            audioClips.Find(ac => ac.languageType == editorWindow.LanguageType).languageGenericType = value.newValue as AudioClip;
        });
        objectField.SetValueWithoutNotify(audioClips.Find(ac => ac.languageType == editorWindow.LanguageType).languageGenericType);

        objectField.AddToClassList(USS01);
        objectField.AddToClassList(USS02);

        return objectField;
    }

    #endregion

    #region Reloading Language

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

    #endregion

    #region Show/Hide Boxes

    private void ShowHideStringEventModifierType(StringEventModifierType value, Box boxContainer){
        if (value == StringEventModifierType.SetTrue || value == StringEventModifierType.SetFalse){
            ShowHide(false, boxContainer);
        } else {
            ShowHide(true, boxContainer);
        }
    }

    private void ShowHideStringEventConditionType(StringEventConditionType value, Box boxContainer){
        if (value == StringEventConditionType.True || value == StringEventConditionType.False){
            ShowHide(false, boxContainer);
        } else {
            ShowHide(true, boxContainer);
        }
    }

    protected void ShowHide(bool show, Box boxContainer){
        string hideUssClass = "Hide";
        if (show == true){
            boxContainer.RemoveFromClassList(hideUssClass);
        } else {
            boxContainer.AddToClassList(hideUssClass);
        }
    }

    protected virtual void DeleteBox(Box boxContainer){
        mainContainer.Remove(boxContainer);
        RefreshExpandedState();
    }

    #endregion

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
