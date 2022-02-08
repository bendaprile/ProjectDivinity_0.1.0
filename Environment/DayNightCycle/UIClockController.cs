using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIClockController : MonoBehaviour
{
    [SerializeField] private bool useMilitaryTime = false; 

    private void OnEnable()
    {
        float currentTime = FindObjectOfType<DayNightController>().CurrentTime;
        long hour = (long)currentTime;
        int minutes = (int)((currentTime - hour) * 60);

        string minuteStr = minutes.ToString();
        if (minutes < 10)
        {
            minuteStr = "0" + minutes.ToString();
        }


        if (useMilitaryTime)
        {
            GetComponent<TextMeshProUGUI>().text = hour.ToString() + ":" + minuteStr;
        }
        else
        {
            string ampm = "AM";
            if (hour >= 12)
            {
                hour = hour - 12;
                ampm = "PM";
            }
            if (hour == 0)
            {
                hour = 12;
            }
            GetComponent<TextMeshProUGUI>().text = hour.ToString() + ":" + minuteStr + ampm;
        }
    }
}
