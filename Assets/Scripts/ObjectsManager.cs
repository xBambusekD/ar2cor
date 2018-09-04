using HoloToolkit.Unity;
using ROSBridgeLib.art_msgs;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsManager : Singleton<ObjectsManager> {

    [SerializeField] private GameObject[] m_Prefabs;
    public GameObject worldAnchor;
    public List<GameObject> virtualObjectList = new List<GameObject>();

    public List<GameObject> objectsList = new List<GameObject>();

    //list of known objects and their dimensions
    private List<ObjectTypeMsg> knownObjects = new List<ObjectTypeMsg>();
    private List<int> existingObjectsID = new List<int>();

    private JSONNode dataFromROS;

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        if (SystemStarter.Instance.calibrated) {
            if (dataFromROS != null) {
                existingObjectsID.Clear();

                for (int i = 0; i < dataFromROS.Count; i++) {
                    existingObjectsID.Add(dataFromROS[i]["name"].AsInt);

                    if (!checkIfObjectExists(dataFromROS[i]["name"].AsInt)) {
                        createObject(dataFromROS[i]);
                    }
                    else {
                        updateObject(dataFromROS[i]);
                    }

                }

                deleteRemovedObjects();
            }
        }
    }

    private void createObject(JSONNode objectData) {
        GameObject detectedObject = Instantiate(m_Prefabs[0]);
        detectedObject.transform.parent = worldAnchor.transform;
        
        DetectedObject obj = detectedObject.GetComponent<DetectedObject>();
        obj._id = objectData["name"].AsInt;
        obj.type = objectData["type"];
        obj.position = new Vector3(objectData["position"]["x"].AsFloat, -objectData["position"]["y"].AsFloat, objectData["position"]["z"].AsFloat);
        detectedObject.transform.localPosition = new Vector3(objectData["position"]["x"].AsFloat, -objectData["position"]["y"].AsFloat, objectData["position"]["z"].AsFloat);
        obj.rotation = new Quaternion(objectData["orientation"]["x"].AsFloat, objectData["orientation"]["y"].AsFloat, objectData["orientation"]["z"].AsFloat, objectData["orientation"]["w"].AsFloat);
        detectedObject.transform.localRotation = new Quaternion(objectData["orientation"]["x"].AsFloat, objectData["orientation"]["y"].AsFloat, objectData["orientation"]["z"].AsFloat, objectData["orientation"]["w"].AsFloat);
        obj.bbox = new Vector3(objectData["bbox"]["x"].AsFloat, objectData["bbox"]["y"].AsFloat, objectData["bbox"]["z"].AsFloat);
        detectedObject.transform.localScale = new Vector3(objectData["bbox"]["x"].AsFloat, objectData["bbox"]["y"].AsFloat, objectData["bbox"]["z"].AsFloat);
        detectedObject.tag = translate(objectData["type"]);
        objectsList.Add(detectedObject);
    }

    private string translate(string objectType) {
        string translatedType;
        switch (objectType) {
            case "Spojka":
                translatedType = "Stretcher";
                break;
            case "Kratka_noha":
                translatedType = "ShortLeg";
                break;
            case "Dlouha_noha":
                translatedType = "LongLeg";
                break;
            default:
                translatedType = objectType;
                break;
        }
        return translatedType;
    }

    private void updateObject(JSONNode objectData) {
        DetectedObject obj = null;
        foreach (GameObject detectedObject in objectsList) {
            obj = detectedObject.GetComponent<DetectedObject>();
            if (obj._id == objectData["name"].AsInt) {
                obj._id = objectData["name"].AsInt;
                obj.type = objectData["type"];
                obj.position = new Vector3(objectData["position"]["x"].AsFloat, -objectData["position"]["y"].AsFloat, objectData["position"]["z"].AsFloat);
                detectedObject.transform.localPosition = new Vector3(objectData["position"]["x"].AsFloat, -objectData["position"]["y"].AsFloat, objectData["position"]["z"].AsFloat);
                obj.rotation = new Quaternion(objectData["orientation"]["x"].AsFloat, objectData["orientation"]["y"].AsFloat, objectData["orientation"]["z"].AsFloat, objectData["orientation"]["w"].AsFloat);
                detectedObject.transform.localRotation = new Quaternion(objectData["orientation"]["x"].AsFloat, objectData["orientation"]["y"].AsFloat, objectData["orientation"]["z"].AsFloat, objectData["orientation"]["w"].AsFloat);
                obj.bbox = new Vector3(objectData["bbox"]["x"].AsFloat, objectData["bbox"]["y"].AsFloat, objectData["bbox"]["z"].AsFloat);
                detectedObject.transform.localScale = new Vector3(objectData["bbox"]["x"].AsFloat, objectData["bbox"]["y"].AsFloat, objectData["bbox"]["z"].AsFloat);

                break;
            }
        }        
    }

    private bool checkIfObjectExists(int id) {
        foreach (GameObject detectedObject in objectsList) {
            DetectedObject obj = detectedObject.GetComponent<DetectedObject>();
            if (obj._id == id) {
                return true;
            }
        }
        return false;
    }

    private void deleteRemovedObjects() {
        for (int i = 0; i < objectsList.Count; i++) {
            DetectedObject obj = objectsList[i].GetComponent<DetectedObject>();
            if (!existingObjectsID.Contains(obj._id)) {
                Destroy(objectsList[i]);
                objectsList.Remove(objectsList[i]);
            }
        }
    }

    public void setDataFromROS(JSONNode data) {
        dataFromROS = data;
    }

    //returns dimensions of object based on it's name
    public Vector3 getObjectDimensions(string objectName) {
        foreach (ObjectTypeMsg knownObject in knownObjects) {
            if (knownObject.GetName().Equals(objectName)) {
                if(knownObject.GetBBox().GetPrimitiveType() == ROSBridgeLib.shape_msgs.primitive_type.BOX) {
                    return new Vector3(knownObject.GetBBox().GetDimesions()[0], knownObject.GetBBox().GetDimesions()[1], knownObject.GetBBox().GetDimesions()[2]);
                }
            }
        }
        return new Vector3(0.046f, 0.046f, 0.154f);

        //Vector3 dimensions = new Vector3();
        //switch(objectName) {
        //    case "Stretcher":
        //    case "Spojka":
        //        dimensions = new Vector3(0.046f, 0.046f, 0.154f);
        //        break;
        //    case "ShortLeg":
        //    case "Kratka_noha":
        //        dimensions = new Vector3(0.046f, 0.046f, 0.298f);
        //        break;
        //    case "LongLeg":
        //    case "Dlouha_noha":
        //        dimensions = new Vector3(0.046f, 0.046f, 0.398f);
        //        break;
        //}
        //return dimensions;
    }
    
    //adds new object dimensions to list of known objects
    public void SetObjectTypeMsgFromROS(ObjectTypeMsg msg) {
        if(!ObjectIsKnown(msg.GetName())) {
            knownObjects.Add(msg);
        }
        foreach (ObjectTypeMsg knownObject in knownObjects) {            
            Debug.Log("KNOWN OBJECT: " + knownObject.GetName());
        }
    }

    //returns true/false if object exists in list of known objects
    public bool ObjectIsKnown(string objectName) {
        foreach (ObjectTypeMsg knownObject in knownObjects) {
            if (knownObject.GetName().Equals(objectName)) {
                return true;
            }
        }
        return false;
    }

    //returns list of detected objects of specific type (Stretcher|ShortLeg|LongLeg) that are in given polygon
    public List<GameObject> GetObjectsFromPolygon(List<Vector2> polygon_points, string objectType) {
        List<GameObject> objectsInPolygon = new List<GameObject>();

        foreach (GameObject detectedObject in objectsList) {
            if(detectedObject.tag.Equals(objectType)) {
                if (PointIsInPolygon(new Vector2(detectedObject.transform.localPosition.x, -detectedObject.transform.localPosition.y), polygon_points)) {
                    objectsInPolygon.Add(detectedObject);
                }
            }
        }

        return objectsInPolygon;
    }

    //returns list of virtual objects of specific type (Stretcher|ShortLeg|LongLeg) that are in given polygon
    public List<GameObject> GetVirtualObjectsFromPolygon(List<Vector2> polygon_points, string objectType) {
        List<GameObject> objectsInPolygon = new List<GameObject>();

        foreach (GameObject detectedObject in virtualObjectList) {
            if (detectedObject.tag.Equals(objectType)) {
                if (PointIsInPolygon(new Vector2(detectedObject.transform.localPosition.x, -detectedObject.transform.localPosition.y), polygon_points)) {
                    objectsInPolygon.Add(detectedObject);
                }
            }
        }

        return objectsInPolygon;
    }

    //determines if the given point is inside the polygon
    private bool PointIsInPolygon(Vector2 testPoint, List<Vector2> polygon) {
        bool result = false;
        int j = polygon.Count - 1;
        for (int i = 0; i < polygon.Count; i++) {
            if (polygon[i].y < testPoint.y && polygon[j].y >= testPoint.y || polygon[j].y < testPoint.y && polygon[i].y >= testPoint.y) {
                if (polygon[i].x + (testPoint.y - polygon[i].y) / (polygon[j].y - polygon[i].y) * (polygon[j].x - polygon[i].x) < testPoint.x) {
                    result = !result;
                }
            }
            j = i;
        }
        return result;
    }

    //returns list of objects placed on the table
    public List<GameObject> GetObjectsFromTable() {
        List<GameObject> objectsOnTable = new List<GameObject>();

        foreach (GameObject detectedObject in objectsList) {
            if (detectedObject.transform.localPosition.z < 0.05) {
                objectsOnTable.Add(detectedObject);
            }
        }

        return objectsOnTable;
    }

    //checks if at least one object is placed on the table, if so, return true
    public bool AnyObjectIsOnTable() {

        foreach (GameObject detectedObject in objectsList) {
            if (detectedObject.transform.localPosition.z < 0.05) {
                return true;
            }
        }

        return false;
    }
}
