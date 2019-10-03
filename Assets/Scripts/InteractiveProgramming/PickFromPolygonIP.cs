using HoloToolkit.Unity;
using ROSBridgeLib.actionlib_msgs;
using ROSBridgeLib.art_msgs;
using ROSBridgeLib.geometry_msgs;
using ROSBridgeLib.std_msgs;
using System;
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

    private Vector3 robotArmPosition;
    private Quaternion robotArmRotation;
    private string objectTypeToPick;

    public GameObject robotArm;
    private GameObject objectToPick;
    //private GameObject objectToPickUnmanipulatable;

    private bool waiting_for_action_response = false;
    private string currentRequestID;

    private List<Vector3> pick_polygon = new List<Vector3>();

    // Start is called before the first frame update
    void Start() {
        cursor = GameObject.FindGameObjectWithTag("cursor");
        world_anchor = GameObject.FindGameObjectWithTag("world_anchor");
    }

    public void StartLearning() {
        robotArm.GetComponent<PR2GripperController>().PlaceGripperToManipulation();
        robotArm.GetComponent<PR2GripperController>().SetCollidersActive(false);
        robotArm.GetComponent<PR2GripperController>().SetArmActive(true);
        robotArm.GetComponent<PR2GripperController>().OpenGripperInstantly();
        robotArm.GetComponent<FollowTransform>().enabled = true;
    }

    public void Visualize() {
        if (ProgramHelper.ItemLearned(programItemMsg)) {
            Debug.Log("Visualizing PICK_FROM_FEEDER");

            //if (objectToPickUnmanipulatable != null)
            //    Destroy(objectToPickUnmanipulatable);

            //show robot arm only if it's not grasping object
            if (!RobotHelper.HasArmGraspedObject(RobotHelper.activeRobot == RobotHelper.RobotType.PR2 ? (robotArmPosition.x > MainMenuManager.Instance.currentSetup.GetTableWidth() / 2f ?
                RobotHelper.RobotArmType.LEFT_ARM : RobotHelper.RobotArmType.RIGHT_ARM) : RobotHelper.RobotArmType.ARM)) {
                //find all physical objects that are in given polygon
                List<GameObject> objectsInPolygon = ObjectsManager.Instance.GetObjectsFromPolygon(programItemMsg.GetPolygon()[0].GetPolygon().GetPointsVector2D());
                float maxZ = 0f;
                if (objectsInPolygon.Count > 0) {
                    foreach (GameObject obj in objectsInPolygon) {
                        if (maxZ < obj.transform.localPosition.z)
                            maxZ = obj.transform.localPosition.z;
                    }
                }
                //convert ros coordinate system to unity coordinate system
                robotArm.GetComponent<PR2GripperController>().PlaceGripperToInit();
                Vector3 robotArmPosition = ROSUnityCoordSystemTransformer.ConvertVector(programItemMsg.GetPolygon()[0].GetPolygon().GetCentroid()) + new Vector3(0f, 0f, maxZ + 0.3f);
                robotArm.transform.localPosition = robotArmPosition;
                robotArm.GetComponent<PR2GripperController>().SetArmActive(true);
            }


            ////show robot arm only if it's not grasping object
            //if (!RobotHelper.HasArmGraspedObject(robotArmPosition.x > MainMenuManager.Instance.currentSetup.GetTableWidth() / 2f ?
            //    RobotHelper.RobotArmType.LEFT_ARM : RobotHelper.RobotArmType.RIGHT_ARM)) {
            //    robotArm.transform.localPosition = robotArmPosition;
            //    robotArm.transform.localRotation = robotArmRotation;
            //    robotArm.GetComponent<PR2GripperController>().SetArmActive(true);
            //    robotArm.GetComponent<PR2GripperController>().CloseGripperInstantly();
            //}

            //Vector3 objectPosition = FakeFeederObjectsPositions.GetObjectPositionInFeeder(programItemMsg.GetObject()[0],
            //    (robotArmPosition.x > MainMenuManager.Instance.currentSetup.GetTableWidth() / 2f ? FakeFeederObjectsPositions.FeederType.right_feeder : FakeFeederObjectsPositions.FeederType.left_feeder));
            //Quaternion objectOrientation = FakeFeederObjectsPositions.GetObjectOrientationInFeeder(programItemMsg.GetObject()[0],
            //    (robotArmPosition.x > MainMenuManager.Instance.currentSetup.GetTableWidth() / 2f ? FakeFeederObjectsPositions.FeederType.right_feeder : FakeFeederObjectsPositions.FeederType.left_feeder));
            //Vector3 objectDims = ObjectsManager.Instance.GetObjectTypeDimensions(programItemMsg.GetObject()[0]);

            //objectToPickUnmanipulatable = Instantiate(BasicObjectUnmanipulatablePrefab, world_anchor.transform);
            //objectToPickUnmanipulatable.transform.localPosition = ROSUnityCoordSystemTransformer.ConvertVector(objectPosition);
            //objectToPickUnmanipulatable.transform.localRotation = ROSUnityCoordSystemTransformer.ConvertQuaternion(objectOrientation);
            //objectToPickUnmanipulatable.transform.GetChild(0).transform.localScale = objectDims;

            //Debug.Log("BEFORE INSTANTIATING");
            //if (objectToPick == null || objectToPick.Equals(null)) {
            //    Debug.Log("INSTANTIATING");
            //    objectToPick = Instantiate(BasicObjectManipulatablePrefab, world_anchor.transform);
            //}
            //objectToPick.GetComponent<ObjectManipulationEnabler>().DisableManipulation();
            //objectToPick.transform.localPosition = ROSUnityCoordSystemTransformer.ConvertVector(objectPosition);
            //objectToPick.transform.localRotation = ROSUnityCoordSystemTransformer.ConvertQuaternion(objectOrientation);
            //objectToPick.transform.GetChild(0).transform.localScale = objectDims;
        }
    }

    public void VisualizeClear() {
        Debug.Log("VISUALIZE CLEAR from Pick");
        if (ProgramHelper.ItemLearned(programItemMsg)) {
            robotArm.GetComponent<PR2GripperController>().PlaceGripperToInit();
            robotArm.GetComponent<PR2GripperController>().SetArmActive(false);

            //if (objectToPickUnmanipulatable != null)
            //    Destroy(objectToPickUnmanipulatable);
            try {
                if (objectToPick != null || !objectToPick.Equals(null)) {
                    objectToPick.GetComponent<ObjectManipulationEnabler>().EnableManipulation();
                    objectToPick.GetComponent<PlaceRotateConfirm>().DestroyItself();
                    objectToPick = null;
                    PlaceToPoseIP.Instance.ObjectDestroyed();
                }
            }
            catch(NullReferenceException e) {
                Debug.Log(e);
            }
                
        }
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
        robotArmPosition = world_anchor.transform.InverseTransformPoint(pointedArea.localPosition + pointedArea.forward * 0.3f);
        robotArmRotation = Quaternion.Inverse(world_anchor.transform.localRotation) * pointedArea.localRotation;


        pick_polygon.Clear();
        pick_polygon = CreatePolygon(detectedObject.position, detectedObject.bbox);

        objectToPick = Instantiate(BasicObjectManipulatablePrefab, detectedObject.position, detectedObject.rotation);
        objectToPick.GetComponent<ObjectManipulationEnabler>().EnableManipulation();
        objectToPick.transform.GetChild(0).transform.localScale = detectedObject.bbox;
        objectToPick.transform.GetChild(0).GetComponent<Collider>().enabled = false;
        objectToPick.transform.parent = world_anchor.transform;
        objectToPick.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        if (RobotHelper.activeRobot == RobotHelper.RobotType.PR2) {
            bool picking_right_feeder = false;
            //rotate gripper to face feeder
            //picking from right feeder
            if (robotArmPosition.x > MainMenuManager.Instance.currentSetup.GetTableWidth() / 2) {
                Debug.Log("PICKING FROM RIGHT FEEDER");
                robotArmRotation.eulerAngles += new Vector3(90, 0, 0);
                picking_right_feeder = true;
            }
            //picking from left feeder
            else {
                Debug.Log("PICKING FROM LEFT FEEDER");
                robotArmRotation.eulerAngles += new Vector3(-90, 0, 0);
                picking_right_feeder = false;
            }

            objectToPick.GetComponent<PlaceRotateConfirm>().Arm =
                picking_right_feeder ? RobotHelper.RobotArmType.LEFT_ARM : RobotHelper.RobotArmType.RIGHT_ARM;
        }
        else if (RobotHelper.activeRobot == RobotHelper.RobotType.NYRIO) {
            objectToPick.GetComponent<PlaceRotateConfirm>().Arm = RobotHelper.RobotArmType.ARM;
        }

        objectToPick.GetComponent<PlaceRotateConfirm>().snapLocalRotation = objectToPick.transform.rotation;

        objectToPick.transform.parent = cursor.transform;
        objectToPick.transform.localPosition = new Vector3(0, 0, detectedObject.bbox.x / 2);

        objectTypeToPick = detectedObject.type;

        objectToPick.GetComponent<PlaceRotateConfirm>().object_attached = true;

        PlaceToPoseIP.Instance.PassObjectToPlaceReference(objectToPick);

        StartCoroutine(SaveToROS());
    }

    private List<Vector3> CreatePolygon(Vector3 object_position, Vector3 object_size) {
        List<Vector3> polygon = new List<Vector3>();

        float w = object_size.x;
        float h = object_size.y;

        polygon.Add(new Vector3(object_position.x + w, -object_position.y + h, 0));
        polygon.Add(new Vector3(object_position.x - w, -object_position.y + h, 0));
        polygon.Add(new Vector3(object_position.x - w, -object_position.y - h, 0));
        polygon.Add(new Vector3(object_position.x + w, -object_position.y - h, 0));

        return polygon;
    }

    private PointMsg[] GeneratePolygonPointsMsg(List<Vector3> polygon) {
        List<PointMsg> points = new List<PointMsg>();
        foreach(Vector3 point in polygon) {
            points.Add(new PointMsg(point));
        }
        return points.ToArray();
    }

    private IEnumerator SaveToROS() {
        robotArm.GetComponent<PR2GripperController>().SetArmActive(false);

        ROSActionHelper.OnLearningActionResult += OnLearningActionResult;

        Debug.Log("PICK FROM POLYGON SAVED");

        //Save parameters for PICK_FROM_POLYGON
        TimeMsg currentTime = ROSTimeHelper.GetCurrentTime();
        InterfaceStateMsg i_msg = new InterfaceStateMsg(
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
                new List<string>() { objectTypeToPick },
                new List<PoseStampedMsg>(),
                new List<PolygonStampedMsg>() { new PolygonStampedMsg(new HeaderMsg(0, currentTime, "marker"), new PolygonMsg(GeneratePolygonPointsMsg(pick_polygon))) },
                programItemMsg.GetRefID(),
                programItemMsg.GetFlags(),
                programItemMsg.GetDoNotClear(),
                programItemMsg.GetLabels()),
            interfaceStateMsg.GetFlags(),
            true,
            InterfaceStateMsg.ErrorSeverity.NONE,
            InterfaceStateMsg.ErrorCode.ERROR_UNKNOWN);
        ROSCommunicationManager.Instance.ros.Publish(InterfaceStatePublisher.GetMessageTopic(), i_msg);

        yield return new WaitForSecondsRealtime(0.1f);

        //Request brain for goal DONE
        currentTime = ROSTimeHelper.GetCurrentTime();
        currentRequestID = ROSActionHelper.GenerateUniqueGoalID((int)learning_request_goal.DONE, ROSTimeHelper.ToSec(currentTime));
        LearningRequestActionGoalMsg requestMsg = new LearningRequestActionGoalMsg(new HeaderMsg(0, currentTime, ""),
            new GoalIDMsg(ROSTimeHelper.GetCurrentTime(), currentRequestID),
            new LearningRequestGoalMsg(learning_request_goal.DONE));
        ROSCommunicationManager.Instance.ros.Publish(LearningRequestActionGoalPublisher.GetMessageTopic(), requestMsg);
        waiting_for_action_response = true;

        //Wait for action server response
        yield return new WaitWhile(() => waiting_for_action_response == true);
        Debug.Log("SERVER RESPONDED!");
        yield return new WaitForSecondsRealtime(0.1f);

        //Switch to next instruction (should be PLACE_TO_POSE)
        //InteractiveProgrammingManager.Instance.PlaceToPoseLearningOverride = true;
        currentTime = ROSTimeHelper.GetCurrentTime();
        i_msg = new InterfaceStateMsg(
            "HOLOLENS",
            InterfaceStateMsg.SystemState.STATE_LEARNING,
            currentTime,
            interfaceStateMsg.GetProgramID(),
            interfaceStateMsg.GetBlockID(),
            ProgramHelper.GetProgramItemById(interfaceStateMsg.GetBlockID(), programItemMsg.GetOnSuccess()),
            interfaceStateMsg.GetFlags(),
            false,
            InterfaceStateMsg.ErrorSeverity.NONE,
            InterfaceStateMsg.ErrorCode.ERROR_UNKNOWN);
        ROSCommunicationManager.Instance.ros.Publish(InterfaceStatePublisher.GetMessageTopic(), i_msg);

        yield return new WaitForSecondsRealtime(0.1f);

        //Request brain for goal GET_READY for learning PLACE_TO_POSE
        currentTime = ROSTimeHelper.GetCurrentTime();
        currentRequestID = ROSActionHelper.GenerateUniqueGoalID((int)learning_request_goal.GET_READY_WITHOUT_ROBOT, ROSTimeHelper.ToSec(currentTime));
        requestMsg = new LearningRequestActionGoalMsg(new HeaderMsg(0, currentTime, ""),
            new GoalIDMsg(currentTime, currentRequestID),
            new LearningRequestGoalMsg(learning_request_goal.GET_READY_WITHOUT_ROBOT));
        ROSCommunicationManager.Instance.ros.Publish(LearningRequestActionGoalPublisher.GetMessageTopic(), requestMsg);
        waiting_for_action_response = true;

        //Wait for action server response
        yield return new WaitWhile(() => waiting_for_action_response == true);
        Debug.Log("SERVER RESPONDED!");

        yield return new WaitForSecondsRealtime(0.1f);

        ROSActionHelper.OnLearningActionResult -= OnLearningActionResult;
        yield return null;


        //robotArm.transform.parent = world_anchor.transform;
        robotArm.GetComponent<FollowTransform>().enabled = false;
        robotArm.GetComponent<PR2GripperController>().CloseGripperInstantly();
        robotArm.GetComponent<PR2GripperController>().PlaceGripperToInit();
        robotArm.GetComponent<PR2GripperController>().SetCollidersActive(true);
        robotArm.GetComponent<PR2GripperController>().SetArmActive(false);

        //spatial_mapping.SetActive(true);
    }

    private void OnLearningActionResult(LearningRequestActionResultMsg msg) {
        //if result ID is identical with request ID
        if (msg.GetStatus().GetGoalID().GetID().Equals(currentRequestID) && msg.GetStatus().GetStatus() == GoalStatusMsg.Status.SUCCEEDED
            && msg.GetResult().GetSuccess() == true) {
            waiting_for_action_response = false;
        }
    }


    public void StaringAtObject(bool active) {
        //robotArm.GetComponent<PR2GripperController>().SetArmActive(active);
    }
}
