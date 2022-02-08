using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiRoomBuildingController : BuildingController
{
    public int currentRoom = 0;
    public int roomNumberToAffectCam = 0;

    protected override void ControlLights(bool turnOnLights)
    {
        base.ControlLights(turnOnLights);
    }

    protected override void ControlRoof(bool turnOnRoof)
    {
        ControlRoomLights(roofs[0].transform.Find("Lights"), turnOnRoof);

        if (!roofShadowsOnly)
        {
            roofs[0].SetActive(turnOnRoof);
        }
        else
        { 
            foreach (MeshRenderer child in roofs[0].GetComponentsInChildren<MeshRenderer>())
            {
                child.shadowCastingMode = turnOnRoof ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
        }
    }

    public void IncreaseRoom(int roomNum)
    {
        if (currentRoom >= roomNum)
        {
            return;
        }

        currentRoom++;

        if (!roofShadowsOnly)
        {
            roofs[currentRoom].SetActive(false);
        }
        else
        {
            ControlRoomLights(roofs[currentRoom].transform.Find("Lights"), true);
            foreach (MeshRenderer child in roofs[currentRoom].GetComponentsInChildren<MeshRenderer>())
            {
                child.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
        }
    }

    public void DecreaseRoom(int roomNum)
    {
        if (currentRoom < roomNum)
        {
            return;
        }

        currentRoom--;

        if (!roofShadowsOnly)
        {
            roofs[currentRoom + 1].SetActive(true);
        }
        else
        {
            ControlRoomLights(roofs[currentRoom + 1].transform.Find("Lights"), false);
            foreach (MeshRenderer child in roofs[currentRoom + 1].GetComponentsInChildren<MeshRenderer>())
            {
                child.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
        }
    }

    private void ControlRoomLights(Transform lights, bool turnOnLights)
    {
        if (lights)
        {
            lights.gameObject.SetActive(turnOnLights);
        }
    }

    public int GetRoomNumberToAffectCam()
    {
        return roomNumberToAffectCam;
    }

    public int GetCurrentRoom()
    {
        return currentRoom;
    }
}
