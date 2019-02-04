using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableDeskInit : MonoBehaviour {

    public Transform childCube;
    //private GameObject world_anchor;

    private void OnEnable() {
        SystemStarter.Instance.OnSystemStarted += InitTable;
    }

    private void OnDisable() {
        SystemStarter.Instance.OnSystemStarted -= InitTable;
    }

    //private void Start() {
    //    world_anchor = GameObject.FindGameObjectWithTag("world_anchor");
    //}

    public void InitTable() {
        //transform.parent = world_anchor.transform;
        transform.localPosition = new Vector3(0f, 0f, 0.001f);

        float x = float.Parse(MainMenuManager.Instance.currentSetup.GetWidth());
        float y = float.Parse(MainMenuManager.Instance.currentSetup.GetLength());
        childCube.localScale = new Vector3(x, y, 0f);
        childCube.localPosition = new Vector3(x / 2, -y / 2, 0f);
        childCube.GetComponent<BoxCollider>().enabled = true;
    }
}
