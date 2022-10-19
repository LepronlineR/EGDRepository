using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryImage : MonoBehaviour
{
    [SerializeField] Image image;

    public void SetImage(Sprite spr){
        image.sprite = spr;
    }

}
