using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterCollider : MonoBehaviour
{

    [SerializeField] private BuildingController buildingController = null;
    [Tooltip("Floor or Room number that player is entering.")]
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
                buildingController.EnterCollider();
            }

            if (buildingController.GetComponent<MultiFloorBuildingController>())
            {
                if (floorOrRoomNumber == 0)
                {
                    bcMaster.EnterCollider(buildingController, true);
                } 
                else
                {
                    buildingController.GetComponent<MultiFloorBuildingController>().IncreaseFloor(floorOrRoomNumber);
                }
            } 
            else if (buildingController.GetComponent<MultiRoomBuildingController>())
            {
                if (floorOrRoomNumber == 0)
                {
                    bcMaster.EnterCollider(buildingController, floorOrRoomNumber == buildingController.GetComponent<MultiRoomBuildingController>().GetRoomNumberToAffectCam());
                }
                else if (floorOrRoomNumber == buildingController.GetComponent<MultiRoomBuildingController>().GetRoomNumberToAffectCam())
                {
                    bcMaster.EnterCamera(buildingController);
                    buildingController.GetComponent<MultiRoomBuildingController>().IncreaseRoom(floorOrRoomNumber);
                }
                else
                {
                    buildingController.GetComponent<MultiRoomBuildingController>().IncreaseRoom(floorOrRoomNumber);
                }
            }
            else if (buildingController.GetComponent<CustomCameraArea>())
            {
                buildingController.GetComponent<CustomCameraArea>().EnterCollider();
            }
            else
            {
                bcMaster.EnterCollider(buildingController, true);
            }
        } 
    }
}
