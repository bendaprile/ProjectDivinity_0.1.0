using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Restoring : Ability
{
    [SerializeField] public bool instant = true;
    [SerializeField] public float duration = 0f;
    [SerializeField] public float health_restore_amount = 0f;
    [SerializeField] public float energy_restore_amount = 0f; //TODO Do later

    private Health playerHealth;

    private float durationLeft;
    private float healPerTick;
    private float energyPerTick;

    void Start()
    {
        Assert.IsFalse(ability_sprite); //Use item sprite
        playerHealth = GameObject.Find("Player").GetComponentInChildren<Health>();
        durationLeft = 0f;
        healPerTick = health_restore_amount / duration;
        energyPerTick = energy_restore_amount / duration;
    }

    protected override void Attack()
    {
        if (instant)
        {
            playerHealth.heal(health_restore_amount);
        }
        else
        {
            durationLeft = duration;
        }
    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if(durationLeft > 0f)
        {
            playerHealth.heal(healPerTick * Time.deltaTime);
            durationLeft -= Time.deltaTime;
        }
    }
}
