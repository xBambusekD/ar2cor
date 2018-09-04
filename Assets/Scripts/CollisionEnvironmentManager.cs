using HoloToolkit.Unity;
using ROSBridgeLib.art_msgs;
using ROSBridgeLib.geometry_msgs;
using ROSBridgeLib.shape_msgs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEnvironmentManager : Singleton<CollisionEnvironmentManager> {
    
    public List<GameObject> collisionPrefabs = new List<GameObject>();

    private List<GameObject> collisionPrimitives = new List<GameObject>();
    private List<string> existingPrimitives = new List<string>();

    private CollisionObjectsMsg collisionObjectsMsg;
    private CollisionObjectsMsg newCollisionObjectsMsg;
    private GameObject worldAnchor;
    
    // Use this for initialization
    void Start () {
        newCollisionObjectsMsg = new CollisionObjectsMsg(new List<CollisionPrimitiveMsg>() { });
        worldAnchor = GameObject.FindGameObjectWithTag("world_anchor");
    }
	
	// Update is called once per frame
	void Update () {
		if(collisionObjectsMsg != null) {
            existingPrimitives.Clear();
            foreach (CollisionPrimitiveMsg primitiveMsg in collisionObjectsMsg.GetPrimitives()) {
                //get all existing primitives names
                existingPrimitives.Add(primitiveMsg.GetName());

                UpdatePrimitive(primitiveMsg);
            }
            DeleteRemovedPrimitives();
            collisionObjectsMsg = null;
        }
    }

    private void DeleteRemovedPrimitives() {
        for (int i = 0; i < collisionPrimitives.Count; i++) {
            CollisionPrimitive colPrimitive = collisionPrimitives[i].GetComponent<CollisionPrimitive>();
            if (!existingPrimitives.Contains(colPrimitive.collisionPrimitiveMsg.GetName())) {
                Destroy(collisionPrimitives[i]);
                collisionPrimitives.Remove(collisionPrimitives[i]);
            }
        }
    }

    //updates existing primitive or creates new one
    private void UpdatePrimitive(CollisionPrimitiveMsg primitiveMsg) {
        bool updated = false;
        foreach (GameObject primitive in collisionPrimitives) {
            CollisionPrimitive colPrimitive = primitive.GetComponent<CollisionPrimitive>();
            //if primitive exists, then just update it
            if (colPrimitive.collisionPrimitiveMsg.GetName().Equals(primitiveMsg.GetName())) {
                colPrimitive.UpdatePrimitive(primitiveMsg);
                updated = true;
            }
        }
        //if primitive wasn't updated, meaning it doesn't exists.. so create new one
        if(!updated) {
            GameObject new_primitive = Instantiate(new GameObject(), worldAnchor.transform);
            CollisionPrimitive collisionPrimitive = new_primitive.AddComponent<CollisionPrimitive>();
            // TODO pick correct prefab due to collision primitive (cube, sphere..)
            collisionPrimitive.InitNewPrimitive(primitiveMsg, collisionPrefabs[0]);
            collisionPrimitives.Add(new_primitive);
        }
    }

    //public void CreateNewPrimitive(GameObject prefab, Vector3 position) {
    //    GameObject new_primitive = Instantiate(new GameObject(), worldAnchor.transform);
    //    new_primitive.transform.localPosition = position;
    //    CollisionPrimitive collisionPrimitive = new_primitive.AddComponent<CollisionPrimitive>();

    //    //CollisionPrimitiveMsg primitiveMsg = new CollisionPrimitiveMsg("DefaultPrimitive", MainMenuManager.Instance.currentSetup.GetSetupID(),
    //    //        new SolidPrimitiveMsg(primitive_type.BOX, new List<float>() { new_primitive.transform.localScale.x, new_primitive.transform.localScale.y, new_primitive.transform.localScale.z }),
    //    //        new PoseStampedMsg(collisionPrimitiveMsg.GetPose().GetHeader(),
    //    //        new PoseMsg(new PointMsg(primitive.transform.localPosition.x, -primitive.transform.localPosition.y, primitive.transform.localPosition.z),
    //    //        new QuaternionMsg(-primitive.transform.localRotation.x, primitive.transform.localRotation.y, -primitive.transform.localRotation.z, primitive.transform.localRotation.w))));

    //    collisionPrimitive.InitNewPrimitive(primitiveMsg, prefab);
    //    collisionPrimitives.Add(new_primitive);
    //}

    public void SetCollisionObjectsMsgFromROS(CollisionObjectsMsg msg) {
        //receive only if messages are not the same
        if(!newCollisionObjectsMsg.Equals(msg)) {
            collisionObjectsMsg = msg;
            newCollisionObjectsMsg = msg;
        }
    }

    public void UpdateCollisionPrimitiveMsg(CollisionPrimitiveMsg msg) {
        newCollisionObjectsMsg.UpdatePrimitiveMsg(msg);
        SendCollisionObjectsMsgToROS(newCollisionObjectsMsg);
    }

    private void SendCollisionObjectsMsgToROS(CollisionObjectsMsg msg) {
        ROSCommunicationManager.Instance.ros.Publish(CollisionEnvPublisher.GetMessageTopic(), msg);
    }
}
