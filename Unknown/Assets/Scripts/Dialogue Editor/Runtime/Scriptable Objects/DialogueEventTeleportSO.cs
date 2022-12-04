using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue Teleport Event", menuName = "Dialogue Event/Dialogue Teleport Event", order = 0)]
public class DialogueEventTeleportSO : DialogueEventSO {

    [SerializeField] int ID;
    [SerializeField] PersonName person;

    public override void RunEvent() {
        GameEvents.Instance.Teleport(ID, person);
    }

}