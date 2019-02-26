using HoloToolkit.Unity;
using ROSBridgeLib.art_msgs;
using ROSBridgeLib.diagnostic_msgs;
using ROSBridgeLib.geometry_msgs;
using ROSBridgeLib.std_msgs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProgramHelper {

    public delegate void InterfaceStateChangedAction(InterfaceStateMsg msg);
    public static event InterfaceStateChangedAction OnInterfaceStateChanged;

    private static bool LoadingProgram = false;
    public static bool LoadProgram = true;
    
    private static ProgramMsg currentProgram;
    private static InterfaceStateMsg interfaceStateMsg = new InterfaceStateMsg("", InterfaceStateMsg.SystemState.STATE_UNKNOWN, new TimeMsg(0, 0), 0, 0,
                                new ProgramItemMsg(0, 0, 0, "", "", new List<string>(), new List<PoseStampedMsg>(),
                                    new List<PolygonStampedMsg>(), new List<ushort>(), new List<KeyValueMsg>(), new List<string>(), new List<SceneLabelMsg>()),
                                new List<KeyValueMsg>(), false, InterfaceStateMsg.ErrorSeverity.NONE, InterfaceStateMsg.ErrorCode.ERROR_UNKNOWN);

    public static ProgramItemMsg GetProgramItemById(ushort block_id, ushort ref_id) {
        ProgramItemMsg item = null;
        try {
            item = currentProgram.GetBlockByID(block_id).GetProgramItemByID(ref_id);
        } catch(NullReferenceException e) {
            Debug.Log(e);
        }
        return item;
    }

    public static bool ItemLearned(ushort block_id, ushort item_id) {
        ProgramItemMsg item = currentProgram.GetBlockByID(block_id).GetProgramItemByID(item_id);

        return InstructionHelper.InstructionLearned(item);
    }

    public static bool ItemLearned(ProgramItemMsg item) {
        return InstructionHelper.InstructionLearned(item);
    }

    public static bool CheckIfInterfaceStateChanged(InterfaceStateMsg currentState, InterfaceStateMsg newState) {
        //Debug.Log("CHECKING INTERFACE STATE CHANGE");
        //Debug.Log("old_state: " + currentState.ToYAMLString());
        //Debug.Log("new_state: " + newState.ToYAMLString());
        return !(currentState.GetSystemState() == newState.GetSystemState() &&
            currentState.GetProgramID() == newState.GetProgramID() &&
            currentState.GetBlockID() == newState.GetBlockID() &&
            currentState.GetEditEnabled() == newState.GetEditEnabled() &&
            currentState.GetProgramCurrentItem().ToYAMLString().Equals(newState.GetProgramCurrentItem().ToYAMLString()));
    }

    public static void SetInterfaceStateMsgFromROS(InterfaceStateMsg msg) {
        if (CheckIfInterfaceStateChanged(interfaceStateMsg, msg)) {
            Debug.Log("PH interface state changed");
            //Debug.Log(interfaceStateMsg.GetProgramID() != msg.GetProgramID());
            //Debug.Log(interfaceStateMsg.GetProgramID());
            //Debug.Log(msg.GetProgramID());
            //load new program if current has changed
            if (LoadProgram) {
                LoadingProgram = true;
                ROSCommunicationManager.Instance.ros.CallService(ROSCommunicationManager.programGetService, "{\"id\": " + msg.GetProgramID() + "}");
            }
            interfaceStateMsg = msg;

            //if currently loading program.. call the action after program was successfully loaded (e.g. in SetProgramMsgFromROS())
            if(OnInterfaceStateChanged != null && !LoadingProgram) {
                OnInterfaceStateChanged(interfaceStateMsg);
            }
        }
    }

    public static void SetProgramMsgFromROS(ProgramMsg msg) {
        LoadingProgram = false;

        //Debug.Log("PH new program loaded");
        currentProgram = msg;

        if (OnInterfaceStateChanged != null) {
            OnInterfaceStateChanged(interfaceStateMsg);
        }
    }
}
