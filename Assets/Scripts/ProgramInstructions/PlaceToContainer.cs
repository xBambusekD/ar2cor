using ROSBridgeLib.art_msgs;
using ROSBridgeLib.geometry_msgs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceToContainer : ProgramInstruction {

    public ProgramItemMsg programItem;
    public GameObject referenceItem;

    public GameObject objectToPlace;

    //private PoseMsg placePose;
    private Vector3 placePosition;
    private Vector3 upPosition;
    private Quaternion placeQuaternion;
    private GameObject world_anchor;

    private bool placedToPose;
    private bool moving_up;
    private bool moving_to_place;
    private bool dropping;

    private Rigidbody objectToPlaceRigidbody;

    Vector2 centroid = new Vector2();
    private List<GameObject> objectsInPolygon = new List<GameObject>();

    public override void Awake() {
        base.Awake();

        runTime = 0f;
        placedToPose = false;
        moving_to_place = true;
        dropping = false;
        objectToPlaceRigidbody = null;

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
                        //move the object above the place position
                        if (moving_to_place) {
                            objectToPlace.transform.localPosition = Vector3.Lerp(objectToPlace.transform.localPosition, upPosition, Time.deltaTime * 2f);
                            objectToPlace.transform.localRotation = Quaternion.Lerp(objectToPlace.transform.localRotation, placeQuaternion, Time.deltaTime * 2f);
                            if (ObjectInPosition(objectToPlace, upPosition)) {
                                moving_to_place = false;
                                dropping = true;
                            }
                        }
                        //drop the object
                        if (dropping) {
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

                                dropping = false;
                                placedToPose = true;
                            }

                            if(objectToPlaceRigidbody == null) {
                                ////turn off collisions with objects in polygon
                                //foreach(GameObject obj in objectsInPolygon) {
                                //    Physics.IgnoreCollision(objectToPlace.GetComponentInChildren<Collider>(), obj.GetComponent<Collider>());
                                //}
                                objectToPlaceRigidbody = objectToPlace.AddComponent<Rigidbody>();
                                objectToPlaceRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
                                objectToPlaceRigidbody.mass = 100f;
                            }
                            
                            //objectToPlace.transform.localPosition = Vector3.Lerp(objectToPlace.transform.localPosition, placePosition, Time.deltaTime * 2f);
                            //if (ObjectInPosition(objectToPlace, placePosition)) {
                            //    dropping = false;
                                
                            //    //object is finally placed to position
                            //    placedToPose = true;
                            //}
                        }
                    }
                }
                else {
                    if (objectToPlaceRigidbody != null) {
                        objectToPlaceRigidbody.isKinematic = true;
                        Destroy(objectToPlaceRigidbody);
                    }
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
        //placePose = programItem.GetPose()[0].GetPose();
        //placePosition = new Vector3(placePose.GetPosition().GetX(), -placePose.GetPosition().GetY(), placePose.GetPosition().GetZ() - 0.023f);
        //placePosition = new Vector3(placePose.GetPosition().GetX(), -placePose.GetPosition().GetY(), 0f);
        //placeQuaternion = new Quaternion(-placePose.GetOrientation().GetX(), placePose.GetOrientation().GetY(), -placePose.GetOrientation().GetZ(), placePose.GetOrientation().GetW());
        //upPosition = objectToPlace.transform.localPosition + new Vector3(0,0,0.1f);


        PolygonMsg polygonMsg = programItem.GetPolygon()[0].GetPolygon();

        //compute centroid of polygon from which to pick
        PointMsg[] _points = polygonMsg.GetPoints();
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < _points.Length; i++) {
            points.Add(new Vector2(_points[i].GetX(), _points[i].GetY()));
        }
        centroid = GetCentroid(points);

        placePosition = new Vector3(centroid.x, -centroid.y, objectToPlace.transform.GetChild(0).transform.localScale.x / 2f);
        


        //rotate object to lay down on the table and to be parallel with world_anchor
        if (objectToPlace.name.Equals("Stretcher") || objectToPlace.name.Equals("LongLeg") || objectToPlace.name.Equals("ShortLeg") ||
            objectToPlace.name.Equals("Spojka") || objectToPlace.name.Equals("Dlouha_noha") || objectToPlace.name.Equals("Kratka_noha")) {
            placeQuaternion = Quaternion.Euler(new Vector3(-90f, 0f, 0f));
        }
        else {
            placeQuaternion = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        }

        //get list of objects in place polygon in order to turn collisions off with the virtual rigidbody object
        objectsInPolygon = ObjectsManager.Instance.GetObjectsFromPolygon(points);

        //place it directly to container
        foreach(var obj in objectsInPolygon) {
            if(obj.name.Contains("kontejner") || obj.name.Contains("container")) {
                placePosition = obj.transform.localPosition;
                break;
            }
        }

        upPosition = placePosition + new Vector3(0, 0, 0.3f);
    }

    private void SkipToEnd() {
        objectToPlace.transform.localPosition = placePosition;
        objectToPlace.transform.localRotation = placeQuaternion;
        if (pr2_arm.transform.parent != world_anchor.transform) {
            pr2_arm.transform.parent = world_anchor.transform;
        }
        pr2_arm.transform.localPosition = upPosition;
        pr2_animator.SetTrigger("open_instantly");
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
        moving_to_place = true;
        dropping = false;

        InitObjectToPlace();
        InitPlacePose();

        base.Run();

        //speechManager.Say("After classification, the robot places the object to the container.");
    }
}
