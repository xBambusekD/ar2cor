using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetupButton : MonoBehaviour {

    public Text buttonText;

    private ARTableSetupMsg _setup;
    
    public void SetButtonText(string text) {
        buttonText.text = text;
    }

    public void SetARTableSetup(ARTableSetupMsg setup) {
        _setup = setup;
    }

    public void OnClickSetupButton() {
        MainMenuManager.Instance.SetActiveSetup(_setup);
        MainMenuManager.Instance.OnSetupListSetupButtonClicked();
    }
}
