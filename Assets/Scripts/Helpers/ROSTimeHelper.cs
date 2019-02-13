using ROSBridgeLib.std_msgs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnbiasedTimeManager;
using UnityEngine;

public static class ROSTimeHelper {

    private static DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    // Update is called once per frame
    public static TimeMsg GetCurrentTime() {
        //double seconds = (DateTime.UtcNow - epochStart).TotalSeconds;
        double seconds = (UnbiasedTime.Instance.dateTime - epochStart).TotalSeconds;
        var values = seconds.ToString().Split('.');
        int secs = int.Parse(values[0]);
        int nsecs = int.Parse(values[1]);

        //Debug.Log("secs: " + secs + " .. nsecs: " + nsecs);

        return new TimeMsg(secs, nsecs);
    }

    public static TimeMsg GetUnsyncCurrentTime() {
        double seconds = (DateTime.UtcNow - epochStart).TotalSeconds;
        //double seconds = (UnbiasedTime.Instance.dateTime - epochStart).TotalSeconds;
        var values = seconds.ToString().Split('.');
        int secs = int.Parse(values[0]);
        int nsecs = int.Parse(values[1]);

        //Debug.Log("secs: " + secs + " .. nsecs: " + nsecs);

        return new TimeMsg(secs, nsecs);
    }

}
