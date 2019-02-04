using HoloToolkit.Unity;
using ROSBridgeLib.art_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveProgrammingManager : Singleton<InteractiveProgrammingManager> {

    public bool StateLearning = false;

    private InterfaceStateMsg interfaceStateMsg;
    private ProgramItemMsg programItemMsg;

    private bool interfaceStateChanged;

    private GameObject world_anchor;

    //private GameObject tableDeskPrefab;
    //private GameObject tableDesk;

    // Use this for initialization
    void Start () {
        interfaceStateChanged = false;
        world_anchor = GameObject.FindGameObjectWithTag("world_anchor");
    }
	
	// Update is called once per frame
	void Update () {
        if (SystemStarter.Instance.calibrated) {
            if (interfaceStateMsg != null) {
                //state learning
                if (interfaceStateMsg.GetSystemState() == 2) {
                    StateLearning = true;
                    
                    //TODO: kontrola jestli se interfaceState zmenil..
                    if (interfaceStateChanged) {
                        Debug.Log("InterfaceState changed!");


                        //already handled current interface state
                        interfaceStateChanged = false;
                    }
                }
                else {
                    StateLearning = false;
                }
            }
        }
    }

    private bool InterfaceStateChanged(InterfaceStateMsg currentState, InterfaceStateMsg newState) {

        return !(currentState.GetSystemState() == newState.GetSystemState() &&
            currentState.GetProgramID() == newState.GetProgramID() &&
            currentState.GetBlockID() == newState.GetBlockID() &&
            currentState.GetEditEnabled() == newState.GetEditEnabled() &&
            currentState.GetProgramCurrentItem().ToYAMLString().Equals(newState.GetProgramCurrentItem().ToYAMLString()));
    }

    public void SetInterfaceStateMsgFromROS(InterfaceStateMsg msg) {
        if ((interfaceStateMsg == null) || InterfaceStateChanged(interfaceStateMsg, msg)) {
            Debug.Log("TRUE InterfaceState changed!");
            interfaceStateMsg = msg;
            interfaceStateChanged = true;

            PickFromFeederIP.Instance.SetInterfaceStateMsgFromROS(msg);
            PlaceToPoseIP.Instance.SetInterfaceStateMsgFromROS(msg);
        }
        else {
            Debug.Log("FALSE InterfaceState did not changed!");
            interfaceStateChanged = false;
        }
    }

    //private void SpawnTableArea() {
    //    tableDesk = Instantiate(tableDeskPrefab, world_anchor.transform);
    //    tableDesk.GetComponent<TableDeskInit>().InitTable();
    //}
}
