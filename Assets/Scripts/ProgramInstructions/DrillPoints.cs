using ROSBridgeLib.art_msgs;
using ROSBridgeLib.geometry_msgs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillPoints : ProgramInstruction {
    public enum drilling_state : Int16 {
        INIT_DRILLING_HOLE_1 = 0,
        DRILL_HOLE_1 = 1,
        INIT_DRILLING_HOLE_2 = 2,
        DRILL_HOLE_2 = 3
    }

    public ProgramItemMsg programItem;
    //public GameObject objectToDrill;
    public GameObject visualizeObjectPrefab;
    private GameObject world_anchor;
    
    Vector2 centroid = new Vector2();
    private Vector3 gripper_init_pose;
    private Quaternion arm_rotation;
    private bool drilled;
    private bool arm_in_starting_position;
    private Vector3 pose1_init;
    private Vector3 pose2_init;
    private Vector3 poseDown;
    private Vector3 poseUp;
    private Quaternion orientation1;
    private Quaternion orientation2;
    private drilling_state drilling_State;
    private List<GameObject> objectsInPolygon = new List<GameObject>();

    private List<GameObject> objectsToDrill;
    private List<GameObject> objectsToDrillReferenceHolder;
    private bool move_down;
    private bool move_up;
    private GameObject pr2_arm_child;
    private bool init;

    public override void Awake() {
        base.Awake();

        runTime = 0f;
        drilled = false;
        move_down = false;
        move_up = false;
        init = false;
        arm_in_starting_position = false;
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
                if (!drilled || speechManager.IsSpeakingOrInQueue()) {
                    runTime += Time.deltaTime;
                                        
                    //move arm to starting position .. in case that it somewhere already exists
                    if (!arm_in_starting_position) {
                        if (ObjectInPosition(pr2_arm, gripper_init_pose)) {
                            arm_in_starting_position = true;
                        }
                        pr2_arm.transform.localPosition = Vector3.Lerp(pr2_arm.transform.localPosition, gripper_init_pose, Time.deltaTime * 2f);
                        //Look at cube .. arm_rotation - transforms arm to point to object with -x axis forward
                        pr2_arm.transform.localRotation = Quaternion.Slerp(pr2_arm.transform.localRotation, Quaternion.LookRotation(gripper_init_pose - new Vector3(0, 0, 0.3f) - pr2_arm.transform.localPosition) * arm_rotation, Time.deltaTime * 2f);
                    }
                    //start drilling
                    else {
                        //if there is still something to drill
                        if (objectsToDrill.Count > 0) {
                            pr2_animator.SetBool("close_gripper", true);
                            //if gripper is closed, move it down to objectgripper_init_pose
                            if (pr2_animator.GetCurrentAnimatorStateInfo(0).IsName("closed")) {
                                pr2_animator.SetBool("close_gripper", false);
                                //make the arm child of the object and adjust init poses for gripper
                                if (pr2_arm.transform.parent != objectsToDrill[0].transform) {
                                    pr2_arm.transform.parent = objectsToDrill[0].transform;
                                    AdjustInitPoses(objectsToDrill[0].transform);
                                }

                                //drilling moves
                                switch (drilling_State) {
                                    //init drilling first hole
                                    case drilling_state.INIT_DRILLING_HOLE_1:
                                        pr2_arm.transform.localPosition = Vector3.Lerp(pr2_arm.transform.localPosition, pose1_init, Time.deltaTime * 2f);
                                        //pr2_arm.transform.localRotation = Quaternion.Lerp(pr2_arm.transform.localRotation, orientation1, Time.deltaTime * 2f);
                                        if (ObjectInPosition(pr2_arm, pose1_init)) {
                                            drilling_State = drilling_state.DRILL_HOLE_1;

                                            move_down = true;
                                            move_up = false;
                                        }
                                        break;
                                    case drilling_state.DRILL_HOLE_1:
                                        if (move_down) {
                                            pr2_arm_child.transform.localPosition = Vector3.Lerp(pr2_arm_child.transform.localPosition, poseDown, Time.deltaTime * 2f);
                                            if (ObjectInPosition(pr2_arm_child, poseDown)) {
                                                move_down = false;
                                                move_up = true;
                                            }
                                        }
                                        if (move_up) {
                                            pr2_arm_child.transform.localPosition = Vector3.Lerp(pr2_arm_child.transform.localPosition, poseUp, Time.deltaTime * 2f);
                                            if (ObjectInPosition(pr2_arm_child, poseUp)) {
                                                //move the arm to its real previous position (Lerp makes the position slightly inaccurate) 
                                                pr2_arm_child.transform.localPosition = poseUp;
                                                move_down = true;
                                                move_up = false;
                                                //arm is up .. drilling of hole done .. move to next hole
                                                drilling_State = drilling_state.INIT_DRILLING_HOLE_2;
                                            }
                                        }
                                        break;
                                    case drilling_state.INIT_DRILLING_HOLE_2:
                                        pr2_arm.transform.localPosition = Vector3.Lerp(pr2_arm.transform.localPosition, pose2_init, Time.deltaTime * 2f);
                                        //pr2_arm.transform.localRotation = Quaternion.Lerp(pr2_arm.transform.localRotation, orientation2, Time.deltaTime * 2f);
                                        if (ObjectInPosition(pr2_arm, pose2_init)) {
                                            drilling_State = drilling_state.DRILL_HOLE_2;

                                            move_down = true;
                                            move_up = false;
                                        }
                                        break;
                                    case drilling_state.DRILL_HOLE_2:
                                        if (move_down) {
                                            pr2_arm_child.transform.localPosition = Vector3.Lerp(pr2_arm_child.transform.localPosition, poseDown, Time.deltaTime * 2f);
                                            if (ObjectInPosition(pr2_arm_child, poseDown)) {
                                                move_down = false;
                                                move_up = true;
                                            }
                                        }
                                        if (move_up) {
                                            pr2_arm_child.transform.localPosition = Vector3.Lerp(pr2_arm_child.transform.localPosition, poseUp, Time.deltaTime * 2f);
                                            if (ObjectInPosition(pr2_arm_child, poseUp)) {
                                                //move the arm to its real previous position (Lerp makes the position slightly inaccurate) 
                                                pr2_arm_child.transform.localPosition = poseUp;
                                                move_down = true;
                                                move_up = false;
                                                //init = true;

                                                //prepare drilling state for another object
                                                drilling_State = drilling_state.INIT_DRILLING_HOLE_1;

                                                //unchild the arm after drilling of object is finished
                                                pr2_arm.transform.parent = world_anchor.transform;

                                                //remove drilled object from list
                                                objectsToDrill.RemoveAt(0);

                                                //create reference to next object for init pose
                                                if (objectsToDrill.Count > 0) {
                                                    gripper_init_pose = objectsToDrill[0].transform.localPosition + new Vector3(0, 0, 0.3f);
                                                }
                                                //move back to initial position
                                                arm_in_starting_position = false;
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                        //if everything has been drilled then move back to init position and end
                        else {
                            pr2_arm.transform.localPosition = Vector3.Lerp(pr2_arm.transform.localPosition, gripper_init_pose, Time.deltaTime * 2f);
                            if (ObjectInPosition(pr2_arm, gripper_init_pose)) {
                                drilled = true;
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

    //swaps coords of init poses to keep the gripper allways above current object -> fokin hack
    private void AdjustInitPoses(Transform objectToDrill) {
        //pokud x je < -0.15, pak vrtam v ose X a osa X je otocena smerem dolu
        if(pr2_arm.transform.localPosition.x < -0.15f) {            
            //pokud sme zacli v x < -0.15 ale meli sme hybat v ose Y, pak vymenime osu X a Y
            if(pose1_init.y < -0.15f || pose1_init.y > 0.15f) {
                pose1_init = new Vector3(pose1_init.y, pose1_init.x, pose1_init.z);
                pose2_init = new Vector3(pose2_init.y, pose2_init.x, pose2_init.z);
            }
            //pokud je x kladne (vcetne nove vymeneneho).. musim invertovat
            if (pose1_init.x > 0f) {
                pose1_init.x = -pose1_init.x;
                pose2_init.x = -pose2_init.x;
            }
        }
        //pokud x je > 0.15, pak vrtam v ose X a osa X je otocena smerem nahoru
        else if (pr2_arm.transform.localPosition.x > 0.15f) {
            //pokud sme zacli v x > 0.15 ale meli sme hybat v ose Y, pak vymenime osu X a Y
            if (pose1_init.y < -0.15f || pose1_init.y > 0.15f) {
                pose1_init = new Vector3(pose1_init.y, pose1_init.x, pose1_init.z);
                pose2_init = new Vector3(pose2_init.y, pose2_init.x, pose2_init.z);
            }
            //pokud je x zaporne.. musim invertovat
            if (pose1_init.x < 0f) {
                pose1_init.x = -pose1_init.x;
                pose2_init.x = -pose2_init.x;
            }
        }
        //pokud y je < -0.15, pak vrtam v ose Y a osa Y je otocena smerem dolu
        else if (pr2_arm.transform.localPosition.y < -0.15f) {
            //pokud sme zacli v y < -0.15 ale meli sme hybat v ose X, pak vymenime osu X a Y
            if (pose1_init.x < -0.15f || pose1_init.x > 0.15f) {
                pose1_init = new Vector3(pose1_init.y, pose1_init.x, pose1_init.z);
                pose2_init = new Vector3(pose2_init.y, pose2_init.x, pose2_init.z);
            }
            //pokud je y kladne.. musim invertovat
            if (pose1_init.y > 0f) {
                pose1_init.y = -pose1_init.y;
                pose2_init.y = -pose2_init.y;
            }
        }
        //pokud y je > 0.15, pak vrtam v ose Y a osa Y je otocena smerem nahoru
        else if (pr2_arm.transform.localPosition.y > 0.15f) {
            //pokud sme zacli v y > 0.15 ale meli sme hybat v ose X, pak vymenime osu X a Y
            if (pose1_init.x < -0.15f || pose1_init.x > 0.15f) {
                pose1_init = new Vector3(pose1_init.y, pose1_init.x, pose1_init.z);
                pose2_init = new Vector3(pose2_init.y, pose2_init.x, pose2_init.z);
            }
            //pokud je y zaporne.. musim invertovat
            if (pose1_init.y < 0f) {
                pose1_init.y = -pose1_init.y;
                pose2_init.y = -pose2_init.y;
            }
        }
    }

    //if there is no detected or virtual object in polygon, then it creates completely new one
    private void InitObjectsToDrill() {
        objectsToDrill = new List<GameObject>();
        objectsToDrillReferenceHolder = new List<GameObject>();

        //get polygon points from current message
        PolygonMsg polygonMsg = programItem.GetPolygon()[0].GetPolygon();
        PointMsg[] _points = polygonMsg.GetPoints();
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < _points.Length; i++) {
            points.Add(new Vector2(_points[i].GetX(), _points[i].GetY()));
        }

        //find all virtual objects that might be in given polygon
        objectsInPolygon = ObjectsManager.Instance.GetVirtualObjectsFromPolygon(points, programItem.GetObject()[0]);
        //HIGHEST PRIORITY - we have some virtual objects placed in polygon
        if(objectsInPolygon.Count > 0) {
            objectsToDrill = new List<GameObject>(objectsInPolygon);
            objectsToDrillReferenceHolder = new List<GameObject>(objectsInPolygon);
        }
        //MID PRIORITY - we try to find real objects that are in polygon
        else {
            //find all physical objects that are in given polygon
            objectsInPolygon = ObjectsManager.Instance.GetObjectsFromPolygon(points, programItem.GetObject()[0]);
            //if in polygon are some detected objects .. instatiate on their poses virtual ones and drill all of them
            if (objectsInPolygon.Count > 0) {
                Debug.Log("POLYGON NOT EMPTY!");

                foreach (GameObject obj in objectsInPolygon) {
                    GameObject objectToDrill = new GameObject();
                    objectToDrill.transform.parent = world_anchor.transform;
                    objectToDrill.transform.localPosition = obj.transform.localPosition;
                    objectToDrill.transform.localRotation = obj.transform.localRotation;
                    objectToDrill.tag = programItem.GetObject()[0];

                    //instantiate actual object to drill and attach it to empty parent
                    GameObject childObjectToDrill = Instantiate(visualizeObjectPrefab, objectToDrill.transform);
                    childObjectToDrill.transform.localScale = ObjectsManager.Instance.getObjectDimensions(programItem.GetObject()[0]);
                    childObjectToDrill.transform.parent = objectToDrill.transform;

                    objectsToDrill.Add(objectToDrill);
                    objectsToDrillReferenceHolder.Add(objectToDrill);

                    //add reference on created object into ObjectsManagers list
                    ObjectsManager.Instance.virtualObjectList.Add(objectToDrill);
                }
            }
            //LOWEST PRIORITY - if there are no objects in polygon.. then create virtual one
            else {
                Debug.Log("POLYGON IS EMPTY!");

                //create empty parent object that has scale 1,1,1 .. when arm attaches it, it won't deform (cube for picking has different scale and it caused mess when arm attached it)
                GameObject objectToDrill = new GameObject();

                //objectToDrill is created at [0,0,0] world coords
                //making him child of world_anchor leaves him still at [0,0,0] world coords.. but shifts it's localPosition to be in respect to world_anchor
                //setting localPosition to [0,0,0] would make it appear on the same position as world_anchor
                objectToDrill.transform.parent = world_anchor.transform;
                objectToDrill.transform.localPosition = new Vector3(0, 0, 0);
                objectToDrill.tag = programItem.GetObject()[0];

                //compute centroid of polygon in which to drill
                centroid = GetCentroid(points);
                //apply this centroid to init of object to pick
                objectToDrill.transform.localPosition = new Vector3(centroid.x, -centroid.y, 0);

                //rotate object to lay down on the table and to be parallel with world_anchor
                objectToDrill.transform.localEulerAngles = new Vector3(-90f, 0f, 0f);

                //instantiate actual object to drill and attach it to empty parent
                GameObject childObjectToDrill = Instantiate(visualizeObjectPrefab, objectToDrill.transform);
                childObjectToDrill.transform.localScale = ObjectsManager.Instance.getObjectDimensions(programItem.GetObject()[0]);
                childObjectToDrill.transform.parent = objectToDrill.transform;

                objectsToDrill.Add(objectToDrill);
                objectsToDrillReferenceHolder.Add(objectToDrill);

                //add reference on created object into ObjectsManagers list
                ObjectsManager.Instance.virtualObjectList.Add(objectToDrill);
            }
        }        
    }

    private void InitRobotGripper() {
        //make sure that the robots arm is set to be the child of world_anchor
        pr2_arm.transform.parent = world_anchor.transform;
        pr2_arm.transform.GetChild(0).gameObject.SetActive(true);
        //spawn arm 0.3 meters above the object
        //pr2_arm.transform.localPosition = objectToPick.transform.localPosition + new Vector3(0, 0, 0.3f);
        
        gripper_init_pose = objectsToDrill[0].transform.localPosition + new Vector3(0, 0, 0.3f);
        //rotate the arm to point the gripper straight down to the table (-90f) and to be aligned with the object (x)
        //pr2_arm.transform.localEulerAngles = new Vector3(objectToPick.transform.localEulerAngles.x, -90f, 0);
        pr2_arm_child = pr2_arm.transform.GetChild(0).gameObject;
        //init poses for moving robot child arm down and up .. down by 20cm in its X coordinance and up by its starting value
        poseDown = pr2_arm_child.transform.localPosition - new Vector3(0.2f, 0f, 0f);
        poseUp = pr2_arm_child.transform.localPosition;
    }

    //inits poses of robot arm for drilling holes
    private void InitDrillingPoses() {
        List<PoseStampedMsg> poses = programItem.GetPose();
        PoseMsg position1 = poses[0].GetPose();
        PoseMsg position2 = poses[1].GetPose();
        //robots gripper local position starts at the grippers joint (not the tip).. so place the joint position as initial position for tip before drilling 
        pose1_init = new Vector3(position1.GetPosition().GetX(), -position1.GetPosition().GetY(), position1.GetPosition().GetZ());
        pose2_init = new Vector3(position2.GetPosition().GetX(), -position2.GetPosition().GetY(), position2.GetPosition().GetZ());

        orientation1 = new Quaternion(position1.GetOrientation().GetX(), position1.GetOrientation().GetY(), position1.GetOrientation().GetZ(), position1.GetOrientation().GetW());
        orientation2 = new Quaternion(position2.GetOrientation().GetX(), position2.GetOrientation().GetY(), position2.GetOrientation().GetZ(), position2.GetOrientation().GetW());

        //drilling_hole_1_init = true;
        drilling_State = drilling_state.INIT_DRILLING_HOLE_1;
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
        if (pr2_arm.transform.parent != world_anchor.transform) {
            pr2_arm.transform.parent = world_anchor.transform;
        }
        pr2_arm.transform.localPosition = gripper_init_pose;        
        pr2_arm.transform.localRotation = Quaternion.LookRotation(gripper_init_pose - new Vector3(0, 0, 0.3f) - pr2_arm.transform.localPosition) * arm_rotation;
        pr2_arm_child.transform.localPosition = poseUp;
        pr2_animator.SetTrigger("close_instantly");
        drilled = true;
    }

    private void GoBackToStart() {
        speechManager.StopSpeaking();
        pr2_arm_child.transform.localPosition = poseUp;
    }

    private void OnDestroy() {
        try {
            pr2_arm.transform.parent = world_anchor.transform;
            PlaceRobotGripperToInit();
        }
        catch(MissingReferenceException) {
            Debug.Log("MissingReferenceException: Object destroyed");
        }
        if (objectsToDrillReferenceHolder != null) {
            foreach (GameObject obj in objectsToDrillReferenceHolder) {
                ObjectsManager.Instance.virtualObjectList.Remove(obj);
                Destroy(obj);
            }
            objectsToDrillReferenceHolder.Clear();
        }
    }

    //called when restart
    void OnEnable() {        

        if (programItem != null) {
            PlaceRobotGripperToInit();
        }
    }

    //called when stop
    void OnDisable() {
        //get the child arm back to initial position
        //only if the instruction was currently running, pr2_arm_child and poseUp will be initialized
        if(run && pr2_arm != null) {
            pr2_arm_child.transform.localPosition = poseUp;
            pr2_arm_child.gameObject.SetActive(false);
            pr2_arm.transform.parent = world_anchor.transform;
        }
        
        if (objectsToDrillReferenceHolder != null) {
            foreach (GameObject obj in objectsToDrillReferenceHolder) {
                if (obj != null) {
                    ObjectsManager.Instance.virtualObjectList.Remove(obj);
                    Destroy(obj);
                    //obj.SetActive(false);
                }
            }
            objectsToDrillReferenceHolder.Clear();
        }
        
        run = false;
    }

    public override void Run() {
        //init everything
        runTime = 0f;
        drilled = false;
        move_down = false;
        move_up = false;
        init = false;
        arm_in_starting_position = false;

        InitObjectsToDrill();
        InitRobotGripper();
        InitDrillingPoses();

        arm_rotation = Quaternion.Euler(-90f, 90f, 0f);

        base.Run();
        //speechManager.Say("Running drill points instruction.");
        speechManager.Say("The robot is applying glue into the holes of the object.");
    }
}
