using HoloToolkit.Unity;
using ROSBridgeLib.art_msgs;
using ROSBridgeLib.geometry_msgs;
using ROSBridgeLib.shape_msgs;
using ROSBridgeLib.std_msgs;
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
        if (SystemStarter.Instance.calibrated) {
            if (collisionObjectsMsg != null) {
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
    }

    //deletes primitives that were removed on ARTable side
    private void DeleteRemovedPrimitives() {
        for (int i = 0; i < collisionPrimitives.Count; i++) {
            CollisionPrimitive colPrimitive = collisionPrimitives[i].GetComponent<CollisionPrimitive>();
            if (!existingPrimitives.Contains(colPrimitive.collisionPrimitiveMsg.GetName())) {
                colPrimitive.DestroyAppBar();
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
            new_primitive.name = "CollisionObj-" + primitiveMsg.GetName();
            CollisionPrimitive collisionPrimitive = new_primitive.AddComponent<CollisionPrimitive>();
            // TODO pick correct prefab due to collision primitive (cube, sphere..)
            collisionPrimitive.InitNewPrimitive(primitiveMsg, collisionPrefabs[0]);
            collisionPrimitives.Add(new_primitive);
        }
    }

    private string GenerateUniqueName() {
        bool name_is_unique;

        while (true) {
            Guid guid = Guid.NewGuid();
            string unique_name = guid.ToString();
            name_is_unique = true;

            //check if generated name is not already present in known primitives
            foreach(GameObject primitive in collisionPrimitives) {
                if(unique_name.Equals(primitive.GetComponent<CollisionPrimitive>().collisionPrimitiveMsg.GetName())) {
                    name_is_unique = false;
                }
            }
            if(name_is_unique) {
                return unique_name;
            }
        }
    }

    public void CreateNewPrimitive(GameObject prefab, Vector3 position) {
        // TODO replace marker to world_frame
        CollisionPrimitiveMsg newMsg = new CollisionPrimitiveMsg(GenerateUniqueName(), MainMenuManager.Instance.currentSetup.GetSetupID(),
                new SolidPrimitiveMsg(primitive_type.BOX, new List<float>() { prefab.transform.localScale.x, prefab.transform.localScale.y, prefab.transform.localScale.z }),
                new PoseStampedMsg(new HeaderMsg(0, new TimeMsg(0, 0), "marker"),
                new ROSBridgeLib.geometry_msgs.PoseMsg(new PointMsg(position.x, -position.y, position.z),
                new QuaternionMsg(0, 0, 0, 1))));

        UpdatePrimitive(newMsg);
        ROSCommunicationManager.Instance.ros.CallService(ROSCommunicationManager.addCollisionPrimitiveService, "{\"primitive\": " + newMsg.ToYAMLString() + "}");
    }

    public void SetCollisionObjectsMsgFromROS(CollisionObjectsMsg msg) {
        //receive only if messages are not the same
        if(!newCollisionObjectsMsg.ToYAMLString().Equals(msg.ToYAMLString())) {
            collisionObjectsMsg = msg;
            newCollisionObjectsMsg = msg;
        }
    }

    public void UpdateCollisionPrimitiveMsg(CollisionPrimitiveMsg msg) {
        newCollisionObjectsMsg.UpdatePrimitiveMsg(msg);
        //SendCollisionObjectsMsgToROS(newCollisionObjectsMsg);
    }

    //private void SendCollisionObjectsMsgToROS(CollisionObjectsMsg msg) {
    //    //ROSCommunicationManager.Instance.ros.Publish(CollisionEnvPublisher.GetMessageTopic(), msg);
    //}

    public void RemoveDestroyedPrimitive(GameObject primitive) {
        collisionPrimitives.Remove(primitive);
    }
}
