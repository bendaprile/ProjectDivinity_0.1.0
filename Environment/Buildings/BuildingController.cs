using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    [SerializeField] public GameObject[] roofs = null;
    [SerializeField] public GameObject lights = null;
    [SerializeField] protected GameObject[] interiorObjectsToDisableWhenOutside = null;
    [SerializeField] protected GameObject[] interiorObjectsToDisableWhenInside = null;
    [SerializeField] protected List<AudioClip> AmbientAudio = null;

    [SerializeField] protected bool lightsAsChildren = false;
    public bool roofShadowsOnly = false;
    protected bool insideBuilding = false;
    public float camDistance = 0f;
    
    private NonDiegeticController AudioControl;

    protected bool lateStart = true;

    protected virtual void Start()
    {
        AudioControl = GameObject.Find("Non Diegetic Audio").GetComponent<NonDiegeticController>();

        foreach (GameObject roof in roofs)
        {
            if (roof && roof.activeSelf == false)
            {
                roof.SetActive(true);
            }
        }
    }

    protected virtual void LateUpdate()
    {
        if (lateStart) {
            lateStart = false;
            if (insideBuilding)
            {
                insideBuilding = false;
                EnterCollider();
            }
            else
            {
                if (lights) { ControlLights(false); }
                if (roofs.Length > 0) { ControlRoof(true); }
                if (interiorObjectsToDisableWhenOutside.Length > 0) { EnableDisableInterior(false, interiorObjectsToDisableWhenOutside); }
                if (interiorObjectsToDisableWhenInside.Length > 0) { EnableDisableInterior(true, interiorObjectsToDisableWhenInside); }
            }
        }
    }

    public virtual void EnterCollider()
    {
        if (insideBuilding == false)
        {
            if (roofs.Length > 0) { ControlRoof(false); }
            if (lights) { ControlLights(true); }
            if (interiorObjectsToDisableWhenOutside.Length > 0) { EnableDisableInterior(true, interiorObjectsToDisableWhenOutside); }
            if (interiorObjectsToDisableWhenInside.Length > 0) { EnableDisableInterior(true, interiorObjectsToDisableWhenInside); }
            insideBuilding = true;
            if (AmbientAudio.Count > 0)
            {
                AudioControl.ChangeAudioSpecific(AmbientAudio);
            }
        }
    }

    public virtual void ExitCollider()
    {
        if (insideBuilding == true)
        {
            if (roofs.Length > 0) { ControlRoof(true); }
            if (lights) { ControlLights(false); }
            if (interiorObjectsToDisableWhenOutside.Length > 0) { EnableDisableInterior(false, interiorObjectsToDisableWhenOutside); }
            if (interiorObjectsToDisableWhenInside.Length > 0) { EnableDisableInterior(true, interiorObjectsToDisableWhenInside); }
            insideBuilding = false;
            if (AmbientAudio.Count > 0)
            {
                AudioControl.ChangeAudioGeneral();
            }
        }
    }

    protected virtual void ControlLights(bool turnOnLights)
    {
        if (!lightsAsChildren)
        {
            lights.SetActive(turnOnLights);
        }
        else
        {
            foreach (Light child in lights.GetComponentsInChildren<Light>())
            {
                child.enabled = turnOnLights;
            }
        }
    }

    protected virtual void ControlRoof(bool turnOnRoof)
    {
        if (!roofShadowsOnly)
        {
            foreach (GameObject roof in roofs)
            {
                roof.SetActive(turnOnRoof);
            }
        }
        else
        {
            foreach (GameObject roof in roofs)
            {
                Transform structure = roof.transform.Find("Structure");
                structure = structure ? structure : roof.transform;
                foreach (MeshRenderer child in structure.GetComponentsInChildren<MeshRenderer>())
                {
                    child.shadowCastingMode = turnOnRoof ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                }

                Transform glass = roof.transform.Find("Glass");
                if (glass)
                {
                    glass.gameObject.SetActive(turnOnRoof);
                }
            }
        }
    }

    private void EnableDisableInterior(bool enable, GameObject[] objects)
    {
        foreach (GameObject interior in objects)
        {
            interior.SetActive(enable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (insideBuilding && other.tag == "Player" && other is CapsuleCollider)
        {
            FindObjectOfType<BuildingControllerMaster>().ForceExitBuilding();
        }
    }
}
