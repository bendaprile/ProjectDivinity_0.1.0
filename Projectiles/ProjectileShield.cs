using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShield : MonoBehaviour
{
    private Health health;
    private GameObject hitbox;
    private bool reflection;
    private bool playerCreated;

    public void Setup(float duration, int hp, bool playerCreated_in, bool reflection_in)
    {
        reflection = reflection_in;
        playerCreated = playerCreated_in;
        health = GetComponentInChildren<Health>();
        hitbox = health.gameObject;
        health.modify_maxHealth(hp);

        if (!reflection)
        {
            if (playerCreated)
            {
                hitbox.tag = "Player";
                hitbox.layer = LayerMask.NameToLayer("Player");
            }
            else
            {
                hitbox.tag = "BasicEnemy";
                hitbox.layer = LayerMask.NameToLayer("BasicEnemy");
            }
        }


        Destroy(gameObject, duration);
    }

    // Update is called once per frame
    void Update()
    {
        float current_hp = health.ReturnCurrentHealth();

        if(current_hp <= 0)
        {
            Destroy(transform.parent.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (reflection)
        {
            AmmoMaster ammo = other.GetComponent<AmmoMaster>();
            if (ammo)
            {
                if (playerCreated && !ammo.Player_Fired) 
                {
                    health.take_damage(ammo.returnDamageStats().Item1, ammo.returnDamageStats().Item2, DT: ammo.returnDamageStats().Item3);
                    ammo.KineticReversalHelper();
                }
            }
        }
    }
}
