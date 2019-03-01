using ROSBridgeLib.art_msgs;
using ROSBridgeLib.geometry_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceToPose : ProgramInstruction {

    public ProgramItemMsg programItem;
    public GameObject referenceItem;

    public GameObject objectToPlace;

    private PoseMsg placePose;
    private Vector3 placePosition;
    private Vector3 upPosition;
    private Quaternion placeQuaternion;
    private GameObject world_anchor;

    private bool placedToPose;
    private bool moving_up;
    private bool moving_to_place;
    private bool moving_down;
    private bool moving_arm_up;

    public override void Awake() {
        base.Awake();

        runTime = 0f;
        placedToPose = false;
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
                if (!placedToPose) {
                    runTime += Time.deltaTime;

                    if (objectToPlace != null) {
                        //grab the object and move it up
                        if (moving_up) {
                            objectToPlace.transform.localPosition = Vector3.Lerp(objectToPlace.transform.localPosition, upPosition, Time.deltaTime * 2f);
                            if (ObjectInPosition(objectToPlace, upPosition)) {
                                moving_up = false;
                                moving_to_place = true;
                            }
                        }
                        //move the object above the place position
                        if (moving_to_place) {
                            objectToPlace.transform.localPosition = Vector3.Lerp(objectToPlace.transform.localPosition, new Vector3(placePose.GetPosition().GetX(), -placePose.GetPosition().GetY(), objectToPlace.transform.localPosition.z), Time.deltaTime * 2f);
                            objectToPlace.transform.localRotation = Quaternion.Lerp(objectToPlace.transform.localRotation, placeQuaternion, Time.deltaTime * 2f);
                            if (ObjectInPosition(objectToPlace, new Vector3(placePose.GetPosition().GetX(), -placePose.GetPosition().GetY(), objectToPlace.transform.localPosition.z))) {
                                moving_to_place = false;
                                moving_down = true;
                            }
                        }
                        //move the object down to place pose
                        if (moving_down) {
                            objectToPlace.transform.localPosition = Vector3.Lerp(objectToPlace.transform.localPosition, placePosition, Time.deltaTime * 2f);
                            if (ObjectInPosition(objectToPlace, placePosition)) {
                                moving_down = false;
                                moving_arm_up = true;
                            }
                        }
                        //move robot gripper up from the object
                        if (moving_arm_up) {

                            //release arm from attached state and set it to be the parent of world anchor
                            if (pr2_arm.transform.parent != world_anchor.transform) {
                                pr2_arm.transform.parent = world_anchor.transform;
                                //open the gripper
                                pr2_animator.SetBool("open_gripper", true);
                            }
                            //if gripper is opened then move the arm up
                            if (pr2_animator.GetCurrentAnimatorStateInfo(0).IsName("opened")) {
                                //prevent from opening again and again
                                pr2_animator.SetBool("open_gripper", false);
                                pr2_arm.transform.localPosition = Vector3.Lerp(pr2_arm.transform.localPosition, placePosition + new Vector3(0, 0, 0.3f), Time.deltaTime * 2f);
                                if (ObjectInPosition(pr2_arm, placePosition + new Vector3(0, 0, 0.3f))) {
                                    moving_arm_up = false;

                                    //object is finally placed to position
                                    placedToPose = true;
                                }
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
        //of object that is going to be manipulated with is not yet referenced then get this reference
        if(objectToPlace == null) {
            switch (referenceItem.tag) {
                case "PICK_FROM_POLYGON":
                    PickFromPolygon pfg = referenceItem.GetComponent<PickFromPolygon>();
                    objectToPlace = pfg.objectToPick;
                    break;
                case "PICK_FROM_FEEDER":
                    PickFromFeeder pff = referenceItem.GetComponent<PickFromFeeder>();
                    objectToPlace = pff.objectToPick;
                    break;
            }
        }        
    }

    private void InitPlacePose() {
        placePose = programItem.GetPose()[0].GetPose();
        placePosition = new Vector3(placePose.GetPosition().GetX(), -placePose.GetPosition().GetY(), placePose.GetPosition().GetZ());
        //placePosition = new Vector3(placePose.GetPosition().GetX(), -placePose.GetPosition().GetY(), 0f);
        placeQuaternion = new Quaternion(-placePose.GetOrientation().GetX(), placePose.GetOrientation().GetY(), -placePose.GetOrientation().GetZ(), placePose.GetOrientation().GetW());
        upPosition = objectToPlace.transform.localPosition + new Vector3(0,0,0.1f);
    }

    private void SkipToEnd() {
        objectToPlace.transform.localPosition = placePosition;
        objectToPlace.transform.localRotation = placeQuaternion;
        if (pr2_arm.transform.parent != world_anchor.transform) {
            pr2_arm.transform.parent = world_anchor.transform;
        }
        pr2_arm.transform.localPosition = placePosition + new Vector3(0, 0, 0.3f);
        pr2_animator.SetTrigger("open_instantly");
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
        placedToPose = false;
        moving_up = true;
        moving_to_place = false;
        moving_down = false;
        moving_arm_up = false;

        InitObjectToPlace();
        InitPlacePose();

        base.Run();
        //speechManager.Say("Running place to pose instruction.");
        //speechManager.Say("The robot is placing the object to preset place pose.");
    }
}
