using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCastingEnemy : EnemyAbility
{
    [SerializeField] private GameObject ProjectileShield = null;

    [SerializeField] private int health = 30;
    [SerializeField] private float max_duration = 10f;
    [SerializeField] private float cast_distance = 5f;
    [SerializeField] private float placement_y_bias = -1f; //should not move the transform because that affect LoS
    [SerializeField] private float CastAmount = 1; //should not move the transform because that affect LoS

    public override bool CastMechanics()
    {
        if (!clean_LoS())
        {
            return false;
        }

        Vector3 target_dir = ETM.Return_Current_Target().transform.position - transform.position;
        float angle = Mathf.Atan2(target_dir.x, target_dir.z);
        Vector3 anglesDeg = new Vector3(0f, angle * Mathf.Rad2Deg, 0f);

        for(int i = 1; i < CastAmount + 1; i++)
        {
            Vector3 mod_transform = new Vector3(transform.position.x + (cast_distance * i * Mathf.Sin(angle)), transform.position.y + placement_y_bias, transform.position.z + (cast_distance * i * Mathf.Cos(angle)));
            GameObject clone = Instantiate(ProjectileShield, mod_transform, Quaternion.Euler(anglesDeg), EnemyProjectiles).gameObject;
            ProjectileShield PS_Script = clone.GetComponent<ProjectileShield>();

            PS_Script.Setup(max_duration, health, false, false);
        }
        return true;
    }

}
