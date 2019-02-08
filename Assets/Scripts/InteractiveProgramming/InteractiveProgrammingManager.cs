using HoloToolkit.Unity;
using ROSBridgeLib.art_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveProgrammingManager : Singleton<InteractiveProgrammingManager> {

    public enum ProgrammingManagerState {
        pick_from_feeder_learn,
        place_to_pose_learn,
        pick_from_feeder_vis,
        place_to_pose_vis,
        def
    }
    
    public ProgrammingManagerState CurrentState { get; private set; }

    private GameObject world_anchor;
    

    private void OnEnable() {
        ProgramHelper.OnInterfaceStateChanged += InterfaceStateChanged;
    }

    private void OnDisable() {
        ProgramHelper.OnInterfaceStateChanged -= InterfaceStateChanged;
    }

    // Use this for initialization
    void Start () {
        world_anchor = GameObject.FindGameObjectWithTag("world_anchor");
        CurrentState = ProgrammingManagerState.def;
    }
	
	// Update is called once per frame
	void Update () {

    }


    private void InterfaceStateChanged(InterfaceStateMsg interfaceStateMsg) {
        if(CurrentState != ProgrammingManagerState.def) {
            VisualizationClear();
        }

        if(interfaceStateMsg.GetSystemState() == 2) {
            switch(interfaceStateMsg.GetProgramCurrentItem().GetIType()) {
                case ProgramTypes.PICK_FROM_FEEDER:
                    PickFromFeederIP.Instance.SetInterfaceStateMsgFromROS(interfaceStateMsg);
                    if (interfaceStateMsg.GetEditEnabled()) {
                        CurrentState = ProgrammingManagerState.pick_from_feeder_learn;
                        PickFromFeederIP.Instance.StartLearning();
                    }
                    else {
                        CurrentState = ProgrammingManagerState.pick_from_feeder_vis;
                        PickFromFeederIP.Instance.Visualize();
                    }
                    break;
                case ProgramTypes.PLACE_TO_POSE:
                    PlaceToPoseIP.Instance.SetInterfaceStateMsgFromROS(interfaceStateMsg);
                    if (interfaceStateMsg.GetEditEnabled()) {
                        CurrentState = ProgrammingManagerState.place_to_pose_learn;
                        PlaceToPoseIP.Instance.StartLearning();
                    }
                    else {
                        CurrentState = ProgrammingManagerState.place_to_pose_vis;
                        PlaceToPoseIP.Instance.Visualize();
                    }
                    break;
                default:
                    CurrentState = ProgrammingManagerState.def;
                    break;
            }
        }
        else {
            CurrentState = ProgrammingManagerState.def;
        }
    }

    private void VisualizationClear() {
        switch(CurrentState) {
            case ProgrammingManagerState.pick_from_feeder_vis:
                PickFromFeederIP.Instance.VisualizeClear();
                break;
            case ProgrammingManagerState.place_to_pose_vis:
                PlaceToPoseIP.Instance.VisualizeClear();
                break;
            default:
                break;
        }
    }

}
