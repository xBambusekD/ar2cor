using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickForPick : MonoBehaviour, IInputClickHandler {

    public void OnInputClicked(InputClickedEventData eventData) {
        if(InteractiveProgrammingManager.Instance.CurrentState == InteractiveProgrammingManager.ProgrammingManagerState.pick_from_feeder_learn &&
            FakeFeederObjectsPositions.CheckIfObjectIsInFeeder(GetComponent<DetectedObject>().type, ROSUnityCoordSystemTransformer.ConvertVector(GetComponent<DetectedObject>().position))) {
            PickFromFeederIP.Instance.MarkClickedArea(GetComponent<DetectedObject>());

            UISoundManager.Instance.PlaySnap();
        }
        else if(InteractiveProgrammingManager.Instance.CurrentState == InteractiveProgrammingManager.ProgrammingManagerState.pick_from_polygon_learn) {
            PickFromPolygonIP.Instance.MarkClickedArea(GetComponent<DetectedObject>());

            UISoundManager.Instance.PlaySnap();
        }
    }

}
