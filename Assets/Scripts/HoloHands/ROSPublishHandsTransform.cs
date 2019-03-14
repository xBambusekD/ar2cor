using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class ROSPublishHandsTransform : MonoBehaviour {

    private Dictionary<uint, GameObject> hands = new Dictionary<uint, GameObject>();
    private GameObject hand; 

    private void OnEnable() {
        InteractionManager.InteractionSourceDetected += SourceDetected;
        InteractionManager.InteractionSourceLost += SourceLost;
        InteractionManager.InteractionSourceUpdated += SourceUpdated;
    }

    private void OnDisable() {
        InteractionManager.InteractionSourceDetected -= SourceDetected;
        InteractionManager.InteractionSourceLost -= SourceLost;
        InteractionManager.InteractionSourceUpdated -= SourceUpdated;
    }

    private void SourceDetected(InteractionSourceDetectedEventArgs args) {
        if(args.state.source.kind != InteractionSourceKind.Hand) {
            return;
        }

        if(hand == null) {
            hand = new GameObject("hololens_hand");
            TransformPublisher tf = hand.AddComponent<TransformPublisher>();
            tf.frame_id = "marker";
            tf.child_frame_id = "hololens_hand";
            tf.parentGameObject = GameObject.FindGameObjectWithTag("world_anchor");
        }

        //GameObject hand = new GameObject("hololens_hand_" + args.state.source.id);
        //TransformPublisher tf = hand.AddComponent<TransformPublisher>();
        //tf.frame_id = "marker";
        //tf.child_frame_id = "hololens_hand_" + args.state.source.id;
        //tf.parentGameObject = GameObject.FindGameObjectWithTag("world_anchor");

        Vector3 position;
        if(args.state.sourcePose.TryGetPosition(out position)) {
            hand.transform.position = position;
        }

        //hands.Add(args.state.source.id, hand);
    }

    private void SourceUpdated(InteractionSourceUpdatedEventArgs args) {
        uint id = args.state.source.id;
        Vector3 position;
        Quaternion rotation;

        //if (args.state.source.kind == InteractionSourceKind.Hand) {
        //    if (hands.ContainsKey(id)) {
        //        if (args.state.sourcePose.TryGetPosition(out position)) {
        //            hands[id].transform.position = position;
        //        }

        //        if (args.state.sourcePose.TryGetRotation(out rotation)) {
        //            hands[id].transform.rotation = rotation;
        //        }
        //    }
        //}

        if (args.state.source.kind == InteractionSourceKind.Hand) {
            if (args.state.sourcePose.TryGetPosition(out position)) {
                hand.transform.position = position;
            }

            if (args.state.sourcePose.TryGetRotation(out rotation)) {
                hand.transform.rotation = rotation;
            }
        }
    }

    private void SourceLost(InteractionSourceLostEventArgs args) {
        uint id = args.state.source.id;

        if (args.state.source.kind != InteractionSourceKind.Hand) {
            return;
        }
        
        //if (hands.ContainsKey(id)) {
        //    var obj = hands[id];
        //    hands.Remove(id);
        //    Destroy(obj);
        //}
    }
}
