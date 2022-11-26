using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThoughtBubble : MonoBehaviour
{
    [SerializeField] TMP_Text thoughtText;

    public void SetText(string str){
        thoughtText.text = str;
    }
}
