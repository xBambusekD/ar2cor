using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationManager : MonoBehaviour {

    public GameObject worldAnchor;
    public GameObject worldAnchorVisualizationCube;
    public GameObject worldAnchorRecalibrationButton;
    private ARUWPController _ARUWPController;

    private void Start() {
        _ARUWPController = GetComponent<ARUWPController>();
    }

    public IEnumerator Calibrate() {
        _ARUWPController.startCalibration = true;
        //activate calibration cube
        worldAnchorVisualizationCube.gameObject.SetActive(true);

        yield return new WaitForSeconds(10);

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
        worldAnchorRecalibrationButton.gameObject.SetActive(true);

        yield return true;
    }

    public IEnumerator Recalibrate()
    {
#if !UNITY_EDITOR
        _ARUWPController.Resume();
#endif
        _ARUWPController.startCalibration = true;
        //activate calibration cube
        worldAnchorVisualizationCube.gameObject.SetActive(true);
        //hide recalibration button
        worldAnchorRecalibrationButton.gameObject.SetActive(false);

        yield return new WaitForSeconds(10);

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
        worldAnchorRecalibrationButton.gameObject.SetActive(true);

        yield return true;
    }

}
