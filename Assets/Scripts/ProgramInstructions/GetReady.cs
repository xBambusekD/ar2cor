using ROSBridgeLib.art_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetReady : ProgramInstruction {

    public ProgramItemMsg programItem;
    private bool robot_ready;
    private GameObject world_anchor;
    private Vector3 initPosition;

    public override void Awake() {
        base.Awake();
        robot_ready = false;
        Vector2 tableSize = MainMenuManager.Instance.GetTableSize();
        initPosition = new Vector3(tableSize.x / 2f, -tableSize.y / 2f, 0.3f);

        world_anchor = GameObject.FindGameObjectWithTag("world_anchor");
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

                    //release arm from attached state and set it to be the parent of world anchor
                    if (pr2_arm.transform.parent != world_anchor.transform) {
                        pr2_arm.transform.parent = world_anchor.transform;
                    }

                    //close robots gripper if was opened
                    pr2_animator.SetBool("open_gripper", false);
                    pr2_animator.SetBool("grab", false);
                    pr2_animator.SetBool("close_gripper", true);
                    if (pr2_animator.GetCurrentAnimatorStateInfo(0).IsName("closed")) {
                        pr2_animator.SetBool("close_gripper", false);

                        pr2_arm.transform.localPosition = Vector3.Lerp(pr2_arm.transform.localPosition, initPosition + new Vector3(0, 0, 0.3f), Time.deltaTime * 2f);
                        if (ObjectInPosition(pr2_arm, initPosition + new Vector3(0, 0, 0.3f))) {
                            robot_ready = true;
                        }
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
        pr2_arm.transform.localPosition = initPosition;
        pr2_arm.transform.parent = world_anchor.transform;
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
        //speechManager.Say("Running get ready instruction.");
        speechManager.Say("The robot is getting back to it's default pose.");
    }
}
