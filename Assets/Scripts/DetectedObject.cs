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

    private Renderer renderer;

    private bool triggered = false;
    private Collider triggering_collider;

    void Start() {
        wireframeMat = GetComponent<Renderer>().material;
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update () {
        transform.localPosition = position;
        
        //transform.localRotation = rotation;
        
        transform.localEulerAngles = new Vector3(-rotation.eulerAngles.x, rotation.eulerAngles.y, -rotation.eulerAngles.z);
        transform.localScale = bbox;

        if(triggered && !triggering_collider) {
            renderer.enabled = true;
            triggered = false;
        }
	}

    public void SetObject(Vector3 pos, Quaternion rot, Vector3 box, string obj_type, int id) {
        position = pos;
        rotation = rot;
        bbox = box;
        type = obj_type;
        _id = id;
    }

    public void OnFocusEnter() {
        if(InteractiveProgrammingManager.Instance.CurrentState == InteractiveProgrammingManager.ProgrammingManagerState.pick_from_feeder_learn && 
            FakeFeederObjectsPositions.CheckIfObjectIsInFeeder(type, ROSUnityCoordSystemTransformer.ConvertVector(position))) {
            wireframeMat.SetColor("_WireColor", blue);
            PickFromFeederIP.Instance.StaringAtObject(true);
        }
    }

    public void OnFocusExit() {
        wireframeMat.SetColor("_WireColor", green);
        if (InteractiveProgrammingManager.Instance.CurrentState == InteractiveProgrammingManager.ProgrammingManagerState.pick_from_feeder_learn) {
            PickFromFeederIP.Instance.StaringAtObject(false);
        }
    }

    void OnTriggerEnter(Collider col) {
        if (col.transform.parent.tag.Equals("manipulatable_object")) {
            renderer.enabled = false;
            triggered = true;
            triggering_collider = col;
        } 
        else if (col.tag.Equals("detected_object") && tag.Equals("feeder_object")) {
            renderer.enabled = false;
            triggered = true;
            triggering_collider = col;
        }
    }

    void OnTriggerStay(Collider col) {
        if (col.tag.Equals("detected_object") && tag.Equals("feeder_object")) {
            renderer.enabled = false;
            triggered = true;
            triggering_collider = col;
        }
    }

    void OnTriggerExit(Collider col) {
        if (col.transform.parent.tag.Equals("manipulatable_object")) {
            renderer.enabled = true;
        }
        else if (col.tag.Equals("detected_object") && tag.Equals("feeder_object")) {
            renderer.enabled = true;
        }
    }

}
