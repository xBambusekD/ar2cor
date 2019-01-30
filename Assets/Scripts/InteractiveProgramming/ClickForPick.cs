using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickForPick : MonoBehaviour, IInputClickHandler {


    public void OnInputClicked(InputClickedEventData eventData) {
        PickFromFeederIP.Instance.MarkClickedArea(GetComponent<DetectedObject>());

        //if(PickFromFeederIP.Instance.StateLearning) {
        //    if (pointedArea == null) {
        //        pointedArea = Instantiate(pointerToSpawn, cursor.transform.position, cursor.transform.rotation);
        //    }
        //    else {
        //        pointedArea.transform.position = cursor.transform.position;
        //        pointedArea.transform.rotation = cursor.transform.rotation;
        //    }
        //}
    }

}
