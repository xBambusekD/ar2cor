using HoloToolkit.Unity;
using ROSBridgeLib.art_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickFromFeederIP : Singleton<PickFromFeederIP> {

    private InterfaceStateMsg interfaceStateMsg;
    private ProgramItemMsg programItemMsg;

    public bool StateLearning = false;

    public GameObject pointerToSpawn;

    public GameObject BasicObjectManipulatablePrefab;

    private GameObject cursor;
    private GameObject pointedArea;
    private GameObject world_anchor;

    private Vector3 robotArmPosition;
    private Quaternion robotArmRotation;

    public GameObject arm;
    private GameObject objectToPlace;

    // Use this for initialization
    void Start () {
        cursor = GameObject.FindGameObjectWithTag("cursor");
        world_anchor = GameObject.FindGameObjectWithTag("world_anchor");
    }

    // Update is called once per frame
    void Update() {
        if (SystemStarter.Instance.calibrated) {
            if (interfaceStateMsg != null) {
                //pick from polygon editing
                if (interfaceStateMsg.GetSystemState() == 2 && programItemMsg.GetIType() == "PickFromFeeder" &&
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


    public void MarkClickedArea(DetectedObject detectedObject) {
        if (pointedArea == null) {
            pointedArea = Instantiate(pointerToSpawn, cursor.transform.position, cursor.transform.rotation);

            robotArmPosition = world_anchor.transform.InverseTransformPoint(pointedArea.transform.position + pointedArea.transform.forward * 0.2f);            
            robotArmRotation = Quaternion.Inverse(world_anchor.transform.rotation) * pointedArea.transform.rotation;


            objectToPlace = Instantiate(BasicObjectManipulatablePrefab, detectedObject.position, detectedObject.rotation);
            objectToPlace.transform.GetChild(0).transform.localScale = detectedObject.bbox;
            objectToPlace.transform.GetChild(0).GetComponent<Collider>().enabled = false;
            objectToPlace.transform.parent = cursor.transform;
            objectToPlace.transform.localPosition = new Vector3(0, 0, detectedObject.bbox.x/2);


            //GameObject robot_arm = Instantiate(arm, world_anchor.transform.position, world_anchor.transform.rotation);
            //robot_arm.transform.localPosition = robotArmPosition;
            //robot_arm.transform.localRotation = robotArmRotation;

            //GameObject child = Instantiate(pointerToSpawn, pointedArea.transform.position + pointedArea.transform.forward * 0.2f, pointedArea.transform.rotation);
            //child.transform.parent = pointedArea.transform;
            //child.transform.localPosition += new Vector3(0f, 0f, 0.2f);

            //InterfaceStateMsg msg = new InterfaceStateMsg("PROJECTED UI", interfaceStateMsg.GetSystemState(), interfaceStateMsg.GetTimestamp(),
            //interfaceStateMsg.GetProgramID(), blockID, programItemMsg, interfaceStateMsg.GetFlags(), interfaceStateMsg.GetEditEnabled(),
            //interfaceStateMsg.GetErrorSeverity(), interfaceStateMsg.GetErrorCode());

            //ROSCommunicationManager.Instance.ros.Publish(InterfaceStatePublisher.GetMessageTopic(), msg);
        }
        else {
            pointedArea.transform.position = cursor.transform.position;
            pointedArea.transform.rotation = cursor.transform.rotation;
        }
    }
}
