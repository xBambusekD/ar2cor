using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.Receivers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMenuHide : InteractionReceiver {

    protected override void InputClicked(GameObject obj, InputClickedEventData eventData) {
        //base.InputClicked(obj, eventData);

        Debug.Log(obj.name + " clicked");
    }
}
