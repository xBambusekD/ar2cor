using HoloToolkit.Unity;
using ROSBridgeLib.art_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickFromPolygonIP : Singleton<PickFromPolygonIP> {

    private InterfaceStateMsg interfaceStateMsg;
    private ProgramItemMsg programItemMsg;

    public GameObject BasicObjectManipulatablePrefab;
    //public GameObject BasicObjectUnmanipulatablePrefab;

    private GameObject cursor;
    private Transform pointedArea;
    private GameObject world_anchor;

    //private Vector3 robotArmPosition;
    //private Quaternion robotArmRotation;
    private string objectTypeToPick;

    //public GameObject robotArm;
    private GameObject objectToPick;
    //private GameObject objectToPickUnmanipulatable;

    private bool waiting_for_action_response = false;
    private string currentRequestID;


    // Start is called before the first frame update
    void Start() {
        cursor = GameObject.FindGameObjectWithTag("cursor");
        world_anchor = GameObject.FindGameObjectWithTag("world_anchor");
    }

    public void StartLearning() {


    }


    public void SetInterfaceStateMsgFromROS(InterfaceStateMsg msg) {
        interfaceStateMsg = msg;
        programItemMsg = msg.GetProgramCurrentItem();
    }

    public void MarkClickedArea(DetectedObject detectedObject) {

        //if (objectToPick != null) {
        //    Destroy(objectToPick);
        //}

        pointedArea = cursor.transform;

        //TODO mozna zalezi na strane feederu s tim forward vektorem..
        //robotArmPosition = world_anchor.transform.InverseTransformPoint(pointedArea.localPosition + pointedArea.forward * 0.3f);
        //robotArmRotation = Quaternion.Inverse(world_anchor.transform.localRotation) * pointedArea.localRotation;

        //bool picking_right_feeder = false;
        ////rotate gripper to face feeder
        ////picking from right feeder
        //if (robotArmPosition.x > MainMenuManager.Instance.currentSetup.GetTableWidth() / 2) {
        //    Debug.Log("PICKING FROM RIGHT FEEDER");
        //    robotArmRotation.eulerAngles += new Vector3(90, 0, 0);
        //    picking_right_feeder = true;
        //}
        ////picking from left feeder
        //else {
        //    Debug.Log("PICKING FROM LEFT FEEDER");
        //    robotArmRotation.eulerAngles += new Vector3(-90, 0, 0);
        //    picking_right_feeder = false;
        //}


        objectToPick = Instantiate(BasicObjectManipulatablePrefab, detectedObject.position, detectedObject.rotation);
        objectToPick.GetComponent<ObjectManipulationEnabler>().EnableManipulation();
        objectToPick.transform.GetChild(0).transform.localScale = detectedObject.bbox;
        objectToPick.transform.GetChild(0).GetComponent<Collider>().enabled = false;
        objectToPick.transform.parent = world_anchor.transform;
        //objectToPick.transform.localEulerAngles = picking_right_feeder ? new Vector3(90f, 90f, -90f) : new Vector3(-90f, 90f, -90f);
        //objectToPick.GetComponent<PlaceRotateConfirm>().Arm =
        //    picking_right_feeder ? RobotHelper.RobotArmType.LEFT_ARM : RobotHelper.RobotArmType.RIGHT_ARM;
        objectToPick.GetComponent<PlaceRotateConfirm>().snapLocalRotation = objectToPick.transform.rotation;

        objectToPick.transform.parent = cursor.transform;
        objectToPick.transform.localPosition = new Vector3(0, 0, detectedObject.bbox.x / 2);

        objectTypeToPick = detectedObject.type;

        objectToPick.GetComponent<PlaceRotateConfirm>().object_attached = true;

        PlaceToPoseIP.Instance.PassObjectToPlaceReference(objectToPick);

        //StartCoroutine(SaveToROS());
    }

}
