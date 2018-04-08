using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsManager : MonoBehaviour {

    [SerializeField] private GameObject[] m_Prefabs;
    public GameObject worldAnchor;
    public List<GameObject> virtualObjectList = new List<GameObject>();
    
    private List<GameObject> objectsList = new List<GameObject>();
    private List<int> existingObjectsID = new List<int>();

    private JSONNode dataFromROS;

    //SINGLETON
    private static ObjectsManager instance;
    public static ObjectsManager Instance {
        get {
            return instance;
        }
    }
    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        }
        else {
            instance = this;
        }
    }

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        if (SystemStarter.Instance.calibrated) {
            existingObjectsID.Clear();

            if (dataFromROS != null) {

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
        GameObject instance = Instantiate(m_Prefabs[0]);
        instance.transform.parent = worldAnchor.transform;
        DetectedObject obj = instance.GetComponent<DetectedObject>();
        obj._id = objectData["name"].AsInt;
        obj.type = objectData["type"];
        obj.position = new Vector3(objectData["position"]["x"].AsFloat, -objectData["position"]["y"].AsFloat, objectData["position"]["z"].AsFloat);
        obj.rotation = new Quaternion(objectData["orientation"]["x"].AsFloat, objectData["orientation"]["y"].AsFloat, objectData["orientation"]["z"].AsFloat, objectData["orientation"]["w"].AsFloat);
        obj.bbox = new Vector3(objectData["bbox"]["x"].AsFloat, objectData["bbox"]["y"].AsFloat, objectData["bbox"]["z"].AsFloat);
        instance.tag = objectData["type"];
        objectsList.Add(instance);
    }

    private void updateObject(JSONNode objectData) {
        DetectedObject obj = null;
        foreach (GameObject detectedObject in objectsList) {
            obj = detectedObject.GetComponent<DetectedObject>();
            if (obj._id == objectData["name"].AsInt) {
                break;
            }
        }
        obj._id = objectData["name"].AsInt;
        obj.type = objectData["type"];
        //ARtable_x = Unity_x
        //ARtable_y = Unity_z
        //ARtable_z = Unity_x
        //z can be inverted
        obj.position = new Vector3(objectData["position"]["x"].AsFloat, -objectData["position"]["y"].AsFloat, objectData["position"]["z"].AsFloat);
        obj.rotation = new Quaternion(objectData["orientation"]["x"].AsFloat, objectData["orientation"]["y"].AsFloat, objectData["orientation"]["z"].AsFloat, objectData["orientation"]["w"].AsFloat);
        //Debug.Log(obj.rotation.eulerAngles);
        obj.bbox = new Vector3(objectData["bbox"]["x"].AsFloat, objectData["bbox"]["y"].AsFloat, objectData["bbox"]["z"].AsFloat);
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
        Vector3 dimensions = new Vector3();
        switch(objectName) {
            case "Stretcher":
                dimensions = new Vector3(0.046f, 0.046f, 0.154f);
                break;
            case "ShortLeg":
                dimensions = new Vector3(0.046f, 0.046f, 0.298f);
                break;
            case "LongLeg":
                dimensions = new Vector3(0.046f, 0.046f, 0.398f);
                break;
        }
        return dimensions;
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

}
