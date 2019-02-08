using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARTableSetupMsg {

    private string _setup_name;
    private string _setup_id;
    private string _ip;
    private string _port;
    private string _width;
    private string _length;

    public ARTableSetupMsg(JSONNode entry) {
        _setup_name = entry["setup_name"];
        _setup_id = entry["setup_id"];
        _ip = entry["connection"]["ip"];
        _port = entry["connection"]["port"];
        _width = entry["table_dims"]["width"];
        _length = entry["table_dims"]["length"];
    }

    public ARTableSetupMsg(string setup_name, string setup_id, string ip, string port, string width, string length) {
        _setup_name = setup_name;
        _setup_id = setup_id;
        _ip = ip;
        _port = port;
        _width = width;
        _length = length;
    }

    public string GetSetupID() {
        return _setup_id;
    }

    public string GetSetupName() {
        return _setup_name;
    }

    public string GetIP() {
        return _ip;
    }

    public string GetPort() {
        return _port;
    }

    public string GetWidth() {
        return _width;
    }

    public string GetLength() {
        return _length;
    }

    public float GetTableWidth() {
        return float.Parse(_width);
    }
    
    public float GetTableLength() {
        return float.Parse(_length);
    }

    public void ActualizeIPConfig(string ip, string port) {
        _ip = ip;
        _port = port;
    }

    public override string ToString() {
        return "{\"setup_name\":\"" + _setup_name + "\"" +
            ",\"setup_id\":\"" + _setup_id + "\"" +
            ",\"connection\":{\"ip\":\"" + _ip + "\",\"port\":\"" + _port + "\"}" +
            ",\"table_dims\":{\"width\":\"" + _width + "\",\"length\":\"" + _length + "\"}}";
    }
}
