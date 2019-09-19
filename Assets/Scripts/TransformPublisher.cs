using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib.geometry_msgs;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.tf2_msgs;

public class TransformPublisher : MonoBehaviour {

    public string frame_id;
    public string child_frame_id;
    public GameObject parentGameObject;

    private TFMessageMsg tfMsg;

	// Update is called once per frame
	void Update () {
        if(SystemStarter.Instance.calibrated) {
            //Vector3 relativePositionToParent = gameObject.transform.InverseTransformPoint(parentGameObject.transform.position);
            Vector3 relativePositionToParent = parentGameObject.transform.InverseTransformPoint(gameObject.transform.position);
            Quaternion relativeRotationToParent = Quaternion.Inverse(parentGameObject.transform.rotation) * gameObject.transform.rotation;
                        

            tfMsg = new TFMessageMsg(new List<TransformStampedMsg>() {new TransformStampedMsg(new HeaderMsg(0, ROSTimeHelper.GetCurrentTime(), frame_id), child_frame_id,
                new TransformMsg(new Vector3Msg(relativePositionToParent.x, -relativePositionToParent.y, relativePositionToParent.z), 
                    new QuaternionMsg(-relativeRotationToParent.x, relativeRotationToParent.y, -relativeRotationToParent.z, relativeRotationToParent.w)))});

            ROSCommunicationManager.Instance.ros.Publish(TFPublisher.GetMessageTopic(), tfMsg, debug_log:false);
        }
	}
}
