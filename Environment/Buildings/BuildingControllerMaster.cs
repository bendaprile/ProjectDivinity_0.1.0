using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingControllerMaster : MonoBehaviour
{
    public bool insideBuilding = false;
    private bool camAffected = false;

    public BuildingController startingBuildingController = null;
    public int startingRoomNumber = 0;

    public bool forceExitCurrentBuilding = false;

    private CameraEnvironmentController ceController;
    private BuildingController currentBuilding;

    private bool lateStart = true;

    void Start()
    {
        ceController = FindObjectOfType<CameraEnvironmentController>();
    }

    private void LateUpdate()
    {
        if (lateStart && startingBuildingController)
        {
            ForceInsideBuilding(startingBuildingController, startingRoomNumber);
            lateStart = false;
        }

        if (forceExitCurrentBuilding)
        {
            ForceExitBuilding();
            forceExitCurrentBuilding = false;
        }
    }

    public void EnterCollider(BuildingController bc, bool affectCamera)
    {
        if (insideBuilding == false)
        {
            insideBuilding = true;
            currentBuilding = bc;
            bc.EnterCollider();
            if (affectCamera && !camAffected)
            {
                EnterCamera(bc);
            }
        }
    }

    public void ExitCollider(BuildingController bc, bool affectCamera)
    {
        if (insideBuilding == true)
        {
            if (affectCamera && camAffected)
            {
                ExitCamera(bc);
            }
            insideBuilding = false;
            currentBuilding = null;
            bc.ExitCollider();
        }
    }

    public void ForceInsideBuilding(BuildingController bc, int roomFloorNumber)
    {
        ForceExitBuilding();

        MultiRoomBuildingController mrBuilding = bc.GetComponent<MultiRoomBuildingController>();
        MultiFloorBuildingController mfBuilding = bc.GetComponent<MultiFloorBuildingController>();
        if (mrBuilding)
        {
            EnterCollider(bc, roomFloorNumber >= mrBuilding.roomNumberToAffectCam);

            while (mrBuilding.currentRoom < roomFloorNumber)
            {
                mrBuilding.IncreaseRoom(mrBuilding.currentRoom + 1);
            }
        }
        else if (mfBuilding)
        {
            EnterCollider(bc, true);

            while (mfBuilding.currentFloor < roomFloorNumber)
            {
                mfBuilding.IncreaseFloor(mfBuilding.currentFloor + 1);
            }
        }
        else
        {
            EnterCollider(bc, true);
        }

        currentBuilding = bc;
    }

    public void ForceExitBuilding()
    {
        if (!insideBuilding || !currentBuilding)
        {
            return;
        }

        if (currentBuilding && insideBuilding)
        {
            if (currentBuilding.GetComponent<MultiRoomBuildingController>())
            {
                MultiRoomBuildingController bc = currentBuilding.GetComponent<MultiRoomBuildingController>();
                while (bc.currentRoom > 0)
                {
                    bc.DecreaseRoom(bc.currentRoom);
                }
            }
            else if (currentBuilding.GetComponent<MultiFloorBuildingController>())
            {
                MultiFloorBuildingController bc = currentBuilding.GetComponent<MultiFloorBuildingController>();
                while (bc.currentFloor > 0)
                {
                    bc.DecreaseFloor(bc.currentFloor);
                }
            }

            currentBuilding.ExitCollider();
            ExitCamera(currentBuilding);
            insideBuilding = false;
            currentBuilding = null;
        }
    }

    public void EnterCamera(BuildingController bc)
    {
        if (!camAffected && insideBuilding)
        {
            camAffected = true;
            StartCoroutine(ceController.EnterBuilding(!bc.roofShadowsOnly, bc.camDistance));
        }
    }

    public void ExitCamera(BuildingController bc)
    {
        if (camAffected && insideBuilding)
        {
            camAffected = false;
            StartCoroutine(ceController.ExitBuilding(!bc.roofShadowsOnly));
        }
    }
}
