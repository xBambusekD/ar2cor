using ROSBridgeLib.art_msgs;
using ROSBridgeLib.geometry_msgs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickFromFeeder : ProgramInstruction {

    public ProgramItemMsg programItem;
    public GameObject objectToPick;
    public GameObject visualizeObjectPrefab;
    private GameObject world_anchor;
    private bool arm_attached;
    private bool arm_in_starting_position;
    private bool moving_to_object;
    //true => left feeder .. false => right feeder
    private bool left_feeder;
    //how robots arm should be rotated
    private Quaternion arm_rotation;
    private PoseMsg gripper_pose;
    private Vector3 gripper_init_pose;

    public override void Awake() {
        base.Awake();

        runTime = 0f;
        arm_attached = false;
        arm_in_starting_position = false;
        moving_to_object = false;
        world_anchor = GameObject.FindGameObjectWithTag("world_anchor");        
    }
	
	// Update is called once per frame
	void Update () {
		if(run) {                       

            //if user says "Next" then skip to end state
            if (next) {
                SkipToEnd();
                run = false;
                next = false;
            }
            //if user says "Previous" then skip to initial state
            else if(previous) {
                GoBackToStart();
                run = false;
                previous = false;
            }
            //normal run
            else { 
                if (!arm_attached || speechManager.IsSpeakingOrInQueue()) {
                    runTime += Time.deltaTime;
                    
                    //move arm to starting position .. in case that it somewhere already exists
                    if (!arm_in_starting_position) {
                        if (ObjectInPosition(pr2_arm.gameObject, gripper_init_pose)) {
                            arm_in_starting_position = true;
                        }
                        pr2_arm.transform.localPosition = Vector3.Lerp(pr2_arm.transform.localPosition, gripper_init_pose, Time.deltaTime * 2f);
                        //Look at cube .. arm_rotation - transforms arm to point to object with -x axis forward
                        pr2_arm.transform.localRotation = Quaternion.Lerp(pr2_arm.transform.localRotation, Quaternion.LookRotation(objectToPick.transform.localPosition - pr2_arm.transform.localPosition) * arm_rotation, Time.deltaTime * 2f);
                    }
                    else {
                        //move the gripper down to the object
                        if (moving_to_object) {
                            //open gripper
                            pr2_animator.SetBool("open_gripper", true);
                            //if gripper is opened, move it down to object
                            if (pr2_animator.GetCurrentAnimatorStateInfo(0).IsName("opened")) {
                                pr2_animator.SetBool("open_gripper", false);
                                pr2_arm.transform.localPosition = Vector3.Lerp(pr2_arm.transform.localPosition, objectToPick.transform.localPosition, Time.deltaTime * 2f);
                                //Look at cube .. arm_rotation - transforms arm to point to object with -x axis forward
                                pr2_arm.transform.localRotation = Quaternion.Lerp(pr2_arm.transform.localRotation, Quaternion.LookRotation(objectToPick.transform.localPosition - pr2_arm.transform.localPosition) * arm_rotation, Time.deltaTime * 2f);
                                if (ObjectInPosition(pr2_arm, objectToPick.transform.localPosition)) {
                                    //make the arm child of the object to move with the object
                                    pr2_arm.transform.parent = objectToPick.transform;

                                    moving_to_object = false;
                                }
                            }
                        }
                        else {
                            //close the gripper to grab the object
                            pr2_animator.SetBool("grab", true);
                            if (pr2_animator.GetCurrentAnimatorStateInfo(0).IsName("grabbed")) {
                                pr2_animator.SetBool("grab", false);
                                arm_attached = true;
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

    private void InitObjectToPick() {

        //If no object created yet, then create new one and set everything needed for the freshly created object
        if (objectToPick == null) {
            //create empty parent object that has scale 1,1,1 .. when arm attaches it, it won't deform (cube for picking has different scale and it caused mess when arm attached it)
            objectToPick = new GameObject();

            //objectToPick is created at [0,0,0] world coords
            //making him child of world_anchor leaves him still at [0,0,0] world coords.. but shifts it's localPosition to be in respect to world_anchor
            //setting localPosition to [0,0,0] would make it appear on the same position as world_anchor
            objectToPick.transform.parent = world_anchor.transform;
            objectToPick.transform.localPosition = new Vector3(0, 0, 0);

            //instantiate actual object to pick and attach it to empty parent        
            GameObject childObjectToPick = Instantiate(visualizeObjectPrefab, objectToPick.transform);
            childObjectToPick.transform.localScale = ObjectsManager.Instance.getObjectDimensions(programItem.GetObject()[0]);
            childObjectToPick.transform.parent = objectToPick.transform;

            objectToPick.name = programItem.GetObject()[0];

            //add reference on created object into ObjectsManagers list
            ObjectsManager.Instance.virtualObjectList.Add(objectToPick);
        }

        //STANDARD SEGMENT OF CODE that will run everytime - places created or existing object to intial position
        gripper_pose = programItem.GetPose()[0].GetPose();

        objectToPick.transform.localPosition = gripper_init_pose;
        //object is placed on left feeder
        if (left_feeder) {
            //rotate the object to make right move
            objectToPick.transform.localEulerAngles = new Vector3(-90f, 90f, 0f);
            //position the object to fit approximately on the feeder
            objectToPick.transform.localPosition += new Vector3(-0.30f, 0f, 0f);
        }
        //object is placed on right feeder
        else {
            //rotate the object to make right move
            objectToPick.transform.localEulerAngles = new Vector3(-90f, -90f, 0f);
            //position the object to fit approximately on the feeder
            objectToPick.transform.localPosition += new Vector3(0.30f, 0f, 0f);
        }
    }

    private void InitRobotGripper() {
        //make sure that the robots arm is set to be the child of world_anchor
        pr2_arm.transform.parent = world_anchor.transform;
        pr2_arm.transform.GetChild(0).gameObject.SetActive(true);
        
        //determine from which feeder the robot is going to grab.. if it's on the left side of the table (<1m) then left feeder and vice versa
        left_feeder = programItem.GetPose()[0].GetPose().GetPosition().GetX() < 1f;
        if (left_feeder) {
            arm_rotation = Quaternion.Euler(90f, 90f, 0f);
        }
        else {
            arm_rotation = Quaternion.Euler(-90f, 90f, 0f);
        }
        gripper_pose = programItem.GetPose()[0].GetPose();
        gripper_init_pose = new Vector3(gripper_pose.GetPosition().GetX(), -gripper_pose.GetPosition().GetY(), gripper_pose.GetPosition().GetZ());

        moving_to_object = true;
    }

    private void SkipToEnd() {
        speechManager.StopSpeaking();
        //if robot is picking from left/right feeder, then stop slightly before gripper reaches it
        if(left_feeder) {
            pr2_arm.transform.localPosition = objectToPick.transform.localPosition + new Vector3(0.001f, 0f, 0f);
        } else {
            pr2_arm.transform.localPosition = objectToPick.transform.localPosition - new Vector3(0.001f, 0f, 0f);
        }
        pr2_arm.transform.localRotation = Quaternion.LookRotation(objectToPick.transform.localPosition - pr2_arm.transform.localPosition) * arm_rotation;
        pr2_arm.transform.parent = objectToPick.transform;
        pr2_animator.SetTrigger("grab_instantly");
        arm_attached = true;
    }

    private void GoBackToStart() {
        speechManager.StopSpeaking();
    }

     private void OnDestroy() {
        try {
            pr2_arm.transform.parent = world_anchor.transform;
            PlaceRobotGripperToInit();
        }
        catch (MissingReferenceException) {
            Debug.Log("MissingReferenceException: Object destroyed");
        }

        try {
            //remove reference on object from ObjectsManager
            ObjectsManager.Instance.virtualObjectList.Remove(objectToPick);
            Destroy(objectToPick);
        }
        catch(NullReferenceException e) {
            Debug.Log(e);
        }
    }

    //called when restart
    void OnEnable() {

        if (objectToPick != null && programItem != null) {
            objectToPick.SetActive(true);

            PoseMsg gripper_pose = programItem.GetPose()[0].GetPose();

            objectToPick.transform.localPosition = new Vector3(gripper_pose.GetPosition().GetX(), -gripper_pose.GetPosition().GetY(), gripper_pose.GetPosition().GetZ());
            if (left_feeder) {
                //rotate the object to make right move
                objectToPick.transform.localEulerAngles = new Vector3(-90f, 90f, 0f);
                //position the object to fit approximately on the feeder
                objectToPick.transform.localPosition += new Vector3(-0.30f, 0f, 0f);
            }
            else {
                //rotate the object to make right move
                objectToPick.transform.localEulerAngles = new Vector3(-90f, -90f, 0f);
                //position the object to fit approximately on the feeder
                objectToPick.transform.localPosition += new Vector3(0.30f, 0f, 0f);
            }

            //InitRobotGripper();
            PlaceRobotGripperToInit();
        }
    }

    //called when stop
    void OnDisable() {
        if (objectToPick != null) {
            objectToPick.SetActive(false);            
        }
        if (pr2_arm != null) {
            pr2_arm.transform.GetChild(0).gameObject.SetActive(false);
            pr2_arm.transform.parent = world_anchor.transform;
        }
        run = false;
    }

    public override void Run() {
        //init everything      
        runTime = 0f;
        arm_attached = false;
        arm_in_starting_position = false;

        InitRobotGripper();
        InitObjectToPick();
        base.Run();
        if(left_feeder) {
            speechManager.Say("The robot is grabbing the object from feeder on your left side.");
        }
        else {
            speechManager.Say("The robot is grabbing the object from feeder on your right side.");
        }
        //speechManager.Say("Running pick from feeder instruction.");
    }
}
