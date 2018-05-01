using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextToSpeechManager : MonoBehaviour {
    public TextToSpeech textToSpeech;

	// Use this for initialization
	void Awake () {
        textToSpeech = GetComponent<TextToSpeech>();       
    }

    void Start() {
        textToSpeech.StartSpeaking("System has started.");
    }
	
	// Update is called once per frame
	void Update () {
		
	}    

    public void OnFirstCalibration() {
        textToSpeech.StartSpeaking("System needs to calibrate. Please put the marker on the left bottom corner of the table.");
    }

    public void OnCalibrationStart() {
        textToSpeech.StartSpeaking("Calibration started. Please look directly at the marker.");
    }

    public void OnCalibrationEnd() {
        textToSpeech.StartSpeaking("Calibration completed.");
    }
    
    public void Say(String textToSay) {
        //if she talks .. stop her
        if (IsSpeakingOrInQueue())
            StopSpeaking();

        textToSpeech.StartSpeaking(textToSay);
    }

    //waits for Zira to talk out sentence and then speaks
    public void WaitAndSay(String textToSay) {
        StartCoroutine(WaitWhileSpeakingAndThenSay(textToSay));        
    }

    IEnumerator WaitWhileSpeakingAndThenSay(String textToSay) {
        yield return new WaitWhile(() => textToSpeech.SpeechTextInQueue() || textToSpeech.IsSpeaking());
        textToSpeech.StartSpeaking(textToSay);
    }

    public bool IsSpeakingOrInQueue() {
        return textToSpeech.IsSpeaking() || textToSpeech.SpeechTextInQueue();
    }

    public void StopSpeaking() {
        textToSpeech.StopSpeaking();
    }
}
