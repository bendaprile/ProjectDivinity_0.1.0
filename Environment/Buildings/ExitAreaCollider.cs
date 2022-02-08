using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitAreaCollider : MonoBehaviour
{
    [SerializeField] private MultiRoomBuildingController mrbc = null;
    [SerializeField] private int roomNumber = 0;

    private BuildingControllerMaster bcMaster;

    private void Start()
    {
        bcMaster = FindObjectOfType<BuildingControllerMaster>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && other is CapsuleCollider && roomNumber == mrbc.GetCurrentRoom())
        {
            bcMaster.ExitCollider(mrbc, true);
        }
    }
}
