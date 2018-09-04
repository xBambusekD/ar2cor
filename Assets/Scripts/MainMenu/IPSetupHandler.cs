using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IPSetupHandler : MonoBehaviour {

    public Text IPInput;
    public Text IPPlaceholder;
    public Text PortInput;
    public Text PortPlaceholder;
    public Text PingMessage;

    private ARTableSetupMsg currentSetup;

    public void OnIPSetupSaveButtonClicked() {
        //use default placeholders if user hasn't set values manually
        string ip = string.IsNullOrEmpty(IPInput.text) ? IPPlaceholder.text : IPInput.text;
        string port = string.IsNullOrEmpty(PortInput.text) ? PortPlaceholder.text : PortInput.text;

        MainMenuManager.Instance.AddIPConfigToSetup(ip, port);
    }

    public void PingIP() {
        //use default placeholders if user hasn't set values manually
        string ip = string.IsNullOrEmpty(IPInput.text) ? IPPlaceholder.text : IPInput.text;
        StartCoroutine(Ping(ip));
    }

    private IEnumerator Ping(string ip) {
        Ping ping = new Ping(ip);

        PingMessage.text = "Pinging...";
        float timeout = 5f;

        while (true) {
            timeout -= Time.deltaTime;
            if(timeout <= 0f) {
                PingMessage.text = "Request timed out..";
                break;
            }
            else if(ping.isDone) {
                PingMessage.text = "Ping OK!";
                break;
            }
            yield return null;
        }
        yield return null;
    }

    public void SetCurrentSetup(ARTableSetupMsg setup) {
        currentSetup = setup;

        IPPlaceholder.text = currentSetup.GetIP();
        PortPlaceholder.text = currentSetup.GetPort();
    }

}
