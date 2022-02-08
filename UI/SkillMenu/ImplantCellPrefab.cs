using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ImplantCellPrefab : MonoBehaviour
{
    private Image img;

    //EXCLUSIVE Below
    private bool is_locked;
    private bool is_temp_unlocked;
    //EXCLUSIVE Above

    private bool is_used;

    private int unlock_cost;

    private TextMeshProUGUI cost_text;
    private ImplantController implantController;

    public void Setup(bool locked, int cost, ImplantController IC)
    {
        implantController = IC;
        img = transform.GetComponent<Image>();
        cost_text = transform.GetComponentInChildren<TextMeshProUGUI>();
        is_used = false;
        unlock_cost = cost;
        is_temp_unlocked = false;

        if (locked)
        {
            is_locked = true;
            img.color = Color.gray;
        }
        else
        {
            is_locked = false;
            img.color = Color.white;
        }
    }

    public void LockInTemp()
    {
        if (is_temp_unlocked)
        {
            img.color = Color.white;
            is_temp_unlocked = false;
        }
    }

    public void DisplayON()
    {
        if (is_locked)
        {
            cost_text.text = unlock_cost.ToString();
        }
    }

    public void DisplayOFF()
    {
        cost_text.text = "";
    }

    public void TempChange()
    {
        if(is_locked || is_temp_unlocked)
        {
            bool valid = implantController.AttemptTempCellChange(unlock_cost, !is_temp_unlocked);

            if (valid)
            {
                if (is_temp_unlocked)
                {
                    img.color = Color.gray;
                    is_temp_unlocked = false;
                    is_locked = true;
                }
                else
                {
                    img.color = Color.cyan;
                    is_temp_unlocked = true;
                    is_locked = false;
                }
            }
        }
    }

    public bool returnLocked()
    {
        return is_locked;
    }

    public bool returnUsed()
    {
        return is_used;
    }

    public void setUsed(bool set)
    {
        is_used = set;
    }
}
