using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointingHandMover : MonoBehaviour {

    private Vector3 spawnPoint;
    private Vector3 movePoint;
    private bool run = false;
    private GameObject detectedObject;
    private bool pointingToDetectedObject = false;
    	
	// Update is called once per frame
	void Update () {
        //if polygon points are same, then user didn't moved with it.. so play hand animation
        if (run) {
            if(detectedObject != null) {
                //Vector3 globPoint = detectedObject.GetComponent<Renderer>().bounds.max;
                //Vector3 edgePoint = worldAnchor.transform.InverseTransformPoint(globPoint.x, -globPoint.y, globPoint.z);
                //spawnPoint = new Vector3(edgePoint.x, edgePoint.y, 0.2f);
                //movePoint = new Vector3(edgePoint.x, edgePoint.y, -0.03f);
                spawnPoint = new Vector3(detectedObject.transform.localPosition.x, detectedObject.transform.localPosition.y, 0.2f);
                movePoint = new Vector3(detectedObject.transform.localPosition.x, detectedObject.transform.localPosition.y, -0.03f);              
                
            }
            else if (pointingToDetectedObject){
                Destroy(gameObject);
            }

            transform.localPosition = Vector3.Lerp(transform.localPosition, movePoint, Time.deltaTime * 1.5f);

            if (ObjectInPosition(gameObject, movePoint, 0.0005f)) {
                transform.localPosition = spawnPoint;
            }
        }
    }

    public void SetDetectedObjectReference(GameObject obj) {
        detectedObject = obj;
        pointingToDetectedObject = true;
    }

    public void Run(Vector3 _spawnPoint, Vector3 _movePoint) {
        gameObject.SetActive(true);        
        spawnPoint = _spawnPoint;
        movePoint = _movePoint;
        transform.localPosition = spawnPoint;

        run = true;
    }

    public void Stop() {
        run = false;
        gameObject.SetActive(false);
    }

    private bool ObjectInPosition(GameObject obj, Vector3 placePosition, float precision = 0.000005f) {
        //if error is approximately less than 5 mm
        if (Vector3.SqrMagnitude(obj.transform.localPosition - placePosition) < precision) {
            return true;
        }
        return false;
    }
}
