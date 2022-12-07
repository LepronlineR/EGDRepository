using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThoughtBubble : MonoBehaviour
{
    [SerializeField] TMP_Text thoughtText;

    public void SetText(string str){
        thoughtText.text = str;
        index = Random.Range(0.0f, omegaY);
    }

    private float amplitudeY = 12.0f;
    private float omegaY = 2.0f;
    float index;

    private void Update()
    {
        // wobble effect
        index += Time.deltaTime;
        float y = amplitudeY * Mathf.Sin(omegaY * index);
        transform.localPosition = new Vector3(0, y, 0);
    }
}
