using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenuController : MonoBehaviour
{
    private enum SettingsPanel
    {
        Gameplay,
        Controls,
        Audio,
        Graphics
    }

    [SerializeField] private Animator[] panelAnimators = null;
    [SerializeField] private Animator[] buttonAnimators = null;

    private SettingsPanel currentScreen = SettingsPanel.Gameplay;

    private void OnEnable()
    {
        MassDisable(true);

        switch (currentScreen)
        {
            case SettingsPanel.Gameplay:
                GameplayEnable(true);
                break;
            case SettingsPanel.Controls:
                ControlEnable(true);
                break;
            case SettingsPanel.Audio:
                AudioEnable(true);
                break;
            case SettingsPanel.Graphics:
                GraphicsEnable(true);
                break;
        }
    }

    public void GameplayEnable(bool overrideCheck = false)
    {
        if (currentScreen == SettingsPanel.Gameplay && !overrideCheck) { return; }

        MassDisable();
        currentScreen = SettingsPanel.Gameplay;
        panelAnimators[0].Play("Panel In");
        buttonAnimators[0].Play("Hover to Pressed");
    }

    public void ControlEnable(bool overrideCheck = false)
    {
        if (currentScreen == SettingsPanel.Controls && !overrideCheck) { return; }

        MassDisable();
        currentScreen = SettingsPanel.Controls;
        panelAnimators[1].Play("Panel In");
        buttonAnimators[1].Play("Hover to Pressed");
    }

    public void AudioEnable(bool overrideCheck = false)
    {
        if (currentScreen == SettingsPanel.Audio && !overrideCheck) { return; }

        MassDisable();
        currentScreen = SettingsPanel.Audio;
        panelAnimators[2].Play("Panel In");
        buttonAnimators[2].Play("Hover to Pressed");
    }

    public void GraphicsEnable(bool overrideCheck = false)
    {
        if (currentScreen == SettingsPanel.Graphics && !overrideCheck) { return; }

        MassDisable();
        currentScreen = SettingsPanel.Graphics;
        panelAnimators[3].Play("Panel In");
        buttonAnimators[3].Play("Hover to Pressed");
    }

    private void MassDisable(bool resetAll = false)
    {
        if (resetAll)
        {
            foreach (Animator animator in buttonAnimators)
            {
                animator.Play("Normal");
            }
            return;
        }

        switch (currentScreen)
        {
            case SettingsPanel.Gameplay:
                buttonAnimators[0].Play("Pressed to Normal");
                panelAnimators[0].Play("Panel Out");
                break;
            case SettingsPanel.Controls:
                buttonAnimators[1].Play("Pressed to Normal");
                panelAnimators[1].Play("Panel Out");
                break;
            case SettingsPanel.Audio:
                buttonAnimators[2].Play("Pressed to Normal");
                panelAnimators[2].Play("Panel Out");
                break;
            case SettingsPanel.Graphics:
                buttonAnimators[3].Play("Pressed to Normal");
                panelAnimators[3].Play("Panel Out");
                break;
        }
    }
}
