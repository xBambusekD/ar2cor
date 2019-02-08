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
}
