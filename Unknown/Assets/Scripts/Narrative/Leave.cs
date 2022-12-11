using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leave : MonoBehaviour
{

    [SerializeField] SceneManagerMult sm;
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player" && MainSystem.Instance.startedGame)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                sm.LoadTrueEnd();
            }
        }
    }

}
