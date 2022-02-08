using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    private enum MenuPanel
    {
        None,
        WorldPanel,
        StatsPanel,
        InventoryPanel,
        SkillPanel,
        AbilitiesPanel,
        SettingsPanel,
        QuitPanel
    }

    [SerializeField] GameObject Map = null;
    [SerializeField] GameObject depthOfField = null;
    [SerializeField] List<AudioClip> audioClips;

    private GameObject WorldMenu;
    private GameObject InventoryMenu;
    private GameObject StatsMenu;
    private GameObject SkillMenu;
    private GameObject AbilitiesMenu;
    private GameObject SettingsMenu;
    private GameObject QuitMenu;
    private GameObject BackgroundImage;

    private Animator navMenu;
    private Animator worldButton;
    private Animator statsButton;
    private Animator inventoryButton;
    private Animator skillsButton;
    private Animator abilitiesButton;
    private Animator settingsButton;
    private Animator quitButton;

    private CombatChecker CombatChecker;
    private UIController uiController;
    private NonDiegeticController AudioControl;
    private PlayerStats playerStats;

    private bool pulsate;
    private bool firstRun = true; 
    private MenuPanel currentPanel = MenuPanel.None;

    private void OnEnable()
    {
        currentPanel = MenuPanel.None;
        if (firstRun)
        {
            FirstRunFunction();
            firstRun = false; 
        }

        navMenu.Play("NavMenu In");
        AudioControl.ChangeAudioSpecific(audioClips);

        if (CombatChecker.enemies_nearby)
        {
            pulsate = true;
        }
        else
        {
            pulsate = false;
            BackgroundImage.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
        }

        MassDisable();
        if (Map.activeSelf) //This brings the map in front of the image
        {
            WorldEnable();
        }
        HandleLevelUp();
    }

    private void FirstRunFunction()
    {
        AudioControl = GameObject.Find("Non Diegetic Audio").GetComponent<NonDiegeticController>();
        CombatChecker = GameObject.Find("CombatChecker").GetComponent<CombatChecker>();
        BackgroundImage = transform.Find("BackgroundImage").gameObject;
        SkillMenu = transform.Find("Panel").Find("SkillMenu").gameObject;
        StatsMenu = transform.Find("Panel").Find("StatsMenu").gameObject;
        WorldMenu = transform.Find("Panel").Find("WorldMenu").gameObject;
        InventoryMenu = transform.Find("Panel").Find("InventoryMenu").gameObject;
        AbilitiesMenu = transform.Find("Panel").Find("AbilitiesMenu").gameObject;
        SettingsMenu = transform.Find("Panel").Find("SettingsMenu").gameObject;
        QuitMenu = transform.Find("Panel").Find("QuitMenu").gameObject;
        worldButton = transform.Find("NavMenu").Find("ButtonList").Find("World").GetComponent<Animator>();
        statsButton = transform.Find("NavMenu").Find("ButtonList").Find("Stats").GetComponent<Animator>();
        inventoryButton = transform.Find("NavMenu").Find("ButtonList").Find("Inventory").GetComponent<Animator>();
        skillsButton = transform.Find("NavMenu").Find("ButtonList").Find("Skills").GetComponent<Animator>();
        abilitiesButton = transform.Find("NavMenu").Find("ButtonList").Find("Abilities").GetComponent<Animator>();
        settingsButton = transform.Find("NavMenu").Find("ButtonList").Find("Settings").GetComponent<Animator>();
        quitButton = transform.Find("NavMenu").Find("ButtonList").Find("Quit").GetComponent<Animator>();
        navMenu = transform.Find("NavMenu").GetComponent<Animator>();
        uiController = GetComponentInParent<UIController>();
        playerStats = FindObjectOfType<PlayerStats>();
    }

    private void Update()
    {
        if (pulsate)
        {
            float redness = 0.4f * (2f + Mathf.Sin((float)Time.unscaledTime*.5f)); //goes between .1 and .2
            BackgroundImage.GetComponent<Image>().color = new Color(255, 0f, 0f, redness);
        }
    }

    private void OnDisable()
    {
        uiController.EnableDisableMap(false);
        AudioControl.ChangeAudioGeneral();
    }

    private void HandleLevelUp()
    {
        if (playerStats.isLevelUpQueued())
        {
            SkillEnable();
        }
    }

    public void SkillEnable()
    {
        if (currentPanel == MenuPanel.SkillPanel) { return; }

        HandlePanelTransition();
        currentPanel = MenuPanel.SkillPanel;
        SkillMenu.SetActive(true);
        SkillMenu.GetComponent<Animator>().Play("Panel In");
        skillsButton.Play("Hover to Pressed");
    }

    public void StatsEnable()
    {
        if (currentPanel == MenuPanel.StatsPanel) { return; }

        HandlePanelTransition();
        currentPanel = MenuPanel.StatsPanel;
        StatsMenu.SetActive(true);
        StatsMenu.GetComponent<Animator>().Play("Panel In");
        statsButton.Play("Hover to Pressed");
    }

    public void WorldEnable()
    {
        if (currentPanel == MenuPanel.WorldPanel) { return; }

        HandlePanelTransition();
        currentPanel = MenuPanel.WorldPanel;
        WorldMenu.SetActive(true);
        WorldMenu.GetComponent<Animator>().Play("Panel In");
        worldButton.Play("Hover to Pressed");
        uiController.EnableDisableMap(true);
    }

    public void AbilitiesEnable()
    {
        if (currentPanel == MenuPanel.AbilitiesPanel) { return; }

        HandlePanelTransition();
        currentPanel = MenuPanel.AbilitiesPanel;
        AbilitiesMenu.SetActive(true);
        AbilitiesMenu.GetComponent<Animator>().Play("Panel In");
        abilitiesButton.Play("Hover to Pressed");
    }

    public void InventoryEnable()
    {
        if (currentPanel == MenuPanel.InventoryPanel) { return; }

        HandlePanelTransition();
        currentPanel = MenuPanel.InventoryPanel;
        InventoryMenu.SetActive(true);
        InventoryMenu.GetComponent<Animator>().Play("Panel In");
        inventoryButton.Play("Hover to Pressed");
    }

    public void SettingsEnable()
    {
        if (currentPanel == MenuPanel.SettingsPanel) { return; }

        HandlePanelTransition();
        currentPanel = MenuPanel.SettingsPanel;
        SettingsMenu.SetActive(true);
        SettingsMenu.GetComponent<Animator>().Play("Panel In");
        settingsButton.Play("Hover to Pressed");
    }

    public void QuitEnable()
    {
        if (currentPanel == MenuPanel.QuitPanel) { return; }

        HandlePanelTransition();
        currentPanel = MenuPanel.QuitPanel;
        QuitMenu.SetActive(true);
        QuitMenu.GetComponent<Animator>().Play("Modal Window In");
        quitButton.Play("Hover to Pressed");
    }

    public void QuitDisable()
    {
        if (currentPanel != MenuPanel.QuitPanel) { return; }

        HandlePanelTransition();
        currentPanel = MenuPanel.None;
    }

    private void MassDisable()
    {
        SkillMenu.SetActive(false);
        StatsMenu.SetActive(false);
        WorldMenu.SetActive(false);
        SettingsMenu.SetActive(false);
        InventoryMenu.SetActive(false);
        QuitMenu.SetActive(false);
        uiController.EnableDisableMap(false);

        worldButton.Play("Normal");
        statsButton.Play("Normal");
        inventoryButton.Play("Normal");
        skillsButton.Play("Normal");
        abilitiesButton.Play("Normal");
        settingsButton.Play("Normal");
        quitButton.Play("Normal");
    }

    private void HandlePanelTransition()
    {
        switch(currentPanel)
        {
            case MenuPanel.WorldPanel:
                worldButton.Play("Pressed to Normal");
                WorldMenu.GetComponent<Animator>().Play("Panel Out");
                StartCoroutine(TurnOffPanel(MenuPanel.WorldPanel, true));
                uiController.EnableDisableMap(false);
                break;
            case MenuPanel.StatsPanel:
                statsButton.Play("Pressed to Normal");
                StatsMenu.GetComponent<Animator>().Play("Panel Out");
                StartCoroutine(TurnOffPanel(MenuPanel.StatsPanel, true));
                break;
            case MenuPanel.InventoryPanel:
                inventoryButton.Play("Pressed to Normal");
                InventoryMenu.GetComponent<Animator>().Play("Panel Out");
                StartCoroutine(TurnOffPanel(MenuPanel.InventoryPanel, true));
                break;
            case MenuPanel.SkillPanel:
                skillsButton.Play("Pressed to Normal");
                SkillMenu.GetComponent<Animator>().Play("Panel Out");
                StartCoroutine(TurnOffPanel(MenuPanel.SkillPanel, true));
                break;
            case MenuPanel.AbilitiesPanel:
                abilitiesButton.Play("Pressed to Normal");
                AbilitiesMenu.GetComponent<Animator>().Play("Panel Out");
                StartCoroutine(TurnOffPanel(MenuPanel.AbilitiesPanel, true));
                break;
            case MenuPanel.SettingsPanel:
                settingsButton.Play("Pressed to Normal");
                SettingsMenu.GetComponent<Animator>().Play("Panel Out");
                StartCoroutine(TurnOffPanel(MenuPanel.SettingsPanel, true));
                break;
            case MenuPanel.QuitPanel:
                quitButton.Play("Pressed to Normal");
                QuitMenu.GetComponent<Animator>().Play("Modal Window Out");
                StartCoroutine(TurnOffPanel(MenuPanel.QuitPanel, true));
                break;
        }
    }

    private IEnumerator TurnOffPanel(MenuPanel menuPanel, bool animationBased = false)
    {
        float timeToWait = 0.3f;
        if (!animationBased)
        {
            timeToWait = 0f;
        }

        yield return new WaitForSecondsRealtime(timeToWait);

        if (menuPanel == currentPanel)
        {
            yield break;
        }

        switch(menuPanel)
        {
            case MenuPanel.WorldPanel:
                WorldMenu.SetActive(false);
                break;
            case MenuPanel.StatsPanel:
                StatsMenu.SetActive(false);
                break;
            case MenuPanel.InventoryPanel:
                InventoryMenu.SetActive(false);
                break;
            case MenuPanel.SkillPanel:
                SkillMenu.SetActive(false);
                break;
            case MenuPanel.AbilitiesPanel:
                AbilitiesMenu.SetActive(false);
                break;
            case MenuPanel.SettingsPanel:
                SettingsMenu.SetActive(false);
                break;
            case MenuPanel.QuitPanel:
                QuitMenu.SetActive(false);
                break;
        }
    }
}
