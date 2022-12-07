using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class InventoryImage : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TMP_InputField input;
    [SerializeField] GameObject selectedImage;

    private List<GameObject> evidences;

    UnityAction<string> valueChangeAction;
    
    void Start() {
        valueChangeAction = (string str) => { ValueChangeCheck(); };
    }

    public TMP_InputField Input { get => input; set => input = value; }
    public Image Img { get => image; set => image = value; }

    public List<GameObject> Evidences { get => evidences; set => evidences = value; }

    // Invoked when the value of the text field changes.
    public void ValueChangeCheck(){
        MainSystem.Instance.ChangeTextForSetCurrentImage(input.text);
    }

    public void SetImage(Sprite spr){
        image.sprite = spr;
    }

    public void SetImageText(string str){
        input.text = str;
    }

    public void DestroyThis(){
        if(MainSystem.Instance.CurrentImageIsSelectedImage(this))
            MainSystem.Instance.RemoveImagePicture();
        Destroy(this.gameObject);
    }

    public void AddAllEvidences(List<GameObject> e){
        evidences = e;
    }

    public bool ContainsEvidence(GameObject evidence){
        return evidences.Contains(evidence);
    }

    public void DelectThisImage(){
        input.onValueChanged.RemoveListener(valueChangeAction);
        selectedImage.SetActive(false);
        evidences = null;
    }

    public void SetAsCurrentImage(){
        // highlight this
        selectedImage.SetActive(true);
        // set this
        input.onValueChanged.AddListener(valueChangeAction);
        MainSystem.Instance.SetCurrentImage(this);

    }
}
