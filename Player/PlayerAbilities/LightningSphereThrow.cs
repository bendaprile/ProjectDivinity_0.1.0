using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningSphereThrow : Ability
{
    public GameObject lightningSphere;


    [SerializeField] private float speed = 0f;
    [SerializeField] private float max_duration = 0f;
    [SerializeField] private int max_targets = 0;
    [SerializeField] private float dps = 0f;

    //[SerializeField] private bool turret_mode = false;
    //[SerializeField] private bool bounce_mode = false;

    LightningSphereThrow()
    {
        abilityType = AbilityType.Lightning;
    }

    protected override void Attack()
    {
        Vector3 mod_transform = new Vector3 (transform.position.x, transform.position.y + .5f, transform.position.z);
        GameObject clone = Instantiate(lightningSphere, mod_transform, transform.rotation, PlayerProjectiles).gameObject;
        Rigidbody RB = clone.GetComponent<Rigidbody>();
        LightningSphereProjectile LSP_Script = clone.GetComponentInChildren<LightningSphereProjectile>();

        RB.velocity = transform.TransformDirection(Vector3.forward * speed);


        float turret_mode = activePerks.LightningSphereCast();

        if (turret_mode > -1)
        {
            LSP_Script.GenericSetup(0f, dps * AbilityEffectMult(), max_targets, turret_mode, 10f);
            RB.drag = speed / 15f;
        }
        else
        {
            LSP_Script.GenericSetup(0f, dps * AbilityEffectMult(), max_targets, 8f, 10f);
        }
    }
}
