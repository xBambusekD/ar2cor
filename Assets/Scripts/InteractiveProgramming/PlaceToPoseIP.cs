using HoloToolkit.Unity;
using ROSBridgeLib.art_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceToPoseIP : Singleton<PlaceToPoseIP> {

    private InterfaceStateMsg interfaceStateMsg;
    private ProgramItemMsg programItemMsg;

    public bool StateLearning = false;

    public GameObject pointerToSpawn;

    private GameObject cursor;
    private GameObject pointedArea;
    private GameObject world_anchor;

    private GameObject objectToPlace;
    private Vector3 snapLocalPosition;

    // Use this for initialization
    void Start () {
        cursor = GameObject.FindGameObjectWithTag("cursor");
        world_anchor = GameObject.FindGameObjectWithTag("world_anchor");
    }
	
	// Update is called once per frame
	void Update () {
        if (SystemStarter.Instance.calibrated) {
            if (interfaceStateMsg != null) {
                //pick from polygon editing
                if (interfaceStateMsg.GetSystemState() == 2 && programItemMsg.GetIType() == "PlaceToPose" &&
                    interfaceStateMsg.GetEditEnabled() == true) {
                    StateLearning = true;
                }
                else {
                    StateLearning = false;
                }
            }
        }
    }

    public void SetInterfaceStateMsgFromROS(InterfaceStateMsg msg) {
        interfaceStateMsg = msg;
        programItemMsg = msg.GetProgramCurrentItem();
    }

    public void MarkClickedArea() {
        if (objectToPlace == null) {
            //pointedArea = Instantiate(pointerToSpawn, cursor.transform.position, cursor.transform.rotation);

            foreach(Transform tr in cursor.transform) {
                if (tr.tag == "manipulatable_object") {
                    objectToPlace = tr.gameObject;
                }
            }

            snapLocalPosition = objectToPlace.transform.localPosition;

        }

        if (objectToPlace.transform.parent == cursor.transform) {

            objectToPlace.transform.parent = world_anchor.transform;
            objectToPlace.transform.GetChild(0).GetComponent<Collider>().enabled = true;
            //pointedArea.transform.position = cursor.transform.position;
            //pointedArea.transform.rotation = cursor.transform.rotation;
        }
        else {
            objectToPlace.transform.parent = cursor.transform;
            objectToPlace.transform.GetChild(0).GetComponent<Collider>().enabled = false;
            objectToPlace.transform.localPosition = snapLocalPosition;
        }
    }
}
