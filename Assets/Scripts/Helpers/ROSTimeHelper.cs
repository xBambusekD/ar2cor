using ROSBridgeLib.std_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ROSTimeHelper {

    private static System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

    // Update is called once per frame
    public static TimeMsg GetCurrentTime() {
        double seconds = (System.DateTime.UtcNow - epochStart).TotalSeconds;
        var values = seconds.ToString().Split('.');
        int secs = int.Parse(values[0]);
        int nsecs = int.Parse(values[1]);

        return new TimeMsg(secs, nsecs);
    }
}
