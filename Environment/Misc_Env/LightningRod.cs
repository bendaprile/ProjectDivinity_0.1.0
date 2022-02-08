using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningRod : MonoBehaviour
{
    [SerializeField] private GameObject Lightning;
    [SerializeField] private Light lamp;
    [SerializeField] private EnableDisableLight EDL;

    [SerializeField] private float Duration = 2f;
    [SerializeField] private float Light_Override_Duration = 10f;
    [SerializeField] private float Brightness_Mult = 100f;

    [SerializeField] private float lightningChancePerFixedUpdate = .001f;

    private float original_brightness;
    private float brightness_iter;
    private float turn_off_time = 0f;

    private bool light_state;

    private void Start()
    {
        if (lamp)
        {
            original_brightness = lamp.intensity;
            brightness_iter = original_brightness * (Brightness_Mult - 1) / Light_Override_Duration;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Lightning.activeSelf)
        {
            if(Time.time >= turn_off_time)
            {
                Lightning.SetActive(false);
            }
        }
        else if (Random.value < lightningChancePerFixedUpdate)
        {
            Lightning.SetActive(true);
            turn_off_time = Time.time + Duration;

            if (lamp && EDL)
            {
                light_state = lamp.enabled;
                EDL.enabled = false;
                lamp.enabled = true;
                lamp.intensity = original_brightness * Brightness_Mult;
            }
        }

        if(lamp && lamp.intensity > original_brightness)
        {
            lamp.intensity -= brightness_iter * Time.fixedDeltaTime;
        }
        else if (EDL && EDL.enabled == false) //Happens once
        {
            EDL.enabled = true;
            lamp.enabled = light_state; //Here so it will turn off a frame sooner
            lamp.intensity = original_brightness;
        }
    }
}
