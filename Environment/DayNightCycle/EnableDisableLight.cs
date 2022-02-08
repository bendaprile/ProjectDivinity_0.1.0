using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDisableLight : MonoBehaviour
{
    public float frontTriggerDistance = 75f;
    public float behindTriggerDistance = 35f;
    public bool onlyEnableAtNight = true;
    public float checkDistanceFrequency = 0.5f;
    private Light lightComp;
    private GameObject Player;
    private DayNightController dayNight;
    private float counter = 0f;
    private float distance;

    void Start()
    {
        lightComp = gameObject.GetComponent<Light>();
        Player = GameObject.FindGameObjectWithTag("Player");
        dayNight = FindObjectOfType<DayNightController>();
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
            lightComp.enabled = false;
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

        //Debug.Log((LightAng * Mathf.Rad2Deg, CamAng * Mathf.Rad2Deg, AngDiff * Mathf.Rad2Deg));

        bool Lightinfront = AngDiff < Mathf.PI / 2 || AngDiff > (3 * Mathf.PI / 2);

        distance = new Vector2(transform.position.x - Player.transform.position.x, transform.position.z - Player.transform.position.z).magnitude;

        // If light is in front of the Camera.main use forward trigger distance
        if (Lightinfront)
        {
            if (distance < frontTriggerDistance)
            {
                lightComp.enabled = true;
            }
            if (distance > frontTriggerDistance)
            {
                lightComp.enabled = false;
            }
        }
        // If light is behind the Camera.main use backward trigger distance
        else
        {
            if (distance < behindTriggerDistance)
            {
                lightComp.enabled = true;
            }
            if (distance > behindTriggerDistance)
            {
                lightComp.enabled = false;
            }
        }
    }
}

