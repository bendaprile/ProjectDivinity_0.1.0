using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class LightEmissionController : BuildingController
{

    [SerializeField] private Material emissiveMaterial = null;
    [SerializeField] private Material nonEmissiveMaterial = null;

    protected override void Start()
    {
        if (insideBuilding)
        {
            toggleLights(true);
        }
        else
        {
            toggleLights(false);
        }
    }

    public override void EnterCollider()
    {
        if (insideBuilding == false)
        {
            toggleLights(true);
            insideBuilding = true;
        }
    }

    public override void ExitCollider()
    {
        if (insideBuilding == true)
        {
            toggleLights(false);
            insideBuilding = false;
        }
    }

    private void toggleLights(bool turnLightsOn)
    {
        foreach (MeshRenderer light in lights.GetComponentsInChildren<MeshRenderer>())
        {
            Material[] lightMaterials = light.materials;
            lightMaterials[1] = turnLightsOn ? emissiveMaterial : nonEmissiveMaterial; ;
            light.materials = lightMaterials;

            Light pointLight = light.GetComponentInChildren<Light>();
            if (pointLight)
            {
                pointLight.enabled = turnLightsOn;
            }
        }
    }
}
