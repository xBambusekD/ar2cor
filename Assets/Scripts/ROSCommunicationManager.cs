using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using SimpleJSON;
using ROSBridgeLib.art_msgs;

public class ROSCommunicationManager : MonoBehaviour {

    [SerializeField] private string serverIP;
    [SerializeField] private int port;

    public static ROSBridgeWebSocketConnection ros = null;

    public static string programGetService = "/art/db/program/get";
    public static string objectGetService = "/art/db/object_type/get";

    private float awake_counter = 0f;

    void Awake () {
        ros = new ROSBridgeWebSocketConnection("ws://" + serverIP, port);
        ros.AddSubscriber(typeof(DetectedObjectsSubscriber));
        ros.AddSubscriber(typeof(InterfaceStateSubscriber));
        ros.AddSubscriber(typeof(HololensStateSubscriber));
        ros.AddPublisher(typeof(HoloLensActivityPublisher));
        ros.AddPublisher(typeof(InterfaceStatePublisher));
        ros.AddServiceResponse(typeof(ROSCommunicationManager));
        ros.Connect();
	}

    void Start() {
        
    }

    void Update () {
        //activate callbacks
        ros.Render();
        
        //every 5 secs publish activity msg
        if(awake_counter <= 0f) {
            ros.Publish(HoloLensActivityPublisher.GetMessageTopic(), new BoolMsg(true));
            awake_counter = 5f;
        }
        awake_counter -= Time.deltaTime;        
    }

    //callback which calls when service is received
    public static void ServiceCallBack(string service, string yaml) {
        //Debug.Log("SERVICE CALLBACK");
        if (service == programGetService) {
            JSONNode node = JSONNode.Parse(yaml);
            ProgramMsg programMsg = new ProgramMsg(node["program"]);
            ProgramManager.Instance.SetProgramMsgFromROS(programMsg);
            PlaceToPoseIE.Instance.SetProgramMsgFromROS(programMsg);
        }
        //1. moznost jak ziskat velikosti objektu .. asi se nebude pouzivat
        if (service == objectGetService) {
            JSONNode node = JSONNode.Parse(yaml);
            ObjectTypeMsg objectTypeMsg = new ObjectTypeMsg(node["object_type"]);
            ProgramManager.Instance.SetObjectTypeMsgFromROS(objectTypeMsg);
        }
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
        //Debug.Log("ParseMessage in DetectedObjectsSubscriber");

        //JSONNode json_msg = JSON.Parse(new StringMsg(msg).GetData());

        ////send data from ROS to objects manager
        //ObjectsManager.setDataFromROS(json_msg);
        
        return new StringMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg) {
        //Debug.Log(GetMessageTopic() + " received");

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
        //Debug.Log("ParseMessage in InterfaceStateSubscriber");
        //Debug.Log(msg);
        //InterfaceStateMsg Imsg = new InterfaceStateMsg(msg);
        //Debug.Log(Imsg.GetProgramID());
        return new InterfaceStateMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg) {
        //Debug.Log(GetMessageTopic() + " received");

        InterfaceStateMsg Imsg = (InterfaceStateMsg) msg;

        VisualizationManager.Instance.SetInterfaceStateMsgFromROS(Imsg);
        InteractiveEditManager.Instance.SetInterfaceStateMsgFromROS(Imsg);
        PickFromPolygonIE.Instance.SetInterfaceStateMsgFromROS(Imsg);
        PlaceToPoseIE.Instance.SetInterfaceStateMsgFromROS(Imsg);

        //Debug.Log(Imsg.ToYAMLString());
        //Debug.Log(Imsg.GetSystemState());

        //funkcni volani sluzby
        //ROSCommunicationManager.ros.CallService("/art/db/program/get", "{\"id\": " + Imsg.GetProgramID() + "}");

        //CallService("/art/db/program/get", "\"id: 22\"");
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

        //Debug.Log(Hmsg.ToYAMLString());
        //if(Hmsg.GetHololensState() == hololens_state.STATE_VISUALIZING) {
        //    Debug.Log("VISUALIZING " + Hmsg.GetVisualizationState());
        //}
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