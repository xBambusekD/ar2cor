/*===============================================================================
Copyright (c) 2017 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/
using UnityEngine;
using Vuforia;

public class CustomTurnOffBehaviour : MonoBehaviour
{
    #region PUBLIC_MEMBERS
    public enum TurnOffRendering{
        PlayModeAndDevice,
        PlayModeOnly,
        Neither
    }

    public TurnOffRendering turnOffRendering =
        TurnOffRendering.PlayModeAndDevice;
    #endregion //PUBLIC_MEMBERS


    #region UNITY_MONOBEHAVIOUR_METHODS

    void Awake ()
    {
        Debug.Log(Application.isEditor);
        if (VuforiaRuntimeUtilities.IsVuforiaEnabled() && 
            turnOffRendering != TurnOffRendering.Neither &&
            ((turnOffRendering == TurnOffRendering.PlayModeAndDevice) ||
            Application.isEditor))
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            MeshFilter meshFilter = GetComponent<MeshFilter>();

            if (meshRenderer)
                Destroy(meshRenderer);
            if (meshFilter)
                Destroy(meshFilter);
        }
    }

    void Start()
    {
    }
    
    #endregion // UNITY_MONOBEHAVIOUR_METHODS
}