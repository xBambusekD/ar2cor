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

    private Vector3 PlacePosition = Vector3.zero;
    private Quaternion PlaceOrientation = new Quaternion(0,0,0,0);
    private Vector3 currentPlacePosition = Vector3.zero;
    private Quaternion currentPlaceOrientation = new Quaternion(0,0,0,0);

    public GameObject BasicObjectManipulatablePrefab;
    //public GameObject BasicObjectUnmanipulatablePrefab;

    //private GameObject objectToPlaceUnmanipulatable;
    private GameObject objectToPlace;
    private bool waiting_for_action_response = false;
    private string currentRequestID;
    
    public bool learning = false;

    private GameObject cursor;

    // Use this for initialization
    void Start () {
        cursor = GameObject.FindGameObjectWithTag("cursor");
        world_anchor = GameObject.FindGameObjectWithTag("world_anchor");
    }
	
	// Update is called once per frame
	public void UpdatePlacePose (Vector3 placePosition, Quaternion placeOrientation) {
        if(ROSUnityCoordSystemTransformer.AlmostEqual(currentPlacePosition, placePosition, 0.001f) &&
            ROSUnityCoordSystemTransformer.AlmostEqual(currentPlaceOrientation, placeOrientation, 0.001f)) {
            return;
        }

        currentPlacePosition = placePosition;
        currentPlaceOrientation = placeOrientation;


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
                            new PoseMsg(new PointMsg(ROSUnityCoordSystemTransformer.ConvertVector(currentPlacePosition)),
                            new QuaternionMsg(ROSUnityCoordSystemTransformer.ConvertQuaternion(currentPlaceOrientation)))) },
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

    public void UpdatePlacePoseFromROS(Vector3 position, Quaternion rotation) {
        if(!objectToPlace.GetComponent<PlaceRotateConfirm>().object_attached) {
            objectToPlace.transform.localPosition = position;
            objectToPlace.transform.localRotation = rotation;

            currentPlacePosition = position;
            currentPlaceOrientation = rotation;
        }
    }


    public void StartLearning() {
        learning = true;
        Debug.Log("PLACE_TO_POSE STARTED LEARNING STANDALONE");

        //if (objectToPlace == null) {
        //    objectToPlace = Instantiate(BasicObjectManipulatablePrefab, world_anchor.transform);
        //}

        Visualize();

        UISoundManager.Instance.PlaySnap();
        objectToPlace.GetComponent<ObjectManipulationEnabler>().EnableManipulation();
        objectToPlace.transform.GetChild(0).GetComponent<Collider>().enabled = false;
        objectToPlace.GetComponent<PlaceRotateConfirm>().snapLocalRotation = objectToPlace.transform.rotation;
        objectToPlace.transform.parent = cursor.transform;
        objectToPlace.transform.localPosition = new Vector3(0, 0, objectToPlace.transform.GetChild(0).transform.localScale.x / 2);
        //objectToPlace.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
        ProgramItemMsg referenceItem = ProgramHelper.GetProgramItemById(interfaceStateMsg.GetBlockID(), programItemMsg.GetRefID()[0]);        
        objectToPlace.GetComponent<PlaceRotateConfirm>().Arm =
            (ROSUnityCoordSystemTransformer.ConvertVector(referenceItem.GetPose()[0].GetPose().GetPosition().GetPoint()).x > MainMenuManager.Instance.currentSetup.GetTableWidth() / 2) ? 
                RobotHelper.RobotArmType.LEFT_ARM : RobotHelper.RobotArmType.RIGHT_ARM;

        objectToPlace.GetComponent<PlaceRotateConfirm>().object_attached = true;

        //objectToPlace.transform.localPosition = placePose;
        //objectToPlace.transform.localRotation = placeOrientation;
        //objectToPlace.transform.GetChild(0).transform.localScale = objectDims;
    }

    public void StartLearningContinuous() {

        FakeFeedersManager.Instance.DisableFakeObjects();

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

            if (objectToPlace == null || objectToPlace.Equals(null)) {
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
        Debug.Log("VISUALIZE CLEAR from Place");

        if (ProgramHelper.ItemLearned(programItemMsg)) {
            if (objectToPlace != null || !objectToPlace.Equals(null)) {
                objectToPlace.GetComponent<ObjectManipulationEnabler>().EnableManipulation();
                objectToPlace.GetComponent<PlaceRotateConfirm>().DestroyItself();
                objectToPlace = null;
                PickFromFeederIP.Instance.ObjectDestroyed();
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
        ROSActionHelper.OnLearningActionResult += OnLearningActionResult;

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
        currentRequestID = ROSActionHelper.GenerateUniqueGoalID((int)learning_request_goal.DONE, ROSTimeHelper.ToSec(currentTime));
        LearningRequestActionGoalMsg requestMsg = new LearningRequestActionGoalMsg(new HeaderMsg(0, currentTime, ""),
            new GoalIDMsg(currentTime, currentRequestID),
            new LearningRequestGoalMsg(learning_request_goal.DONE));
        ROSCommunicationManager.Instance.ros.Publish(LearningRequestActionGoalPublisher.GetMessageTopic(), requestMsg);
        waiting_for_action_response = true;

        //Wait for action server response
        yield return new WaitWhile(() => waiting_for_action_response == true);
        Debug.Log("SERVER RESPONDED!");
        yield return new WaitForSecondsRealtime(0.1f);

        ROSActionHelper.OnLearningActionResult -= OnLearningActionResult;

        
        //if(InteractiveProgrammingManager.Instance.followedLearningPlacePoseOverride) {
        //    InteractiveProgrammingManager.Instance.followedLearningPlacePoseOverride = false;
        //    InteractiveProgrammingManager.Instance.CurrentState = InteractiveProgrammingManager.ProgrammingManagerState.place_to_pose_vis;
        //    Visualize();
        //}

        yield return null;
    }


    private void OnLearningActionResult(LearningRequestActionResultMsg msg) {
        //if result ID is identical with request ID
        if (msg.GetStatus().GetGoalID().GetID().Equals(currentRequestID) && msg.GetStatus().GetStatus() == GoalStatusMsg.Status.SUCCEEDED
            && msg.GetResult().GetSuccess() == true) {
            waiting_for_action_response = false;
        }
    }

    public void ObjectDestroyed() {
        objectToPlace = null;
    }
}
