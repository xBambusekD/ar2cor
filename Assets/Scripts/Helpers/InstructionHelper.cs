using HoloToolkit.Unity;
using ROSBridgeLib.art_msgs;
using ROSBridgeLib.diagnostic_msgs;
using ROSBridgeLib.geometry_msgs;
using ROSBridgeLib.std_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InstructionHelper {

    public static bool InstructionLearned(ProgramItemMsg item) {
        switch (item.GetIType()) {
            case ProgramTypes.PICK_FROM_FEEDER:
                return ObjectSet(item.GetObject()) && PoseSet(item.GetPose()[0].GetPose());
            case ProgramTypes.PLACE_TO_POSE:
                return PoseSet(item.GetPose()[0].GetPose()) && RefIDSet(item.GetRefID());
            case ProgramTypes.PICK_FROM_POLYGON:
                return ObjectSet(item.GetObject()) && PolygonSet(item.GetPolygon()[0].GetPolygon());
            case ProgramTypes.VISUAL_INSPECTION:
                return PoseSet(item.GetPose()[0].GetPose()) && RefIDSet(item.GetRefID());
            case ProgramTypes.PLACE_TO_CONTAINER:
                return ObjectSet(item.GetObject()) && PolygonSet(item.GetPolygon()[0].GetPolygon());
            default:
                break;
        }
        return true;
    }

    private static bool ObjectSet(List<string> objects) {
        if(objects.Count == 0)
            return false;

        return true;
    }

    private static bool PoseSet(PoseMsg pose) {
        if (pose.GetPosition().GetX() == 0f &&
            pose.GetPosition().GetY() == 0f &&
            pose.GetPosition().GetZ() == 0f &&            
            pose.GetOrientation().GetX() == 0f &&
            pose.GetOrientation().GetY() == 0f &&
            pose.GetOrientation().GetZ() == 0f &&
            pose.GetOrientation().GetW() == 0f)
            return false;

        return true;
    }


    private static bool PolygonSet(PolygonMsg polygon) {
        if (polygon.GetPoints().Length == 0)
            return false;

        return true; 
    }

    private static bool RefIDSet(List<ushort> ref_id) {
        if(ref_id.Count == 0)
            return false;

        return true;
    }


}
