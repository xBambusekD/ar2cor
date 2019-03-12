using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideSpecifiedObjectOnClick : MonoBehaviour, IInputHandler {

    [SerializeField]
    private List<GameObject> ObjectsToHide = new List<GameObject>();
    
    public void OnInputDown(InputEventData eventData) {
        foreach(GameObject obj in ObjectsToHide) {
            obj.SetActive(!obj.activeSelf);
        }
    }

    public void OnInputUp(InputEventData eventData) {
        
    }
}
