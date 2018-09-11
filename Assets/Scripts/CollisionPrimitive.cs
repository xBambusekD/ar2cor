using HoloToolkit.Unity.UX;
using ROSBridgeLib.art_msgs;
using ROSBridgeLib.geometry_msgs;
using ROSBridgeLib.shape_msgs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionPrimitive : MonoBehaviour {

    private GameObject primitivePrefab;

    public CollisionPrimitiveMsg collisionPrimitiveMsg;
    private GameObject primitive;
    private AppBar appBarInstance;
    private Vector3 lastPosition;
    private Vector3 lastScale;
    private Quaternion lastRotation;
    private bool paramsSet;

    private void Awake() {
        paramsSet = false;
    }

    private void Update() {
        if(paramsSet) {
            if (primitive.transform.localPosition != lastPosition || primitive.transform.localScale != lastScale || primitive.transform.localRotation != lastRotation) {
                Debug.Log(collisionPrimitiveMsg.GetName() + " TRANSFORM HAS CHANGED");
                primitive.transform.hasChanged = false;
                CollisionPrimitiveMsg newMsg = new CollisionPrimitiveMsg(collisionPrimitiveMsg.GetName(), collisionPrimitiveMsg.GetSetup(),
                    new SolidPrimitiveMsg(primitive_type.BOX, new List<float>() { primitive.transform.localScale.x, primitive.transform.localScale.y, primitive.transform.localScale.z }),
                    new PoseStampedMsg(collisionPrimitiveMsg.GetPose().GetHeader(),
                    new PoseMsg(new PointMsg(primitive.transform.localPosition.x, -primitive.transform.localPosition.y, primitive.transform.localPosition.z),
                    new QuaternionMsg(primitive.transform.localRotation.x, primitive.transform.localRotation.y, primitive.transform.localRotation.z, primitive.transform.localRotation.w))));
                collisionPrimitiveMsg = newMsg;
                CollisionEnvironmentManager.Instance.UpdateCollisionPrimitiveMsg(collisionPrimitiveMsg);

                //update transform of this object in ROS
                ROSCommunicationManager.Instance.ros.CallService(ROSCommunicationManager.addCollisionPrimitiveService, "{\"primitive\": " + newMsg.ToYAMLString() + "}");
                lastPosition = primitive.transform.localPosition;
                lastScale = primitive.transform.localScale;
                lastRotation = primitive.transform.localRotation;
            }
        }
    }

    //creates new primitive based on given prefab and position in received CollisionPrimitiveMsg
    public void InitNewPrimitive(CollisionPrimitiveMsg msg, GameObject prefab) {
        collisionPrimitiveMsg = msg;
        primitivePrefab = prefab;
        CreateNewPrimitive();
    }

    public void UpdatePrimitive(CollisionPrimitiveMsg msg) {
        //don't update primitive if new msg is same as old one
        if (!collisionPrimitiveMsg.ToYAMLString().Equals(msg.ToYAMLString())) {
            collisionPrimitiveMsg = msg;
            UpdateExistingPrimitive();
        }
    }

    private void UpdateExistingPrimitive() {
        Debug.Log("UPDATING EXISTING PRIMITIVE: " + collisionPrimitiveMsg.GetName());
        SetPrimitiveTransform(collisionPrimitiveMsg);        
    }

    private void CreateNewPrimitive() {
        primitive = Instantiate(primitivePrefab, gameObject.transform);
        SetPrimitiveTransform(collisionPrimitiveMsg);
    }

    private void SetPrimitiveTransform(CollisionPrimitiveMsg msg) {
        primitive.transform.localPosition = new Vector3(msg.GetPose().GetPose().GetPosition().GetX(), -msg.GetPose().GetPose().GetPosition().GetY(),
            msg.GetPose().GetPose().GetPosition().GetZ());
        primitive.transform.localRotation = new Quaternion(msg.GetPose().GetPose().GetOrientation().GetX(), msg.GetPose().GetPose().GetOrientation().GetY(),
            msg.GetPose().GetPose().GetOrientation().GetZ(), msg.GetPose().GetPose().GetOrientation().GetW());
        primitive.transform.localScale = new Vector3(msg.GetBBox().GetDimesions()[0], msg.GetBBox().GetDimesions()[1],
            msg.GetBBox().GetDimesions()[2]);
        lastPosition = primitive.transform.localPosition;
        lastScale = primitive.transform.localScale;
        lastRotation = primitive.transform.localRotation;
        paramsSet = true;
        //primitive.transform.hasChanged = false;
    }

    public void OnDestroy() {
        // Destroy the target object, Bounding Box, Bounding Box Rig and App Bar
        //boundingBox.Target.GetComponent<BoundingBoxRig>().Deactivate();
        //Destroy(boundingBox.Target.GetComponent<BoundingBoxRig>());
        //Destroy(boundingBox.Target);
        //Destroy(gameObject);
        
    }

    public void SetAppBar(AppBar appBar) {
        appBarInstance = appBar;
    }

    public void DestroyAppBar() {
        appBarInstance.DestroyThis();
    }

    public void DestroyThis() {
        CollisionEnvironmentManager.Instance.RemoveDestroyedPrimitive(gameObject);
        ROSCommunicationManager.Instance.ros.CallService(ROSCommunicationManager.deleteCollisionPrimitiveService, "{\"str\": \"" + collisionPrimitiveMsg.GetName() + "\"}");
        Destroy(gameObject);
    }
}
