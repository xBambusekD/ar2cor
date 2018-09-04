using HoloToolkit.Unity;
using ROSBridgeLib.art_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizationManager : Singleton<VisualizationManager> {

    private InterfaceStateMsg interfaceStateMsg;
    private HololensStateMsg hololensStateMsg;
    private HololensStateMsg hololensLastStateMsg;
    private bool visualization_running;
    private bool visualization_stopped;
    
    // Use this for initialization
    void Start () {
        visualization_running = false;
        visualization_stopped = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (SystemStarter.Instance.calibrated) {
            if (interfaceStateMsg != null) {
                if (interfaceStateMsg.GetSystemState() == 6) {
                    if (hololensStateMsg != null) {
                        if (hololensStateMsg.GetHololensState() == hololens_state.STATE_VISUALIZING) {
                            //sets current visualization state for program manager
                            ProgramManager.Instance.SetVisualizationState(hololensStateMsg.GetVisualizationState());
                            switch (hololensStateMsg.GetVisualizationState()) {
                                case visualization_state.VISUALIZATION_DISABLED:
                                    Debug.Log("VISUALIZATION_DISABLED");
                                    //when 'back to blocks' pressed
                                    ProgramManager.Instance.ClearProgram();
                                    hololensStateMsg = null;
                                    visualization_running = false;
                                    break;
                                case visualization_state.VISUALIZATION_RUN:
                                    if (!visualization_running) {
                                        Debug.Log("VISUALIZATION_RUN");
                                        ProgramManager.Instance.StartVisualization(interfaceStateMsg);
                                        //call for service to load current program.. response comes in ROSCommunicationManager's method ServiceCallBack()
                                        ROSCommunicationManager.Instance.ros.CallService("/art/db/program/get", "{\"id\": " + interfaceStateMsg.GetProgramID() + "}");                                        
                                        visualization_running = true;
                                        visualization_stopped = false;
                                        hololensStateMsg = null;
                                    }
                                    break;
                                case visualization_state.VISUALIZATION_PAUSE:
                                    if(visualization_running) {
                                        Debug.Log("VISUALIZATION_PAUSE");
                                        Time.timeScale = 0;
                                        visualization_running = false;
                                        hololensStateMsg = null;
                                    }
                                    break;
                                case visualization_state.VISUALIZATION_RESUME:
                                    if(!visualization_running) {
                                        Debug.Log("VISUALIZATION_RESUME");
                                        Time.timeScale = 1;
                                        visualization_running = true;
                                        hololensStateMsg = null;
                                    }
                                    break;
                                case visualization_state.VISUALIZATION_STOP:
                                    if (!visualization_stopped) {
                                        Debug.Log("VISUALIZATION_STOP");
                                        Time.timeScale = 1;
                                        ProgramManager.Instance.StopVisualization();
                                        visualization_running = false;
                                        visualization_stopped = true;
                                        hololensStateMsg = null;
                                    }
                                    break;
                                case visualization_state.VISUALIZATION_REPLAY:
                                    if (!visualization_running) {
                                        Debug.Log("VISUALIZATION_REPLAY");
                                        ProgramManager.Instance.ReplayVisualization();
                                        visualization_running = true;
                                        visualization_stopped = false;
                                        hololensStateMsg = null;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }
	}

    public void VisualizationInProcess(bool in_process) {
        visualization_running = in_process;
    }

    public void SetInterfaceStateMsgFromROS(InterfaceStateMsg msg) {
        interfaceStateMsg = msg;
    }

    public void SetHololensStateMsgFromROS(HololensStateMsg msg) {
        hololensStateMsg = msg;
        hololensLastStateMsg = msg;
    }
}
