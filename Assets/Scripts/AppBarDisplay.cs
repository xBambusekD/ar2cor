using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.Receivers;
using HoloToolkit.Unity.UX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppBarDisplay : MonoBehaviour, IInputHandler {

    private AppBar appBarInstance;
    private CollisionPrimitive colPrimitive;

    private void Start() {
        colPrimitive = GetComponentInParent<CollisionPrimitive>();
        appBarInstance = GetComponent<BoundingBoxRig>().appBarInstance;
    }

    public void OnInputDown(InputEventData eventData) {
        if(appBarInstance.State == AppBar.AppBarStateEnum.Hidden) {
            appBarInstance.State = AppBar.AppBarStateEnum.Default;
        }
        //appBarInstance.gameObject.SetActive(true);

        CollisionEnvironmentManager.Instance.manipulatingWithObject = true;
    }

    public void OnInputUp(InputEventData eventData) {
        CollisionEnvironmentManager.Instance.manipulatingWithObject = false;
        colPrimitive.UpdatePositionToROS();
    }
}
