using HoloToolkit.Unity;
using ROSBridgeLib.art_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ROSTextToSpeech : Singleton<ROSTextToSpeech> {

    public SpeechManager speechManagerAzure;

    private GuiNotificationMsg currentGuiNotification;

    public void SetGuiNotificationMsg(GuiNotificationMsg msg) {
        currentGuiNotification = msg;
        speechManagerAzure.Speak(currentGuiNotification.GetMsg());
    }
}
