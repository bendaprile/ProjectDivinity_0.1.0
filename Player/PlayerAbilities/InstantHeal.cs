using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstantHeal : Ability
{
    [SerializeField] float heal_percent_cap = .3f;
    private Health health;
    private Image healthGhostImg;

    InstantHeal()
    {
        abilityType = AbilityType.Regeneration;
    }

    void OnEnable()
    {
        health = GameObject.Find("Player").GetComponentInChildren<Health>();
        healthGhostImg = GameObject.Find("HealthbarGhost_p").GetComponent<Image>();
        healthGhostImg.enabled = true;
    }

    private void OnDisable()
    {
        if (healthGhostImg)
        {
            healthGhostImg.enabled = false;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        float diff = healthGhostImg.fillAmount - (health.ReturnCurrentHealth() / health.ReturnMaxHealth());
        if (diff > 0)
        {
            if(diff < (heal_percent_cap * AbilityEffectMult()))
            {
                healthGhostImg.fillAmount -= Time.fixedDeltaTime / 32;
            }
            else
            {
                healthGhostImg.fillAmount += (heal_percent_cap - diff);
            }
        }
        else
        {
            healthGhostImg.fillAmount -= diff;
        }
    }

    protected override void Attack()
    {
        float healAmount = (healthGhostImg.fillAmount * health.ReturnMaxHealth()) - health.ReturnCurrentHealth();
        Debug.Log(healAmount);
        health.heal(healAmount);
    }
}
