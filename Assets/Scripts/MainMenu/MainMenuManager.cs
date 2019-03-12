using HoloToolkit.Unity;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : Singleton<MainMenuManager> {

    public bool AutoConnect = false;

    public GameObject SetupPicker;
    public GameObject SetupList;
    public GameObject CollisionShapes;
    public GameObject IPSetup;
    public GameObject TableSetup;
    public GameObject SetupGrid;
    public GameObject SetupButtonPrefab;

    public GameObject MainMenuSetupButton;
    public GameObject MainMenuIPButton;
        
    private SetupsFileMsg setupsFileMsg;
    public ARTableSetupMsg currentSetup;
    private GameObject worldAnchor;

    public GameObject SpatialMapping;

    private void OnEnable() {
        SystemStarter.Instance.OnSystemStarted += PositionMenu;
    }

    private void OnDisable() {
        SystemStarter.Instance.OnSystemStarted -= PositionMenu;
    }

    // Use this for initialization
    void Start () {
        LoadAvailableSetups();

        SetupPicker.SetActive(true);
        SetupList.SetActive(false);
        CollisionShapes.SetActive(false);
        IPSetup.SetActive(false);
        TableSetup.SetActive(false);

        //turn off spatial mapping to enable user to connect
        SpatialMapping.SetActive(false);

        worldAnchor = GameObject.FindGameObjectWithTag("world_anchor");

        if(AutoConnect) {
            OnSetupPickerConnectButtonClicked();
        }
    }

    private void PositionMenu() {
        gameObject.transform.parent = worldAnchor.transform;
        gameObject.transform.localPosition = new Vector3(1.4f, -1f, 1.0f);
        gameObject.transform.localRotation = Quaternion.Euler(124f, -90f, -90f);

        if (AutoConnect) {
            gameObject.SetActive(false);
        }
    }

    public void OnSetupPickerSetupButtonClicked() {
        SetupPicker.SetActive(false);
        SetupList.SetActive(true);
    }

    public void OnSetupPickerIPButtonClicked() {
        IPSetup.SetActive(true);
    }

    //saves setups and connects to ros
    public void OnSetupPickerConnectButtonClicked() {
        SaveSetups();
        ROSCommunicationManager.Instance.ConnectToROS();

        SetupPicker.SetActive(false);
        CollisionShapes.SetActive(true);

        SetupList.SetActive(false);
        IPSetup.SetActive(false);
        TableSetup.SetActive(false);

        //turn on spatial mapping after user connects
        SpatialMapping.SetActive(true);
    }

    public void OnSetupListCreateNewButtonClicked() {
        TableSetup.SetActive(true);
    }

    public void OnIPSetupSaveButtonClicked() {
        IPSetup.SetActive(false);
    }

    public void OnTableSetupSaveButtonClicked() {
        TableSetup.SetActive(false);
    }

    public void OnTableSetupCancelButtonClicked() {
        TableSetup.SetActive(false);
    }
    
    private void LoadAvailableSetups() {
        string file;
        if (File.Exists(Application.persistentDataPath + "/saved_setups")) {
            file = File.ReadAllText(Application.persistentDataPath + "/saved_setups");
        }
        else {
            //init file with default setups in case that no save file exists
            file = "{\"last_setup\":\"ARTABLE SETUP 1\",\"saved_setups\":[{\"setup_name\":\"ARTABLE SETUP 1\",\"connection\":{\"ip\":\"192.168.104.200\",\"port\":\"9090\"},\"table_dims\":{\"width\":\"1.5\",\"length\":\"0.7\"}},{\"setup_name\":\"ARTABLE SETUP 3\",\"connection\":{\"ip\":\"192.168.1.227\",\"port\":\"9090\"},\"table_dims\":{\"width\":\"1\",\"length\":\"0.6\"}}]}";
            File.WriteAllText(Application.persistentDataPath + "/saved_setups", file);
        }

        setupsFileMsg = new SetupsFileMsg(JSONNode.Parse(file));
        
        foreach (ARTableSetupMsg setup in setupsFileMsg.GetSavedSetups()) {
            var button = Instantiate(SetupButtonPrefab, SetupGrid.transform);
            SetupButton setupButton = button.GetComponent<SetupButton>();
            setupButton.SetARTableSetup(setup);
            setupButton.SetButtonText(setup.GetSetupName());
        }

        //set last session's active setup
        SetActiveSetup(setupsFileMsg.GetActiveSetupMsg());
    }

    //saves all setups into file
    private void SaveSetups() {
        setupsFileMsg.SetActiveSetup(currentSetup.GetSetupName());
        File.WriteAllText(Application.persistentDataPath + "/saved_setups", setupsFileMsg.PrintCurrentString());
    }

    public void SetActiveSetup(ARTableSetupMsg setup) {
        currentSetup = setup;
        setupsFileMsg.SetActiveSetup(currentSetup.GetSetupName());

        MainMenuSetupButton.GetComponent<SetupButton>().SetButtonText(currentSetup.GetSetupName());
        MainMenuIPButton.GetComponent<SetupButton>().SetButtonText(currentSetup.GetIP());
        IPSetup.GetComponent<IPSetupHandler>().SetCurrentSetup(currentSetup);
        //actualize IP config for ROS connection
        ROSCommunicationManager.Instance.SetIPConfig(currentSetup.GetIP(), currentSetup.GetPort());
    }

    public void OnSetupListSetupButtonClicked() {
        SetupPicker.SetActive(true);
        SetupList.SetActive(false);
    }

    public void AddNewSetup(ARTableSetupMsg new_setup) {
        setupsFileMsg.StoreNewSetup(new_setup);

        var button = Instantiate(SetupButtonPrefab, SetupGrid.transform);
        SetupButton setupButton = button.GetComponent<SetupButton>();
        setupButton.SetARTableSetup(new_setup);
        setupButton.SetButtonText(new_setup.GetSetupName());

        SaveSetups();
    }

    public void AddIPConfigToSetup(string ip, string port) {
        setupsFileMsg.StoreIPConfigToSetup(currentSetup.GetSetupName(), ip, port);

        MainMenuIPButton.GetComponent<SetupButton>().SetButtonText(currentSetup.GetIP());

        //actualize IP config for ROS connection
        ROSCommunicationManager.Instance.SetIPConfig(currentSetup.GetIP(), currentSetup.GetPort());
        SaveSetups();
    }

    public Vector2 GetTableSize() {
        return new Vector2(float.Parse(currentSetup.GetWidth()), float.Parse(currentSetup.GetLength()));
    }
}
