using HoloToolkit.Unity;
using ROSBridgeLib.art_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Instruction : Singleton<Instruction> {

    private InterfaceStateMsg interfaceStateMsg;
    private ProgramItemMsg programItemMsg;

    public void SetInterfaceStateMsgFromROS(InterfaceStateMsg msg) {
        interfaceStateMsg = msg;
        programItemMsg = msg.GetProgramCurrentItem();
    }

}
