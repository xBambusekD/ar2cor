using ROSBridgeLib.std_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ROSClickPublish : MonoBehaviour {

    private int click_counter = 0;

    private void OnEnable() {        
        GeneralClick.OnClicked += PublishClick;
    }

    private void OnDisable() {
        GeneralClick.OnClicked -= PublishClick;
    }

    private void PublishClick() {
        click_counter++;

        if(ROSCommunicationManager.Instance.ros._connected)
            ROSCommunicationManager.Instance.ros.Publish(HoloLensClickPublisher.GetMessageTopic(), new HeaderMsg(click_counter, ROSTimeHelper.GetCurrentTime(), "hololens"));
    }
}
