using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue End Tutorial Event", menuName = "Dialogue Event/Dialogue End Tutorial Event", order = 0)]
public class DialogueEventEndTutorial : DialogueEventSO
{
    public override void RunEvent() {
        MainSystem.Instance.EndTutorial();
    }
}
