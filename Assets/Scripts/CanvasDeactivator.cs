using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.UX;
using UnityEngine.UI;

public class CanvasDeactivator : MonoBehaviour {

    //Script deactivates GraphicRaycaster to interaction with canvas when whole window is in adjust mode

    public BoundingBoxRig boundingBoxRig;

    private GraphicRaycaster graphicRaycaster;

	// Use this for initialization
	void Start () {
        graphicRaycaster = this.GetComponent<GraphicRaycaster>();
	}
	
	// Update is called once per frame
	void Update () {
		if(boundingBoxRig.appBarInstance.State == AppBar.AppBarStateEnum.Manipulation) {
            graphicRaycaster.enabled = false;
        }
        else {
            graphicRaycaster.enabled = true;
        }
	}
}
