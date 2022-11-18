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

    private DialogueData dialogueData = new DialogueData();
    public DialogueData DialogueData { get => dialogueData; set => dialogueData = value; }

    public DialogueNode() { }

    public DialogueNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView){
        
        editorWindow = _editorWindow;
        graphView = _graphView;

        StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Nodes/DialogueNodeStyleSheet");
        styleSheets.Add(styleSheet);
        
        title = "Dialogue";
        SetPosition(new Rect(_position, defaultNodeSize));
        nodeGuid = Guid.NewGuid().ToString();

        AddInputPort("Input", Port.Capacity.Multi);
        AddOutputPort("Continue");

        TopContainer();
    }

    private void TopContainer(){
        AddPortButton();
        AddDropdownMenu();
    }

    private void AddPortButton(){
        Button btn = new Button() {
            text = "Add Choice",
        };
        btn.AddToClassList("TopButton");

        btn.clicked += () => {
            AddChoicePort(this);
        };

        titleButtonContainer.Add(btn);
    }

    private void AddDropdownMenu(){
        ToolbarMenu menu = new ToolbarMenu();
        menu.text = "Add Content";

        menu.menu.AppendAction("Text", new Action<DropdownMenuAction>(x => TextLine()));
        menu.menu.AppendAction("Image", new Action<DropdownMenuAction>(x => ImagePic()));
        menu.menu.AppendAction("Name", new Action<DropdownMenuAction>(x => CharacterName()));

        titleButtonContainer.Add(menu);
    }

    #region Port

    public Port AddChoicePort(BaseNode baseNode, DialogueDataPort dialoguePort = null){
        Port port = GetPortInstance(Direction.Output);
        DialogueDataPort newDialoguePort = new DialogueDataPort();

        // Check if we load it in with values
        if (dialoguePort != null){
            newDialoguePort.inputGuid = dialoguePort.inputGuid;
            newDialoguePort.outputGuid = dialoguePort.outputGuid;
            newDialoguePort.portGuid = dialoguePort.portGuid;
        } else {
            newDialoguePort.portGuid = Guid.NewGuid().ToString();
        }

        // Delete button
        {
            Button deleteButton = new Button(() => DeletePort(baseNode, port)) {
                text = "X",
            };
            port.contentContainer.Add(deleteButton);
        }

        port.portName = newDialoguePort.portGuid;                      // We use portName as port ID
        Label portNameLabel = port.contentContainer.Q<Label>("type");   // Get Labal in port that is used to contain the port name.
        portNameLabel.AddToClassList("PortName");                       // Here we add a uss class to it so we can hide it in the editor window.

        // Set color of the port.
        port.portColor = Color.yellow;

        dialogueData.dialogueDataPorts.Add(newDialoguePort);

        baseNode.outputContainer.Add(port);

        // Refresh
        baseNode.RefreshPorts();
        baseNode.RefreshExpandedState();

        return port;
    }

    private void DeletePort(BaseNode node, Port port){
        DialogueDataPort tmp = dialogueData.dialogueDataPorts.Find(findPort => findPort.portGuid == port.portName);
        dialogueData.dialogueDataPorts.Remove(tmp);

        IEnumerable<Edge> portEdge = graphView.edges.ToList().Where(edge => edge.output == port);

        if (portEdge.Any()){
            Edge edge = portEdge.First();
            edge.input.Disconnect(edge);
            edge.output.Disconnect(edge);
            graphView.RemoveElement(edge);
        }

        node.outputContainer.Remove(port);

        // Refresh
        node.RefreshPorts();
        node.RefreshExpandedState();
    }

    #endregion

    #region Menu Dropdown

    public void TextLine(DialogueDataText data = null){
        DialogueDataText newDialogueBaseContainerText = new DialogueDataText();
        DialogueData.dialogueBaseContainers.Add(newDialogueBaseContainerText);

        // Add Container Box
        Box boxContainer = new Box();
        boxContainer.AddToClassList("DialogueBox");

        // Add Fields
        AddLabelAndButton(newDialogueBaseContainerText, boxContainer, "Text", "TextColor");
        AddTextField(newDialogueBaseContainerText, boxContainer);
        AddAudioClips(newDialogueBaseContainerText, boxContainer);

        // Load in data if it got any
        if (data != null) {
            // Guid ID
             newDialogueBaseContainerText.GuidID = data.GuidID;

            // Text
            foreach (LanguageGeneric<string> data_text in data.texts){
                foreach (LanguageGeneric<string> text in newDialogueBaseContainerText.text){
                    if (text.languageType == data_text.languageType){
                        text.languageGenericType = data_text.languageGenericType;
                    }
                }
            }

            // Audio
            foreach (LanguageGeneric<AudioClip> data_audioclip in data.audioClips){
                foreach (LanguageGeneric<AudioClip> audioclip in newDialogueBaseContainerText.audioClips){
                    if (audioclip.languageType == data_audioclip.languageType){
                        audioclip.languageGenericType = data_audioclip.languageGenericType;
                    }
                }
            }
        } else {
            // Make New Guid ID
            newDialogueBaseContainerText.GuidID.value = Guid.NewGuid().ToString();
        }

        // Reload the current selected language
        ReloadLanguage();

        mainContainer.Add(boxContainer);
    }

    public void ImagePic(DialogueDataImages data = null){
        DialogueDataImages dialogueImages = new DialogueDataImages();
        if (data != null){
            dialogueImages.sprite.Value = data.sprite.Value;
            dialogueImages.sprite.Value = data.sprite.Value;
        }
        
        dialogueData.dialogueBaseContainers.Add(dialogueImages);

        Box boxContainer = new Box();
        boxContainer.AddToClassList("DialogueBox");

        AddLabelAndButton(dialogueImages, boxContainer, "Image", "ImageColor");
        AddImages(dialogueImages, boxContainer);

        mainContainer.Add(boxContainer);
    }

    public void CharacterName(DialogueDataName data = null){
        DialogueDataName dialogueName = new DialogueDataName();
        if (data != null){
            dialogueName.characterName.value = data.characterName.value;
        }
        
        dialogueData.dialogueBaseContainers.Add(dialogueName);

        Box boxContainer = new Box();
        boxContainer.AddToClassList("CharacterNameBox");

        AddLabelAndButton(dialogueName, boxContainer, "Name", "NameColor");
        AddTextFieldCharacterName(dialogueName, boxContainer);

        mainContainer.Add(boxContainer);
    }

    #endregion

    #region Fields

    private void AddLabelAndButton(DialogueData_BaseContainer container, Box boxContainer, string labelName, string uniqueUSS = "")
        {
            Box topBoxContainer = new Box();
            topBoxContainer.AddToClassList("TopBox");

            // Label Name
            Label label_texts = GetNewLabel(labelName, "LabelText", uniqueUSS);

            // Remove button.
            Button btn = GetNewButton("X", "TextBtn");
            btn.clicked += () =>
            {
                DeleteBox(boxContainer);
                DialogueData.Dialogue_BaseContainers.Remove(container);
            };

            topBoxContainer.Add(label_texts);
            topBoxContainer.Add(btn);

            boxContainer.Add(topBoxContainer);
        }

        private void AddTextField_CharacterName(DialogueData_Name container, Box boxContainer)
        {
            TextField textField = GetNewTextField(container.CharacterName, "Name", "CharacterName");

            boxContainer.Add(textField);
        }

        private void AddTextField(DialogueData_Text container, Box boxContainer)
        {
            TextField textField = GetNewTextField_TextLanguage(container.Text, "Text areans", "TextBox");

            container.TextField = textField;

            boxContainer.Add(textField);
        }

        private void AddAudioClips(DialogueData_Text container, Box boxContainer)
        {
            ObjectField objectField = GetNewObjectField_AudioClipsLanguage(container.AudioClips, "AudioClip");

            container.ObjectField = objectField;

            boxContainer.Add(objectField);
        }

        private void AddImages(DialogueData_Images container, Box boxContainer)
        {
            Box ImagePreviewBox = new Box();
            Box ImagesBox = new Box();

            ImagePreviewBox.AddToClassList("BoxRow");
            ImagesBox.AddToClassList("BoxRow");

            // Set up Image Preview.
            Image leftImage = GetNewImage("ImagePreview", "ImagePreviewLeft");
            Image rightImage = GetNewImage("ImagePreview", "ImagePreviewRight");

            ImagePreviewBox.Add(leftImage);
            ImagePreviewBox.Add(rightImage);

            // Set up Sprite.
            ObjectField objectField_Left = GetNewObjectField_Sprite(container.Sprite_Left, leftImage, "SpriteLeft");
            ObjectField objectField_Right = GetNewObjectField_Sprite(container.Sprite_Right, rightImage, "SpriteRight");

            ImagesBox.Add(objectField_Left);
            ImagesBox.Add(objectField_Right);

            // Add to box container.
            boxContainer.Add(ImagePreviewBox);
            boxContainer.Add(ImagesBox);
        }

    #endregion


    #region Misc

    public override void ReloadLanguage(){
        base.ReloadLanguage();
    }

    public override void LoadValueInToField(){}

    #endregion
}
