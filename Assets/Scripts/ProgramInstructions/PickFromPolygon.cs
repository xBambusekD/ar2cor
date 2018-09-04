using ROSBridgeLib.art_msgs;
using ROSBridgeLib.geometry_msgs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickFromPolygon : ProgramInstruction {

    public ProgramItemMsg programItem;
    public GameObject objectToPick;
    public GameObject visualizeObjectPrefab;
    private GameObject world_anchor;
    private bool arm_attached;
    private bool arm_in_starting_position;
    private bool moving_down;
    Vector2 centroid = new Vector2();
    private Vector3 gripper_init_pose;
    private Quaternion arm_rotation;

    public override void Awake() {
        base.Awake();

        runTime = 0f;
        arm_attached = false;
        arm_in_starting_position = false;
        moving_down = false;
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
            else if (previous) {
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
                        pr2_arm.transform.localRotation = Quaternion.Slerp(pr2_arm.transform.localRotation, Quaternion.LookRotation(objectToPick.transform.localPosition - pr2_arm.transform.localPosition) * arm_rotation, Time.deltaTime * 2f);
                    }
                    else {
                        //move the gripper down to the object
                        if (moving_down) {
                            //open gripper
                            pr2_animator.SetBool("open_gripper", true);
                            //if gripper is opened, move it down to object
                            if (pr2_animator.GetCurrentAnimatorStateInfo(0).IsName("opened")) {
                                pr2_animator.SetBool("open_gripper", false);
                                pr2_arm.transform.localPosition = Vector3.Lerp(pr2_arm.transform.localPosition, objectToPick.transform.localPosition, Time.deltaTime * 2f);
                                if (ObjectInPosition(pr2_arm, objectToPick.transform.localPosition)) {
                                    //make the arm child of the object to move with the object
                                    pr2_arm.transform.parent = objectToPick.transform;

                                    moving_down = false;
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

            objectToPick.tag = programItem.GetObject()[0];
            //add reference on created object into ObjectsManagers list
            ObjectsManager.Instance.virtualObjectList.Add(objectToPick);
        }      

        //TODO: overit, ze v polygonu neni virtualni objekt .. pokud je, tak beru ho 
        PolygonMsg polygonMsg = programItem.GetPolygon()[0].GetPolygon();

        //compute centroid of polygon from which to pick
        PointMsg[] _points = polygonMsg.GetPoints();
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < _points.Length; i++) {
            points.Add(new Vector2(_points[i].GetX(), _points[i].GetY()));
        }
        centroid = GetCentroid(points);
        //apply this centroid to init of object to pick
        objectToPick.transform.localPosition = new Vector3(centroid.x, -centroid.y, 0);

        //rotate object to lay down on the table and to be parallel with world_anchor
        if(objectToPick.tag.Equals("Stretcher") || objectToPick.tag.Equals("LongLeg") || objectToPick.tag.Equals("ShortLeg") ||
            objectToPick.tag.Equals("Spojka") || objectToPick.tag.Equals("Dlouha_noha") || objectToPick.tag.Equals("Kratka_noha")) {
            objectToPick.transform.localEulerAngles = new Vector3(-90f, 0f, 0f);
        }
        else {
            objectToPick.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        }
    }

    private void InitRobotGripper() {
        //make sure that the robots arm is set to be the child of world_anchor
        pr2_arm.transform.parent = world_anchor.transform;
        pr2_arm.transform.GetChild(0).gameObject.SetActive(true);
        //spawn arm 0.3 meters above the object
        gripper_init_pose = objectToPick.transform.localPosition + new Vector3(0, 0, 0.3f);

        //move the gripper down to the object
        moving_down = true;
    }

    //computes centroid of polygon
    private Vector2 GetCentroid(List<Vector2> poly) {
        float accumulatedArea = 0.0f;
        float centerX = 0.0f;
        float centerY = 0.0f;

        for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++) {
            float temp = poly[i].x * poly[j].y - poly[j].x * poly[i].y;
            accumulatedArea += temp;
            centerX += (poly[i].x + poly[j].x) * temp;
            centerY += (poly[i].y + poly[j].y) * temp;
        }

        if (Math.Abs(accumulatedArea) < 1E-7f)
            return new Vector2();  // Avoid division by zero

        accumulatedArea *= 3f;
        return new Vector2(centerX / accumulatedArea, centerY / accumulatedArea);
    }


    private void SkipToEnd() {
        speechManager.StopSpeaking();
        //stop slightly before gripper reaches the object
        pr2_arm.transform.localPosition = objectToPick.transform.localPosition + new Vector3(0f, 0f, 0.001f);
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
        //remove reference on object from ObjectsManager
        ObjectsManager.Instance.virtualObjectList.Remove(objectToPick);
        Destroy(objectToPick);
    }

    //called when restart
    void OnEnable() {

        if (objectToPick != null && programItem != null) {
            objectToPick.SetActive(true);

            //apply this centroid to init of object to pick
            objectToPick.transform.localPosition = new Vector3(centroid.x, -centroid.y, 0);

            //rotate object to lay down on the table
            objectToPick.transform.localEulerAngles = new Vector3(-90f, 0f, 0f);
            //objectToPick.transform.localScale = ObjectsManager.Instance.getObjectDimensions(programItem.GetObject()[0]);

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
        moving_down = false;

        InitObjectToPick();
        InitRobotGripper();

        arm_rotation = Quaternion.Euler(-90f, 90f, 0f);         

        base.Run();
        //speechManager.Say("Running pick from polygon instruction.");
        speechManager.Say("The robot is grabbing the object from preset polygon.");
    }
}
