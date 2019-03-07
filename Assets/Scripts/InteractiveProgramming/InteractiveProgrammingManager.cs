using HoloToolkit.Unity;
using ROSBridgeLib.art_msgs;
using ROSBridgeLib.std_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveProgrammingManager : Singleton<InteractiveProgrammingManager> {

    public enum ProgrammingManagerState {
        pick_from_feeder_learn,
        place_to_pose_learn_followed,
        place_to_pose_learn,
        pick_from_feeder_vis,
        place_to_pose_vis,
        def
    }
    
    public ProgrammingManagerState CurrentState { get; set; }

    public bool followedLearningPlacePoseOverride = false;
    public bool holoLearningEnabled = true;

    private void OnEnable() {
        ProgramHelper.OnInterfaceStateChanged += InterfaceStateChanged;
    }

    private void OnDisable() {
        ProgramHelper.OnInterfaceStateChanged -= InterfaceStateChanged;
    }

    // Use this for initialization
    void Start () {
        CurrentState = ProgrammingManagerState.def;
    }
	
	// Update is called once per frame
	void Update () {

    }


    private void InterfaceStateChanged(InterfaceStateMsg interfaceStateMsg) {
        if(CurrentState != ProgrammingManagerState.def) {
            VisualizationClear();
        }
        Debug.Log("NEW INTERFACE STATE! " + interfaceStateMsg.ToYAMLString());

        if (interfaceStateMsg.GetSystemState() == InterfaceStateMsg.SystemState.STATE_LEARNING) {
            switch (interfaceStateMsg.GetProgramCurrentItem().GetIType()) {
                case ProgramTypes.PICK_FROM_FEEDER:
                    PickFromFeederIP.Instance.SetInterfaceStateMsgFromROS(interfaceStateMsg);
                    if (interfaceStateMsg.GetEditEnabled() && holoLearningEnabled) {
                        CurrentState = ProgrammingManagerState.pick_from_feeder_learn;
                        PickFromFeederIP.Instance.StartLearning();
                        followedLearningPlacePoseOverride = true;
                    }
                    else if (!interfaceStateMsg.GetEditEnabled() && !followedLearningPlacePoseOverride) {
                        CurrentState = ProgrammingManagerState.pick_from_feeder_vis;
                        PickFromFeederIP.Instance.Visualize();
                    }
                    break;
                case ProgramTypes.PLACE_TO_POSE:
                    PlaceToPoseIP.Instance.SetInterfaceStateMsgFromROS(interfaceStateMsg);
                    if (interfaceStateMsg.GetEditEnabled() && holoLearningEnabled && followedLearningPlacePoseOverride &&
                        CurrentState != ProgrammingManagerState.place_to_pose_learn_followed) {
                        CurrentState = ProgrammingManagerState.place_to_pose_learn_followed;
                        PlaceToPoseIP.Instance.StartLearningContinuous();
                        //followedLearningPlacePoseOverride = false;
                    }
                    else if (interfaceStateMsg.GetEditEnabled() && holoLearningEnabled && !followedLearningPlacePoseOverride &&
                        CurrentState != ProgrammingManagerState.place_to_pose_learn) {
                        CurrentState = ProgrammingManagerState.place_to_pose_learn;
                        PlaceToPoseIP.Instance.StartLearning();
                    }
                    else if (!interfaceStateMsg.GetEditEnabled() && !followedLearningPlacePoseOverride) {
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
            followedLearningPlacePoseOverride = false;
        }
        Debug.Log(CurrentState);
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
                //PickFromFeederIP.Instance.VisualizeClear();
                //PlaceToPoseIP.Instance.VisualizeClear();
                break;
        }
    }

    public void EnableHoloLearning(bool holoEnabled) {
        holoLearningEnabled = holoEnabled;
        ROSCommunicationManager.Instance.ros.Publish(HoloLensLearningPublisher.GetMessageTopic(), new BoolMsg(holoLearningEnabled), debug_log: false);
    }

}
