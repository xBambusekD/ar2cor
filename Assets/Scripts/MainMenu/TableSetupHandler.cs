using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableSetupHandler : MonoBehaviour {

    public Text TableWidthInput;
    public Text TableWidthPlaceholder;
    public Text TableLengthInput;
    public Text TableLengthPlaceholder;
    public Text SetupNameInput;
    public Text SetupNamePlaceholder;


    public void OnTableSetupSaveButtonClicked() {
        //use default placeholders if user hasn't set values manually
        string setup_name = string.IsNullOrEmpty(SetupNameInput.text) ? SetupNamePlaceholder.text : SetupNameInput.text;
        string width = string.IsNullOrEmpty(TableWidthInput.text) ? TableWidthPlaceholder.text : TableWidthInput.text;
        string length = string.IsNullOrEmpty(TableLengthInput.text) ? TableLengthPlaceholder.text : TableLengthInput.text;
        
        MainMenuManager.Instance.AddNewSetup(new ARTableSetupMsg(setup_name, "5", "192.168.104.100", "9090", width, length));
    }
}
