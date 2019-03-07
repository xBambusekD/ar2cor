using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PR2GripperController : MonoBehaviour {

    [SerializeField]
    private Vector3 GripperInitPosition = new Vector3(0.75f, -0.5f, 0.5f);

    [SerializeField]
    private Vector3 GripperInitEulerRotation = new Vector3(-90f, 90f, 0f);

    private GameObject world_anchor;
    private Animator animator;


    // Use this for initialization
    void Start () {
        world_anchor = GameObject.FindGameObjectWithTag("world_anchor");
        animator = GetComponent<Animator>();
    }

    public void PlaceGripperToInit() {
        transform.localPosition = GripperInitPosition;
        transform.localEulerAngles = GripperInitEulerRotation;
    }

    public void ParentToWorldAnchor() {
        transform.parent = world_anchor.transform;
    }

    public void ParentToObject(GameObject parent) {
        transform.parent = parent.transform;
    }

    public void SetArmActive(bool active) {
        transform.GetChild(0).gameObject.SetActive(active);
    }

    public void SetCollidersActive(bool active) {
        SetArmActive(true);
        foreach(Collider col in GetComponentsInChildren<Collider>()) {
            col.enabled = active;
        }
        SetArmActive(false);
    }

    public void OpenGripperInstantly() {
        animator.SetTrigger("open_instantly");
    }

    public void CloseGripperInstantly() {
        animator.SetTrigger("close_instantly");
    }
}
