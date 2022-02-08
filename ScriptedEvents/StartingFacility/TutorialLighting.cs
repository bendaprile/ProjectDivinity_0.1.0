using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLighting : MonoBehaviour
{
    private enum LightingStage
    {
        Prometheus,
        PrometheusToPallas,
        Pallas,
        PallasToEpimetheus,
        Epimetheus,
        EpimetheusToPrometheus,
        End
    }

    [SerializeField] QuestTemplate tutorialQuest = null;

    [Header("Lighting Colliders")]
    [SerializeField] private Collider leavePrometheusRoom = null;
    [SerializeField] private Collider initalPallasTrigger = null;
    [SerializeField] private Collider leavePallasRoom = null;
    [SerializeField] private Collider initalEpimetheusTrigger = null;
    [SerializeField] private Collider leaveEpimetheusRoom = null;
    [SerializeField] private Collider prometheusRoom2 = null;

    [Header("Lighting Segments")]
    [SerializeField] private Transform PrometheusToPallas = null;
    [SerializeField] private Transform PallasToEpimetheus = null;
    [SerializeField] private Transform EpimetheusToPrometheus = null;
    [SerializeField] private Transform TheRest = null;
    [SerializeField] private Transform EntryLights = null;

    [SerializeField] private Material emissiveMaterial = null;
    [SerializeField] private Material nonEmissiveMaterial = null;

    private LightingStage lightingStage = LightingStage.Prometheus;


    /*    [SerializeField] private SlidingDoorController ExteriorDoor = null;
        [SerializeField] private SlidingDoorController PallasNearDoor = null;
        [SerializeField] private SlidingDoorController PallasFarDoor = null;
        [SerializeField] private SlidingDoorController EpimetheusDoor = null;*/

    private void Start()
    {
        TriggerLightsSegmented(GetFirstChildren(PrometheusToPallas), false);
        TriggerLightsSegmented(GetFirstChildren(PallasToEpimetheus), false);
        TriggerLightsSegmented(GetFirstChildren(EpimetheusToPrometheus), false);
        TriggerLights(TheRest.GetComponentsInChildren<MeshRenderer>(), false);
        TriggerLights(EntryLights.GetComponentsInChildren<MeshRenderer>(), false);
    }

    void Update()
    {
        ControlHallLights();
    }
   

    private void ControlHallLights()
    {
        if (lightingStage == LightingStage.End)
        {
            return;
        }

        if (!leavePrometheusRoom.enabled && lightingStage == LightingStage.Prometheus)
        {
            // Turn on hall lights from Prometheus to Pallas' Room
            StartCoroutine(TriggerLightsCoroutine(GetFirstChildren(PrometheusToPallas), true));
            lightingStage = LightingStage.PrometheusToPallas;
        }

        if (!initalPallasTrigger.enabled && lightingStage == LightingStage.PrometheusToPallas)
        {
            // Turn off hall lights from PrometheusToPallas and set lighting stage to PrometheusToPallas
            TriggerLightsSegmented(GetFirstChildren(PrometheusToPallas), false);
            lightingStage = LightingStage.Pallas;
            PrometheusToPallas.Find("LightSegment").SetParent(EpimetheusToPrometheus);
        }

        if (!leavePallasRoom.enabled && lightingStage == LightingStage.Pallas)
        {
            // Turn on hall lights from Pallas to Epimetheus
            StartCoroutine(TriggerLightsCoroutine(GetFirstChildren(PallasToEpimetheus), true));
            lightingStage = LightingStage.PallasToEpimetheus;
        }

        if (!initalEpimetheusTrigger.enabled && lightingStage == LightingStage.PallasToEpimetheus)
        {
            // Turn off lights from PrometheusToPallas and set lighting stage to PallasToEpimetheus
            TriggerLightsSegmented(GetFirstChildren(PallasToEpimetheus), false);
            lightingStage = LightingStage.Epimetheus;
            leaveEpimetheusRoom.enabled = true;
            Transform tempLightSegement = PallasToEpimetheus.Find("LightSegment (8)");
            tempLightSegement.SetParent(EpimetheusToPrometheus);
            tempLightSegement.SetAsFirstSibling();
        }

        if (!leaveEpimetheusRoom.enabled && lightingStage == LightingStage.Epimetheus)
        {
            // Turn on lights from EpimetheusToPallas and set lighting stage to EpimetheusToPrometheus
            StartCoroutine(TriggerLightsCoroutine(GetFirstChildren(EpimetheusToPrometheus), true));
            lightingStage = LightingStage.EpimetheusToPrometheus;
            prometheusRoom2.enabled = true;
        }

        if (!prometheusRoom2.enabled && lightingStage == LightingStage.EpimetheusToPrometheus)
        {
            TriggerLightsSegmented(GetFirstChildren(PrometheusToPallas), true);
            TriggerLightsSegmented(GetFirstChildren(PallasToEpimetheus), true);
            TriggerLightsSegmented(GetFirstChildren(EpimetheusToPrometheus), true);
            TriggerLights(TheRest.GetComponentsInChildren<MeshRenderer>(), true);
            TriggerLights(EntryLights.GetComponentsInChildren<MeshRenderer>(), true);
            lightingStage = LightingStage.End;
            gameObject.SetActive(false);
        }
    }

    private void TriggerLights(MeshRenderer[] lights, bool turnLightsOn)
    {
        foreach (MeshRenderer light in lights)
        {
            Material[] lightMaterials = light.materials;
            lightMaterials[1] = turnLightsOn ? emissiveMaterial : nonEmissiveMaterial; ;
            light.materials = lightMaterials;

            Light areaLight = light.GetComponentInChildren<Light>();
            if (areaLight)
            {
                areaLight.enabled = turnLightsOn;
            }
        }
    }

    private void TriggerLightsSegmented(Transform[] lightSegments, bool turnLightsOn)
    {
        foreach (Transform lightSegment in lightSegments)
        {
            foreach (MeshRenderer light in lightSegment.GetComponentsInChildren<MeshRenderer>())
            {
                Material[] lightMaterials = light.materials;
                lightMaterials[1] = turnLightsOn ? emissiveMaterial : nonEmissiveMaterial; ;
                light.materials = lightMaterials;

                Light areaLight = light.GetComponentInChildren<Light>();
                if (areaLight)
                {
                    areaLight.enabled = turnLightsOn;
                }
            }
        }
    }

    private IEnumerator TriggerLightsCoroutine(Transform[] lightSegments, bool turnLightsOn)
    {
        foreach (Transform lightSegment in lightSegments)
        {
            foreach (MeshRenderer light in lightSegment.GetComponentsInChildren<MeshRenderer>())
            {
                Material[] lightMaterials = light.materials;
                lightMaterials[1] = turnLightsOn ? emissiveMaterial : nonEmissiveMaterial; ;
                light.materials = lightMaterials;

                Light areaLight = light.GetComponentInChildren<Light>();
                if (areaLight)
                {
                    areaLight.enabled = turnLightsOn;
                }
            }
            yield return new WaitForSeconds(0.4f);
        }
    }

    private Transform[] GetFirstChildren(Transform parent)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>();
        Transform[] firstChildren = new Transform[parent.childCount];
        int index = 0;
        foreach (Transform child in children)
        {
            if (child.parent == parent)
            {
                firstChildren[index] = child;
                index++;
            }
        }
        return firstChildren;
    }
}
