using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOpen = false;

    [SerializeField] float speed = 1.0f;
    [SerializeField] float rotationAmount = 90.0f;
    [SerializeField] float forwardDirection = 0.0f;

    private Vector3 startRotation;
    private Vector3 forward;

    private Coroutine AnimationCoroutine;

    void Awake() {
        startRotation = transform.rotation.eulerAngles;
        forward = transform.right;
    }

    public void Open(Vector3 userPos){
        if(!isOpen){
            if(AnimationCoroutine != null){
                StopCoroutine(AnimationCoroutine);
            }
            
            float dot = Vector3.Dot(forward, (userPos - transform.position).normalized);
            AnimationCoroutine = StartCoroutine(RotationOpen(dot));
        }
    }

    IEnumerator RotationOpen(float amount){
        Quaternion start = transform.rotation;
        Quaternion end;
        if(amount >= forwardDirection){
            end = Quaternion.Euler(new Vector3(0, start.y - rotationAmount, 0));
        } else {
            end = Quaternion.Euler(new Vector3(0, start.y + rotationAmount, 0));
        }

        isOpen = true;

        float time = 0;
        while(time < 1){
            transform.rotation = Quaternion.Slerp(start, end, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
    }

    public void Close(){
        if(isOpen){
            if(AnimationCoroutine != null){
                StopCoroutine(AnimationCoroutine);
            }
        
            AnimationCoroutine = StartCoroutine(RotationClose());
        }
    }

    IEnumerator RotationClose() {
        Quaternion start = transform.rotation;
        Quaternion end = Quaternion.Euler(startRotation);

        isOpen = false;

        float time = 0;
        while(time < 1){
            transform.rotation = Quaternion.Slerp(start, end, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
    }
}
