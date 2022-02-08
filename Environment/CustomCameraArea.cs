using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CustomCameraArea : BuildingController
{
    [SerializeField] private float cameraRotationIncrementX = 8f;
    private CameraStateController camController;

    private void Awake()
    {
        camController = FindObjectOfType<CameraStateController>();
    }

    public override void EnterCollider()
    {
        if (insideBuilding == false)
        {
            insideBuilding = true;
            camController.SetCustomCameraSettings(camDistance, cameraRotationIncrementX);
            camController.SetCamState(CameraStateController.CameraState.CustomAreaCam);
        }
    }

    public override void ExitCollider()
    {
        if (insideBuilding == true)
        {
            insideBuilding = false;
            camController.SetCamState(CameraStateController.CameraState.AdventureCombatCam);
        }
    }
}
