using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiFloorBuildingController : BuildingController
{
    public int currentFloor = 0;

    protected override void ControlLights(bool turnOnLights)
    {
        base.ControlLights(turnOnLights);
    }

    protected override void ControlRoof(bool turnOnRoof)
    {
        if (!roofShadowsOnly)
        {
            foreach (GameObject roof in roofs)
            {
                ControlFloorLights(roof.transform.Find("Lights"), turnOnRoof);
                roof.SetActive(turnOnRoof);
            }
        }
        else
        {
            foreach (GameObject roof in roofs)
            {
                ControlFloorLights(roof.transform.Find("Lights"), turnOnRoof);

                Transform structure = roof.transform.Find("Structure");
                structure = structure ? structure : roof.transform;
                foreach (MeshRenderer child in structure.GetComponentsInChildren<MeshRenderer>(true))
                {
                    if (child.shadowCastingMode != UnityEngine.Rendering.ShadowCastingMode.Off)
                    {
                        child.shadowCastingMode = turnOnRoof ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                    }
                    else
                    {
                        child.gameObject.SetActive(turnOnRoof);
                    }
                }
            }
        }
    }

    public void IncreaseFloor(int floorNum)
    {
        if (currentFloor >= floorNum)
        {
            return;
        }
        Debug.Log("Called Increase Floor");
        currentFloor++;

        if (!roofShadowsOnly)
        {
            roofs[currentFloor - 1].SetActive(true);
        }
        else
        {
            ControlFloorLights(roofs[currentFloor - 1].transform.Find("Lights"), true);

            Transform structure = roofs[currentFloor-1].transform.Find("Structure");
            structure = structure ? structure : roofs[currentFloor - 1].transform;
            foreach (MeshRenderer child in structure.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (child.shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly)
                {
                    child.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }
                else
                {
                    child.transform.gameObject.SetActive(true);
                }
            }
        }
    }

    public void DecreaseFloor(int floorNum)
    {
        if (currentFloor < floorNum)
        {
            return;
        }
        Debug.Log("Called Decrease Floor");
        currentFloor--;

        if (!roofShadowsOnly)
        {
            roofs[currentFloor].SetActive(false);
        }
        else
        {
            ControlFloorLights(roofs[currentFloor].transform.Find("Lights"), false);

            Transform structure = roofs[currentFloor].transform.Find("Structure");
            structure = structure ? structure : roofs[currentFloor].transform;
            foreach (MeshRenderer child in structure.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (child.shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.On)
                {
                    child.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                }
                else
                {
                    child.transform.gameObject.SetActive(false);
                }
            }
        }
    }

    private void ControlFloorLights(Transform lights, bool turnOnLights)
    {
        if (lights)
        {
            lights.gameObject.SetActive(turnOnLights);
        }
    }
}
