using ROSBridgeLib.art_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetReady : ProgramInstruction {

    public ProgramItemMsg programItem;
    private bool robot_ready;

    public override void Awake() {
        base.Awake();
        robot_ready = false;
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
                if (!robot_ready || speechManager.IsSpeakingOrInQueue()) {
                    //close robots gripper if was opened
                    pr2_animator.SetBool("open_gripper", false);
                    pr2_animator.SetBool("grab", false);
                    pr2_animator.SetBool("close_gripper", true);
                    if (pr2_animator.GetCurrentAnimatorStateInfo(0).IsName("closed")) {
                        pr2_animator.SetBool("close_gripper", false);
                        robot_ready = true;
                    }
                }
                else {
                    run = false;
                }
            }
        }
    }

    private void SkipToEnd() {
        speechManager.StopSpeaking();
        pr2_animator.SetTrigger("close_instantly");
    }

    private void GoBackToStart() {
        speechManager.StopSpeaking();
    }

    void OnDisable() {
        run = false;
    }

    public override void Run() {
        robot_ready = false;

        base.Run();
        speechManager.Say("Running get ready instruction");
    }
}
