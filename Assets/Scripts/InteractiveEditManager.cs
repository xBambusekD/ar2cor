using ROSBridgeLib.art_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveEditManager : MonoBehaviour {

    private InterfaceStateMsg interfaceStateMsg;
    private InterfaceStateMsg interfaceStateMsgOldState;

    private bool interfaceStateChanged;

    //SINGLETON
    private static InteractiveEditManager instance;
    public static InteractiveEditManager Instance {
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
        interfaceStateChanged = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (SystemStarter.Instance.calibrated) {
            if (interfaceStateMsg != null) {                 
                //state learning
                if (interfaceStateMsg.GetSystemState() == 2) {
                    //TODO: kontrola jestli se interfaceState zmenil..
                    if (interfaceStateChanged) {
                        Debug.Log("InterfaceState changed!");


                        //already handled current interface state
                        interfaceStateChanged = false;
                    }
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
        }
        else {
            Debug.Log("FALSE InterfaceState did not changed!");
            interfaceStateChanged = false;
        }
    }
}
