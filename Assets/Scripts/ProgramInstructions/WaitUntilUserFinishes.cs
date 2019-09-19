using ROSBridgeLib.art_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitUntilUserFinishes : ProgramInstruction {

    public ProgramItemMsg programItem;

    public override void Awake() {
        base.Awake();
    }
	
	// Update is called once per frame
	void Update () {
        if (run) {
            //if user says "Next" then skip to end state
            if (next) {
                SkipToEnd();
                run = false;
                next = false;
            }
            //if user says "Previous" then skip to initial state
            else if (previous) {
                GoBackToStart();
                run = false;
                previous = false;
            }
            //normal run
            else {
                run = false;
            }
        }
    }

    private void SkipToEnd() {
    }

    private void GoBackToStart() {
    }

    void OnDisable() {
        run = false;
    }

    public override void Run() {
        base.Run();
        //speechManager.Say("Running wait until user finishes instruction.");
        //speechManager.Say("The robot will wait until you finish interaction with objects in current task.");
    }
}
