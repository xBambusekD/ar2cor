using System.Collections;
using System.Collections.Generic;
using UnbiasedTimeManager;
using UnityEngine;

public class FakeTimeManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        UnbiasedTime.Init();
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log(UnbiasedTime.Instance.dateTime);
	}
}
