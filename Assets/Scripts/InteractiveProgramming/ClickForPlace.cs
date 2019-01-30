using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickForPlace : MonoBehaviour, IInputClickHandler {
    
    public void OnInputClicked(InputClickedEventData eventData) {
        PlaceToPoseIP.Instance.MarkClickedArea();
    }
}
