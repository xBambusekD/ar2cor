using ROSBridgeLib.art_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ROSActionHelper {

    public delegate void ROSLearningActionResult(LearningRequestActionResultMsg msg);
    public static event ROSLearningActionResult OnLearningActionResult;

    private static LearningRequestActionResultMsg learningResult;

    public static string HoloLensID = "HoloLens";

    public static void SetLearningRequestActionResult(LearningRequestActionResultMsg msg) {
        learningResult = msg;
        Debug.Log(learningResult.ToYAMLString());
        if (OnLearningActionResult != null)
            OnLearningActionResult(learningResult);
    }

    public static string GenerateUniqueGoalID(int goal, double time) {
        return HoloLensID + "-" + goal.ToString() + "-" + time.ToString();
    }
    public static string GenerateUniqueGoalID(string goal, double time) {
        return HoloLensID + "-" + goal + "-" + time.ToString();
    }
}
