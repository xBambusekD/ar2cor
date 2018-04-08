using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScaler : MonoBehaviour, IFocusable {
    private bool shrink = true;
    private bool enlarge = false;
    private float normalScale = 0.09f;
    private float smallScale = 0.03f;
    private float scaleSpeed = 4f;
	
	// Update is called once per frame
	void Update () {
        if (SystemStarter.Instance.calibrated) {
            if (shrink) {
                transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(smallScale, smallScale, smallScale), Time.deltaTime * scaleSpeed);
            }
            if (enlarge) {
                transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(normalScale, normalScale, normalScale), Time.deltaTime * scaleSpeed);
            }
        }
        else { //if system is calibrating
            transform.localScale = new Vector3(normalScale, normalScale, normalScale);
        }
	}

    public void OnFocusEnter() {
        shrink = false;
        enlarge = true;
    }

    public void OnFocusExit() {
        shrink = true;
        enlarge = false;
    }
}
