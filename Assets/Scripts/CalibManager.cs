using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

public class CalibManager : Singleton<CalibManager> {

    public List<GameObject> detectedMarkersList = new List<GameObject>();
    public bool allMarkersDetected;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddDetectedMarker(GameObject marker) {
        detectedMarkersList.Add(marker);

        if (detectedMarkersList.Count == 3) {
            allMarkersDetected = true;
        }
        else {
            allMarkersDetected = false;
        }
    }

    public void RemoveDetectedMarker(GameObject marker) {
        detectedMarkersList.Remove(marker);
        allMarkersDetected = false;
    }

    public void RefreshMarkers() {
        foreach (GameObject marker in detectedMarkersList) {
            marker.GetComponent<UnparentObjectOnClick>().RefreshObjectAndHide();
        }
        detectedMarkersList.Clear();
        allMarkersDetected = false;
    }
}
