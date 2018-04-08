using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.Buttons;

public class RecalibrationButton : MonoBehaviour {

    public SystemStarter systemStarter;

    private CompoundButton button;

    // Use this for initialization
    void Start () {
        button = GetComponent<CompoundButton>();
        button.OnButtonClicked += systemStarter.OnRecalibrationButtonClicked;
	}

}
