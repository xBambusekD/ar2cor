using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
public class SystemStarter : MonoBehaviour {
    
    public CalibrationManager calibrationManager;
    public bool calibrated;
    public GameObject worldAnchor;
    public GameObject worldAnchorVisualizationCube;
    public GameObject worldAnchorRecalibrationButton;
    public float worldAnchorOffset = 0.045f;
    public TextToSpeechManager speechManager;
    
    private bool anchorLoaded;
    private bool calibration_launched;

    //SINGLETON
    private static SystemStarter instance;
    public static SystemStarter Instance {
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
        worldAnchorRecalibrationButton.gameObject.SetActive(false);
        anchorLoaded = false;
        calibrated = false;
        calibration_launched = false;

#if UNITY_EDITOR
        calibrated = true;
#endif
        
    }
    	
	// Update is called once per frame
	void Update() {
        if(!anchorLoaded && !calibrated && !calibration_launched && (WorldAnchorManager.Instance.AnchorStore != null)) {
            string[] ids = WorldAnchorManager.Instance.AnchorStore.GetAllIds();
            //world anchor is present
            if(ids.Length == 1) {
                //attaching an existing anchor name will load it instead
                WorldAnchorManager.Instance.AttachAnchor(worldAnchor.gameObject, ids[0]);
                worldAnchorVisualizationCube.gameObject.SetActive(true);
                worldAnchorRecalibrationButton.gameObject.SetActive(true);
                
                anchorLoaded = true;
                calibrated = true;
            }
            else {
                StartCoroutine(startCalibration());
                calibration_launched = true;
            }
        }
    }

    private IEnumerator startCalibration() {
        //wait in case that Zira is still speaking
        yield return new WaitWhile(() => speechManager.textToSpeech.SpeechTextInQueue() || speechManager.textToSpeech.IsSpeaking());
        //announce that calibration will start
        speechManager.OnFirstCalibration();
        //wait until Zira stops speaking
        yield return new WaitWhile(() => speechManager.textToSpeech.SpeechTextInQueue() || speechManager.textToSpeech.IsSpeaking());
        yield return new WaitForSeconds(1);
        speechManager.OnCalibrationStart();

        yield return StartCoroutine(calibrationManager.Calibrate());
        //adjust world anchor position

        ApplyOffsetToWorldAnchor();

        //worldAnchor.transform.position += new Vector3(-worldAnchorOffset, worldAnchorOffset, -worldAnchorOffset);
        //worldAnchorVisualizationCube.transform.localPosition += new Vector3(-worldAnchorOffset, worldAnchorOffset, -worldAnchorOffset);
        WorldAnchorManager.Instance.AttachAnchor(worldAnchor.gameObject);
        calibrated = true;

        yield return new WaitWhile(() => speechManager.textToSpeech.SpeechTextInQueue() || speechManager.textToSpeech.IsSpeaking());
        speechManager.OnCalibrationEnd();
        calibration_launched = false;
    }

    private IEnumerator startRecalibration() {
        yield return new WaitWhile(() => speechManager.textToSpeech.SpeechTextInQueue() || speechManager.textToSpeech.IsSpeaking());
        yield return new WaitForSeconds(1);
        speechManager.OnCalibrationStart();

        calibrated = false;

        WorldAnchorManager.Instance.RemoveAnchor(worldAnchor.gameObject);

        yield return StartCoroutine(calibrationManager.Recalibrate());

        ApplyOffsetToWorldAnchor();

        //worldAnchor.transform.position += new Vector3(-worldAnchorOffset, worldAnchorOffset, -worldAnchorOffset);;
        //worldAnchorVisualizationCube.transform.localPosition += new Vector3(-worldAnchorOffset, worldAnchorOffset, -worldAnchorOffset);
        WorldAnchorManager.Instance.AttachAnchor(worldAnchor.gameObject);
        calibrated = true;

        yield return new WaitWhile(() => speechManager.textToSpeech.SpeechTextInQueue() || speechManager.textToSpeech.IsSpeaking());
        speechManager.OnCalibrationEnd();
        calibration_launched = false;
    }

    private void ApplyOffsetToWorldAnchor() {
        //adjust world anchor position
        Vector3 oldCubeLocalPosition = worldAnchorVisualizationCube.transform.localPosition;
        worldAnchorVisualizationCube.transform.localScale = new Vector3(1, 1, 1);
        worldAnchorVisualizationCube.transform.localPosition += new Vector3(-worldAnchorOffset, worldAnchorOffset, -worldAnchorOffset);
        ARUWPUtils.SetMatrix4x4ToGameObject(ref worldAnchor, worldAnchorVisualizationCube.transform.localToWorldMatrix);
        worldAnchorVisualizationCube.transform.localScale = new Vector3(0.09f, 0.09f, 0.09f);
        worldAnchorVisualizationCube.transform.localPosition = oldCubeLocalPosition;

        Debug.Log("WA glob " + worldAnchor.transform.eulerAngles);
        Debug.Log("WA local " + worldAnchor.transform.localEulerAngles);
        Debug.Log("CUBE glob " + worldAnchorVisualizationCube.transform.eulerAngles);
        Debug.Log("CUBE local " + worldAnchorVisualizationCube.transform.localEulerAngles);
    }

    public void OnRecalibrationButtonClicked(GameObject obj) {
        if (!calibration_launched) {
            calibration_launched = true;
            StartCoroutine(startRecalibration());
        }
    }

}
