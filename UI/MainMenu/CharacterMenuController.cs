using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CharacterMenuController : MonoBehaviour
{
    private enum CharacterPanel
    {
        Body,
        Head,
        Face,
        Unused
    }

    [SerializeField] Transform playerBody = null;
    [SerializeField] private CinemachineVirtualCamera bodyCam = null;
    [SerializeField] private CinemachineVirtualCamera faceCam = null;
    [SerializeField] private Animator[] panelAnimators = null;
    [SerializeField] private Animator[] buttonAnimators = null;

    private CharacterPanel currentScreen = CharacterPanel.Body;

    private void OnEnable()
    {
        MassDisable(true);

        switch (currentScreen)
        {
            case CharacterPanel.Body:
                BodyEnable(true);
                break;
            case CharacterPanel.Head:
                HeadEnable(true);
                break;
            case CharacterPanel.Face:
                FaceEnable(true);
                break;
            case CharacterPanel.Unused:
                UnusedEnable(true);
                break;
        }
    }

    public void BodyEnable(bool overrideCheck = false)
    {
        if (currentScreen == CharacterPanel.Body && !overrideCheck) { return; }

        MassDisable();
        bodyCam.Priority = 3;
        faceCam.Priority = 1;
        currentScreen = CharacterPanel.Body;
        panelAnimators[0].Play("Panel In");
        buttonAnimators[0].Play("Hover to Pressed");
    }

    public void HeadEnable(bool overrideCheck = false)
    {
        if (currentScreen == CharacterPanel.Head && !overrideCheck) { return; }

        faceCam.Follow = playerBody.FindDeepChild("Head");
        MassDisable();
        faceCam.Priority = 3;
        bodyCam.Priority = 1;
        currentScreen = CharacterPanel.Head;
        panelAnimators[1].Play("Panel In");
        buttonAnimators[1].Play("Hover to Pressed");
    }

    public void FaceEnable(bool overrideCheck = false)
    {
        if (currentScreen == CharacterPanel.Face && !overrideCheck) { return; }

        faceCam.Follow = playerBody.FindDeepChild("Head");
        MassDisable();
        faceCam.Priority = 3;
        bodyCam.Priority = 1;
        currentScreen = CharacterPanel.Face;
        panelAnimators[2].Play("Panel In");
        buttonAnimators[2].Play("Hover to Pressed");
    }

    public void UnusedEnable(bool overrideCheck = false)
    {
        if (currentScreen == CharacterPanel.Unused && !overrideCheck) { return; }

        MassDisable();
        currentScreen = CharacterPanel.Unused;
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
            case CharacterPanel.Body:
                buttonAnimators[0].Play("Pressed to Normal");
                panelAnimators[0].Play("Panel Out");
                break;
            case CharacterPanel.Head:
                buttonAnimators[1].Play("Pressed to Normal");
                panelAnimators[1].Play("Panel Out");
                break;
            case CharacterPanel.Face:
                buttonAnimators[2].Play("Pressed to Normal");
                panelAnimators[2].Play("Panel Out");
                break;
            case CharacterPanel.Unused:
                buttonAnimators[3].Play("Pressed to Normal");
                panelAnimators[3].Play("Panel Out");
                break;
        }
    }
}
