using HoloToolkit.Unity;
using ROSBridgeLib.art_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceToPoseIP : Singleton<PlaceToPoseIP> {

    private InterfaceStateMsg interfaceStateMsg;
    private ProgramItemMsg programItemMsg;

    public bool StateLearning = false;
    
    private GameObject world_anchor;

    private Vector3 PlacePosition;
    private Quaternion PlaceOrientation;

    // Use this for initialization
    void Start () {
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

    public void SaveObjectPosition(Vector3 position, Quaternion rotation) {
        PlacePosition = position;
        PlaceOrientation = rotation;

        SaveToROS();
    }

    public void SaveToROS() {
        //save instruction to ROS

        PickFromFeederIP.Instance.SaveToROS();
    }
}
