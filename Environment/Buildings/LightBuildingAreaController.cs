using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBuildingAreaController : MonoBehaviour
{
    public bool onlyEnableAtNight = true;
    [SerializeField] private bool toggleEntireLightObject = false;
    [SerializeField] private bool disableWhenOutsideBuilding = false;
    [SerializeField] private bool useBuildingAreaCollider = false;
    public GameObject[] lights = null;
    private DayNightController dayNight;
    private bool insideBuildingArea = false;

    private void Awake()
    {
        LoopEnableDisable(false);
    }

    void Start()
    {
        dayNight = FindObjectOfType<DayNightController>();
    }

    public void ForceTriggerEnter()
    {
        if (!useBuildingAreaCollider && !disableWhenOutsideBuilding)
        {
            return;
        }
        insideBuildingArea = true;
        LoopEnableDisable(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!useBuildingAreaCollider && !disableWhenOutsideBuilding)
        {
            return;
        }
        if (other.tag == "Player" && other is CapsuleCollider)
        {
            insideBuildingArea = true;
            LoopEnableDisable(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!useBuildingAreaCollider && !disableWhenOutsideBuilding)
        {
            return;
        }
        if (other.tag == "Player" && other is CapsuleCollider)
        {
            insideBuildingArea = false;
            LoopEnableDisable(false);
        }
    }

    void Update()
    {
        if (onlyEnableAtNight && dayNight.isLightOutside)
        {
            LoopEnableDisable(false);
            return;
        }
    }

    private void LoopEnableDisable(bool enable)
    {
        if (disableWhenOutsideBuilding && !insideBuildingArea)
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
