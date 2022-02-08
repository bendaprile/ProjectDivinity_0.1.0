using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShieldCast : Ability
{
    [SerializeField] private GameObject ProjectileShield = null;

    [SerializeField] private float health = 100;
    [SerializeField] private float max_duration = 10f;

    private float distance = 5f;

    ProjectileShieldCast()
    {
        abilityType = AbilityType.Spacetime;
    }

    protected override float AbilityEffectMult() //TODO
    {
        float Mult = base.AbilityEffectMult();
        return Mult;
    }

    protected override void Attack()
    {
        Vector3 diffVector = cursorLogic.ReturnPlayer2Cursor();
        float angle = Mathf.Atan2(diffVector.x, diffVector.z);
        Vector3 anglesDeg = new Vector3(0f, angle * Mathf.Rad2Deg, 0f);

        Vector3 mod_transform = new Vector3(transform.position.x + (distance * Mathf.Sin(angle)), transform.position.y, transform.position.z + (distance * Mathf.Cos(angle)));

        GameObject clone = Instantiate(ProjectileShield, mod_transform, Quaternion.Euler(anglesDeg), PlayerProjectiles).gameObject;
        ProjectileShield PS_Script = clone.GetComponentInChildren<ProjectileShield>();

        PS_Script.Setup(max_duration, (int)(health * AbilityEffectMult()), true, activePerks.ProjShieldReflection());
    }
}
