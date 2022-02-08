using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Health
{
    [SerializeField] Image[] HitMarkers = new Image[4];

    private float current_marker_value = 0;

    public override void take_damage(float damage, DamageSource DS, bool PlayerIsSource = false, bool knockback = false, Vector3 force = new Vector3(), float stun_duration = 0f, DamageType DT = DamageType.Regular, bool isDoT = false)
    {
        base.take_damage(damage, DS, PlayerIsSource, knockback, force, stun_duration, DT, isDoT);
        current_marker_value += 4 * (ActualDamageDealt / maxHealth);

        if(health <= 0)
        {
            GetComponentInParent<PlayerMaster>().PlayerDeath(true);
        }
    }

    public void OnDeathFunc()
    {
        health = maxHealth;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if(current_marker_value > 1) //Over
        {
            current_marker_value = 1;
        }
        if (current_marker_value > 0.1f) //Exp
        {
            current_marker_value *= .95f;
        }
        else if (current_marker_value > 0) //linear
        {
            current_marker_value -= 0.1f * Time.deltaTime;
        }

        foreach (Image im in HitMarkers)
        {
            im.color = new Color(1, 0, 0, current_marker_value);
        }
    }
}
