using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.Receivers;
using HoloToolkit.Unity.UX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnparentObjectOnClick : MonoBehaviour, IInputHandler {

    public GameObject calibrationCube;
    private GameObject parent;

    private void Start() {
        parent = gameObject.transform.parent.gameObject;
    }

    public void OnInputDown(InputEventData eventData) {
        UISoundManager.Instance.PlayHololensClick();

        if(transform.parent != null) {
            transform.parent = null;
            CalibManager.Instance.AddDetectedMarker(gameObject);
            calibrationCube.GetComponent<VisualizationCubeColorChanger>().ColorizeCube();
        }
        else {
            RefreshObject();
            CalibManager.Instance.RemoveDetectedMarker(gameObject);
            calibrationCube.GetComponent<VisualizationCubeColorChanger>().UncolorizeCube();
        }
    }

    public void OnInputUp(InputEventData eventData) {
    }

    public void RefreshObjectAndHide() {
        RefreshObject();
        HideComponentsInChildren();
    }

    private void RefreshObject() {
        transform.parent = parent.transform;
        transform.localPosition = new Vector3(0f, 0f, 0f);
        transform.localEulerAngles = new Vector3(-90f, 0f, 0f);
        calibrationCube.GetComponent<VisualizationCubeColorChanger>().UncolorizeCube();
    }

    private void HideComponentsInChildren() {
        var rendererComponents = transform.parent.GetComponentsInChildren<Renderer>(true);
        var colliderComponents = transform.parent.GetComponentsInChildren<Collider>(true);
        var canvasComponents = transform.parent.GetComponentsInChildren<Canvas>(true);

        // Disable rendering:
        foreach (var component in rendererComponents)
            component.enabled = false;

        // Disable colliders:
        foreach (var component in colliderComponents)
            component.enabled = false;

        // Disable canvas':
        foreach (var component in canvasComponents)
            component.enabled = false;
    }

}
