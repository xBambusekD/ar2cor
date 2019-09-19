using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideSpecifiedObjectOnClick : MonoBehaviour, IInputClickHandler {

    [SerializeField]
    private List<GameObject> ObjectsToHide = new List<GameObject>();

    public void OnInputClicked(InputClickedEventData eventData) {
        foreach (GameObject obj in ObjectsToHide) {
            obj.SetActive(!obj.activeSelf);
        }
    }

}
