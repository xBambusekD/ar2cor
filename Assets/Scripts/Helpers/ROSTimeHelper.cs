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
        double seconds = (UnbiasedTime.Instance.dateTime - epochStart).TotalSeconds;
        return FromSec(seconds);
    }

    public static TimeMsg GetUnsyncCurrentTime() {
        double seconds = (DateTime.UtcNow - epochStart).TotalSeconds;
        return FromSec(seconds);
    }

    public static TimeMsg FromSec(double seconds) {
        int secs = 0;
        try {
            secs = (int)seconds;
        }
        catch (IndexOutOfRangeException e) {
            Debug.Log(e);
        }
        int nsecs = 0;
        try {
            nsecs = (int)((seconds - secs) * 1000000000);
        }
        catch (IndexOutOfRangeException e) {
            Debug.Log(e);
        }

        return new TimeMsg(secs, nsecs);
    }

    public static double ToSec(int secs, int nsecs) {
        return (double)secs + (double)nsecs / 1e9;
    }

    public static double ToSec(TimeMsg time) {
        return (double)time.GetSecs() + (double)time.GetNsecs() / 1e9;
    }

    public static double ToNsec(int secs, int nsecs) {
        return secs * (double)1e9 + nsecs;
    }

    public static double ToNsec(TimeMsg time) {
        return time.GetSecs() * (double)1e9 + time.GetNsecs();
    }
}
