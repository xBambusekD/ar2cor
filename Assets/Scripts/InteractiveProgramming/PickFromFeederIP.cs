using HoloToolkit.Unity;
using ROSBridgeLib.actionlib_msgs;
using ROSBridgeLib.art_msgs;
using ROSBridgeLib.diagnostic_msgs;
using ROSBridgeLib.geometry_msgs;
using ROSBridgeLib.std_msgs;
using System.Collections;
using System.Collections.Generic;
//using System.Threading;
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
    private GameObject objectToPickUnmanipulatable;

    // Use this for initialization
    void Start () {
        cursor = GameObject.FindGameObjectWithTag("cursor");
        world_anchor = GameObject.FindGameObjectWithTag("world_anchor");
    }

    // Update is called once per frame
    void Update() {

    }


    public void StartLearning() {
        //TODO READY request se posle z GUI artablu
        //LearningRequstActionGoalMsg getReadyMsg = new LearningRequstActionGoalMsg(new HeaderMsg(0, ROSTimeHelper.GetCurrentTime(), ""), new GoalIDMsg(ROSTimeHelper.GetCurrentTime(), ""), 
        //    new LearningRequstGoalMsg(learning_request_goal.GET_READY));

        //change interface state to learning enabled
        //InterfaceStateMsg msg = new InterfaceStateMsg(
        //    "HOLOLENS", 
        //    InterfaceStateMsg.SystemState.STATE_LEARNING, 
        //    ROSTimeHelper.GetCurrentTime(), 
        //    interfaceStateMsg.GetProgramID(), 
        //    interfaceStateMsg.GetBlockID(), 
        //    programItemMsg, 
        //    interfaceStateMsg.GetFlags(), 
        //    true, 
        //    InterfaceStateMsg.ErrorSeverity.NONE, 
        //    InterfaceStateMsg.ErrorCode.ERROR_UNKNOWN);
        //ROSCommunicationManager.Instance.ros.Publish(InterfaceStatePublisher.GetMessageTopic(), msg);


        //TODO pri ukonceni learningu poslat request DONE
        //LearningRequstActionGoalMsg doneMsg = new LearningRequstActionGoalMsg(new HeaderMsg(0, ROSTimeHelper.GetCurrentTime(), ""), new GoalIDMsg(ROSTimeHelper.GetCurrentTime(), ""),
        //    new LearningRequstGoalMsg(learning_request_goal.DONE));
    }

    public void Visualize() {
        if(ProgramHelper.ItemLearned(programItemMsg)) {
            Debug.Log("Visualizing PICK_FROM_FEEDER");

            if (objectToPickUnmanipulatable != null)
                Destroy(objectToPickUnmanipulatable);

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

            objectToPickUnmanipulatable = Instantiate(BasicObjectUnmanipulatablePrefab, world_anchor.transform);
            objectToPickUnmanipulatable.transform.localPosition = ROSUnityCoordSystemTransformer.ConvertVector(objectPosition);
            objectToPickUnmanipulatable.transform.localRotation = ROSUnityCoordSystemTransformer.ConvertQuaternion(objectOrientation);
            objectToPickUnmanipulatable.transform.GetChild(0).transform.localScale = objectDims;
        }
    }

    public void VisualizeClear() {
        if (ProgramHelper.ItemLearned(programItemMsg)) {
            robotArm.GetComponent<PR2GripperController>().PlaceGripperToInit();
            robotArm.GetComponent<PR2GripperController>().SetArmActive(false);

            if (objectToPickUnmanipulatable != null)
                Destroy(objectToPickUnmanipulatable);
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

        robotArmPosition = world_anchor.transform.InverseTransformPoint(pointedArea.position + pointedArea.forward * 0.3f);            
        robotArmRotation = Quaternion.Inverse(world_anchor.transform.rotation) * pointedArea.rotation;
        //rotate gripper to face feeder
        robotArmRotation.eulerAngles += new Vector3(90f, 90f, 0);


        objectToPick = Instantiate(BasicObjectManipulatablePrefab, detectedObject.position, detectedObject.rotation);
        objectToPick.transform.GetChild(0).transform.localScale = detectedObject.bbox;
        objectToPick.transform.GetChild(0).GetComponent<Collider>().enabled = false;
        objectToPick.transform.parent = cursor.transform;
        objectToPick.transform.localPosition = new Vector3(0, 0, detectedObject.bbox.x/2);
        
        objectTypeToPick = detectedObject.type;

        SaveToROS();        
    }

    public void SaveToROS() {
        Debug.Log("PICK FROM FEEDER SAVED");

        //Save parameters for PICK_FROM_FEEDER
        InterfaceStateMsg i_msg = new InterfaceStateMsg(
            "HOLOLENS", 
            InterfaceStateMsg.SystemState.STATE_LEARNING, 
            ROSTimeHelper.GetCurrentTime(), 
            interfaceStateMsg.GetProgramID(), 
            interfaceStateMsg.GetBlockID(), 
            new ProgramItemMsg(
                programItemMsg.GetID(), 
                programItemMsg.GetOnSuccess(), 
                programItemMsg.GetOnFailure(), 
                programItemMsg.GetIType(), 
                programItemMsg.GetName(), 
                new List<string>() {objectTypeToPick}, 
                new List<PoseStampedMsg>() {
                    new PoseStampedMsg(new HeaderMsg(0, ROSTimeHelper.GetCurrentTime(), "marker"), 
                    new PoseMsg(new PointMsg(ROSUnityCoordSystemTransformer.ConvertVector(robotArmPosition)), 
                                new QuaternionMsg(ROSUnityCoordSystemTransformer.ConvertQuaternion(robotArmRotation)))) },
                new List<PolygonStampedMsg>(),
                programItemMsg.GetRefID(), 
                programItemMsg.GetFlags(), 
                programItemMsg.GetDoNotClear(), 
                programItemMsg.GetLabels()),
            interfaceStateMsg.GetFlags(), 
            true, 
            InterfaceStateMsg.ErrorSeverity.NONE, 
            InterfaceStateMsg.ErrorCode.ERROR_UNKNOWN);
        ROSCommunicationManager.Instance.ros.Publish(InterfaceStatePublisher.GetMessageTopic(), i_msg);

        //Request brain for goal DONE
        LearningRequstActionGoalMsg requestMsg = new LearningRequstActionGoalMsg(new HeaderMsg(0, ROSTimeHelper.GetCurrentTime(), ""), new GoalIDMsg(ROSTimeHelper.GetCurrentTime(), ""),
            new LearningRequstGoalMsg(learning_request_goal.DONE));
        ROSCommunicationManager.Instance.ros.Publish(LearningRequestActionGoalPublisher.GetMessageTopic(), requestMsg);

        //TODO POCKAT NA ODPOVED OD ACTION GOALU
        //Thread.Sleep(100);

        //Switch to next instruction (should be PLACE_TO_POSE)
        //InteractiveProgrammingManager.Instance.PlaceToPoseLearningOverride = true;
        i_msg = new InterfaceStateMsg(
            "HOLOLENS",
            InterfaceStateMsg.SystemState.STATE_LEARNING,
            ROSTimeHelper.GetCurrentTime(),
            interfaceStateMsg.GetProgramID(),
            interfaceStateMsg.GetBlockID(),
            ProgramHelper.GetProgramItemById(programItemMsg.GetOnSuccess()),
            interfaceStateMsg.GetFlags(),
            false,
            InterfaceStateMsg.ErrorSeverity.NONE,
            InterfaceStateMsg.ErrorCode.ERROR_UNKNOWN);
        ROSCommunicationManager.Instance.ros.Publish(InterfaceStatePublisher.GetMessageTopic(), i_msg);

        //Request brain for goal GET_READY for learning PLACE_TO_POSE
        requestMsg = new LearningRequstActionGoalMsg(new HeaderMsg(0, ROSTimeHelper.GetCurrentTime(), ""), new GoalIDMsg(ROSTimeHelper.GetCurrentTime(), ""),
            new LearningRequstGoalMsg(learning_request_goal.GET_READY_WITHOUT_ROBOT));
        ROSCommunicationManager.Instance.ros.Publish(LearningRequestActionGoalPublisher.GetMessageTopic(), requestMsg);

    }


}
