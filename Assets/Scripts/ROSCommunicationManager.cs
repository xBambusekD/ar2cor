using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using SimpleJSON;
using ROSBridgeLib.art_msgs;
using ROSBridgeLib.tf2_msgs;
using System;
using HoloToolkit.Unity;
using UnbiasedTimeManager;
using ROSBridgeLib.tf2_web_republisher_msgs;
using ROSBridgeLib.actionlib_msgs;

public class ROSCommunicationManager : Singleton<ROSCommunicationManager> {

    [SerializeField] private string serverIP;
    [SerializeField] private int port;

    private bool connectToROS = false;
    public bool connectedToROS = false;

    public ROSBridgeWebSocketConnection ros = new ROSBridgeWebSocketConnection();

    public static string programGetService = "/art/db/program/get";
    public static string objectGetService = "/art/db/object_type/get";
    public static string getAllObjectTypesService = "/art/db/object_type/get_all";
    public static string addCollisionPrimitiveService = "/art/collision_env/artificial/add/primitive";
    public static string deleteCollisionPrimitiveService = "/art/collision_env/artificial/clear/name";
    public static string saveAllCollisionPrimitiveService = "/art/collision_env/artificial/save_all";
    public static string clearAllCollisionPrimitiveService = "/art/collision_env/artificial/clear/all";
    public static string reloadAllCollisionPrimitiveService = "/art/collision_env/artificial/reload";
    public static string robotLookAtLeftFeederService = "/art/robot/look_at/left_feeder";
    public static string robotLookAtRightFeederService = "/art/robot/look_at/right_feeder";
    public static string rosparamGetService = "/rosparam_get";

    private float awake_counter = 0f;

    void Start() {
        
    }

    void Update () {
        if(connectToROS) {
            ros.SetIPConfig(serverIP, port);
            ros.AddSubscriber(typeof(DetectedObjectsSubscriber));
            ros.AddSubscriber(typeof(InterfaceStateSubscriber));
            ros.AddSubscriber(typeof(HololensStateSubscriber));
            ros.AddSubscriber(typeof(CollisionEnvSubscriber));
            ros.AddSubscriber(typeof(LearningRequestActionResultSubscriber));
            ros.AddPublisher(typeof(HoloLensClickPublisher));
            ros.AddPublisher(typeof(HoloLensActivityPublisher));
            ros.AddPublisher(typeof(HoloLensLearningPublisher));
            ros.AddPublisher(typeof(InterfaceStatePublisher));
            ros.AddPublisher(typeof(CollisionEnvPublisher));
            ros.AddPublisher(typeof(TFPublisher));
            ros.AddPublisher(typeof(LearningRequestActionGoalPublisher));

            ros.AddPublisher(typeof(TF2WebRepublisherGoalPublisher));
            ros.AddPublisher(typeof(TF2WebRepublisherCancelPublisher));
            ros.AddSubscriber(typeof(TF2WebRepublisherFeedbackSubscriber));

            ros.AddSubscriber(typeof(GuiNotificationsSubscriber));

            ros.AddSubscriber(typeof(RobotLeftArmGraspedObjectSubscriber));
            ros.AddSubscriber(typeof(RobotRightArmGraspedObjectSubscriber));

            ros.AddServiceResponse(typeof(ROSCommunicationManager));
            ros.Connect();

            connectToROS = false;
        }

        if(ros._connected) {
            connectedToROS = true;

            //activate callbacks
            ros.Render();

            //every 5 secs publish activity msg
            if (awake_counter <= 0f) {
                ros.Publish(HoloLensActivityPublisher.GetMessageTopic(), new BoolMsg(true), debug_log:false);
                ros.Publish(HoloLensLearningPublisher.GetMessageTopic(), new BoolMsg(InteractiveProgrammingManager.Instance.holoLearningEnabled), debug_log:false);

                awake_counter = 5f;
            }
            awake_counter -= Time.deltaTime;

            //if(UnbiasedTime.Instance.TimeSynchronized) {
            //    try {
            //        Debug.Log(ROSTimeHelper.GetCurrentTime().ToYAMLString());
            //    }
            //    catch (Exception e) {

            //    }
            //}

        }

        if(!ros._connected) {
            connectedToROS = false;
        }
    }

    //callback which calls when service is received
    public static void ServiceCallBack(string service, string yaml) {
        Debug.Log("SERVICE CALLBACK " + service);
        if (service.Equals(programGetService)) {
            JSONNode node = JSONNode.Parse(yaml);            
            ProgramMsg programMsg = new ProgramMsg(node["program"]);
            //Debug.Log(programMsg.ToYAMLString());
            ProgramManager.Instance.SetProgramMsgFromROS(programMsg);
            ProgramHelper.SetProgramMsgFromROS(programMsg);
        }
        //1. moznost jak ziskat velikosti objektu .. asi se nebude pouzivat
        if (service.Equals(objectGetService)) {
            JSONNode node = JSONNode.Parse(yaml);
            ObjectTypeMsg objectTypeMsg = new ObjectTypeMsg(node["object_type"]);
            ObjectsManager.Instance.SetObjectTypeMsgFromROS(objectTypeMsg);
            //Debug.Log(objectTypeMsg.ToYAMLString());
        }
        if (service.Equals(getAllObjectTypesService)) {
            JSONNode node = JSONNode.Parse(yaml);
            ObjectsManager.Instance.SetObjectTypesFromROS(node);
        }
        if (service.Equals(rosparamGetService)) {
            JSONNode node = JSONNode.Parse(yaml);
            try {
                JSONNode parsed = JSONNode.Parse(node["message"]);
                Debug.Log(parsed);
                if (parsed["arms"] != null) {
                    RobotHelper.SetRobotRadiusParam(parsed["arms"]);
                }
                if (parsed["locale"] != null) {
                    TextToSpeechManager.Instance.SetLanguage(parsed["locale"]);
                    Debug.Log(parsed["locale"]);
                }
            }
            catch(NullReferenceException e) {
                Debug.Log(e);
            }
        }
        //if (service.Equals(addCollisionPrimitiveService)) {
        //    JSONNode node = JSONNode.Parse(yaml);
        //    //Debug.Log(node);
        //}
        //if(service.Equals(deleteCollisionPrimitiveService)) {
        //    JSONNode node = JSONNode.Parse(yaml);
        //    //Debug.Log(node);
        //}
        //if (service.Equals(saveAllCollisionPrimitiveService)) {
        //    JSONNode node = JSONNode.Parse(yaml);
        //    //Debug.Log(node);
        //}
        //if (service.Equals(clearAllCollisionPrimitiveService)) {
        //    JSONNode node = JSONNode.Parse(yaml);
        //    //Debug.Log(node);
        //}
        //if (service.Equals(reloadAllCollisionPrimitiveService)) {
        //    JSONNode node = JSONNode.Parse(yaml);
        //    //Debug.Log(node);
        //}
        //if (service.Equals(robotLookAtLeftFeederService)) {
        //    JSONNode node = JSONNode.Parse(yaml);
        //    //Debug.Log(node);
        //}
        //if (service.Equals(robotLookAtRightFeederService)) {
        //    JSONNode node = JSONNode.Parse(yaml);
        //    //Debug.Log(node);
        //}
    }

    public void SetIPConfig(string ip, string portStr) {
        serverIP = ip;
        port = Int32.Parse(portStr);
    }

    public void ConnectToROS() {
        connectToROS = true;
    }

    //unsubscribe form topics
#if UNITY_EDITOR
    void OnApplicationQuit() {        
        if (ros != null) {
            ros.Publish(HoloLensActivityPublisher.GetMessageTopic(), new BoolMsg(false));
            ros.Disconnect();
        }
    }
#endif
    //may be used for unsubscribing in hololens.. gotta try how it will work
#if !UNITY_EDITOR
    void OnApplicationFocus(bool focus) {
        if(!focus) {
            if(ros != null) {
                //send message of hololens inactivity
                ros.Publish(HoloLensActivityPublisher.GetMessageTopic(), new BoolMsg(false));
                //not needed .. system automatically disconnects from rosbridge
                //ros.Disconnect();
            }
        }
    }
#endif
}

public class DetectedObjectsSubscriber : ROSBridgeSubscriber {    
    public new static string GetMessageTopic() {
        return "/objects_string";
    }

    public new static string GetMessageType() {
        return "std_msgs/String";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg) {        
        return new StringMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg) {

        JSONNode json_msg = JSON.Parse(((StringMsg)msg).GetData());

        //send data from ROS to objects manager
        ObjectsManager.Instance.setDataFromROS(json_msg);
    }
}


#region hololens
public class HoloLensClickPublisher : ROSBridgePublisher {
    public new static string GetMessageTopic() {
        return "/art/interface/hololens/click";
    }

    public new static string GetMessageType() {
        return "std_msgs/Header";
    }

    public static string ToYAMLString(HeaderMsg msg) {
        return msg.ToYAMLString();
    }

    public static ROSBridgeMsg ParseMessage(JSONNode msg) {
        return new HeaderMsg(msg);
    }
}

public class HoloLensActivityPublisher : ROSBridgePublisher {
    public new static string GetMessageTopic() {
        return "/art/interface/hololens/active";
    }

    public new static string GetMessageType() {
        return "std_msgs/Bool";
    }

    public static string ToYAMLString(BoolMsg msg) {
        return msg.ToYAMLString();
    }

    public static ROSBridgeMsg ParseMessage(JSONNode msg) {
        return new BoolMsg(msg);
    }
}

public class HoloLensLearningPublisher : ROSBridgePublisher {
    public new static string GetMessageTopic() {
        return "/art/interface/hololens/learning";
    }

    public new static string GetMessageType() {
        return "std_msgs/Bool";
    }

    public static string ToYAMLString(BoolMsg msg) {
        return msg.ToYAMLString();
    }

    public static ROSBridgeMsg ParseMessage(JSONNode msg) {
        return new BoolMsg(msg);
    }
}

public class HololensStateSubscriber : ROSBridgeSubscriber {
    public new static string GetMessageTopic() {
        return "/art/interface/hololens/state";
    }

    public new static string GetMessageType() {
        return "art_msgs/HololensState";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg) {
        return new HololensStateMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg) {
        HololensStateMsg Hmsg = (HololensStateMsg)msg;

        VisualizationManager.Instance.SetHololensStateMsgFromROS(Hmsg);
    }
}
#endregion

#region interface_state
public class InterfaceStatePublisher : ROSBridgePublisher {
    public new static string GetMessageTopic() {
        return "/art/interface/state_evt";
    }

    public new static string GetMessageType() {
        return "art_msgs/InterfaceState";
    }

    public static string ToYAMLString(InterfaceStateMsg msg) {
        return msg.ToYAMLString();
    }

    public static ROSBridgeMsg ParseMessage(JSONNode msg) {
        return new InterfaceStateMsg(msg);
    }
}

public class InterfaceStateSubscriber : ROSBridgeSubscriber {
    public new static string GetMessageTopic() {
        return "/art/interface/state";
    }

    public new static string GetMessageType() {
        return "art_msgs/InterfaceState";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg) {
        return new InterfaceStateMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg) {

        InterfaceStateMsg Imsg = (InterfaceStateMsg)msg;

        VisualizationManager.Instance.SetInterfaceStateMsgFromROS(Imsg);
        //InteractiveEditManager.Instance.SetInterfaceStateMsgFromROS(Imsg);
        //PickFromPolygonIE.Instance.SetInterfaceStateMsgFromROS(Imsg);
        //PlaceToPoseIE.Instance.SetInterfaceStateMsgFromROS(Imsg);

        //InteractiveProgrammingManager.Instance.SetInterfaceStateMsgFromROS(Imsg);
        ProgramHelper.SetInterfaceStateMsgFromROS(Imsg);
    }
}
#endregion

#region gui notifications
public class GuiNotificationsSubscriber : ROSBridgeSubscriber {
    public new static string GetMessageTopic() {
        return "/art/interface/projected_gui/notifications";
    }

    public new static string GetMessageType() {
        return "art_msgs/GuiNotification";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg) {
        return new GuiNotificationMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg) {
        GuiNotificationMsg GNmsg = (GuiNotificationMsg)msg;
        TextToSpeechManager.Instance.SetGuiNotificationMsg(GNmsg);
    }
}
#endregion

#region collision_environment
public class CollisionEnvPublisher : ROSBridgePublisher {
    public new static string GetMessageTopic() {
        return "/art/collision_env/artificial";
    }

    public new static string GetMessageType() {
        return "art_msgs/CollisionObjects";
    }

    public static string ToYAMLString(CollisionObjectsMsg msg) {
        return msg.ToYAMLString();
    }

    public static ROSBridgeMsg ParseMessage(JSONNode msg) {
        return new CollisionObjectsMsg(msg);
    }
}

public class CollisionEnvSubscriber : ROSBridgeSubscriber {
    public new static string GetMessageTopic() {
        return "/art/collision_env/artificial";
    }

    public new static string GetMessageType() {
        return "art_msgs/CollisionObjects";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg) {
        return new CollisionObjectsMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg) {
        CollisionObjectsMsg Cmsg = (CollisionObjectsMsg)msg;

        CollisionEnvironmentManager.Instance.SetCollisionObjectsMsgFromROS(Cmsg);
    }
}
#endregion

#region learning_request action
public class LearningRequestActionGoalPublisher : ROSBridgePublisher {
    public new static string GetMessageTopic() {
        return "/art/brain/learning_request/goal";
    }

    public new static string GetMessageType() {
        return "art_msgs/LearningRequestActionGoal";
    }

    public static string ToYAMLString(LearningRequestActionGoalMsg msg) {
        return msg.ToYAMLString();
    }

    public static ROSBridgeMsg ParseMessage(JSONNode msg) {
        return new LearningRequestActionGoalMsg(msg);
    }       
}

public class LearningRequestActionResultSubscriber : ROSBridgeSubscriber {
    public new static string GetMessageTopic() {
        return "/art/brain/learning_request/result";
    }

    public new static string GetMessageType() {
        return "art_msgs/LearningRequestActionResult";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg) {
        return new LearningRequestActionResultMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg) {
        LearningRequestActionResultMsg Lmsg = (LearningRequestActionResultMsg)msg;

        ROSActionHelper.SetLearningRequestActionResult(Lmsg);
    }
}
#endregion

#region tf2_web_republisher action
public class TF2WebRepublisherGoalPublisher : ROSBridgePublisher {
    public new static string GetMessageTopic() {
        return "/tf2_web_republisher/goal";
    }

    public new static string GetMessageType() {
        return "tf2_web_republisher/TFSubscriptionActionGoal";
    }

    public static string ToYAMLString(TFSubscriptionActionGoalMsg msg) {
        return msg.ToYAMLString();
    }

    public static ROSBridgeMsg ParseMessage(JSONNode msg) {
        return new TFSubscriptionActionGoalMsg(msg);
    }
}

public class TF2WebRepublisherCancelPublisher : ROSBridgePublisher {
    public new static string GetMessageTopic() {
        return "/tf2_web_republisher/cancel";
    }

    public new static string GetMessageType() {
        return "actionlib_msgs/GoalID";
    }

    public static string ToYAMLString(GoalIDMsg msg) {
        return msg.ToYAMLString();
    }

    public static ROSBridgeMsg ParseMessage(JSONNode msg) {
        return new GoalIDMsg(msg);
    }
}

public class TF2WebRepublisherFeedbackSubscriber : ROSBridgeSubscriber {
    public new static string GetMessageTopic() {
        return "/tf2_web_republisher/feedback";
    }

    public new static string GetMessageType() {
        return "tf2_web_republisher/TFSubscriptionActionFeedback";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg) {
        return new TFSubscriptionActionFeedbackMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg) {
        TFSubscriptionActionFeedbackMsg TFmsg = (TFSubscriptionActionFeedbackMsg)msg;
        RobotHelper.SetTF2Feedback(TFmsg);
    }
}
#endregion

#region tf
public class TFPublisher : ROSBridgePublisher {
    public new static string GetMessageTopic() {
        //return "/remote_device_tf";
        return "/tf";
    }

    public new static string GetMessageType() {
        return "tf2_msgs/TFMessage";
    }

    public static string ToYAMLString(TFMessageMsg msg) {
        return msg.ToYAMLString();
    }

    public static ROSBridgeMsg ParseMessage(JSONNode msg) {
        return new TFMessageMsg(msg);
    }
}
#endregion

#region robot
public class RobotLeftArmGraspedObjectSubscriber : ROSBridgeSubscriber {
    public new static string GetMessageTopic() {
        return "/art/robot/left_arm/grasped_object";
    }

    public new static string GetMessageType() {
        return "art_msgs/ObjInstance";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg) {
        return new ObjInstanceMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg) {
        ObjInstanceMsg OImsg = (ObjInstanceMsg)msg;
        RobotHelper.SetLeftArmGraspedObject(OImsg);
    }
}

public class RobotRightArmGraspedObjectSubscriber : ROSBridgeSubscriber {
    public new static string GetMessageTopic() {
        return "/art/robot/right_arm/grasped_object";
    }

    public new static string GetMessageType() {
        return "art_msgs/ObjInstance";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg) {
        return new ObjInstanceMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg) {
        ObjInstanceMsg OImsg = (ObjInstanceMsg)msg;
        RobotHelper.SetRightArmGraspedObject(OImsg);
    }
}
#endregion