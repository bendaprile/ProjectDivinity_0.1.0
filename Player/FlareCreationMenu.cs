using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class FlareCreationMenu : MonoBehaviour
{
    private enum FlareCreate
    {
        Skills,
        Aptitude,
    }

    [SerializeField] private Animator[] panelAnimators = null;
    [SerializeField] private Animator[] buttonAnimators = null;
    [SerializeField] private Animator confirmButton = null;
    [SerializeField] private GameObject[] incrementButtons = null;
    [SerializeField] private GameObject[] decrementButtons = null;
    [SerializeField] private GameObject[] aptitudePoints = null;
    [SerializeField] private TextMeshProUGUI pointsText = null;
    private Transform StatInfoTooltip;

    private FlareCreate currentScreen = FlareCreate.Skills;

    private int[] skillPoints = new int[7];
    private int skillPointsRemaining = 10;
    private bool aptitudeEnabled = false;
    private bool buttonEnabled = true;

    private void Start()
    {
        StatInfoTooltip = transform.Find("Content").Find("Panels").Find("StatsInfoTooltip");
        FindObjectOfType<DragSelector>().Setup(aptitudePoints);
        foreach (GameObject button in decrementButtons)
        {
            button.SetActive(false);
        }
        MassDisable(true);
        SkillsEnable(true);
    }

    public void EnableMethod()
    {
        pointsText.text = skillPointsRemaining.ToString();
        buttonEnabled = true;
    }

    private void LateUpdate()
    {
        if (aptitudeEnabled && skillPointsRemaining == 0 && !buttonEnabled)
        {
            buttonEnabled = true;
            ToggleButton(buttonEnabled);
        }
        else if (buttonEnabled && (!aptitudeEnabled || skillPointsRemaining > 0))
        {
            buttonEnabled = false;
            ToggleButton(buttonEnabled);
        }
    }

    public void ToggleButton(bool turnOn)
    {
        if (turnOn)
        {
            confirmButton.gameObject.GetComponent<Button>().enabled = true;
            confirmButton.SetTrigger("Normal");
        }
        else
        {
            confirmButton.gameObject.GetComponent<Button>().enabled = false;
            confirmButton.SetTrigger("Disabled");
        }
    }

    public void EnableStatPanel(int statNum, Vector2 pos)
    {
        DisableStatPanel();

        float yPos = statNum > 6 ? pos.y -110f : pos.y;
        float xPos = statNum > 6 ? pos.x : StatInfoTooltip.position.x;
        StatInfoTooltip.position = new Vector3(xPos, yPos, StatInfoTooltip.position.z);

        StatInfoTooltip.GetComponent<Animator>().Play("In");

        StatInfoTooltip.Find("Content").Find("StatName").GetComponent<TextMeshProUGUI>().text = STARTUP_DECLARATIONS.skills_AptitudesDescriptions[statNum].Item1.ToUpper();
        StatInfoTooltip.Find("Content").Find("Description").GetComponent<TextMeshProUGUI>().text = STARTUP_DECLARATIONS.skills_AptitudesDescriptions[statNum].Item2;
    }

    public void DisableStatPanel(bool startup = false)
    {
        if (!startup)
        {
            StatInfoTooltip.gameObject.GetComponent<Animator>().Play("Out");
        }
    }


    public void SkillsEnable(bool overrideCheck = false)
    {
        if (currentScreen == FlareCreate.Skills && !overrideCheck) { return; }

        MassDisable();
        currentScreen = FlareCreate.Skills;
        panelAnimators[0].Play("Panel In");
        buttonAnimators[0].Play("Hover to Pressed");
    }

    public void AptitudeEnable(bool overrideCheck = false)
    {
        if (currentScreen == FlareCreate.Aptitude && !overrideCheck) { return; }

        aptitudeEnabled = true;
        MassDisable();
        currentScreen = FlareCreate.Aptitude;
        panelAnimators[1].Play("Panel In");
        buttonAnimators[1].Play("Hover to Pressed");
    }

    public void ConfirmAllSelections()
    {
        //TODO: apply all skill and aptitude selections
        FindObjectOfType<MainMenuController>().FinalizeFlare();
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
            case FlareCreate.Skills:
                buttonAnimators[0].Play("Pressed to Normal");
                panelAnimators[0].Play("Panel Out");
                break;
            case FlareCreate.Aptitude:
                buttonAnimators[1].Play("Pressed to Normal");
                panelAnimators[1].Play("Panel Out");
                break;
        }
    }

    private int Translation_Cerebral(Vector2 pos)
    {
        Vector3 cerPoint = aptitudePoints[0].transform.Find("Point").GetComponent<RectTransform>().position;
        Vector3 finPoint = aptitudePoints[2].transform.Find("Point").GetComponent<RectTransform>().position;

        Vector2 vec = new Vector2(pos.x - cerPoint.x, pos.y - cerPoint.y);
        //cerPoint.y = finPoint.y
        //y = root(3) * (x - finPoint.x) + cerPoint.y //Fixed side of triangle
        //y = (vec.y / vec.x) * (x - cerPoint.x) + cerPoint.y //Extending the line through the selector to the side
        /////////
        // root(3) * (x - finPoint.x) - (vec.y / vec.x) * (x - cerPoint.x) = 0
        // (root(3) - vec.y / vec.x) * x =  root(3) * finPoint.x - (vec.y / vec.x) * cerPoint.x
        // x = (root(3) * finPoint.x - (vec.y / vec.x) * cerPoint.x) / (root(3) - vec.y / vec.x)

        float max_x = (Mathf.Sqrt(3) * finPoint.x - (vec.y / vec.x) * cerPoint.x) / (Mathf.Sqrt(3) - vec.y / vec.x);
        float max_y = Mathf.Sqrt(3) * (max_x - finPoint.x) + cerPoint.y;
        float max_dist = Vector2.Distance(new Vector2(max_x, max_y), new Vector2(cerPoint.x, cerPoint.y));

        float percent = Vector2.Distance(pos, new Vector2(cerPoint.x, cerPoint.y)) / max_dist;

        //Debug.Log((max_x - cerPoint.x, pos.x - cerPoint.x, percent));
        float value = 8 - 6 * percent;

        return Mathf.RoundToInt(value);
    }

    private int Translation_Finesse(Vector2 pos)
    {
        Vector3 cerPoint = aptitudePoints[0].transform.Find("Point").GetComponent<RectTransform>().position;
        Vector3 finPoint = aptitudePoints[2].transform.Find("Point").GetComponent<RectTransform>().position;

        Vector2 vec = new Vector2(pos.x - finPoint.x, pos.y - finPoint.y);

        float max_x = (Mathf.Sqrt(3) * cerPoint.x + (vec.y / vec.x) * finPoint.x) / (Mathf.Sqrt(3) + vec.y / vec.x);
        float max_y = -Mathf.Sqrt(3) * (max_x - cerPoint.x) + cerPoint.y;
        float max_dist = Vector2.Distance(new Vector2(max_x, max_y), new Vector2(finPoint.x, finPoint.y));

        float percent = Vector2.Distance(pos, new Vector2(finPoint.x, finPoint.y)) / max_dist;

        //Debug.Log((max_x - finPoint.x, pos.x - finPoint.x, percent));
        float value = 8 - 6 * percent;

        return Mathf.RoundToInt(value);
    }

    public void UpdateAptitude(Vector2 pos)
    {
        int cerebral = Translation_Cerebral(pos);
        int finesse = Translation_Finesse(pos);
        int vigor = 12 - cerebral - finesse;

        aptitudePoints[0].GetComponentInChildren<TextMeshProUGUI>().text = cerebral.ToString();
        aptitudePoints[2].GetComponentInChildren<TextMeshProUGUI>().text = finesse.ToString();
        aptitudePoints[1].GetComponentInChildren<TextMeshProUGUI>().text = vigor.ToString();
    }

    public void IncrementSkill(int skillNum) {
        if (skillPointsRemaining > 0)
        {
            skillPoints[skillNum] += 1;
            skillPointsRemaining -= 1;
            pointsText.text = skillPointsRemaining.ToString();
            if (skillPointsRemaining == 0)
            {
                ToggleButtons(incrementButtons, false);
            }
            if (skillPoints[skillNum] > 0)
            {
                decrementButtons[skillNum].SetActive(true);                                                                                                         
            }
        }
    }

    private void ToggleButtons(GameObject[] buttons, bool enable)
    {
        foreach (GameObject button in buttons)
        {
            button.SetActive(enable);
        }
    }

    public void DecrementSkill(int skillNum)
    {
        if (skillPointsRemaining < 10)
        {
            skillPoints[skillNum] -= 1;
            skillPointsRemaining += 1;
            pointsText.text = skillPointsRemaining.ToString();

            if (skillPointsRemaining > 0)
            {
                ToggleButtons(incrementButtons, true);
            }
            if (skillPoints[skillNum] == 0)
            {
                decrementButtons[skillNum].SetActive(false);
            }
        }
    }
}
