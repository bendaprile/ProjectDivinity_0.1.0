using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDistanceController : MonoBehaviour
{
    public float frontTriggerDistance = 75f;
    public float behindTriggerDistance = 40f;
    public bool onlyEnableAtNight = true;
    [SerializeField] private bool toggleEntireLightObject = false;
    [SerializeField] private bool disableWhenOutsideBuilding = false;
    public GameObject[] lights = null;
    public float checkDistanceFrequency = 0.5f;
    private float distance;
    private GameObject Player;
    private DayNightController dayNight;
    private BuildingControllerMaster bcMaster;
    private float counter = 0f;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        dayNight = FindObjectOfType<DayNightController>();
        bcMaster = FindObjectOfType<BuildingControllerMaster>();
    }

    void Update()
    {
        if (counter < checkDistanceFrequency)
        {
            counter += Time.deltaTime;
            return;
        }
        counter = 0f;

        if (onlyEnableAtNight && dayNight.isLightOutside)
        {
            EnableDisableLights(false);
            return;
        }

        Vector2 LightVec = new Vector2(transform.position.x - Player.transform.position.x, transform.position.z - Player.transform.position.z);
        float CamAng = Camera.main.transform.rotation.eulerAngles.y * Mathf.Deg2Rad;

        float LightAng = Mathf.Atan2(LightVec.x, LightVec.y);
        if (LightAng < 0)
        {
            LightAng += Mathf.PI * 2;
        }

        float AngDiff = Mathf.Abs(LightAng - CamAng);


        //Debug.Log((LightAng, CamAng, AngDiff));
        bool Lightinfront = AngDiff < Mathf.PI / 2 || AngDiff > (3 * Mathf.PI / 2);

        distance = new Vector2(transform.position.x - Player.transform.position.x, transform.position.z - Player.transform.position.z).magnitude;

        // If light is in front of the player use forward trigger distance
        if (Lightinfront)
        {
            if (distance < frontTriggerDistance)
            {
                EnableDisableLights(true);
            }
            if (distance > frontTriggerDistance)
            {
                EnableDisableLights(false);
            }
        }
        // If light is behind the player use backward trigger distance
        else
        {
            if (distance < behindTriggerDistance)
            {
                EnableDisableLights(true);
            }
            if (distance > behindTriggerDistance)
            {
                EnableDisableLights(false);
            }
        }
    }

    private void EnableDisableLights(bool enable)
    {
        if (lights.Length <= 0)
        {
            return;
        }

        //TODO: Implement Multi-Room handling
        MultiFloorBuildingController mfController = GetComponent<MultiFloorBuildingController>();
        MultiRoomBuildingController mrController = GetComponent<MultiRoomBuildingController>();

        if (mrController)
        {
            if (!bcMaster.insideBuilding)
            {
                LoopEnableDisable(lights, enable);
            }
            else
            {
                int length = mrController.currentRoom + 1;
                if (length > lights.Length) { length = lights.Length; }

                GameObject[] result = new GameObject[length];
                Array.Copy(lights, 0, result, 0, length);
                LoopEnableDisable(lights, false);
                LoopEnableDisable(result, enable);
            }
        }
        else
        {
            LoopEnableDisable(lights, enable);
        }
        if (mfController)
        {
            if (!bcMaster.insideBuilding)
            {
                LoopEnableDisable(lights, enable);
            }
            else
            {
                int length = mfController.currentFloor + 1;
                if (length > lights.Length) { length = lights.Length;  }

                GameObject[] result = new GameObject[length];
                Array.Copy(lights, 0, result, 0, length);
                LoopEnableDisable(lights, false);
                LoopEnableDisable(result, enable);
            }
        }
        else
        {
            LoopEnableDisable(lights, enable);
        }
    }

    private void LoopEnableDisable(GameObject[] lights, bool enable)
    {
        if (disableWhenOutsideBuilding && !bcMaster.insideBuilding)
        {
            enable = false;
        }

        if (lights.Length > 0)
        {
            foreach (GameObject light in lights)
            {
                if (!toggleEntireLightObject)
                {
                    foreach (Transform l in light.transform)
                    {
                        foreach (Light l1 in l.GetComponentsInChildren<Light>())
                        {
                            l1.enabled = enable;
                        }
                    }
                }
                else
                {
                    light.SetActive(enable);
                }
            }
        }
    }
}
