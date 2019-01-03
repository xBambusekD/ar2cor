using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationManager : MonoBehaviour {

    //public GameObject worldAnchor;
    //public GameObject worldAnchorVisualizationCube;
    //public GameObject worldAnchorHelpCube;
    //public GameObject worldAnchorRecalibrationButton;
    private ARUWPController _ARUWPController;
    private ARUWPMarker _ARUWPMarker;

    private void Start() {
        _ARUWPController = GetComponent<ARUWPController>();
        _ARUWPMarker = GetComponent<ARUWPMarker>();
    }

    public IEnumerator Calibrate(GameObject worldAnchor) {
#if !UNITY_EDITOR
        _ARUWPMarker.SetTarget(worldAnchor);
#endif
        GameObject worldAnchorVisualizationCube = worldAnchor.transform.Find("VisualizationCube").gameObject;
        GameObject worldAnchorRecalibrationButton = null;
        try {
            worldAnchorRecalibrationButton = worldAnchorVisualizationCube.transform.Find("RecalibrationButton").gameObject;
        }
        catch (NullReferenceException e) {
            Debug.Log(e);
        }

        _ARUWPController.startCalibration = true;
        //activate calibration cube
        worldAnchorVisualizationCube.gameObject.SetActive(true);

        yield return new WaitForSeconds(15);

        _ARUWPController.startCalibration = false;
#if !UNITY_EDITOR
        _ARUWPController.Pause();
        //magic ..wait till async task ends
        //yield return new WaitForSeconds(10);
        //_ARUWPController.stopRunning();
        //_ARUWPController.RemoveAllMarkers();
        //_ARUWPController.shutDownAR();
        
#endif
        //hide calibration cube
        //worldAnchorVisualizationCube.gameObject.SetActive(false);
        //activate recalibration button
        if(worldAnchorRecalibrationButton != null) 
            worldAnchorRecalibrationButton.gameObject.SetActive(true);

        yield return true;
    }

    public IEnumerator Recalibrate(GameObject worldAnchor)
    {
#if !UNITY_EDITOR
        _ARUWPMarker.SetTarget(worldAnchor);
#endif
        GameObject worldAnchorVisualizationCube = worldAnchor.transform.Find("VisualizationCube").gameObject;
        GameObject worldAnchorRecalibrationButton = null;
        try {
            worldAnchorRecalibrationButton = worldAnchorVisualizationCube.transform.Find("RecalibrationButton").gameObject;
        }
        catch (NullReferenceException e) {
            Debug.Log(e);
        }

#if !UNITY_EDITOR
        _ARUWPController.Resume();
#endif
        _ARUWPController.startCalibration = true;
        //activate calibration cube
        worldAnchorVisualizationCube.gameObject.SetActive(true);
        //hide recalibration button
        if (worldAnchorRecalibrationButton != null)
            worldAnchorRecalibrationButton.gameObject.SetActive(false);

        yield return new WaitForSeconds(15);

        _ARUWPController.startCalibration = false;
#if !UNITY_EDITOR
        _ARUWPController.Pause();
        //magic ..wait till async task ends
        //yield return new WaitForSeconds(10);
        //_ARUWPController.stopRunning();
        //_ARUWPController.RemoveAllMarkers();
        //_ARUWPController.shutDownAR();
        
#endif
        //hide calibration cube
        //worldAnchorVisualizationCube.gameObject.SetActive(false);
        //activate recalibration button
        if (worldAnchorRecalibrationButton != null)
            worldAnchorRecalibrationButton.gameObject.SetActive(true);

        yield return true;
    }

}
