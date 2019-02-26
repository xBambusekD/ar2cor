using HoloToolkit.Unity;
using ROSBridgeLib.art_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotArmRadius {
    
    public string BaseLink { get; private set; }
    public float MinRange { get; private set; }
    public float MaxRange { get; private set; }

    public Vector3 BaseLinkPosition { get; private set; }

    public RobotArmRadius(string base_link, float min_range, float max_range) {
        BaseLink = base_link;
        MinRange = min_range;
        MaxRange = max_range;
    }

    public void SetBaseLinkPosition(Vector3 position) {
        BaseLinkPosition = ROSUnityCoordSystemTransformer.ConvertVector(position);
    }

    public Vector2 GetBaseLinkPosition2d() {
        return new Vector2(BaseLinkPosition.x, BaseLinkPosition.y);
    }
}
