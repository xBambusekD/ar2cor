using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ROSUnityCoordSystemTransformer {

    //converts vector from ROS coordinate system to Unity coordinate system and vice versa
    public static Vector3 ConvertVector(Vector3 vec) {
        return new Vector3(vec.x, -vec.y, vec.z);
    }

    //converts quaternion from ROS coordinate system to Unity coordinate system and vice versa
    public static Quaternion ConvertQuaternion(Quaternion qv) {
        return new Quaternion(-qv.x, qv.y, -qv.z, qv.w);
    }

    public static bool AlmostEqual(Vector3 v1, Vector3 v2, float precision) {
        return (Mathf.Abs(v1.x - v2.x) < precision) && (Mathf.Abs(v1.y - v2.y) < precision) && (Mathf.Abs(v1.z - v2.z) < precision);
    }

    public static bool AlmostEqual(Quaternion q1, Quaternion q2, float precision) {
        return Quaternion.Angle(q1, q2) < precision;
    }
}
