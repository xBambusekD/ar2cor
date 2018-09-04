using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupsFileMsg {
    private string _last_setup;
    private List<ARTableSetupMsg> _saved_setups = new List<ARTableSetupMsg>();

    private string _new_setup;

    public SetupsFileMsg(JSONNode entry) {
        _last_setup = entry["last_setup"];
        foreach (JSONNode item in entry["saved_setups"].AsArray) {
            _saved_setups.Add(new ARTableSetupMsg(item));
        }
        _new_setup = _last_setup;
    }

    public SetupsFileMsg(string last_setup, List<ARTableSetupMsg> saved_setups) {
        _last_setup = last_setup;
        _saved_setups = saved_setups;
        _new_setup = _last_setup;
    }

    public string GetLastSetup() {
        return _last_setup;
    }

    public List<ARTableSetupMsg> GetSavedSetups() {
        return _saved_setups;
    }

    public void SetActiveSetup(string new_setup) {
        _new_setup = new_setup;
    }

    public void StoreNewSetup(ARTableSetupMsg new_setup) {
        _saved_setups.Add(new_setup);
    }

    public ARTableSetupMsg GetActiveSetupMsg() {
        foreach(ARTableSetupMsg setup in _saved_setups) {
            if(setup.GetSetupName().Equals(_new_setup)) {
                return setup;
            }
        }
        return null;
    }

    public void StoreIPConfigToSetup(string setupName, string ip, string port) {
        foreach (ARTableSetupMsg setup in _saved_setups) {
            if (setup.GetSetupName().Equals(setupName)) {
                setup.ActualizeIPConfig(ip, port);
            }
        }
    }

    public override string ToString() {
        string itemsString = "[";
        for (int i = 0; i < _saved_setups.Count; i++) {
            itemsString = itemsString + _saved_setups[i].ToString();
            if (_saved_setups.Count - i > 1) itemsString += ",";
        }
        itemsString += "]";

        return "{\"last_setup\":\"" + _last_setup +
            "\",\"saved_setups\":" + itemsString + "}";
    }

    public string PrintCurrentString() {
        string itemsString = "[";
        for (int i = 0; i < _saved_setups.Count; i++) {
            itemsString = itemsString + _saved_setups[i].ToString();
            if (_saved_setups.Count - i > 1) itemsString += ",";
        }
        itemsString += "]";

        return "{\"last_setup\":\"" + _new_setup +
            "\",\"saved_setups\":" + itemsString + "}";
    }
}
