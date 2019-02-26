using HoloToolkit.Unity;
using ROSBridgeLib.actionlib_msgs;
using ROSBridgeLib.art_msgs;
using ROSBridgeLib.geometry_msgs;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.tf2_web_republisher_msgs;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RobotRadiusHelper {

    public enum RobotArm : int {
        LEFT_ARM = 1,
        RIGHT_ARM = 2
    }


    private static RobotArmRadius leftArm;
    private static RobotArmRadius rightArm;
    private static Vector2 tableSize;


    public static void LoadRobotRadius() {
        ROSCommunicationManager.Instance.ros.CallService(ROSCommunicationManager.rosparamGetService, "{\"param_name\": \"art/robot/\" }");
    }

    public static void LoadTableSize() {
        tableSize = new Vector2(MainMenuManager.Instance.currentSetup.GetTableWidth(), MainMenuManager.Instance.currentSetup.GetTableLength());
    }

    public static void SetRobotRadiusParam(JSONNode msg) {
        leftArm = new RobotArmRadius(msg["left_arm"]["base_link"], float.Parse(msg["left_arm"]["min_range"]), float.Parse(msg["left_arm"]["max_range"]));
        rightArm = new RobotArmRadius(msg["right_arm"]["base_link"], float.Parse(msg["right_arm"]["min_range"]), float.Parse(msg["right_arm"]["max_range"]));

        GetArmsPosition();
    }

    private static void GetArmsPosition() {
        TimeMsg currentTime = ROSTimeHelper.GetCurrentTime();
        TFSubscriptionActionGoalMsg msg = new TFSubscriptionActionGoalMsg(new HeaderMsg(1, currentTime, ""), 
            new GoalIDMsg(currentTime, ROSActionHelper.GenerateUniqueGoalID("robot_radius", ROSTimeHelper.ToSec(currentTime))),
            new TFSubscriptionGoalMsg(new List<string>() { leftArm.BaseLink, rightArm.BaseLink }, "marker", 0f, 0f, 1f));
        ROSCommunicationManager.Instance.ros.Publish(TF2WebRepublisherGoalPublisher.GetMessageTopic(), msg);
    }

    public static void SetTF2Feedback(TFSubscriptionActionFeedbackMsg msg) {
        CancelTF2Action();

        foreach(TransformStampedMsg tf in msg.GetFeedback().GetTransforms()) {
            if(leftArm.BaseLink.Equals(tf.GetChildFrameId())) {
                leftArm.SetBaseLinkPosition(tf.GetTransform().GetTranslation().GetVector());
            }
            else if (rightArm.BaseLink.Equals(tf.GetChildFrameId())) {
                rightArm.SetBaseLinkPosition(tf.GetTransform().GetTranslation().GetVector());
            }
        }
    }

    private static void CancelTF2Action() {
        GoalIDMsg msg = new GoalIDMsg(ROSTimeHelper.GetCurrentTime(), "");
        ROSCommunicationManager.Instance.ros.Publish(TF2WebRepublisherCancelPublisher.GetMessageTopic(), msg);
    }

    //checks if object is within radius of both arms
    public static bool IsObjectWithinRobotRadius(Vector2 object_position_2D) {
        return IsObjectWithinRobotArmRadius(RobotArm.LEFT_ARM, object_position_2D)
            && IsObjectWithinRobotArmRadius(RobotArm.RIGHT_ARM, object_position_2D);
    }

    public static bool IsObjectWithinRobotRadius(Vector3 object_position) {
        return IsObjectWithinRobotArmRadius(RobotArm.LEFT_ARM, new Vector2(object_position.x, object_position.y))
            && IsObjectWithinRobotArmRadius(RobotArm.RIGHT_ARM, new Vector2(object_position.x, object_position.y));
    }
    
    //checks if object is within radius of specified robot arm
    public static bool IsObjectWithinRobotArmRadius(RobotArm arm, Vector2 object_position) {
        if(arm == RobotArm.LEFT_ARM) {
            return IsObjectWithinArmRadius(leftArm, object_position);
        }
        else if (arm == RobotArm.RIGHT_ARM) {
            return IsObjectWithinArmRadius(rightArm, object_position);
        }
        return false;
    }

    private static bool IsObjectWithinArmRadius(RobotArmRadius arm, Vector2 object_position) {
        float distance = Vector2.Distance(arm.GetBaseLinkPosition2d(), object_position);
        if (distance >= arm.MinRange && distance <= arm.MaxRange) {
            return true;
        }
        return false;
    }

    public static bool IsObjectOnTable(Vector2 object_position) {
        Debug.Log(object_position);
        Debug.Log(tableSize);
        return (object_position.x >= 0 && object_position.x <= tableSize.x &&
            -object_position.y >= 0 && -object_position.y <= tableSize.y);
    }

}
