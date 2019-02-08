using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickForPick : MonoBehaviour, IInputClickHandler {

    public void OnInputClicked(InputClickedEventData eventData) {
        if(InteractiveProgrammingManager.Instance.CurrentState == InteractiveProgrammingManager.ProgrammingManagerState.pick_from_feeder_learn) {
            PickFromFeederIP.Instance.MarkClickedArea(GetComponent<DetectedObject>());
        }
    }

}
