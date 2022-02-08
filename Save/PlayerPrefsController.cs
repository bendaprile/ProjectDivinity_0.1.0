using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Michsky.UI.ModernUIPack;

public class PlayerPrefsController : MonoBehaviour
{
    // Should be set in order Master, Music, Effects
    [SerializeField] private AudioMixer audioMixer = null;

    private Transform audioPanel;

    // Start is called before the first frame update
    void Start()
    {
        audioPanel = transform.Find("Content").Find("Panels").Find("Audio").Find("Content").Find("List");
    }

    private void Update()
    {
        audioMixer.SetFloat("MasterVolume", audioPanel.Find("Master").Find("Slider").GetComponent<Slider>().value - 80f);
        audioMixer.SetFloat("MusicVolume", audioPanel.Find("Music").Find("Slider").GetComponent<Slider>().value - 80f);
        audioMixer.SetFloat("EffectsVolume", audioPanel.Find("Effects").Find("Slider").GetComponent<Slider>().value - 80f);
    }

    public void SetScreenResolution(HorizontalSelector hz)
    {
        Screen.SetResolution(STARTUP_DECLARATIONS.screenResolutions[hz.index].Item1,
            STARTUP_DECLARATIONS.screenResolutions[hz.index].Item2, true);
    }

    public void SetGraphicsQuality(HorizontalSelector hz)
    {
        QualitySettings.SetQualityLevel(hz.index);
        //Debug.Log("Quality Settings: " + QualitySettings.GetQualityLevel());
        //Debug.Log("Particle Limit: " + QualitySettings.particleRaycastBudget);
    }
}
