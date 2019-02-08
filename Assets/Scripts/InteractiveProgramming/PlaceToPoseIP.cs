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

    public GameObject BasicObjectPrefab;
    private GameObject objectToPlace;

    // Use this for initialization
    void Start () {
        world_anchor = GameObject.FindGameObjectWithTag("world_anchor");
    }
	
	// Update is called once per frame
	void Update () {
        //if (SystemStarter.Instance.calibrated) {
        //    if (interfaceStateMsg != null) {
        //        //pick from polygon editing
        //        if (interfaceStateMsg.GetSystemState() == 2 && programItemMsg.GetIType() == "PlaceToPose" &&
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

    }

    public void Visualize() {
        if (ProgramHelper.ItemLearned(programItemMsg)) {
            //TODO get reference ID of previous PICK and get ObjectType
            ProgramItemMsg referenceItem = ProgramHelper.GetProgramItemById(programItemMsg.GetRefID()[0]);
            Vector3 placePose = ROSUnityCoordSystemTransformer.ConvertVector(programItemMsg.GetPose()[0].GetPose().GetPosition().GetPoint());
            Quaternion placeOrientation = ROSUnityCoordSystemTransformer.ConvertQuaternion(programItemMsg.GetPose()[0].GetPose().GetOrientation().GetQuaternion());
            Vector3 objectDims = ObjectsManager.Instance.GetObjectTypeDimensions(referenceItem.GetObject()[0]);

            objectToPlace = Instantiate(BasicObjectPrefab, world_anchor.transform);
            objectToPlace.transform.localPosition = placePose;
            objectToPlace.transform.localRotation = placeOrientation;
            objectToPlace.transform.GetChild(0).transform.localScale = objectDims;
        }
    }
    public void VisualizeClear() {
        if (ProgramHelper.ItemLearned(programItemMsg)) {
            Destroy(objectToPlace);
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
    }
}
