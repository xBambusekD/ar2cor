using HoloToolkit.Unity;
using ROSBridgeLib.art_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveEditManager : Singleton<InteractiveEditManager> {

    private InterfaceStateMsg interfaceStateMsg;
    private InterfaceStateMsg interfaceStateMsgOldState;

    private bool interfaceStateChanged;

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

    public void SetInterfaceStateMsgFromROS(InterfaceStateMsg msg) {
        if ((interfaceStateMsg == null) || ProgramHelper.CheckIfInterfaceStateChanged(interfaceStateMsg, msg)) {
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
