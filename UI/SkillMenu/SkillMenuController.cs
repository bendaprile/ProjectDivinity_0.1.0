using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillMenuController : MonoBehaviour
{
    [SerializeField] private Transform Name_ValuePrefab;
    [SerializeField] private GameObject LevelButton;

    [SerializeField] private GameObject[] ImplantScreens;

    private PlayerStats playerStats;

    private Transform[] AptitudesArray;

    private Transform[] SkillsArray;
    private int[] TempPoints_SkillsArray;
    private int TempPoints = -1; //-1 if not leveling currently

    private Transform ImplantStatsTooltip;
    private Transform StatInfoTooltip;
    private ImplantUIHolder IuiH;

    void Start()
    {
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();

        AptitudesArray = new Transform[3];
        AptitudesArray[(int)AptitudeEnum.Vigor] = transform.Find("AptitudeLayout").Find("VigorPanel");
        AptitudesArray[(int)AptitudeEnum.Cerebral] = transform.Find("AptitudeLayout").Find("CerebralPanel");
        AptitudesArray[(int)AptitudeEnum.Finesse] = transform.Find("AptitudeLayout").Find("FinessePanel");

        SkillsArray = new Transform[7];
        SkillsArray[(int)SkillsEnum.Larceny] = transform.Find("SkillsLayout").Find("LarcenyPanel");
        SkillsArray[(int)SkillsEnum.Science] = transform.Find("SkillsLayout").Find("SciencePanel");
        SkillsArray[(int)SkillsEnum.Medicine] = transform.Find("SkillsLayout").Find("MedicinePanel");
        SkillsArray[(int)SkillsEnum.Speech] = transform.Find("SkillsLayout").Find("SpeechPanel");
        SkillsArray[(int)SkillsEnum.Survival] = transform.Find("SkillsLayout").Find("SurvivalPanel");
        SkillsArray[(int)SkillsEnum.Gadgeteer] = transform.Find("SkillsLayout").Find("GadgeteerPanel");
        SkillsArray[(int)SkillsEnum.NightOwl] = transform.Find("SkillsLayout").Find("NightOwlPanel");
        TempPoints_SkillsArray = new int[7];

        ImplantStatsTooltip = transform.Find("ImplantStatsTooltip");
        StatInfoTooltip = transform.Find("StatsInfoTooltip");

        IuiH = GetComponentInChildren<ImplantUIHolder>();

        IncrementButtonState(false);
        DisableImplantStatPanel(true);
        DisableStatPanel(true);
        ImplantSwitchButton(0);
    }

    private void IncrementButtonState(bool set)
    {
        foreach (SkillsEnum i in Enum.GetValues(typeof(SkillsEnum)))
        {
            SkillsArray[(int)i].transform.Find("Decrease").gameObject.SetActive(set);
            SkillsArray[(int)i].transform.Find("Increase").gameObject.SetActive(set);
        }
    }

    // Update is called once per frame
    void Update()
    {
        AptitudesArray[(int)AptitudeEnum.Vigor].Find("StatValue").GetComponent<TextMeshProUGUI>().text = playerStats.ReturnAptitude(AptitudeEnum.Vigor).ToString();
        AptitudesArray[(int)AptitudeEnum.Cerebral].Find("StatValue").GetComponent<TextMeshProUGUI>().text = (playerStats.ReturnAptitude(AptitudeEnum.Cerebral)).ToString();
        AptitudesArray[(int)AptitudeEnum.Finesse].Find("StatValue").GetComponent<TextMeshProUGUI>().text = (playerStats.ReturnAptitude(AptitudeEnum.Finesse)).ToString();

        foreach (SkillsEnum i in Enum.GetValues(typeof(SkillsEnum)))
        {
            int final_value = playerStats.ReturnSkill(i);
            if (TempPoints_SkillsArray[(int)i] > 0)
            {
                Debug.Log(TempPoints_SkillsArray[(int)i]);
                final_value += TempPoints_SkillsArray[(int)i];
                SkillsArray[(int)i].Find("StatValue").GetComponent<TextMeshProUGUI>().color = STARTUP_DECLARATIONS.goldColor;
            }
            else
            {
                SkillsArray[(int)i].Find("StatValue").GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            SkillsArray[(int)i].Find("StatValue").GetComponent<TextMeshProUGUI>().text = final_value.ToString();
        }

        LevelButton.SetActive(playerStats.isLevelUpQueued() || TempPoints == 0);
        LevelButton.GetComponentInChildren<TextMeshProUGUI>().text = playerStats.isLevelUpQueued() ? "Level Up" : "Finalize Leveling";
    }

    public void EnableStatPanel(int statNum, float posY)
    {
        DisableStatPanel();

        StatInfoTooltip.position = new Vector3(StatInfoTooltip.position.x, posY, StatInfoTooltip.position.z);

        if (StatInfoTooltip.localPosition.y < -230f)
        {
            StatInfoTooltip.localPosition = new Vector3(StatInfoTooltip.localPosition.x, -230f, StatInfoTooltip.localPosition.z);
        }
        else if (StatInfoTooltip.localPosition.y > 130f)
        {
            StatInfoTooltip.localPosition = new Vector3(StatInfoTooltip.localPosition.x, 130f, StatInfoTooltip.localPosition.z);
        }

        StatInfoTooltip.GetComponent<Animator>().Play("In");

        StatInfoTooltip.Find("Content").Find("StatName").GetComponent<TextMeshProUGUI>().text = STARTUP_DECLARATIONS.skills_AptitudesDescriptions[statNum].Item1.ToUpper();
        StatInfoTooltip.Find("Content").Find("Description").GetComponent<TextMeshProUGUI>().text = STARTUP_DECLARATIONS.skills_AptitudesDescriptions[statNum].Item2;
    }

    public void EnableImplantStatPanel(string itemName, string description, ItemQuality itemClass, List<(string, string)> data, Vector3 pos, bool staticPos, float sizeX = 0)
    {
        DisableImplantStatPanel();
        if (staticPos)
        {
            ImplantStatsTooltip.position = new Vector3(pos.x, pos.y, ImplantStatsTooltip.position.z);
            ImplantStatsTooltip.localPosition = new Vector3(ImplantStatsTooltip.localPosition.x - 450f, ImplantStatsTooltip.localPosition.y, ImplantStatsTooltip.localPosition.z);
        }
        else
        {
            ImplantStatsTooltip.position = new Vector3(pos.x, pos.y, ImplantStatsTooltip.position.z);
            ImplantStatsTooltip.localPosition = new Vector3(ImplantStatsTooltip.localPosition.x + 200 + (sizeX/2), ImplantStatsTooltip.localPosition.y, ImplantStatsTooltip.localPosition.z);
        }

        if (ImplantStatsTooltip.localPosition.y < -230f)
        {
            ImplantStatsTooltip.localPosition = new Vector3(ImplantStatsTooltip.localPosition.x, -230f, ImplantStatsTooltip.localPosition.z);
        }
        else if (ImplantStatsTooltip.localPosition.y > 130f)
        {
            ImplantStatsTooltip.localPosition = new Vector3(ImplantStatsTooltip.localPosition.x, 130f, ImplantStatsTooltip.localPosition.z);
        }

        ImplantStatsTooltip.GetComponent<Animator>().Play("In");
        ImplantStatsTooltip.Find("Content").Find("ItemName").GetComponent<TextMeshProUGUI>().text = itemName.ToUpper();
        ImplantStatsTooltip.Find("Content").Find("Border").GetComponent<Image>().color = STARTUP_DECLARATIONS.itemQualityColors[itemClass];
        ImplantStatsTooltip.Find("Content").Find("Description").GetComponent<TextMeshProUGUI>().text = description;

        foreach ((string, string) item in data)
        {
            Transform temp = Instantiate(Name_ValuePrefab, ImplantStatsTooltip.Find("Content").Find("Stats"));
            temp.Find("Name").GetComponent<TextMeshProUGUI>().text = item.Item1;
            temp.Find("Value").GetComponent<TextMeshProUGUI>().text = item.Item2;
        }
    }

    public void DisableStatPanel(bool startup = false)
    {
        if (!startup)
        {
            StatInfoTooltip.gameObject.GetComponent<Animator>().Play("Out");
        }
    }

    public void DisableImplantStatPanel(bool startup = false)
    {
        foreach (Transform child in ImplantStatsTooltip.Find("Content").Find("Stats"))
        {
            Destroy(child.gameObject);
        }

        if (!startup)
        {
            ImplantStatsTooltip.gameObject.GetComponent<Animator>().Play("Out");
        }
    }

    /// <summary>
    /// ////////////////////////////////////////////////////////////////////
    /// </summary>


    public void ImplantSwitchButton(int i)
    {
        for(int iter = 0; iter < 3; iter++)
        {
            if(iter != i)
            {
                ImplantScreens[iter].SetActive(false);
                AptitudesArray[iter].GetComponent<SkillsMenuTooltip>().SetColor(new Color(0, 0, 0, 0));
            }
        }

        IuiH.ChangeType((AptitudeEnum)i);
        ImplantScreens[i].SetActive(true);
        switch (i)
        {
            case 0:
                AptitudesArray[i].GetComponent<SkillsMenuTooltip>().SetColor(new Color32(200, 0, 0, 155));
                break;
            case 1:
                AptitudesArray[i].GetComponent<SkillsMenuTooltip>().SetColor(new Color32(0, 160, 255, 155));
                break;
            case 2:
                AptitudesArray[i].GetComponent<SkillsMenuTooltip>().SetColor(new Color32(124, 0, 195, 155));
                break;
        }
    }

    public void LevelButtonPressed()
    {
        if (playerStats.isLevelUpQueued()) //Level Up
        {
            if(TempPoints == -1)
            {
                TempPoints = 0;
            }

            TempPoints += 5 * playerStats.LevelUp();
            IncrementButtonState(true);
        }
        else //Finish setting skills
        {
            foreach (SkillsEnum i in Enum.GetValues(typeof(SkillsEnum)))
            {
                playerStats.ModifySkill(i, TempPoints_SkillsArray[(int)i]);
            }

            foreach (int i in Enum.GetValues(typeof(SkillsEnum)))
            {
                TempPoints_SkillsArray[i] = 0;
            }

            TempPoints = -1; //Stop Leveling
            IncrementButtonState(false);
        }
    }

    public void DecrementSkill(int i)
    {
        if(TempPoints_SkillsArray[i] > 0)
        {
            TempPoints_SkillsArray[i] -= 1;
            TempPoints += 1;
        }
    }

    public void IncrementSkill(int i)
    {
        if (TempPoints > 0 && playerStats.ReturnSkill((SkillsEnum)i) < 100)
        {
            TempPoints_SkillsArray[i] += 1;
            TempPoints -= 1;
        }
    }
}
