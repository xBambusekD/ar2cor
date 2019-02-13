using HoloToolkit.Unity;
using ROSBridgeLib.art_msgs;
using ROSBridgeLib.geometry_msgs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickFromPolygonIE : Singleton<PickFromPolygonIE> {

    public GameObject pointingHand;
    public GameObject pointingHandPrefab;

    private InterfaceStateMsg interfaceStateMsg;
    private ProgramItemMsg programItemMsg;

    private GameObject worldAnchor;
    private GameObject speechManagerObj;
    private TextToSpeechManager speechManager;

    private bool sayReward;
    private bool sayPlaceObject;
    private bool saySelectObject;
    private bool sayAdjustArea;
    private bool instructionProgrammed;

    private Vector3 spawnPoint;
    private Vector3 movePoint;
    private PolygonMsg originalPolygonMsg;
    private bool animationShowed;
    private List<GameObject> objectsOnTable = new List<GameObject>();
    private List<GameObject> pointingHandsList = new List<GameObject>();

    // Use this for initialization
    void Start () {
        worldAnchor = GameObject.FindGameObjectWithTag("world_anchor");
        speechManagerObj = GameObject.FindGameObjectWithTag("speech_manager");
        speechManager = speechManagerObj.GetComponent<TextToSpeechManager>();
        sayReward = false;
        sayPlaceObject = false;
        saySelectObject = false;
        sayAdjustArea = false;
        instructionProgrammed = false;

        animationShowed = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (SystemStarter.Instance.calibrated) {
            if (interfaceStateMsg != null) {
                //pick from polygon editing
                if (interfaceStateMsg.GetSystemState() == InterfaceStateMsg.SystemState.STATE_LEARNING && programItemMsg.GetIType() == "PickFromPolygon" &&
                    interfaceStateMsg.GetEditEnabled() == true) {
                    
                    //if no object is selected
                    if(programItemMsg.GetObject().Count == 0) {

                        //if no objects are placed on the table
                        //TODO: pokud je objekt detekovan v podavaci.. pocita se.. nemel by.. zkusit kontrolovat aj z souradnici
                        if(!ObjectsManager.Instance.AnyObjectIsOnTable() && !sayPlaceObject) {
                            speechManager.Say("I don't see any objects placed on the table. You have to place the object on the table that you want the robot to lift.");
                            Debug.Log("I don't see any objects placed on the table. You have to place the object on the table that you want the robot to lift.");
                            sayPlaceObject = true;
                            //in case that user removes, places and removes objects from the table
                            saySelectObject = false;
                        }
                        //some object is placed on the table
                        if (ObjectsManager.Instance.AnyObjectIsOnTable()) {
                            if (!saySelectObject) {
                                speechManager.Say("Select object type to be picked up by tapping on its outline.");
                                Debug.Log("Select object type to be picked up by tapping on its outline.");
                                saySelectObject = true;
                                //in case that user removes objects from the table
                                sayPlaceObject = false;

                                //init pointing hands
                                objectsOnTable = ObjectsManager.Instance.GetObjectsFromTable();
                                SpawnAndRunPointingHands();
                            }
                            
                        }

                        sayReward = true;
                    }
                    //if some object is selected
                    else if (programItemMsg.GetObject().Count > 0) {
                        StopAndDestroyPointingHands();
                        
                        if(sayReward) {
                            speechManager.Say("Perfect!");
                            Debug.Log("Perfect!");
                            sayReward = false;
                        }

                        if (!sayAdjustArea) {
                            speechManager.WaitAndSay("Adjust pick area as you want - or you can select another object type. When you are finished, click on done.");
                            Debug.Log("Adjust pick area as you want - or you can select another object type. When you are finished, click on done.");
                            sayAdjustArea = true;
                        }

                        //show hand and play it's animation
                        if(!pointingHand.activeSelf && !animationShowed) {
                            //pointingHand.SetActive(true);

                            originalPolygonMsg = programItemMsg.GetPolygon()[0].GetPolygon();

                            // 4-------3
                            // |       |
                            // |       |
                            // 1-------2
                            //load first point of polygon
                            Vector3 firstPoint = new Vector3(programItemMsg.GetPolygon()[0].GetPolygon().GetPoints()[0].GetX(),
                                                     -programItemMsg.GetPolygon()[0].GetPolygon().GetPoints()[0].GetY(),
                                                     programItemMsg.GetPolygon()[0].GetPolygon().GetPoints()[0].GetZ());
                            Vector3 secondPoint = new Vector3(programItemMsg.GetPolygon()[0].GetPolygon().GetPoints()[1].GetX(),
                                                           -programItemMsg.GetPolygon()[0].GetPolygon().GetPoints()[1].GetY(),
                                                           programItemMsg.GetPolygon()[0].GetPolygon().GetPoints()[1].GetZ());
                            //get middle point of bottom line
                            spawnPoint = firstPoint + (secondPoint - firstPoint) / 2 + new Vector3(0f, -0.05f, 0f);

                            movePoint = spawnPoint + new Vector3(0f, 0.15f, 0f);

                            //pointingHand.transform.localPosition = spawnPoint;

                            pointingHand.GetComponent<PointingHandMover>().Run(spawnPoint, movePoint);
                        }

                        //if polygon points are same, then user didn't moved with it.. so play hand animation
                        if(originalPolygonMsg.ToYAMLString().Equals(programItemMsg.GetPolygon()[0].GetPolygon().ToYAMLString())) {
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
                        instructionProgrammed = true;
                    }

                } 
                //reset all variables
                else {
                    if (instructionProgrammed) {
                        speechManager.Say("Good job! You have successfully programmed pick from polygon instruction.");
                        Debug.Log("Good job! You have successfully programmed pick from polygon instruction.");
                        instructionProgrammed = false;
                        pointingHand.GetComponent<PointingHandMover>().Stop();
                    }
                    //if user ended programming without doing anything
                    if (pointingHandsList.Count > 0) {
                        StopAndDestroyPointingHands();
                    }
                    sayReward = false;
                    sayPlaceObject = false;
                    saySelectObject = false;
                    sayAdjustArea = false;
                    animationShowed = false;
                }
            }
        }
    }

    private void SpawnAndRunPointingHands() {
        foreach(GameObject detectedObject in objectsOnTable) {
            GameObject pointHand = Instantiate(pointingHandPrefab);
            pointHand.transform.parent = worldAnchor.transform;
            pointHand.transform.localPosition = new Vector3(0, 0, 0);
            //set magic rotation
            pointHand.transform.localEulerAngles = new Vector3(14.3f, 44.7f, -152.1f);
            //Vector3 globPoint = detectedObject.GetComponent<Renderer>().bounds.max;
            //Vector3 edgePoint = worldAnchor.transform.InverseTransformPoint(globPoint.x, -globPoint.y, globPoint.z);
            pointHand.GetComponent<PointingHandMover>().SetDetectedObjectReference(detectedObject);
            pointHand.GetComponent<PointingHandMover>().Run(new Vector3(detectedObject.transform.localPosition.x, detectedObject.transform.localPosition.y, 0.2f), new Vector3(detectedObject.transform.localPosition.x, detectedObject.transform.localPosition.y, -0.03f));
            //pointHand.GetComponent<PointingHandMover>().Run(new Vector3(edgePoint.x, edgePoint.y, 0.2f), new Vector3(edgePoint.x, edgePoint.y, -0.03f));
            pointingHandsList.Add(pointHand);
        }
    }

    private void StopAndDestroyPointingHands() {
        foreach(GameObject pointHand in pointingHandsList) {
            Destroy(pointHand);
        }
        pointingHandsList.Clear();
    }

    //when user says "Repeat" keyword.. to repeat what Zira just said
    public void OnRepeat() {
        sayPlaceObject = false;
        saySelectObject = false;
        sayAdjustArea = false;
        animationShowed = false;
        StopAndDestroyPointingHands();
    }

    public void SetInterfaceStateMsgFromROS(InterfaceStateMsg msg) {
        interfaceStateMsg = msg;
        programItemMsg = msg.GetProgramCurrentItem();
    }

    private bool ObjectInPosition(GameObject obj, Vector3 placePosition, float precision = 0.000005f) {
        //if error is approximately less than 5 mm
        if (Vector3.SqrMagnitude(obj.transform.localPosition - placePosition) < precision) {
            return true;
        }
        return false;
    }
}
