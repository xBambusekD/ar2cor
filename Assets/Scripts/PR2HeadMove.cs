using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PR2HeadMove : MonoBehaviour {

    public GameObject Cursor;

    private GameObject world_anchor;
    private float half_table_width = 0f;

    private bool robot_looking_left = false;
    private bool robot_looking_right = false;

	// Use this for initialization
	void Start () {
        world_anchor = GameObject.FindGameObjectWithTag("world_anchor");
    }
	
	// Update is called once per frame
	void Update () {
        if(SystemStarter.Instance.calibrated) {
            if (half_table_width == 0f) {
                half_table_width = float.Parse(MainMenuManager.Instance.currentSetup.GetWidth()) / 2;
            }


            if (InteractiveProgrammingManager.Instance.CurrentState == InteractiveProgrammingManager.ProgrammingManagerState.pick_from_feeder_learn) {
                // look on the left feeder
                if (world_anchor.transform.InverseTransformPoint(Cursor.transform.position).x < half_table_width) {
                    if (!robot_looking_left) {
                        ROSCommunicationManager.Instance.ros.CallService(ROSCommunicationManager.robotLookAtLeftFeederService, "{}");
                        robot_looking_left = true;
                        robot_looking_right = false;
                    }
                }
                // look on the right feeder
                else {
                    if (!robot_looking_right) {
                        ROSCommunicationManager.Instance.ros.CallService(ROSCommunicationManager.robotLookAtRightFeederService, "{}");
                        robot_looking_right = true;
                        robot_looking_left = false;
                    }
                }
            }
        }
	}
}
