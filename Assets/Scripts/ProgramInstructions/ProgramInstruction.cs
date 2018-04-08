using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProgramInstruction : MonoBehaviour {

    public bool run;

    public float runTime;
    public GameObject speechManagerObj;
    public TextToSpeechManager speechManager;
    public GameObject pr2_arm;
    public Animator pr2_animator;

    public bool next;
    public bool previous;

    //has to be called from child .. base.Awake()
    public virtual void Awake() {
        speechManagerObj = GameObject.FindGameObjectWithTag("speech_manager");
        speechManager = speechManagerObj.GetComponent<TextToSpeechManager>();
        run = false;
        next = false;
        previous = false;

        pr2_arm = GameObject.FindGameObjectWithTag("pr2_arm");
        pr2_animator = pr2_arm.GetComponent<Animator>();
    }

    //has to be called from child .. base.Run()
    public virtual void Run() {
        run = true;
    }

    public bool IsRunning() {
        return run;
    }

    //checks if object is in position on given precision
    public bool ObjectInPosition(GameObject obj, Vector3 placePosition, float precision=0.000005f) {
        //if error is approximately less than 5 mm
        if (Vector3.SqrMagnitude(obj.transform.localPosition - placePosition) < precision) {
            return true;
        }
        return false;
    }

    //places the robot gripper into initial position
    public void PlaceRobotGripperToInit() {
        pr2_arm.transform.localPosition = new Vector3(0.75f, -0.5f, 0.5f);
        pr2_arm.transform.localEulerAngles = new Vector3(90f, -90f, 0f);
    }

    public void OnNextInstruction() {
        next = true;
        pr2_animator.SetBool("open_gripper", false);
        pr2_animator.SetBool("close_gripper", false);
        pr2_animator.SetBool("grab", false);
        Debug.Log("Skipping instruction");
    }

    public void OnPreviousInstruction() {
        previous = true;
        pr2_animator.SetBool("open_gripper", false);
        pr2_animator.SetBool("close_gripper", false);
        pr2_animator.SetBool("grab", false);
        Debug.Log("Going back in instruction");
    }
}
