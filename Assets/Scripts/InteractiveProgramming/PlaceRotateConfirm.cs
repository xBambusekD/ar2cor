using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.UX;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceRotateConfirm : MonoBehaviour, IInputClickHandler {

    private AppBar appBarInstance;

    private bool object_attached = true;

    private GameObject cursor;
    private GameObject world_anchor;
    private Vector3 snapLocalPosition;
    private Quaternion snapLocalRotation;

    private Material material;
    private Color basic = new Color32(131, 131, 255, 255);
    private Color red = new Color32(255, 20, 20, 255);

    public RobotHelper.RobotArmType Arm { get; set; }


    private void OnEnable() {
        ClickForPlace.OnClicked += PlaceObject;
    }

    private void OnDisable() {
        ClickForPlace.OnClicked -= PlaceObject;
    }

    private void Start() {        
        appBarInstance = GetComponent<BoundingBoxRig>().appBarInstance;
        cursor = GameObject.FindGameObjectWithTag("cursor");
        world_anchor = GameObject.FindGameObjectWithTag("world_anchor");
        InvokeRepeating("UpdatePlacePosition", 0f, 0.1f);
        material = GetComponentInChildren<Renderer>().material;
    }

    private void Update() {
        if(RobotHelper.IsObjectWithinRobotArmRadius(Arm, world_anchor.transform.InverseTransformPoint(transform.position)) && 
            RobotHelper.IsObjectOnTable(world_anchor.transform.InverseTransformPoint(transform.position))) {
            material.SetColor("_Color", basic);
        }
        else {
            material.SetColor("_Color", red);
        }
    }

    private void UpdatePlacePosition() {
        if ((InteractiveProgrammingManager.Instance.CurrentState == InteractiveProgrammingManager.ProgrammingManagerState.place_to_pose_learn ||
            InteractiveProgrammingManager.Instance.CurrentState == InteractiveProgrammingManager.ProgrammingManagerState.place_to_pose_learn_followed) &&
            PlaceToPoseIP.Instance.learning) {
            ProgramHelper.LoadProgram = false;
            PlaceToPoseIP.Instance.UpdatePlacePose(world_anchor.transform.InverseTransformPoint(transform.position),
                Quaternion.Inverse(world_anchor.transform.localRotation) * transform.rotation);
        }
        else {
            ProgramHelper.LoadProgram = true;
        }
    }

    //called whenever user clicks on the table in order to place virtual object
    private void PlaceObject() {
        if (InteractiveProgrammingManager.Instance.CurrentState == InteractiveProgrammingManager.ProgrammingManagerState.place_to_pose_learn ||
            InteractiveProgrammingManager.Instance.CurrentState == InteractiveProgrammingManager.ProgrammingManagerState.place_to_pose_learn_followed) {
            if (object_attached && RobotHelper.IsObjectWithinRobotArmRadius(Arm, world_anchor.transform.InverseTransformPoint(transform.position))) {
                object_attached = false;

                snapLocalPosition = transform.localPosition;
                snapLocalRotation = transform.localRotation;

                transform.parent = world_anchor.transform;
                transform.GetChild(0).GetComponent<Collider>().enabled = true;

                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.GetChild(0).localScale.x/2);

                EnableRotation();
            }
        }
    }

    private void EnableRotation() {
        appBarInstance.State = AppBar.AppBarStateEnum.Manipulation;
        GetComponent<BoundingBoxRig>().Activate();

        //subscribe to event when user clicks OK in AppBar
        appBarInstance.OnDoneClicked += ConfirmRotation;
    }

    private void ConfirmRotation() {
        appBarInstance.State = AppBar.AppBarStateEnum.Hidden;

        //unsubscribe to event when user clicks OK in AppBar
        appBarInstance.OnDoneClicked -= ConfirmRotation;
        //rotate object around z axis to face it with y axis down
        transform.Rotate(0f, 0f, -90f, Space.Self);
        PlaceToPoseIP.Instance.SaveObjectPosition(transform.localPosition, transform.localRotation);
        PlaceToPoseIP.Instance.PassObjectToPlaceReference(gameObject);

        DeactivateItself();
    }

    //called whenever user clicks on this object
    public void OnInputClicked(InputClickedEventData eventData) {        
        if (!object_attached) {
            appBarInstance.OnDoneClicked -= ConfirmRotation;

            appBarInstance.State = AppBar.AppBarStateEnum.Hidden;
            GetComponent<BoundingBoxRig>().Deactivate();

            object_attached = true;
            
            transform.parent = cursor.transform;
            transform.GetChild(0).GetComponent<Collider>().enabled = false;
            transform.localPosition = snapLocalPosition;
            transform.localScale = Vector3.one;
            transform.localRotation = snapLocalRotation;
        }
    }

    public void DestroyItself() {
        gameObject.GetComponent<BoundingBoxRig>().Deactivate();
        gameObject.GetComponent<BoundingBoxRig>().DestroyBoxInstance();
        try {
            Destroy(gameObject.GetComponent<BoundingBoxRig>().appBarInstance.gameObject);
        } catch(NullReferenceException e) {
            Debug.Log(e);
        }
        Destroy(gameObject.GetComponent<BoundingBoxRig>());
        Destroy(gameObject);
    }

    private void DeactivateItself() {
        GetComponent<ObjectManipulationEnabler>().DisableManipulation();

        //gameObject.GetComponent<BoundingBoxRig>().Deactivate();
        //gameObject.GetComponent<BoundingBoxRig>().DestroyBoxInstance();
        //Destroy(gameObject.GetComponent<BoundingBoxRig>().appBarInstance.gameObject);
        //Destroy(gameObject.GetComponent<BoundingBoxRig>());
        //Destroy(gameObject);
    }
}
