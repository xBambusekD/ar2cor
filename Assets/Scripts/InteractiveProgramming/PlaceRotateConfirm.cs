using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.UX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceRotateConfirm : MonoBehaviour, IInputClickHandler {

    private AppBar appBarInstance;

    private bool object_attached = true;

    private GameObject cursor;
    private GameObject world_anchor;
    private Vector3 snapLocalPosition;

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
    }

    //called whenever user clicks on the table in order to place virtual object
    private void PlaceObject() {
        if (InteractiveProgrammingManager.Instance.CurrentState == InteractiveProgrammingManager.ProgrammingManagerState.place_to_pose_learn) {
            if (object_attached) {
                object_attached = false;

                snapLocalPosition = transform.localPosition;

                transform.parent = world_anchor.transform;
                transform.GetChild(0).GetComponent<Collider>().enabled = true;

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

        PlaceToPoseIP.Instance.SaveObjectPosition(transform.position, transform.rotation);
        DestroyItself();
    }

    //called whenever user clicks on this object
    public void OnInputClicked(InputClickedEventData eventData) {        
        if (!object_attached) {
            appBarInstance.State = AppBar.AppBarStateEnum.Hidden;
            GetComponent<BoundingBoxRig>().Deactivate();

            object_attached = true;
            
            transform.parent = cursor.transform;
            transform.GetChild(0).GetComponent<Collider>().enabled = false;
            transform.localPosition = snapLocalPosition;
        }

    }

    private void DestroyItself() {
        gameObject.GetComponent<BoundingBoxRig>().Deactivate();
        gameObject.GetComponent<BoundingBoxRig>().DestroyBoxInstance();
        Destroy(gameObject.GetComponent<BoundingBoxRig>().appBarInstance.gameObject);
        Destroy(gameObject.GetComponent<BoundingBoxRig>());
        Destroy(gameObject);
    }
}
