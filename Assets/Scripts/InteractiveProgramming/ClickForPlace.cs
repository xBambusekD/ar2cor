using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickForPlace : MonoBehaviour, IInputClickHandler {

    public delegate void ClickAction();
    public static event ClickAction OnClicked;

    public void OnInputClicked(InputClickedEventData eventData) {
        if(OnClicked != null)
            OnClicked();
    }
}
