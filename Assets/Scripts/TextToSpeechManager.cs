using HoloToolkit.Unity;
using ROSBridgeLib.art_msgs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextToSpeechManager : Singleton<TextToSpeechManager> {
    public TextToSpeech textToSpeechDefault;
    public SpeechManager textToSpeechAzure;

    [HideInInspector]
    public bool languageSet = false;

    private GuiNotificationMsg currentGuiNotification = new GuiNotificationMsg("", 0, false, GuiNotificationMsg.MessageType.INFO);

    private bool texttospeech_enabled = true;

    private Texts.Languages language;

    //Based on set language, different speech synthesizer are used.
    public void Speak(string textToSay) {
        if(texttospeech_enabled) {
            switch(language) {
                //use embeded text to speech
                case Texts.Languages.ENGLISH:
                    if (IsSpeakingOrInQueue())
                        StopSpeaking();
                    textToSpeechDefault.StartSpeaking(textToSay);
                    break;
                //use azure's text to speech cloud
                case Texts.Languages.CZECH:
                    textToSpeechAzure.Speak(textToSay);
                    break;
            }
        }
        Debug.Log(textToSay);
    }

    //waits for Zira to talk out sentence and then speaks
    public void WaitAndSay(string textToSay) {
        StartCoroutine(WaitWhileSpeakingAndThenSay(textToSay));        
    }

    IEnumerator WaitWhileSpeakingAndThenSay(string textToSay) {
        yield return new WaitWhile(() => textToSpeechDefault.SpeechTextInQueue() || textToSpeechDefault.IsSpeaking());
        textToSpeechDefault.StartSpeaking(textToSay);
    }

    //Based on set language, function returns true/false if synthesizer is speaking
    public bool IsSpeakingOrInQueue() {
        switch(language) {
            //use embeded text to speech
            case Texts.Languages.ENGLISH:
                return textToSpeechDefault.IsSpeaking() || textToSpeechDefault.SpeechTextInQueue();
            //use azure's text to speech cloud
            case Texts.Languages.CZECH:
                return textToSpeechAzure.isSpeaking;
            default:
                return false;
        }
    }

    public void StopSpeaking() {
        textToSpeechDefault.StopSpeaking();
    }

    //Sets current GUI notification from ARCOR. If TextToSpeech is enabled, notification is synthesized.
    public void SetGuiNotificationMsg(GuiNotificationMsg msg) {
        //prevent duplicate messages
        if(!currentGuiNotification.GetMsg().Equals(msg.GetMsg())) {
            currentGuiNotification = msg;
            Debug.Log("SPEAKING: " + currentGuiNotification.GetMsg());
            if (texttospeech_enabled) {
                Speak(currentGuiNotification.GetMsg());
            }
        }
    }

    //For enabling/disabling TextToSpeech from main menu.
    public void EnableTextToSpeech(bool tts_enabled) {
        texttospeech_enabled = tts_enabled;
    }

    //Loads set language from ROS param server.
    public void LoadLanguage() {
        languageSet = true;
        ROSCommunicationManager.Instance.ros.CallService(ROSCommunicationManager.rosparamGetService, "{\"param_name\": \"art/interface/projected_gui/app/\" }");
    }

    //Called from ROSCommunicationManager when response to LoadLanguage service arrives. Sets text translations.
    public void SetLanguage(string lang) {
        switch(lang) {
            case "en_US":
                language = Texts.Languages.ENGLISH;
                Texts.Language = Texts.Languages.ENGLISH;
                break;
            case "cs_CZ":
                language = Texts.Languages.CZECH;
                Texts.Language = Texts.Languages.CZECH;
                break;
        }
        Debug.Log(language);
    }
}
