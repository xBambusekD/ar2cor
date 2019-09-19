using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.UX;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceRotateConfirm : MonoBehaviour, IInputClickHandler {

    [HideInInspector]
    public AppBar appBarInstance;

    [HideInInspector]
    public bool object_attached = false;

    private GameObject cursor;
    private GameObject world_anchor;
    private Vector3 snapLocalPosition;
    public Quaternion snapLocalRotation;

    private Material material;
    private Color basic = new Color32(131, 131, 255, 255);
    private Color red = new Color32(255, 20, 20, 255);

    public RobotHelper.RobotArmType Arm { get; set; }


    private void OnEnable() {
        ClickForPlace.OnClicked += PlaceObject;
        GeneralClick.OnClicked += PlayErrorSound;
    }

    private void OnDisable() {
        ClickForPlace.OnClicked -= PlaceObject;
        GeneralClick.OnClicked -= PlayErrorSound;

        try {
            appBarInstance.BoundingBox.Target.GetComponent<BoundingBoxRig>().Deactivate();
            appBarInstance.State = AppBar.AppBarStateEnum.Hidden;
            appBarInstance.OnDoneClicked -= ConfirmRotation;
        }
        catch (NullReferenceException e) {

        }        
    }

    private void Start() {        
        appBarInstance = GetComponent<BoundingBoxRig>().appBarInstance;
        cursor = GameObject.FindGameObjectWithTag("cursor");
        world_anchor = GameObject.FindGameObjectWithTag("world_anchor");
        InvokeRepeating("UpdatePlacePosition", 0f, 0.1f);
        material = GetComponentInChildren<Renderer>().material;

        //snapLocalRotation = transform.rotation;
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

    void LateUpdate() {
        if(object_attached) {
            transform.rotation = snapLocalRotation;
        } else {
            snapLocalRotation = transform.rotation;
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
                UISoundManager.Instance.PlayPlace();

                object_attached = false;
                
                //transform.parent = world_anchor.transform;
                transform.SetParent(world_anchor.transform, true);
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
        //transform.Rotate(0f, 0f, -90f, Space.Self);
        PlaceToPoseIP.Instance.SaveObjectPosition(transform.localPosition, transform.localRotation);
        //PlaceToPoseIP.Instance.PassObjectToPlaceReference(gameObject);

        DeactivateItself();
    }


    //called whenever user clicks on this object
    public void OnInputClicked(InputClickedEventData eventData) {        
        if (!object_attached) {
            UISoundManager.Instance.PlaySnap();

            appBarInstance.OnDoneClicked -= ConfirmRotation;

            appBarInstance.State = AppBar.AppBarStateEnum.Hidden;
            GetComponent<BoundingBoxRig>().Deactivate();


            //save location because the object rotates when transfering parent to cursor (if the cursor is in different rotation than world_anchor)
            snapLocalRotation = transform.rotation;

            object_attached = true;


            //transform.parent = cursor.transform;
            transform.SetParent(cursor.transform, true);
            transform.GetChild(0).GetComponent<Collider>().enabled = false;
            transform.localPosition = new Vector3(0, 0, transform.GetChild(0).localScale.x / 2);            
            transform.localScale = Vector3.one;

            Quaternion difference = Quaternion.Inverse(world_anchor.transform.rotation) * cursor.transform.rotation;

            //transform.localRotation = snapLocalRotation;
            //transform.rotation *= difference;
            //transform.localEulerAngles -= difference.eulerAngles;
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

    private void PlayErrorSound() {
        if (InteractiveProgrammingManager.Instance.CurrentState == InteractiveProgrammingManager.ProgrammingManagerState.place_to_pose_learn ||
            InteractiveProgrammingManager.Instance.CurrentState == InteractiveProgrammingManager.ProgrammingManagerState.place_to_pose_learn_followed) {
            if (object_attached && !RobotHelper.IsObjectWithinRobotArmRadius(Arm, world_anchor.transform.InverseTransformPoint(transform.position))) {
                UISoundManager.Instance.PlayError();
            }
        }
    }
}
