using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatMenuController : MonoBehaviour
{
    private enum GeneralStatsPanel
    {
        Reputation
    }


    [SerializeField] GameObject StatPrefab = null;
    [SerializeField] GameObject StatSpacerPrefab = null;
    [SerializeField] GameObject StatDeriv = null;
    [SerializeField] GameObject ReputationPrefab = null;
    [SerializeField] Transform StatsPanelContent = null;
    [SerializeField] Transform GeneralStatsPanelContent = null;
    private Transform StatsInfoPanel;
    //private Transform SystemStats;
    private Transform InfoParent;
    private PlayerStats playerStats;
    private FactionLogic FL;
    private GeneralStatsPanel lastPanel = GeneralStatsPanel.Reputation;

    private Animator reputationButton;
    private Transform GeneralStatisticsMenu;

    private bool first_enable;
    void Awake()
    {
        first_enable = true;
    }

    void OnEnable()
    {
        if (first_enable)
        {
            StatsPanelContent = transform.Find("Content").Find("StatsPanel").Find("Scroll View").Find("Viewport").Find("Content");
            StatsInfoPanel = transform.Find("Content").Find("StatsInfoTooltip");
            InfoParent = StatsInfoPanel.Find("Content").Find("Derivation");
            playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
            FL = FindObjectOfType<FactionLogic>();
            first_enable = false;

            GeneralStatisticsMenu = transform.Find("Content").Find("GeneralStatistics");
            reputationButton = GeneralStatisticsMenu.Find("ButtonList").Find("FactionReputationButton").GetComponent<Animator>();
        }

        foreach (Transform child in StatsPanelContent)
        {
            Destroy(child.gameObject);
        }

        MassDisable(true);
        GameObject temp;
        temp = Instantiate(StatSpacerPrefab, StatsPanelContent);
        temp.GetComponentInChildren<TextMeshProUGUI>().text = "Player Attributes";
        for (int i = 0; i < STARTUP_DECLARATIONS.Number_of_Attributes; i++)
        {
            temp = Instantiate(StatPrefab, StatsPanelContent);
            string finalValue = playerStats.ReturnAttribute((AttributesEnum)i).ToString();
            List<(string, float)> AddDeriv = playerStats.ReturnAttributeEffects((AttributesEnum)i, true);
            List<(string, float)> MultDeriv = playerStats.ReturnAttributeEffects((AttributesEnum)i, false);
            temp.GetComponent<StatUIPrefab>().Setup(finalValue, AddDeriv, MultDeriv, STARTUP_DECLARATIONS.AttributeDescriptions[i]);
        }

        //SystemStats.Find("LevelPanel").Find("LevelVar").GetComponent<Text>().text = playerStats.returnLevel().ToString();
        transform.Find("Content").Find("StatsHeader").Find("CharacterLevel").GetComponent<TextMeshProUGUI>().text = "LEVEL " + playerStats.returnLevel().ToString();
        transform.Find("Content").Find("StatsHeader").Find("CharacterName").GetComponent<TextMeshProUGUI>().text = playerStats.getPlayerName().ToUpper();

        switch (lastPanel)
        {
            case GeneralStatsPanel.Reputation:
                ReputationEnable(true);
                break;
        }
    }

    public void EnableStatsInfoPanel(string finalValue, List<(string, float)> Add_in, List<(string, float)> Mult_in, (string, string) desc, float posY)
    {
        StatsInfoPanel.position = new Vector3(StatsInfoPanel.position.x, posY, StatsInfoPanel.position.z);

        if (StatsInfoPanel.localPosition.y < -230f)
        {
            StatsInfoPanel.localPosition = new Vector3(StatsInfoPanel.localPosition.x, -230f, StatsInfoPanel.localPosition.z);
        }
        else if (StatsInfoPanel.localPosition.y > 210f)
        {
            StatsInfoPanel.localPosition = new Vector3(StatsInfoPanel.localPosition.x, 210f, StatsInfoPanel.localPosition.z);
        }

        StatsInfoPanel.GetComponent<Animator>().Play("In");
        StatsInfoPanel.Find("Content").Find("StatName").GetComponent<TextMeshProUGUI>().text = desc.Item1.ToUpper();
        StatsInfoPanel.Find("Content").Find("Description").GetComponent<TextMeshProUGUI>().text = desc.Item2;

        foreach(Transform child in InfoParent)
        {
            Destroy(child.gameObject);
        }

        foreach((string, float) i in Add_in)
        {
            string string_add = "";
            if(i.Item2 > 0)
            {
                string_add = "+";
            }
            InstantiateInfoPrefab(i.Item1, string_add + i.Item2.ToString());
        }

        foreach ((string, float) i in Mult_in)
        {
            string string_add = "";
            if (i.Item2 > 0)
            {
                string_add = "+";
            }

            int value = (int)(i.Item2 * 100); //rounding to nearest 1
            if((i.Item2 * 100) % 1 > .5)
            {
                value += 1;
            }

            InstantiateInfoPrefab(i.Item1, string_add + (value).ToString() + "%");
        }

        InstantiateInfoPrefab("Final Value: ", finalValue, true);
    }

    public void DisableStatPanel(bool startup = false)
    {
        foreach (Transform child in StatsInfoPanel.Find("Content").Find("Derivation"))
        {
            Destroy(child.gameObject);
        }

        if (!startup && StatsInfoPanel.gameObject.activeInHierarchy)
        {
            StatsInfoPanel.gameObject.GetComponent<Animator>().Play("Out");
        }
    }

    private void InstantiateInfoPrefab(string Name, string Var, bool special = false)
    {
        if(Var != "")
        {
            GameObject temp = Instantiate(StatDeriv, InfoParent);
            temp.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = Name;
            temp.transform.Find("Var").GetComponent<TextMeshProUGUI>().text = Var;

            if (special)
            {
                temp.GetComponent<Image>().color = STARTUP_DECLARATIONS.goldColorTransparent;
            }
        }
    }


    ////////////////////////////////////////////////////////////////////////////////////////


    public void ReputationEnable(bool overrideCheck = false)
    {
        if (lastPanel == GeneralStatsPanel.Reputation && !overrideCheck) { return; }

        float[] PlayerFaction = FL.return_ReputationMatrix()[1];
        MassDisable();
        lastPanel = GeneralStatsPanel.Reputation;

        for (int i = 4; i < PlayerFaction.Length; ++i)
        {
            Transform temp = Instantiate(ReputationPrefab, GeneralStatsPanelContent).transform;
            temp.Find("Name").GetComponent<TextMeshProUGUI>().text = STARTUP_DECLARATIONS.FactionsEnumReverse[i + 1];
            temp.Find("RepBar").GetComponent<Image>().fillAmount = PlayerFaction[i] / 2000;
        }

        reputationButton.Play("Hover to Pressed");
    }



    private void MassDisable(bool resetAll = false)
    {
        DisableStatPanel();
        ResetButtons(resetAll);

        for(int i = 0; i < GeneralStatsPanelContent.childCount; ++i)
        {
            Destroy(GeneralStatsPanelContent.GetChild(i).gameObject);
        }
    }

    private void ResetButtons(bool resetAll)
    {
        if (resetAll)
        {
            reputationButton.Play("Normal");
            return;
        }

        switch (lastPanel)
        {
            case GeneralStatsPanel.Reputation:
                reputationButton.Play("Pressed to Normal");
                break;
        }
    }
}
