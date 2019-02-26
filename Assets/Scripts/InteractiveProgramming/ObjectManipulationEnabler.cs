using HoloToolkit.Unity.UX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManipulationEnabler : MonoBehaviour {

    private BoundingBoxRig boundingBoxRig;
    private PlaceRotateConfirm placeRotateConfirm;

	// Use this for initialization
	void Awake () {
        boundingBoxRig = gameObject.GetComponent<BoundingBoxRig>();
        placeRotateConfirm = gameObject.GetComponent<PlaceRotateConfirm>();
	}
	
    public void EnableManipulation() {
        boundingBoxRig.enabled = true;
        placeRotateConfirm.enabled = true;
    }

    public void DisableManipulation() {
        boundingBoxRig.enabled = false;
        placeRotateConfirm.enabled = false;
    }
}
