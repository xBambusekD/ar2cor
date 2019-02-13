using HoloToolkit.Unity;
using ROSBridgeLib.art_msgs;
using ROSBridgeLib.geometry_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceToPoseIE : Singleton<PlaceToPoseIE> {

    public GameObject pointingHand;
    public Vector3 spawnPointOnTable;

    private InterfaceStateMsg interfaceStateMsg;
    private ProgramItemMsg programItemMsg;
    private ProgramMsg programMsg;

    private GameObject speechManagerObj;
    private TextToSpeechManager speechManager;
    
    private bool sayAdjustArea;
    private bool sayUnknownObjectType;
    private bool objTypeReferenceSet;

    private bool serviceCalled;
    private bool instructionProgrammed;

    private Vector3 spawnPoint;
    private Vector3 movePoint;
    private PoseMsg originalPoseMsg;
    private bool animationShowed;

    // Use this for initialization
    void Start() {
        speechManagerObj = GameObject.FindGameObjectWithTag("speech_manager");
        speechManager = speechManagerObj.GetComponent<TextToSpeechManager>();
        sayAdjustArea = false;
        sayUnknownObjectType = false;
        objTypeReferenceSet = false;

        serviceCalled = false;
        instructionProgrammed = false;
    }

    // Update is called once per frame
    void Update() {
        if (SystemStarter.Instance.calibrated) {
            if (interfaceStateMsg != null) {
                //pick from polygon editing
                if (interfaceStateMsg.GetSystemState() == InterfaceStateMsg.SystemState.STATE_LEARNING && programItemMsg.GetIType() == "PlaceToPose" &&
                    interfaceStateMsg.GetEditEnabled() == true) {

                    //check that object type is set
                    if (programMsg == null && !serviceCalled) {
                        ROSCommunicationManager.Instance.ros.CallService("/art/db/program/get", "{\"id\": " + interfaceStateMsg.GetProgramID() + "}");
                        serviceCalled = true;
                    }
                    else if (programMsg != null && !objTypeReferenceSet) {
                        ProgramBlockMsg programBlockMsg = programMsg.GetBlockByID(interfaceStateMsg.GetBlockID());
                        ProgramItemMsg refItem = programBlockMsg.GetProgramItemByID(programItemMsg.GetRefID()[0]);
                        if (refItem.GetObject().Count == 0 && !sayUnknownObjectType) {
                            speechManager.Say("Robot doesn't know which object you want to place. You have to program picking instruction first.");
                            Debug.Log("Robot doesn't know which object you want to place. You have to program picking instruction first.");
                            sayUnknownObjectType = true;
                        }
                        else if (refItem.GetObject().Count > 0) {
                            objTypeReferenceSet = true;
                        }
                    }

                    //if object type is set
                    if (!sayAdjustArea && objTypeReferenceSet) {
                        speechManager.Say("Drag object outline to set place pose and blue point to set orientation. When you are finished, click on done.");
                        Debug.Log("Drag object outline to set place pose and blue point to set orientation. When you are finished, click on done.");
                        sayAdjustArea = true;
                    }

                    //show hand and play it's animation
                    if (!pointingHand.activeSelf && !animationShowed) {                        
                        //pointingHand.SetActive(true);

                        originalPoseMsg = programItemMsg.GetPose()[0].GetPose();

                        //get middle point of bottom line
                        spawnPoint = new Vector3(programItemMsg.GetPose()[0].GetPose().GetPosition().GetX(),
                                                 -programItemMsg.GetPose()[0].GetPose().GetPosition().GetY(),
                                                 programItemMsg.GetPose()[0].GetPose().GetPosition().GetZ());
                        //ARTABLE BUG - place pose not actualizing interface state.. initially set to 0.. if so, set spawn point on the middle of the table (where it appears)
                        if(spawnPoint.Equals(new Vector3(0f, 0f, 0f))) {
                            spawnPoint = spawnPointOnTable;
                        }
                        movePoint = spawnPoint + new Vector3(0f, 0.15f, 0f);

                        //pointingHand.transform.localPosition = spawnPoint;
                        pointingHand.GetComponent<PointingHandMover>().Run(spawnPoint, movePoint);
                    }

                    //if pose points are same, then user didn't moved with it.. so play hand animation
                    if (originalPoseMsg.ToYAMLString().Equals(programItemMsg.GetPose()[0].GetPose().ToYAMLString())) {
                        //pointingHand.transform.localPosition = Vector3.Lerp(pointingHand.transform.localPosition, movePoint, Time.deltaTime * 1.5f);

                        //if (ObjectInPosition(pointingHand, movePoint, 0.0005f)) {
                        //    pointingHand.transform.localPosition = spawnPoint;
                        //}
                    }
                    else {
                        //pointingHand.SetActive(false);
                        pointingHand.GetComponent<PointingHandMover>().Stop();
                        animationShowed = true;
                    }

                    //check if everything is set for this instruction
                    if (objTypeReferenceSet && programItemMsg.GetPose()[0].GetPose()._position.GetX() != 0.0f) {
                        instructionProgrammed = true;
                    }
                }
                //reset all variables
                else {
                    if (instructionProgrammed) {
                        speechManager.Say("Good job! You have successfully programmed place to pose instruction.");
                        Debug.Log("Good job! You have successfully programmed place to pose instruction.");
                        instructionProgrammed = false;
                        pointingHand.GetComponent<PointingHandMover>().Stop();
                    }
                    //if just object type reference is set but not place pose
                    else if (objTypeReferenceSet) {
                        speechManager.Say("You forgot to set the place pose. You have to move with it.");
                        Debug.Log("You forgot to set the place pose. You have to move with it.");
                        pointingHand.GetComponent<PointingHandMover>().Stop();
                    }
                    sayAdjustArea = false;
                    sayUnknownObjectType = false;
                    objTypeReferenceSet = false;
                    serviceCalled = false;
                    animationShowed = false;
                    programMsg = null;
                }
            }
        }
    }

    //when user says "Repeat" keyword.. to repeat what Zira just said
    public void OnRepeat() {
        sayAdjustArea = false;
        sayUnknownObjectType = false;
        animationShowed = false;
    }

    public void SetInterfaceStateMsgFromROS(InterfaceStateMsg msg) {
        interfaceStateMsg = msg;
        programItemMsg = msg.GetProgramCurrentItem();
    }

    public void SetProgramMsgFromROS(ProgramMsg msg) {
        //set only if system is really in edit mode of this particullar instruction
        if (interfaceStateMsg != null) {
            if (interfaceStateMsg.GetSystemState() == InterfaceStateMsg.SystemState.STATE_LEARNING && programItemMsg.GetIType() == "PlaceToPose" &&
                    interfaceStateMsg.GetEditEnabled() == true) {
                programMsg = msg;
            }
        }
    }

    private bool ObjectInPosition(GameObject obj, Vector3 placePosition, float precision = 0.000005f) {
        //if error is approximately less than 5 mm
        if (Vector3.SqrMagnitude(obj.transform.localPosition - placePosition) < precision) {
            return true;
        }
        return false;
    }
}
