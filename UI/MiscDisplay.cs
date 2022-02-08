using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiscDisplay : MonoBehaviour
{
    private TextMeshProUGUI mainText;
    private TextMeshProUGUI secondaryText;
    private Color32 defaultMainColor;
    private Color32 defaultSecondaryColor;

    private float counter;
    private bool LOCKED = false;

    void Start()
    {
        mainText = transform.Find("MainText").GetComponent<TextMeshProUGUI>();
        secondaryText = transform.Find("SecondaryText").GetComponent<TextMeshProUGUI>();
        defaultMainColor = mainText.color;
        defaultSecondaryColor = secondaryText.color;
    }

    public void enableDisplay(string mtext, string stext, SkillCheckStatus skillCheck = SkillCheckStatus.NoCheck, float dur = 0.1f, bool LOCK_in = false)
    {
        if (LOCKED)
        {
            return;
        }
        mainText.text = mtext;
        secondaryText.text = stext;
        LOCKED = LOCK_in;

        switch (skillCheck)
        {
            case SkillCheckStatus.NoCheck:
                mainText.color = defaultMainColor;
                secondaryText.color = defaultSecondaryColor;
                break;
            case SkillCheckStatus.Failure:
                mainText.color = STARTUP_DECLARATIONS.checkFailColor;
                secondaryText.color = STARTUP_DECLARATIONS.checkFailColor;
                break;
            case SkillCheckStatus.Success:
                mainText.color = STARTUP_DECLARATIONS.checkSuccessColor;
                secondaryText.color = STARTUP_DECLARATIONS.checkSuccessColor;
                break;
        }

        counter = dur;
    }

    // Update is called once per frame
    void Update()
    {
        if(counter > 0)
        {
            counter -= Time.deltaTime;
        }
        else
        {
            mainText.text = "";
            secondaryText.text = "";
            LOCKED = false;
        }
    }
}
