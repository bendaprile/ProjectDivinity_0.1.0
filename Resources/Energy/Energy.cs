using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Energy : MonoBehaviour
{

    private float maxEnergy;
    private float OverheatCapacity;
    private float energyRegen;
    private float CooldownRate;

    private Image energySlider = null;
    private Image OverheatSlider = null;

    private float currentEnergy;
    private float currentOverheat;

    void Start()
    {
        currentEnergy = maxEnergy;
        currentOverheat = 0;
        energySlider = GameObject.Find("EnergyBar").GetComponent<Image>();
        OverheatSlider = GameObject.Find("OverheatBar").GetComponent<Image>();
    }

    public void OnDeathFunc()
    {
        currentEnergy = maxEnergy;
    }

    public void modify_maxEnergy(float value)
    {
        currentEnergy += (value - maxEnergy);
        maxEnergy = value;
    }
    public void modify_OverheatCapacity(float value)
    {
        OverheatCapacity = value;
    }

    public void modify_energyRegen(float value)
    {
        energyRegen = value;
    }
    public void modify_CooldownRate(float value)
    {
        CooldownRate = value;
    }

    public bool Drain_ES(bool Overheat, float amount)
    {
        if (Overheat)
        {
            if (currentEnergy >= OverheatMult(amount))
            {
                currentEnergy -= OverheatMult(amount);
                currentOverheat += amount;
                if (currentOverheat > OverheatCapacity)
                {
                    currentOverheat = OverheatCapacity;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (currentEnergy >= amount)
            {
                currentEnergy -= amount;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public bool Drain_ES_Greater(bool Overheat, float amount, bool NewCommand, float new_required_amount) //Used for some time-based drains. A new command requires the required_amount
    {
        if(NewCommand && (new_required_amount > currentEnergy))
        {
            return false;
        }
        return Drain_ES(Overheat, amount);
    }

    void FixedUpdate()
    {
        if(currentEnergy >= maxEnergy)
        {
            currentEnergy = maxEnergy;
        }
        else
        {
            currentEnergy += energyRegen * Time.fixedDeltaTime;
        }

        if(currentOverheat < 0)
        {
            currentOverheat = 0;
        }
        else
        {
            currentOverheat -= CooldownRate * OverheatCapacity * Time.fixedDeltaTime;
        }

        if(energySlider)
        {
            UpdateEnergyBar();
        }
    }

    void UpdateEnergyBar()
    {
        if (currentOverheat >= (0.9f * OverheatCapacity))
        {
            energySlider.color = Color.red;
        }
        else if (currentOverheat >= (0.5f * OverheatCapacity))
        {
            energySlider.color = Color.yellow;
        }
        else
        {
            energySlider.color = Color.green;
        }

        energySlider.fillAmount = currentEnergy / maxEnergy;
        OverheatSlider.fillAmount = currentOverheat / OverheatCapacity;
    }

    private float OverheatMult(float original)
    {
        if(currentOverheat >= (0.9f * OverheatCapacity))
        {
            return 2 * original;  
        }
        else if(currentOverheat >= (0.5f * OverheatCapacity))
        {
            return 1.5f * original;
        }
        else
        {
            return original;
        }
    }
}
