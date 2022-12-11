using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] List<AudioClip> clipList = new List<AudioClip>();
    [SerializeField] AudioSource audioSource;

    int selected = -1;

    void Awake()
    {
        StartCoroutine(PlayMusic());
    }

    IEnumerator PlayMusic()
    {
        // select random
        selected = Random.Range(0, clipList.Count);
        audioSource.clip = clipList[selected];
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        StartCoroutine(PlayMusic());
    }
}
