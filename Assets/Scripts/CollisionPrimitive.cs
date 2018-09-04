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

    private void Update() {
        
        if (primitive.transform.hasChanged) {
            Debug.Log("TRANSFORM HAS CHANGED");
            primitive.transform.hasChanged = false;
            CollisionPrimitiveMsg newMsg = new CollisionPrimitiveMsg(collisionPrimitiveMsg.GetName(), collisionPrimitiveMsg.GetSetup(),
                new SolidPrimitiveMsg(primitive_type.BOX, new List<float>() { primitive.transform.localScale.x, primitive.transform.localScale.y, primitive.transform.localScale.z }),
                new PoseStampedMsg(collisionPrimitiveMsg.GetPose().GetHeader(),
                new PoseMsg(new PointMsg(primitive.transform.localPosition.x, -primitive.transform.localPosition.y, primitive.transform.localPosition.z),
                new QuaternionMsg(-primitive.transform.localRotation.x, primitive.transform.localRotation.y, -primitive.transform.localRotation.z, primitive.transform.localRotation.w))));
            collisionPrimitiveMsg = newMsg;
            CollisionEnvironmentManager.Instance.UpdateCollisionPrimitiveMsg(collisionPrimitiveMsg);
        }
    }

    public void InitNewPrimitive(CollisionPrimitiveMsg msg, GameObject prefab) {
        collisionPrimitiveMsg = msg;
        primitivePrefab = prefab;
        CreateNewPrimitive();
    }

    public void UpdatePrimitive(CollisionPrimitiveMsg msg) {
        collisionPrimitiveMsg = msg;
        UpdateExistingPrimitive();
    }

    private void UpdateExistingPrimitive() {
        primitive.transform.localPosition = new Vector3(collisionPrimitiveMsg.GetPose().GetPose().GetPosition().GetX(), -collisionPrimitiveMsg.GetPose().GetPose().GetPosition().GetY(),
            collisionPrimitiveMsg.GetPose().GetPose().GetPosition().GetZ());
        primitive.transform.localRotation = new Quaternion(-collisionPrimitiveMsg.GetPose().GetPose().GetOrientation().GetX(), collisionPrimitiveMsg.GetPose().GetPose().GetOrientation().GetY(),
            -collisionPrimitiveMsg.GetPose().GetPose().GetOrientation().GetZ(), collisionPrimitiveMsg.GetPose().GetPose().GetOrientation().GetW());
        primitive.transform.localScale = new Vector3(collisionPrimitiveMsg.GetBBox().GetDimesions()[0], collisionPrimitiveMsg.GetBBox().GetDimesions()[1],
            collisionPrimitiveMsg.GetBBox().GetDimesions()[2]);
        primitive.transform.hasChanged = false;
    }

    private void CreateNewPrimitive() {
        primitive = Instantiate(primitivePrefab, gameObject.transform);
        primitive.transform.localPosition = new Vector3(collisionPrimitiveMsg.GetPose().GetPose().GetPosition().GetX(), -collisionPrimitiveMsg.GetPose().GetPose().GetPosition().GetY(),
            collisionPrimitiveMsg.GetPose().GetPose().GetPosition().GetZ());
        primitive.transform.localRotation = new Quaternion(-collisionPrimitiveMsg.GetPose().GetPose().GetOrientation().GetX(), collisionPrimitiveMsg.GetPose().GetPose().GetOrientation().GetY(),
            -collisionPrimitiveMsg.GetPose().GetPose().GetOrientation().GetZ(), collisionPrimitiveMsg.GetPose().GetPose().GetOrientation().GetW());
        primitive.transform.localScale = new Vector3(collisionPrimitiveMsg.GetBBox().GetDimesions()[0], collisionPrimitiveMsg.GetBBox().GetDimesions()[1],
            collisionPrimitiveMsg.GetBBox().GetDimesions()[2]);
        primitive.transform.hasChanged = false;
    }

    public void OnDestroy() {
        // Destroy the target object, Bounding Box, Bounding Box Rig and App Bar
        //boundingBox.Target.GetComponent<BoundingBoxRig>().Deactivate();
        //Destroy(boundingBox.Target.GetComponent<BoundingBoxRig>());
        //Destroy(boundingBox.Target);
        //Destroy(gameObject);
    }
}
