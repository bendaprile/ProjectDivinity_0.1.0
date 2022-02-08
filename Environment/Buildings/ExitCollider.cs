using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitCollider : MonoBehaviour
{
    [SerializeField] private BuildingController buildingController = null;
    [Tooltip("Floor or Room number that player is exiting.")]
    [SerializeField] private int floorOrRoomNumber = 0;
    [SerializeField] private bool ignoreMaster = false;

    private BuildingControllerMaster bcMaster;

    private void Start()
    {
        bcMaster = FindObjectOfType<BuildingControllerMaster>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other is CapsuleCollider)
        {
            if (ignoreMaster)
            {
                buildingController.ExitCollider();
            }

            if (buildingController.GetComponent<MultiFloorBuildingController>())
            {
                if (floorOrRoomNumber == 0)
                {
                    bcMaster.ExitCollider(buildingController, true);
                }
                else
                {
                    buildingController.GetComponent<MultiFloorBuildingController>().DecreaseFloor(floorOrRoomNumber);
                }
            }
            else if (buildingController.GetComponent<MultiRoomBuildingController>())
            {
                if (floorOrRoomNumber == 0)
                {
                    bcMaster.ExitCollider(buildingController, floorOrRoomNumber == buildingController.GetComponent<MultiRoomBuildingController>().GetRoomNumberToAffectCam());
                }
                else if (floorOrRoomNumber == buildingController.GetComponent<MultiRoomBuildingController>().GetRoomNumberToAffectCam())
                {
                    bcMaster.ExitCamera(buildingController);
                    buildingController.GetComponent<MultiRoomBuildingController>().DecreaseRoom(floorOrRoomNumber);
                }
                else
                {
                    buildingController.GetComponent<MultiRoomBuildingController>().DecreaseRoom(floorOrRoomNumber);
                }
            }
            else if (buildingController.GetComponent<CustomCameraArea>())
            {
                buildingController.GetComponent<CustomCameraArea>().ExitCollider();
            }
            else
            {
                bcMaster.ExitCollider(buildingController, true);
            }
        }
    }
}
