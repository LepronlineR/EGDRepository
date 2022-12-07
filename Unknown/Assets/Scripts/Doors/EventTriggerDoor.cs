using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTriggerDoor : MonoBehaviour
{

    [SerializeField] List<Door> doors;
    [SerializeField] GameObject player;

    bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !activated)
        {
            foreach (Door door in doors)
            {
                if (!door.isOpen)
                {
                    door.Open(other.transform.position);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && !activated)
        {
            foreach (Door door in doors)
            {
                if (door.isOpen)
                {
                    door.Close();
                    // trigger event
                }
            }
        }
    }
}
