using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue Start Tutorial Event", menuName = "Dialogue Event/Dialogue Start Tutorial Event", order = 0)]
public class DialogueEventStartTutorial : DialogueEventSO
{
    public override void RunEvent() {
        MainSystem.Instance.StartTutorial();
    }
}

