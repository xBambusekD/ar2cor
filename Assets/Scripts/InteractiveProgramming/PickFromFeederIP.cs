using HoloToolkit.Unity;
using ROSBridgeLib.actionlib_msgs;
using ROSBridgeLib.art_msgs;
using ROSBridgeLib.std_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickFromFeederIP : Singleton<PickFromFeederIP> {

    private InterfaceStateMsg interfaceStateMsg;
    private ProgramItemMsg programItemMsg;
    
    public GameObject pointerToSpawn;

    public GameObject BasicObjectManipulatablePrefab;
    public GameObject BasicObjectUnmanipulatablePrefab;

    private GameObject cursor;
    private Transform pointedArea;
    private GameObject world_anchor;

    private Vector3 robotArmPosition;
    private Quaternion robotArmRotation;
    private string objectTypeToPick;

    public GameObject robotArm;
    private GameObject objectToPick;

    // Use this for initialization
    void Start () {
        cursor = GameObject.FindGameObjectWithTag("cursor");
        world_anchor = GameObject.FindGameObjectWithTag("world_anchor");
    }

    // Update is called once per frame
    void Update() {
        //if (SystemStarter.Instance.calibrated) {
        //    if (interfaceStateMsg != null) {
        //        //pick from polygon editing
        //        if (interfaceStateMsg.GetSystemState() == 2 && programItemMsg.GetIType() == "PickFromFeeder" &&
        //            interfaceStateMsg.GetEditEnabled() == true) {
        //            StateLearning = true;
        //        }
        //        else {
        //            StateLearning = false;
        //        }
        //    }
        //}
    }


    public void StartLearning() {
        //TODO poslat READY request
        LearningRequstActionGoalMsg getReadyMsg = new LearningRequstActionGoalMsg(new HeaderMsg(0, ROSTimeHelper.GetCurrentTime(), ""), new GoalIDMsg(ROSTimeHelper.GetCurrentTime(), ""), 
            new LearningRequstGoalMsg(learning_request_goal.GET_READY));

        //TODO zmenit interface state


        //TODO pri ukonceni learningu poslat request DONE
        LearningRequstActionGoalMsg doneMsg = new LearningRequstActionGoalMsg(new HeaderMsg(0, ROSTimeHelper.GetCurrentTime(), ""), new GoalIDMsg(ROSTimeHelper.GetCurrentTime(), ""),
            new LearningRequstGoalMsg(learning_request_goal.DONE));
    }

    public void Visualize() {
        if(ProgramHelper.ItemLearned(programItemMsg)) {
            Debug.Log("Visualizing PICK_FROM_FEEDER");

            if (objectToPick != null)
                Destroy(objectToPick);

            //convert ros coordinate system to unity coordinate system
            Vector3 robotArmPosition = ROSUnityCoordSystemTransformer.ConvertVector(programItemMsg.GetPose()[0].GetPose().GetPosition().GetPoint());
            Quaternion robotArmRotation = ROSUnityCoordSystemTransformer.ConvertQuaternion(programItemMsg.GetPose()[0].GetPose().GetOrientation().GetQuaternion());

            //show robot arm
            robotArm.transform.localPosition = robotArmPosition;
            robotArm.transform.localRotation = robotArmRotation;
            robotArm.GetComponent<PR2GripperController>().SetArmActive(true);

            Vector3 objectPosition = FakeFeederObjectsPositions.GetObjectPositionInFeeder(programItemMsg.GetObject()[0],
                (robotArmPosition.x > MainMenuManager.Instance.currentSetup.GetTableWidth() / 2f ? FakeFeederObjectsPositions.FeederType.right_feeder : FakeFeederObjectsPositions.FeederType.left_feeder));
            Quaternion objectOrientation = FakeFeederObjectsPositions.GetObjectOrientationInFeeder(programItemMsg.GetObject()[0],
                (robotArmPosition.x > MainMenuManager.Instance.currentSetup.GetTableWidth() / 2f ? FakeFeederObjectsPositions.FeederType.right_feeder : FakeFeederObjectsPositions.FeederType.left_feeder));
            Vector3 objectDims = ObjectsManager.Instance.GetObjectTypeDimensions(programItemMsg.GetObject()[0]);

            objectToPick = Instantiate(BasicObjectUnmanipulatablePrefab, world_anchor.transform);
            objectToPick.transform.localPosition = ROSUnityCoordSystemTransformer.ConvertVector(objectPosition);
            objectToPick.transform.localRotation = ROSUnityCoordSystemTransformer.ConvertQuaternion(objectOrientation);
            objectToPick.transform.GetChild(0).transform.localScale = objectDims;
        }
    }

    public void VisualizeClear() {
        if (ProgramHelper.ItemLearned(programItemMsg)) {
            robotArm.GetComponent<PR2GripperController>().PlaceGripperToInit();
            robotArm.GetComponent<PR2GripperController>().SetArmActive(false);

            if (objectToPick != null)
                Destroy(objectToPick);
        }
    }
     
    public void SetInterfaceStateMsgFromROS(InterfaceStateMsg msg) {
        interfaceStateMsg = msg;
        programItemMsg = msg.GetProgramCurrentItem();
    }


    public void MarkClickedArea(DetectedObject detectedObject) {
        if (objectToPick != null) {
            Destroy(objectToPick);
        }

        pointedArea = cursor.transform;

        robotArmPosition = world_anchor.transform.InverseTransformPoint(pointedArea.position + pointedArea.forward * 0.2f);            
        robotArmRotation = Quaternion.Inverse(world_anchor.transform.rotation) * pointedArea.rotation;


        objectToPick = Instantiate(BasicObjectManipulatablePrefab, detectedObject.position, detectedObject.rotation);
        objectToPick.transform.GetChild(0).transform.localScale = detectedObject.bbox;
        objectToPick.transform.GetChild(0).GetComponent<Collider>().enabled = false;
        objectToPick.transform.parent = cursor.transform;
        objectToPick.transform.localPosition = new Vector3(0, 0, detectedObject.bbox.x/2);
        
        objectTypeToPick = detectedObject.type;

        SaveToROS();

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

    public void SaveToROS() {
        Debug.Log("PICK FROM FEEDER SAVED");
    }
}
