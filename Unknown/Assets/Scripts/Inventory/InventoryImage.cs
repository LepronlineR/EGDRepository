using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryImage : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TMP_InputField input;
    [SerializeField] GameObject selectedImage;

    private List<GameObject> evidences;

    public void SetImage(Sprite spr){
        image.sprite = spr;
    }

    public void SetImageText(string str){
        input.text = str;
    }

    public void DestroyThis(){
        Destroy(this.gameObject);
    }

    public void AddAllEvidences(List<GameObject> e){
        evidences = e;
    }

    public bool ContainsEvidence(GameObject evidence){
        return evidences.Contains(evidence);
    }

    public void SetAsCurrentImage(){
        // highlight this
        selectedImage.SetActive(true);
        // set this
        MainSystem.Instance.SetCurrentImage(this);
    }
}
