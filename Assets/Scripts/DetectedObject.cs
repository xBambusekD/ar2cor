using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectedObject : MonoBehaviour, IFocusable {

    public Vector3 position;
    public Quaternion rotation;
    public Vector3 bbox;
    public string type;
    public int _id;

    private Material wireframeMat;
    private Color green = new Color32(0, 255, 0, 255);
    private Color blue = new Color32(131, 131, 255, 255);

    void Start() {
        wireframeMat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update () {
        transform.localPosition = position;
        
        //transform.localRotation = rotation;
        
        transform.localEulerAngles = new Vector3(-rotation.eulerAngles.x, rotation.eulerAngles.y, -rotation.eulerAngles.z);
        transform.localScale = bbox;
	}

    public void OnFocusEnter() {
        if(PickFromFeederIP.Instance.StateLearning) {
            wireframeMat.SetColor("_WireColor", blue);
        }
    }

    public void OnFocusExit() {
        if (PickFromFeederIP.Instance.StateLearning) {
            wireframeMat.SetColor("_WireColor", green);
        }
    }

}
