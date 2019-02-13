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
            ros.AddPublisher(typeof(HoloLensActivityPublisher));
            ros.AddPublisher(typeof(InterfaceStatePublisher));
            ros.AddPublisher(typeof(CollisionEnvPublisher));
            ros.AddPublisher(typeof(TFPublisher));
            ros.AddPublisher(typeof(LearningRequestActionGoalPublisher));
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
                ros.Publish(HoloLensActivityPublisher.GetMessageTopic(), new BoolMsg(true));

                awake_counter = 5f;
            }
            awake_counter -= Time.deltaTime;

            try {
                Debug.Log(ROSTimeHelper.GetCurrentTime().ToYAMLString());
            } catch(Exception e) {

            }

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
        if (service.Equals(addCollisionPrimitiveService)) {
            JSONNode node = JSONNode.Parse(yaml);
            //Debug.Log(node);
        }
        if(service.Equals(deleteCollisionPrimitiveService)) {
            JSONNode node = JSONNode.Parse(yaml);
            //Debug.Log(node);
        }
        if (service.Equals(saveAllCollisionPrimitiveService)) {
            JSONNode node = JSONNode.Parse(yaml);
            //Debug.Log(node);
        }
        if (service.Equals(clearAllCollisionPrimitiveService)) {
            JSONNode node = JSONNode.Parse(yaml);
            //Debug.Log(node);
        }
        if (service.Equals(reloadAllCollisionPrimitiveService)) {
            JSONNode node = JSONNode.Parse(yaml);
            //Debug.Log(node);
        }
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

        InterfaceStateMsg Imsg = (InterfaceStateMsg) msg;

        VisualizationManager.Instance.SetInterfaceStateMsgFromROS(Imsg);
        //InteractiveEditManager.Instance.SetInterfaceStateMsgFromROS(Imsg);
        //PickFromPolygonIE.Instance.SetInterfaceStateMsgFromROS(Imsg);
        //PlaceToPoseIE.Instance.SetInterfaceStateMsgFromROS(Imsg);

        //InteractiveProgrammingManager.Instance.SetInterfaceStateMsgFromROS(Imsg);
        ProgramHelper.SetInterfaceStateMsgFromROS(Imsg);
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

    public new static ROSBridgeMsg ParseMessage(JSONNode msg) {
        return new BoolMsg(msg);
    }
}

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

    public new static ROSBridgeMsg ParseMessage(JSONNode msg) {
        return new InterfaceStateMsg(msg);
    }
}

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

    public new static ROSBridgeMsg ParseMessage(JSONNode msg) {
        return new CollisionObjectsMsg(msg);
    }
}

public class TFPublisher : ROSBridgePublisher {
    public new static string GetMessageTopic() {
        return "/remote_device_tf";
        //return "/tf";
    }

    public new static string GetMessageType() {
        return "tf2_msgs/TFMessage";
    }

    public static string ToYAMLString(TFMessageMsg msg) {
        return msg.ToYAMLString();
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg) {
        return new TFMessageMsg(msg);
    }
}

public class LearningRequestActionGoalPublisher : ROSBridgePublisher {
    public new static string GetMessageTopic() {
        return "/art/brain/learning_request/goal";
    }

    public new static string GetMessageType() {
        return "art_msgs/LearningRequestActionGoal";
    }

    public static string ToYAMLString(LearningRequstActionGoalMsg msg) {
        return msg.ToYAMLString();
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg) {
        return new LearningRequstActionGoalMsg(msg);
    }
}