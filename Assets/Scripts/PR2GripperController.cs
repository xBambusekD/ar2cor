using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PR2GripperController : MonoBehaviour {

    [SerializeField]
    private Vector3 GripperInitPosition = new Vector3(0.75f, -0.5f, 0.5f);

    [SerializeField]
    private Vector3 GripperInitEulerRotation = new Vector3(-90f, 90f, 0f);

    [SerializeField]
    private Vector3 GripperManipulationPosition = new Vector3(0f, 0f, 0.3f);

    [SerializeField]
    private Vector3 GripperManipulationEulerRotation = new Vector3(0f, 90f, 0f);

    private GameObject world_anchor;
    private Animator animator;

    private Dictionary<MeshRenderer, Color> mesh_renderers = new Dictionary<MeshRenderer, Color>();
    private Color red = new Color32(255, 20, 20, 255);
    private Color green = new Color32(0, 255, 0, 255);

    private bool followTransform = false;
    private Transform tfToFollow;

    private Vector3 m_refPos;
    private Vector3 m_refRot;

    public List<MeshRenderer> renderers = new List<MeshRenderer>();

    // Use this for initialization
    void Start () {
        world_anchor = GameObject.FindGameObjectWithTag("world_anchor");
        animator = GetComponent<Animator>();

       
        foreach (MeshRenderer rnd in renderers) {
            mesh_renderers.Add(rnd, rnd.material.color);
        }
    }

    //private void Update() {
    //    if (followTransform) {
    //        m_refPos *= Time.smoothDeltaTime;
    //        //transform.position = Vector3.SmoothDamp(transform.position, tfToFollow.position + new Vector3(1f, 0f, 0f), ref m_refPos, 0.05f);
    //        ////transform.position = Vector3.Lerp(transform.position, tfToFollow.position, Time.deltaTime * 100);
    //        ////transform.rotation = Quaternion.Lerp(transform.rotation, tfToFollow.rotation, Time.deltaTime * 100);

    //        //transform.rotation = SmoothDampRotation(transform.rotation, XLookRotation(tfToFollow.position + new Vector3(1f, 0f, 0f) - transform.position), ref m_refRot, 0.05f);

    //        // Subtract the parents rotation from the current rotation
    //        Quaternion actual = transform.parent.localRotation * Quaternion.Inverse(transform.parent.localRotation);

    //        // Rotate to the actual target
    //        //Quaternion.Lerp(transform.rotation, actual, Time.deltaTime * 2.0f);
    //        transform.localRotation = SmoothDampRotation(transform.localRotation, actual, ref m_refRot, 0.05f);
    //    }
    //}

    //public static Quaternion SmoothDampRotation(Quaternion ObjectToRotate, Quaternion WantedRotation, ref Vector3 SavedSmoothSteps, float RoationSpeed) {

    //    var euler = ObjectToRotate.eulerAngles;
    //    var eulerTarget = WantedRotation.eulerAngles;

    //    // Smooth out the reference to have smoother steps and without gittering
    //    SavedSmoothSteps *= Time.smoothDeltaTime;

    //    euler.x = Mathf.SmoothDampAngle(euler.x, eulerTarget.x, ref SavedSmoothSteps.x, RoationSpeed);
    //    euler.y = Mathf.SmoothDampAngle(euler.y, eulerTarget.y, ref SavedSmoothSteps.y, RoationSpeed);
    //    euler.z = Mathf.SmoothDampAngle(euler.z, eulerTarget.z, ref SavedSmoothSteps.z, RoationSpeed);

    //    return Quaternion.Euler(euler);

    //}

    public void PlaceGripperToInit() {
        transform.localPosition = GripperInitPosition;
        transform.localEulerAngles = GripperInitEulerRotation;
    }

    public void PlaceGripperToManipulation() {
        transform.localPosition = GripperManipulationPosition;
        transform.localEulerAngles = GripperManipulationEulerRotation;
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
        foreach (AnimatorControllerParameter p in animator.parameters) {
            if (p.type == AnimatorControllerParameterType.Trigger) {
                animator.ResetTrigger(p.name);
            }
        }
        animator.SetTrigger("open_instantly");
    }

    public void CloseGripperInstantly() {
        foreach (AnimatorControllerParameter p in animator.parameters) {
            if (p.type == AnimatorControllerParameterType.Trigger) {
                animator.ResetTrigger(p.name);
            }
        }
        animator.SetTrigger("close_instantly");
    }

    public void MaterialColorToRed() {
        foreach (var mesh in mesh_renderers) {
            mesh.Key.material.SetColor("_Color", red);
        }
    }

    public void MaterialColorToGreen() {
        foreach (var mesh in mesh_renderers) {
            mesh.Key.material.SetColor("_Color", green);
        }
    }

    public void MaterialColorToDefault() {
        foreach(var mesh in mesh_renderers) {
            mesh.Key.material.SetColor("_Color", mesh.Value);
        }
    }

    //public void FollowTransform(Transform tf) {
    //    followTransform = true;
    //    tfToFollow = tf;
    //    tfToFollow.position += new Vector3(10f, 0f, 0f);
    //    //tfToFollow.rotation = XLookRotation(tfToFollow.position);
    //}

    //private Quaternion XLookRotation(Vector3 direction) {
    //    Quaternion xToForward = Quaternion.Euler(90f, -90f, 0f);
    //    Quaternion forwardToTarget = Quaternion.LookRotation(direction, Vector3.up);

    //    return forwardToTarget * xToForward;
    //}
}
