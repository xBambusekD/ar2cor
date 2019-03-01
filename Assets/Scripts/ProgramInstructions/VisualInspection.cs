using ROSBridgeLib.art_msgs;
using ROSBridgeLib.geometry_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualInspection : ProgramInstruction {

    public ProgramItemMsg programItem;
    public GameObject referenceItem;

    public GameObject objectToMove;

    private PoseMsg placePose;
    private Vector3 placePosition;
    private Vector3 upPosition;
    private Quaternion placeQuaternion;
    private GameObject world_anchor;

    private bool movedToPose;
    private bool moving_up;
    private bool moving_to_place;
    private bool moving_down;
    private bool moving_arm_up;

    public override void Awake() {
        base.Awake();

        runTime = 0f;
        movedToPose = false;
        moving_up = true;
        moving_to_place = false;
        moving_down = false;
        moving_arm_up = false;

        world_anchor = GameObject.FindGameObjectWithTag("world_anchor");
    }
    	
	// Update is called once per frame
	void Update () {
        if (run) {            

            //if user says "Next" then skip to end state
            if (next) {
                SkipToEnd();
                run = false;
                next = false;
            }
            //if user says "Previous" then skip to initial state
            else if (previous) {
                GoBackToStart();
                run = false;
                previous = false;
            }
            //normal run
            else {
                if (!movedToPose) {
                    runTime += Time.deltaTime;

                    if (objectToMove != null) {
                        //grab the object and move it up
                        if (moving_up) {
                            objectToMove.transform.localPosition = Vector3.Lerp(objectToMove.transform.localPosition, upPosition, Time.deltaTime * 2f);
                            if (ObjectInPosition(objectToMove, upPosition)) {
                                moving_up = false;
                                moving_to_place = true;
                            }
                        }
                        //move the object above the place position
                        if (moving_to_place) {
                            objectToMove.transform.localPosition = Vector3.Lerp(objectToMove.transform.localPosition, new Vector3(placePose.GetPosition().GetX(), -placePose.GetPosition().GetY(), placePose.GetPosition().GetZ()), Time.deltaTime * 2f);
                            objectToMove.transform.localRotation = Quaternion.Lerp(objectToMove.transform.localRotation, placeQuaternion, Time.deltaTime * 2f);
                            if (ObjectInPosition(objectToMove, new Vector3(placePose.GetPosition().GetX(), -placePose.GetPosition().GetY(), placePose.GetPosition().GetZ()))) {
                                moving_to_place = false;
                                //moving_down = true;
                                movedToPose = true;
                            }
                        }
                    }
                }
                else {
                    run = false;
                }
            }
        }
    }

    private void InitObjectToPlace() {
        //if object that is going to be manipulated with is not yet referenced then get this reference
        if(objectToMove == null) {
            switch (referenceItem.tag) {
                case "PICK_FROM_POLYGON":
                    PickFromPolygon pfg = referenceItem.GetComponent<PickFromPolygon>();
                    objectToMove = pfg.objectToPick;
                    break;
                case "PICK_FROM_FEEDER":
                    PickFromFeeder pff = referenceItem.GetComponent<PickFromFeeder>();
                    objectToMove = pff.objectToPick;
                    break;
            }
        }        
    }

    private void InitPlacePose() {
        placePose = programItem.GetPose()[0].GetPose();
        //placePosition = new Vector3(placePose.GetPosition().GetX(), -placePose.GetPosition().GetY(), placePose.GetPosition().GetZ() - 0.023f);
        placePosition = new Vector3(placePose.GetPosition().GetX(), -placePose.GetPosition().GetY(), placePose.GetPosition().GetZ());
        
        placeQuaternion = new Quaternion(-placePose.GetOrientation().GetX(), placePose.GetOrientation().GetY(), -placePose.GetOrientation().GetZ(), placePose.GetOrientation().GetW());
        upPosition = objectToMove.transform.localPosition + new Vector3(0,0,0.1f);
    }

    private void SkipToEnd() {
        objectToMove.transform.localPosition = placePosition;
        objectToMove.transform.localRotation = placeQuaternion;
    }

    private void GoBackToStart() {
    }

    void OnEnable() {
    }

    void OnDisable() {
        run = false;
    }

    public override void Run() {
        //init everything      
        runTime = 0f;
        movedToPose = false;
        moving_up = true;
        moving_to_place = false;
        moving_down = false;
        moving_arm_up = false;

        InitObjectToPlace();
        InitPlacePose();

        base.Run();

        //speechManager.Say("Sensor checks the quality of the object and classifies it as good or bad.");
    }
}
