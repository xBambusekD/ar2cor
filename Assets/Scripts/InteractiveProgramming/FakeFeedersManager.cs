using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeFeedersManager : Singleton<FakeFeedersManager> {

    public GameObject feederObjectPrefab;

    private List<GameObject> leftFeeder = new List<GameObject>();
    private List<GameObject> rightFeeder = new List<GameObject>();
    private GameObject world_anchor;

    private void OnEnable() {
        SystemStarter.Instance.OnSystemStarted += InitFeeders;
    }

    private void OnDisable() {
        SystemStarter.Instance.OnSystemStarted += InitFeeders;
    }
    
    private void InitFeeders() {
        world_anchor = GameObject.FindGameObjectWithTag("world_anchor");

        InitFeeder(FakeFeederObjectsPositions.FeederType.left_feeder, leftFeeder);
        InitFeeder(FakeFeederObjectsPositions.FeederType.right_feeder, rightFeeder);
    }

    private void InitFeeder(FakeFeederObjectsPositions.FeederType feederType, List<GameObject> feederObjectsList) {
        feederObjectsList.Add(InitObjectInFeeder(feederType, FakeFeederObjectsPositions.FeederObjects.Stretcher));
        feederObjectsList.Add(InitObjectInFeeder(feederType, FakeFeederObjectsPositions.FeederObjects.ShortLeg));
        feederObjectsList.Add(InitObjectInFeeder(feederType, FakeFeederObjectsPositions.FeederObjects.LongLeg));
    }

    private GameObject InitObjectInFeeder(FakeFeederObjectsPositions.FeederType feederType, string objectType) {
        GameObject objectInFeeder = Instantiate(feederObjectPrefab, world_anchor.transform);
        objectInFeeder.transform.localPosition = ROSUnityCoordSystemTransformer.ConvertVector(FakeFeederObjectsPositions.GetObjectPositionInFeeder(objectType, feederType));
        objectInFeeder.transform.localRotation = ROSUnityCoordSystemTransformer.ConvertQuaternion(FakeFeederObjectsPositions.GetObjectOrientationInFeeder(objectType, feederType));
        objectInFeeder.GetComponent<DetectedObject>().SetObject(
            ROSUnityCoordSystemTransformer.ConvertVector(FakeFeederObjectsPositions.GetObjectPositionInFeeder(objectType, feederType)),
            ROSUnityCoordSystemTransformer.ConvertQuaternion(FakeFeederObjectsPositions.GetObjectOrientationInFeeder(objectType, feederType)),
            ObjectsManager.Instance.GetObjectTypeDimensions(objectType),
            objectType,
            0);
        objectInFeeder.name = feederType + "_" + objectType;
        objectInFeeder.tag = "feeder_object";
        objectInFeeder.SetActive(false);

        return objectInFeeder;
    }

    public void EnableFakeObjects() {
        ChangeFeederVisibility(leftFeeder, true);
        ChangeFeederVisibility(rightFeeder, true);
    }

    public void DisableFakeObjects() {
        ChangeFeederVisibility(leftFeeder, false);
        ChangeFeederVisibility(rightFeeder, false);
    }

    private void ChangeFeederVisibility(List<GameObject> feederObjectsList, bool visible) {
        foreach(GameObject objectInFeeder in feederObjectsList) {
            objectInFeeder.SetActive(visible);
        }
    }
}
