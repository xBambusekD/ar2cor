using HoloToolkit.Unity;
using ROSBridgeLib.actionlib_msgs;
using ROSBridgeLib.art_msgs;
using ROSBridgeLib.geometry_msgs;
using ROSBridgeLib.std_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceToPoseIP : Singleton<PlaceToPoseIP> {

    private InterfaceStateMsg interfaceStateMsg;
    private ProgramItemMsg programItemMsg;
        
    private GameObject world_anchor;

    private Vector3 PlacePosition;
    private Quaternion PlaceOrientation;

    public GameObject BasicObjectPrefab;
    private GameObject objectToPlaceUnmanipulatable;
    private GameObject objectToPlace;

    // Use this for initialization
    void Start () {
        world_anchor = GameObject.FindGameObjectWithTag("world_anchor");
    }
	
	// Update is called once per frame
	void Update () {

    }


    public void StartLearning() {
        //LearningRequstActionGoalMsg requestMsg = new LearningRequstActionGoalMsg(new HeaderMsg(0, ROSTimeHelper.GetCurrentTime(), ""), new GoalIDMsg(ROSTimeHelper.GetCurrentTime(), ""),
        //    new LearningRequstGoalMsg(learning_request_goal.GET_READY_WITHOUT_ROBOT));
        //ROSCommunicationManager.Instance.ros.Publish(LearningRequestActionGoalPublisher.GetMessageTopic(), requestMsg);

        Debug.Log("PLACE_TO_POSE REQUEST LEARNING");

        
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
    }

    public void Visualize() {
        if (ProgramHelper.ItemLearned(programItemMsg)) {
            //TODO get reference ID of previous PICK and get ObjectType
            ProgramItemMsg referenceItem = ProgramHelper.GetProgramItemById(programItemMsg.GetRefID()[0]);
            Vector3 placePose = ROSUnityCoordSystemTransformer.ConvertVector(programItemMsg.GetPose()[0].GetPose().GetPosition().GetPoint());
            Quaternion placeOrientation = ROSUnityCoordSystemTransformer.ConvertQuaternion(programItemMsg.GetPose()[0].GetPose().GetOrientation().GetQuaternion());
            Vector3 objectDims = ObjectsManager.Instance.GetObjectTypeDimensions(referenceItem.GetObject()[0]);

            objectToPlaceUnmanipulatable = Instantiate(BasicObjectPrefab, world_anchor.transform);
            objectToPlaceUnmanipulatable.transform.localPosition = placePose;
            objectToPlaceUnmanipulatable.transform.localRotation = placeOrientation;
            objectToPlaceUnmanipulatable.transform.GetChild(0).transform.localScale = objectDims;
        }
    }
    public void VisualizeClear() {
        if (ProgramHelper.ItemLearned(programItemMsg)) {
            Destroy(objectToPlaceUnmanipulatable);
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
        Debug.Log("PLACE TO POSE SAVED");

        InterfaceStateMsg msg = new InterfaceStateMsg(
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
                new List<string>(),
                new List<PoseStampedMsg>() {
                            new PoseStampedMsg(new HeaderMsg(0, ROSTimeHelper.GetCurrentTime(), "marker"),
                            new PoseMsg(new PointMsg(ROSUnityCoordSystemTransformer.ConvertVector(PlacePosition)), 
                            new QuaternionMsg(ROSUnityCoordSystemTransformer.ConvertQuaternion(PlaceOrientation)))) },
                new List<PolygonStampedMsg>(),
                programItemMsg.GetRefID(),
                programItemMsg.GetFlags(),
                programItemMsg.GetDoNotClear(),
                programItemMsg.GetLabels()),
            interfaceStateMsg.GetFlags(),
            true,
            InterfaceStateMsg.ErrorSeverity.NONE,
            InterfaceStateMsg.ErrorCode.ERROR_UNKNOWN);
        ROSCommunicationManager.Instance.ros.Publish(InterfaceStatePublisher.GetMessageTopic(), msg);

        LearningRequstActionGoalMsg requestMsg = new LearningRequstActionGoalMsg(new HeaderMsg(0, ROSTimeHelper.GetCurrentTime(), ""), new GoalIDMsg(ROSTimeHelper.GetCurrentTime(), ""),
            new LearningRequstGoalMsg(learning_request_goal.DONE));
        ROSCommunicationManager.Instance.ros.Publish(LearningRequestActionGoalPublisher.GetMessageTopic(), requestMsg);

        //Destroy(objectToPlaceUnmanipulatable);
    }
}
