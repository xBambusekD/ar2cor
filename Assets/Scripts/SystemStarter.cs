using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using SimpleJSON;
using System.IO;
using Vuforia;
using UnbiasedTimeManager;

public class SystemStarter : Singleton<SystemStarter> {

    public delegate void SystemStartedAction();
    public event SystemStartedAction OnSystemStarted;

    public bool calibrated;
    public GameObject worldAnchor;
    public GameObject worldAnchorVisualizationCube;
    public GameObject worldAnchorRecalibrationButton;
    public GameObject helpAnchor;
    public GameObject helpAnchorVisualizationCube;
    public float worldAnchorOffset = 0.045f;
    //public TextToSpeechManager speechManager;
    
    private bool anchorLoaded;
    private bool calibration_launched;
    private List<GameObject> childrenToHide = new List<GameObject>();

    private bool ntpTimeSet = false;
    private bool robotRadiusCalled = false;
    private bool languageCalled = false;

    // Use this for initialization
    void Start() {
        worldAnchorRecalibrationButton.gameObject.SetActive(true);
        anchorLoaded = false;
        calibrated = false;
        calibration_launched = false;
    }
    	
	// Update is called once per frame
	void Update() {
        if(ROSCommunicationManager.Instance.connectedToROS) {

            if (!languageCalled) {
                TextToSpeechManager.Instance.LoadLanguage();
                languageCalled = true;
            }

            if (!ntpTimeSet) {
                ntpTimeSet = true;
                UnbiasedTime.Init(MainMenuManager.Instance.currentSetup.GetIP());
            }

            //Load known object types from database
            if (!ObjectsManager.Instance.objectReloadInitiated) {
                ObjectsManager.Instance.ReloadObjectTypes();
                ObjectsManager.Instance.objectReloadInitiated = true;
            }

            if (!ObjectsManager.Instance.objectTypesLoaded) {
                return;
            }

            //wait until time synchronizes with ntp
            if (!UnbiasedTime.Instance.TimeSynchronized) {
                //Debug.Log("TIME STILL NOT SYNCHRONIZED");
                return;
            }

            //Load robot reach radius
            if (!robotRadiusCalled) {
                robotRadiusCalled = true;
                RobotHelper.LoadRobotRadius();
                RobotHelper.LoadTableSize();
            }

            if (!TextToSpeechManager.Instance.languageSet) {
                return;
            }

#if UNITY_EDITOR
            if (!calibrated) {
                calibrated = true;

                //StartCoroutine(startFakeCalibration());
                //GameObject super_root = new GameObject("super_root");
                //super_root.transform.position = Vector3.zero;
                //worldAnchor.transform.parent = super_root.transform;
                //super_root.transform.eulerAngles = new Vector3(-90f, 0f, 0f);
                worldAnchor.transform.eulerAngles = new Vector3(-90f, 20f, 0f);
                //worldAnchor.transform.Rotate(180f, 0f, 0f, Space.Self);

                if (OnSystemStarted != null) {
                    OnSystemStarted();
                }

                //textToSpeechManager.Speak("Ahoj, já jsem Váš průvodce tímto programem. Musíte kliknout na objekt a poté na stůl. To je vše. A nečum.");
                //SpeechManagerAzure.Speak("Ahoj, ja som Váš sprievodca týmto programom. Musíte kliknúť na objekt a potom na stôl. To je všetko. A nečum.");
            }
#endif
#if !UNITY_EDITOR
            if (!anchorLoaded && !calibrated && !calibration_launched && (WorldAnchorManager.Instance.AnchorStore != null)) {
                string[] ids = WorldAnchorManager.Instance.AnchorStore.GetAllIds();
                //world anchor is present
                if (ids.Length == 1) {
                    //attaching an existing anchor name will load it instead
                    WorldAnchorManager.Instance.AttachAnchor(worldAnchor.gameObject, ids[0]);
                    worldAnchorVisualizationCube.gameObject.SetActive(true);
                    worldAnchorRecalibrationButton.gameObject.SetActive(true);
                    //helpAnchorVisualizationCube.gameObject.SetActive(false);

                    anchorLoaded = true;
                    calibrated = true;

                    if (OnSystemStarted != null) {
                        OnSystemStarted();
                    }
                }
                else {
                    StartCoroutine(startCalibration());
                    calibration_launched = true;
                }
            }
#endif
        }
    }

    //private IEnumerator startFakeCalibration() {
    //    TextToSpeechManager.Instance.Speak(Texts.OnCalibrationStarts);
    //    yield return new WaitWhile(() => TextToSpeechManager.Instance.IsSpeakingOrInQueue());
    //    TextToSpeechManager.Instance.Speak(Texts.OnCalibrationContinues);
    //}

    private IEnumerator startCalibration() {
        SetVuforiaActive(true);

        HideActiveChildrenObjects(true, worldAnchor);

        calibrated = false;
        //if it's recalibration.. remove current world anchor
        WorldAnchorManager.Instance.RemoveAnchor(worldAnchor.gameObject);

        TextToSpeechManager.Instance.Speak(Texts.OnCalibrationStarts);
        yield return new WaitWhile(() => TextToSpeechManager.Instance.IsSpeakingOrInQueue());
        TextToSpeechManager.Instance.Speak(Texts.OnCalibrationContinues);

        yield return new WaitWhile(() => CalibManager.Instance.allMarkersDetected == false);

        GameObject marker10 = new GameObject();
        GameObject marker11 = new GameObject();
        GameObject marker13 = new GameObject();

        foreach (GameObject marker in CalibManager.Instance.detectedMarkersList) {
            switch (marker.name) {
                case "HelpAnchor_10":
                    marker10 = marker;
                    break;
                case "HelpAnchor_11":
                    marker11 = marker;
                    break;
                case "HelpAnchor_13":
                    marker13 = marker;
                    break;
            }
        }

        worldAnchor.transform.position = marker10.transform.position;
        worldAnchor.transform.rotation = marker10.transform.rotation;


        //get directions from anchor to reference markers
        //
        //          ROBOT
        //   11 ------------- 12
        //    |               |
        //    |               |
        //   10 ------------- 13
        Vector3 pp03 = marker13.transform.position - worldAnchor.transform.position;
        Vector3 pp01 = marker11.transform.position - worldAnchor.transform.position;
        Vector3 n = Vector3.Cross(pp03, pp01);
        Matrix4x4 m = new Matrix4x4(new Vector4(pp03.x, pp01.x, n.x, 0),
                                    new Vector4(pp03.y, pp01.y, n.y, 0),
                                    new Vector4(pp03.z, pp01.z, n.z, 0),
                                    new Vector4(0, 0, 0, 1));

        worldAnchor.transform.rotation = m.inverse.rotation;
        //rotate around x axis to inverse Y axis
        worldAnchor.transform.Rotate(180f, 0f, 0f, Space.Self);


        //apply offset to anchor due to marker paper
        worldAnchorVisualizationCube.transform.localPosition = new Vector3(-0.208f, 0.121f, 0f);
        worldAnchor.transform.position = worldAnchorVisualizationCube.transform.position;
        worldAnchorVisualizationCube.transform.localPosition = new Vector3(0f, 0f, 0f);

        WorldAnchorManager.Instance.AttachAnchor(worldAnchor.gameObject);

        CalibManager.Instance.RefreshMarkers();

        calibrated = true;
        calibration_launched = false;

        HideActiveChildrenObjects(false, worldAnchor);

        TextToSpeechManager.Instance.Speak(Texts.OnCalibrationEnd);

        if (OnSystemStarted != null) {
            OnSystemStarted();
        }

        SetVuforiaActive(false);
    }

    private void HideActiveChildrenObjects(bool hide, GameObject parent) {
        // hide everything
        if (hide) {
            foreach(Transform child in parent.transform) {
                if(child.gameObject.activeSelf == true) {
                    child.gameObject.SetActive(false);
                    childrenToHide.Add(child.gameObject);
                }
            }
        }
        // show everything
        else {
            foreach(GameObject child in childrenToHide) {
                child.SetActive(true);
            }
            worldAnchorVisualizationCube.gameObject.SetActive(true);
            worldAnchorRecalibrationButton.gameObject.SetActive(true);
            childrenToHide.Clear();
        }
    }

    //look at in X direction pointing forward .. takes parameter of direction of looking (target - actual_position)
    private Quaternion XLookRotation(Vector3 direction) {
        Quaternion xToForward = Quaternion.Euler(90f, -90f, 0f);
        Quaternion forwardToTarget = Quaternion.LookRotation(direction, Vector3.up);

        return forwardToTarget * xToForward;
    }

    private Quaternion YLookRotation(Vector3 direction) {
        Quaternion yToForward = Quaternion.Euler(-90f, 0f, 0f);
        Quaternion forwardToTarget = Quaternion.LookRotation(direction, Vector3.up);

        return forwardToTarget * yToForward;
    }


    public void OnRecalibrationButtonClicked(GameObject obj) {
        if (!calibration_launched) {
            calibration_launched = true;
            StartCoroutine(startCalibration());
        }
    }

    private void SetVuforiaActive(bool vuforiaState) {
        VuforiaBehaviour.Instance.enabled = vuforiaState;
    }

}
