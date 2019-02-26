using HoloToolkit.Unity.InputModule;
using ROSBridgeLib.std_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCollisionEnvHandler : MonoBehaviour {

    public Text InfoMessage;
    public Text ShowHideButtonText;
    public Text VoiceCommandsButtonText;
    public Text LearningButtonText;

    private float timer;
    private bool env_hidden;
    private bool commands_disabled;
    private bool learning_disabled;
    public SpeechInputSource speechInputSource;

    // Use this for initialization
    void Start () {
        timer = 5f;
        env_hidden = false;
        commands_disabled = true;
        learning_disabled = false;
    }
	
	// Update is called once per frame
	void Update () {

        //leave text active only for 5 seconds
		if(InfoMessage.text != "") {
            if(timer <= 0f) {
                InfoMessage.text = "";
                timer = 5f;
            }
            timer -= Time.deltaTime;
        }
	}
    
    //calls service which permanently saves current collision_env to drive
    public void SaveCollisionEnvironment() {
        ROSCommunicationManager.Instance.ros.CallService(ROSCommunicationManager.saveAllCollisionPrimitiveService, "{}");
        InfoMessage.text = "Successfully saved!";
    }

    public void ReloadCollisionEnvironment() {
        ROSCommunicationManager.Instance.ros.CallService(ROSCommunicationManager.reloadAllCollisionPrimitiveService, "{}");
        InfoMessage.text = "Successfully reloaded!";
    }

    public void ClearCollisionEnvironment() {
        ROSCommunicationManager.Instance.ros.CallService(ROSCommunicationManager.clearAllCollisionPrimitiveService, "{}");
        InfoMessage.text = "Successfully cleared!";
    }

    public void ShowHideCollisionEnvironment() {
        if(env_hidden) {
            ShowHideButtonText.text = "HIDE ENVIRONMENT";
            CollisionEnvironmentManager.Instance.HideCollisionEnvironment(false);
            env_hidden = false;
        }
        else {
            ShowHideButtonText.text = "SHOW ENVIRONMENT";
            CollisionEnvironmentManager.Instance.HideCollisionEnvironment(true);
            env_hidden = true;
        }
    }

    public void DisableVoiceCommands() {
        //enable voice commands and change text to future disable option
        if(commands_disabled) {
            VoiceCommandsButtonText.text = "DISABLE VOICE COMMANDS";
            speechInputSource.enabled = true;
            commands_disabled = false;
        }
        //disable voice commands and change text to future enable option
        else {
            VoiceCommandsButtonText.text = "ENABLE VOICE COMMANDS";
            speechInputSource.enabled = false;
            commands_disabled = true;
        }
    }

    public void DisableHoloLearning() {
        //enable voice commands and change text to future disable option
        if (learning_disabled) {
            LearningButtonText.text = "DISABLE LEARNING";
            learning_disabled = false;
        }
        //disable voice commands and change text to future enable option
        else {
            LearningButtonText.text = "ENABLE LEARNING";
            learning_disabled = true;
        }
        InteractiveProgrammingManager.Instance.EnableHoloLearning(!learning_disabled);
    }

}
