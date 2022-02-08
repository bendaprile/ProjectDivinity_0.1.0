using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UMA.CharacterSystem;

public class MainMenuController : MonoBehaviour
{
    private enum MenuPanel
    {
        None,
        MainPanel,
        SettingsPanel,
        QuitPanel
    }

    [SerializeField] CinemachineVirtualCamera consoleCam = null;
    [SerializeField] CinemachineVirtualCamera newCharCam = null;
    [SerializeField] Transform player = null;

    private GameObject MainMenu;
    private GameObject SettingsMenu;
    private GameObject QuitMenu;
    private GameObject FlareCreationMenu;
    private GameObject CharacterMenu;
    private Transform NavMenu;
    private Transform umaBody;

    private Animator mainMenuButton;
    private Animator settingsButton;
    private Animator quitButton;
    private Animator continueButton;

    private MenuPanel currentPanel = MenuPanel.None;

    private void Start()
    {
        currentPanel = MenuPanel.None;
        MainMenu = transform.Find("MainMenu").Find("MainPanel").gameObject;
        SettingsMenu = transform.Find("MainMenu").Find("SettingsMenu").gameObject;
        QuitMenu = transform.Find("MainMenu").Find("QuitMenu").gameObject;
        FlareCreationMenu = transform.Find("MainMenu").Find("FlareCreationMenu").gameObject;
        CharacterMenu = transform.Find("MainMenu").Find("CharacterCreator").gameObject;
        NavMenu = transform.Find("MainMenu").Find("NavMenu").transform;
        mainMenuButton = NavMenu.Find("Button List").Find("Home").GetComponent<Animator>();
        settingsButton = NavMenu.Find("Settings").GetComponent<Animator>();
        quitButton = NavMenu.Find("Quit").GetComponent<Animator>();
        continueButton = NavMenu.Find("Button List").Find("Continue").GetComponent<Animator>();
        umaBody = player.Find("Body");

        Debug.Log(Application.persistentDataPath);
        if (!File.Exists(Application.persistentDataPath + "/save.dat"))
        {
            continueButton.gameObject.SetActive(false);
            umaBody.gameObject.SetActive(false);
        }

        MassDisable();
        NavMenu.GetComponent<Animator>().Play("Panel In");
        MainEnable();
        StartCoroutine(FixButtons());
    }

    private IEnumerator FixButtons()
    {
        yield return new WaitForEndOfFrame();
        foreach (HorizontalLayoutGroup group in FindObjectsOfType<HorizontalLayoutGroup>())
        {
            group.enabled = false;
            group.enabled = true;
        }
    }

    public void MainEnable(bool ov = false)
    {
        if (currentPanel == MenuPanel.MainPanel && !ov) { return; }

        HandlePanelTransition();
        currentPanel = MenuPanel.MainPanel;
        MainMenu.SetActive(true);
        MainMenu.GetComponent<Animator>().Play("Panel In");
        mainMenuButton.Play("Hover to Pressed");
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

    public void NewGame()
    {
        StartCoroutine(NewGameCoroutine());
    }

    private IEnumerator NewGameCoroutine()
    {
        HandlePanelTransition();
        NavMenu.GetComponent<Animator>().Play("Panel Out");
        MassDisable();
        consoleCam.Priority = 3;
        yield return new WaitForSecondsRealtime(2.75f);
        FlareCreationMenu.GetComponent<FlareCreationMenu>().EnableMethod();
        FlareCreationMenu.GetComponent<Animator>().Play("Panel In");
    }

    public void NewGameStart()
    {
        PlayerPrefs.SetInt("StartTypeOverride", (int)StartTypeEnum.NEW_GAME);
        StartCoroutine(PlayGameCoroutine(true));
    }

    public void ContinueGame()
    {
        PlayerPrefs.SetInt("StartTypeOverride", (int)StartTypeEnum.LOAD);
        StartCoroutine(PlayGameCoroutine(false));
    }

    IEnumerator PlayGameCoroutine(bool fromCharMenu)
    {
        if (!fromCharMenu)
        {
            HandlePanelTransition();
        }
        yield return new WaitForSecondsRealtime(1f);
        StartCoroutine(FindObjectOfType<LoadMainScene>().EnableMainScene());
    }

    public void QuitDisable()
    {
        if (currentPanel != MenuPanel.QuitPanel) { return; }
        MainEnable();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void CancelCreate()
    {
        StartCoroutine(CancelCreateCoroutine());
    }

    public void CancelCreateChar()
    {
        StartCoroutine(CancelCreateCharCoroutine());
    }

    public void FinalizeFlare()
    {
        StartCoroutine(FinalizeFlareCoroutine());
    }

    private IEnumerator CancelCreateCoroutine()
    {
        FlareCreationMenu.GetComponent<Animator>().Play("Panel Out");
        MassDisable();
        consoleCam.Priority = 1;
        yield return new WaitForSecondsRealtime(2.75f);
        NavMenu.GetComponent<Animator>().Play("Panel In");
        MainEnable(true);
    }

    private IEnumerator CancelCreateCharCoroutine()
    {
        CharacterMenu.GetComponent<Animator>().Play("Panel Out");
        MassDisable();
        consoleCam.Priority = 3;
        newCharCam.Priority = 1;
        yield return new WaitForSecondsRealtime(2.75f);
        player.rotation = Quaternion.Euler(player.rotation.x, -90f, player.rotation.z);
        FlareCreationMenu.GetComponent<FlareCreationMenu>().EnableMethod();
        FlareCreationMenu.GetComponent<Animator>().Play("Panel In");
    }

    private IEnumerator FinalizeFlareCoroutine()
    {
        umaBody.gameObject.SetActive(true);
        player.rotation = Quaternion.Euler(player.rotation.x, -75f, player.rotation.z);
        FlareCreationMenu.GetComponent<Animator>().Play("Panel Out");
        MassDisable();
        newCharCam.Priority = 3;
        consoleCam.Priority = 1;
        yield return new WaitForSecondsRealtime(2.75f);
        CharacterMenu.GetComponent<Animator>().Play("Panel In");
    }

    private void MassDisable()
    {
        mainMenuButton.Play("Normal");
        settingsButton.Play("Normal");
        quitButton.Play("Normal");
    }

    private void HandlePanelTransition()
    {
        switch (currentPanel)
        {
            case MenuPanel.MainPanel:
                mainMenuButton.Play("Pressed to Normal");
                MainMenu.GetComponent<Animator>().Play("Panel Out");
                break;
            case MenuPanel.SettingsPanel:
                settingsButton.Play("Pressed to Normal");
                SettingsMenu.GetComponent<Animator>().Play("Panel Out");
                break;
            case MenuPanel.QuitPanel:
                quitButton.Play("Pressed to Normal");
                QuitMenu.GetComponent<Animator>().Play("Modal Window Out");
                break;
        }
    }
}
