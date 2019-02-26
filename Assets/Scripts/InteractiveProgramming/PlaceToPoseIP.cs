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

    public GameObject BasicObjectManipulatablePrefab;
    //public GameObject BasicObjectUnmanipulatablePrefab;

    //private GameObject objectToPlaceUnmanipulatable;
    private GameObject objectToPlace;

    public bool learning = false;

    private GameObject cursor;

    // Use this for initialization
    void Start () {
        cursor = GameObject.FindGameObjectWithTag("cursor");
        world_anchor = GameObject.FindGameObjectWithTag("world_anchor");
    }
	
	// Update is called once per frame
	public void UpdatePlacePose (Vector3 placePosition, Quaternion placeOrientation) {
        if(learning) {
            TimeMsg currentTime = ROSTimeHelper.GetCurrentTime();

            InterfaceStateMsg msg = new InterfaceStateMsg(
                "HOLOLENS",
                InterfaceStateMsg.SystemState.STATE_LEARNING,
                currentTime,
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
                            new PoseStampedMsg(new HeaderMsg(0, currentTime, "marker"),
                            new PoseMsg(new PointMsg(ROSUnityCoordSystemTransformer.ConvertVector(placePosition)),
                            new QuaternionMsg(ROSUnityCoordSystemTransformer.ConvertQuaternion(placeOrientation)))) },
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
        }
    }


    public void StartLearning() {
        learning = true;
        Debug.Log("PLACE_TO_POSE STARTED LEARNING STANDALONE");

        //if (objectToPlace == null) {
        //    objectToPlace = Instantiate(BasicObjectManipulatablePrefab, world_anchor.transform);
        //}

        Visualize();
        objectToPlace.GetComponent<ObjectManipulationEnabler>().EnableManipulation();
        objectToPlace.transform.GetChild(0).GetComponent<Collider>().enabled = false;
        objectToPlace.transform.parent = cursor.transform;
        objectToPlace.transform.localPosition = new Vector3(0, 0, objectToPlace.transform.GetChild(0).transform.localScale.x / 2);
        objectToPlace.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
        ProgramItemMsg referenceItem = ProgramHelper.GetProgramItemById(interfaceStateMsg.GetBlockID(), programItemMsg.GetRefID()[0]);        
        objectToPlace.GetComponent<PlaceRotateConfirm>().Arm =
            (ROSUnityCoordSystemTransformer.ConvertVector(referenceItem.GetPose()[0].GetPose().GetPosition().GetPoint()).x > MainMenuManager.Instance.currentSetup.GetTableWidth() / 2) ? 
                RobotRadiusHelper.RobotArm.LEFT_ARM : RobotRadiusHelper.RobotArm.RIGHT_ARM;

        //objectToPlace.transform.localPosition = placePose;
        //objectToPlace.transform.localRotation = placeOrientation;
        //objectToPlace.transform.GetChild(0).transform.localScale = objectDims;
    }

    public void StartLearningContinuous() {
        learning = true;
        Debug.Log("PLACE_TO_POSE STARTED LEARNING CONTINUOUS");
    }

    public void Visualize() {
        if (ProgramHelper.ItemLearned(programItemMsg)) {
            //TODO get reference ID of previous PICK and get ObjectType
            ProgramItemMsg referenceItem = ProgramHelper.GetProgramItemById(interfaceStateMsg.GetBlockID(), programItemMsg.GetRefID()[0]);
            Vector3 placePose = ROSUnityCoordSystemTransformer.ConvertVector(programItemMsg.GetPose()[0].GetPose().GetPosition().GetPoint());
            Quaternion placeOrientation = ROSUnityCoordSystemTransformer.ConvertQuaternion(programItemMsg.GetPose()[0].GetPose().GetOrientation().GetQuaternion());
            Vector3 objectDims = ObjectsManager.Instance.GetObjectTypeDimensions(referenceItem.GetObject()[0]);

            //objectToPlaceUnmanipulatable = Instantiate(BasicObjectUnmanipulatablePrefab, world_anchor.transform);
            //objectToPlaceUnmanipulatable.transform.localPosition = placePose;
            //objectToPlaceUnmanipulatable.transform.localRotation = placeOrientation;
            //objectToPlaceUnmanipulatable.transform.GetChild(0).transform.localScale = objectDims;

            if (objectToPlace == null) {
                objectToPlace = Instantiate(BasicObjectManipulatablePrefab, world_anchor.transform);
                Debug.Log("Instantiating object");
            }
            objectToPlace.GetComponent<ObjectManipulationEnabler>().DisableManipulation();
            objectToPlace.transform.localPosition = placePose;
            objectToPlace.transform.localRotation = placeOrientation;
            objectToPlace.transform.GetChild(0).transform.localScale = objectDims;
        }
    }
    public void VisualizeClear() {
        if (ProgramHelper.ItemLearned(programItemMsg)) {
            if (objectToPlace != null) {
                objectToPlace.GetComponent<ObjectManipulationEnabler>().EnableManipulation();
                objectToPlace.GetComponent<PlaceRotateConfirm>().DestroyItself();
                objectToPlace = null;
            }
            //Destroy(objectToPlaceUnmanipulatable);
        }
    }

    public void SetInterfaceStateMsgFromROS(InterfaceStateMsg msg) {
        interfaceStateMsg = msg;
        programItemMsg = msg.GetProgramCurrentItem();
    }

    public void PassObjectToPlaceReference(GameObject obj) {
        objectToPlace = obj;
    }

    public void SaveObjectPosition(Vector3 position, Quaternion rotation) {
        learning = false;
        ProgramHelper.LoadProgram = true;

        PlacePosition = new Vector3(position.x, position.y,
            ObjectsManager.Instance.GetObjectTypeDimensions(ProgramHelper.GetProgramItemById(interfaceStateMsg.GetBlockID(), programItemMsg.GetRefID()[0]).GetObject()[0]).x/2);
        PlaceOrientation = rotation;

        StartCoroutine(SaveToROS());
    }

    public IEnumerator SaveToROS() {
        //save instruction to ROS
        Debug.Log("PLACE TO POSE SAVED");

        TimeMsg currentTime = ROSTimeHelper.GetCurrentTime();

        InterfaceStateMsg msg = new InterfaceStateMsg(
            "HOLOLENS",
            InterfaceStateMsg.SystemState.STATE_LEARNING,
            currentTime,
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
                            new PoseStampedMsg(new HeaderMsg(0, currentTime, "marker"),
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

        yield return new WaitForSecondsRealtime(0.1f);

        currentTime = ROSTimeHelper.GetCurrentTime();
        string currentRequestID = ROSActionHelper.GenerateUniqueGoalID((int)learning_request_goal.DONE, ROSTimeHelper.ToSec(currentTime));
        LearningRequestActionGoalMsg requestMsg = new LearningRequestActionGoalMsg(new HeaderMsg(0, currentTime, ""), 
            new GoalIDMsg(currentTime, currentRequestID),
            new LearningRequestGoalMsg(learning_request_goal.DONE));
        ROSCommunicationManager.Instance.ros.Publish(LearningRequestActionGoalPublisher.GetMessageTopic(), requestMsg);

        //Destroy(objectToPlaceUnmanipulatable);
        InteractiveProgrammingManager.Instance.followedLearningPlacePoseOverride = false;
        yield return null;
    }
}
