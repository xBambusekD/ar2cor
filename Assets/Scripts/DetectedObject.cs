using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectedObject : MonoBehaviour {

    public Vector3 position;
    public Quaternion rotation;
    public Vector3 bbox;
    public string type;
    public int _id;
	
	// Update is called once per frame
	void Update () {
        transform.localPosition = position;
        
        //transform.localRotation = rotation;
        
        transform.localEulerAngles = new Vector3(-rotation.eulerAngles.x, rotation.eulerAngles.y, -rotation.eulerAngles.z);
        transform.localScale = bbox;
	}
}
