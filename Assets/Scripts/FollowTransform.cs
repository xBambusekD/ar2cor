using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour {

    public Transform TransformToFollow;
    public float RotateSmoothTime = 0.1f;
    public float MoveSpeed = 10f;

    private float AngularVelocity = 0.0f;
    
    private Vector3 startParentPosition;
    private Quaternion startParentRotationQ;
    private Vector3 startParentScale;

    private Vector3 startChildPosition;
    private Quaternion startChildRotationQ;
    private Vector3 startChildScale;

    private Matrix4x4 parentMatrix;

    void Start() {
        startParentPosition = TransformToFollow.position;
        startParentRotationQ = TransformToFollow.rotation;
        startParentScale = TransformToFollow.lossyScale;

        startChildPosition = transform.position;
        startChildRotationQ = transform.rotation;
        startChildScale = transform.lossyScale;

        // Keeps child position from being modified at the start by the parent's initial transform
        startChildPosition = DivideVectors(Quaternion.Inverse(TransformToFollow.rotation) * (startChildPosition - startParentPosition), startParentScale);
    }

    void Update() {

        parentMatrix = Matrix4x4.TRS(TransformToFollow.position, TransformToFollow.rotation, TransformToFollow.lossyScale);

        transform.position = Vector3.Lerp(transform.position, parentMatrix.MultiplyPoint3x4(startChildPosition), Time.deltaTime * MoveSpeed);

        var target_rot = (TransformToFollow.rotation * Quaternion.Inverse(startParentRotationQ)) * startChildRotationQ;
        var delta = Quaternion.Angle(transform.rotation, target_rot);
        if (delta > 0.0f) {
            var t = Mathf.SmoothDampAngle(delta, 0.0f, ref AngularVelocity, RotateSmoothTime);
            t = 1.0f - t / delta;
            transform.rotation = Quaternion.Slerp(transform.rotation, target_rot, t);
        }

        //transform.rotation = (TransformToFollow.rotation * Quaternion.Inverse(startParentRotationQ)) * startChildRotationQ;
    }

    Vector3 DivideVectors(Vector3 num, Vector3 den) {

        return new Vector3(num.x / den.x, num.y / den.y, num.z / den.z);

    }
}
