using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralClick : MonoBehaviour, IInputClickHandler {

    public delegate void ClickAction();
    public static event ClickAction OnClicked;

    public void OnInputClicked(InputClickedEventData eventData) {
        if (OnClicked != null)
            OnClicked();
    }
}
