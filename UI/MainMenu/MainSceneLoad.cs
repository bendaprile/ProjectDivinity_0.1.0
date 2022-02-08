using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainSceneLoad : MonoBehaviour
{
    [SerializeField] private StartTypeEnum StartType = StartTypeEnum.DEBUG;
    [SerializeField] private TerrainLoader terrainLoader;
    [SerializeField] private Image loadingBar;
    [SerializeField] private List<AudioClip> audioClip;
    private List<string> loadingDescriptions = new List<string>
    {
        "Don't forget to equip implants. They can often be the difference between life and death.",
        "Use 'Q' & 'E' to shift camera angle.",
        "Be careful what you spend energy on when the overheating bar is near the top.",
        "Use 'Space' to dodge and avoid big attacks. Timing is everything as dodging uses a lot of energy.",
        "Parasympathetic Arrest 10, PA X, or pax is a virus that can affect the nervous system causing the host to turn into a remnant."
    };
    [SerializeField] private TextMeshProUGUI descriptionText = null;

    private float progress = 0f;
    private float timer = 0f;
    private bool SetupComplete = false;

    private NonDiegeticController NDC;



    private void Start()
    {
        if (PlayerPrefs.HasKey("StartTypeOverride") && Application.platform != RuntimePlatform.WindowsEditor) //Load from the MainMenu
        {
            StartType = (StartTypeEnum)PlayerPrefs.GetInt("StartTypeOverride");
        }
        NDC = FindObjectOfType<NonDiegeticController>();
        NDC.ChangeAudioSpecific(audioClip);

        descriptionText.text = loadingDescriptions[Random.Range(0, loadingDescriptions.Count - 1)];
    }

    private void MasterStartupLogic()
    {
        Spawner[] spawns = FindObjectsOfType<Spawner>(true);
        foreach(Spawner s in spawns)
        {
            s.External_Initalize(StartType != StartTypeEnum.LOAD);
        }
    }

    private void Update()
    {
        HandleLoadScreen();
    }

    private void HandleLoadScreen()
    {
        timer += Time.deltaTime;
        progress = terrainLoader.GetProgress();
        loadingBar.fillAmount = progress;

        if (progress > 0 && !SetupComplete) //Do once early on while loading
        {
            SetupComplete = true;
            if (StartType == StartTypeEnum.LOAD)
            {
                FindObjectOfType<SaveData>().LoadFile();
            }
            else if (StartType == StartTypeEnum.NEW_GAME)
            {
                FindObjectOfType<SaveData>().NewGameStart();
            }
            MasterStartupLogic(); //Happens after load
        }

        if (timer >= 15f)
        {
            descriptionText.text = "Don't worry if this load time feels long, you won't experience a single one in game!";
        }

        if (progress >= 1f)
        {
            NDC.ChangeAudioGeneral();
            if (StartType == StartTypeEnum.DEBUG) //Same logic below for now (prom still walks to you for both cases)
            {
                FindObjectOfType<CameraStateController>().SetCamState(CameraStateController.CameraState.AdventureCombatCam);

            }
            else
            {
                FindObjectOfType<PlayerMaster>().PlayerDeath(false);
            }
            Destroy(gameObject);
        }
    }
}
