using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeContact : MonoBehaviour
{
    List<Collider> BoxTriggerList;
    List<Collider> SphereTriggerList;

    [SerializeField] private LightningMelee lightMelee;


    private void Start()
    {
        BoxTriggerList = transform.Find("Box").GetComponent<ColliderChild>().TriggerList;
        SphereTriggerList = transform.Find("Sphere").GetComponent<ColliderChild>().TriggerList;
    }

    public void MeleeContactFunc(DamageSource DS, bool AOEsmash, float forceMult, float damage)
    {
        if (AOEsmash)
        {
            MeleeContactHelper(SphereTriggerList, DS, forceMult, damage);
        }
        else
        {
            MeleeContactHelper(BoxTriggerList, DS, forceMult, damage);
        }
    }

    public void MeleeContactHelper(List<Collider> TriggerList, DamageSource DS, float forceMult, float damage)
    {
        int potential_enemies_in_range = TriggerList.Count; //Includes recently dead enemies
        for (int i = potential_enemies_in_range - 1; i >= 0; i--)
        {
            Collider col = TriggerList[i];
            if (!col || col.tag != "BasicEnemy")
            {
                TriggerList.Remove(col);
            }
            else
            {
                Vector3 directionToTarget = col.transform.position - transform.position;
                directionToTarget.y = 0;
                directionToTarget.Normalize();
                Vector3 force = new Vector3(forceMult * directionToTarget.x, 0, forceMult * directionToTarget.z);
                lightMelee.MeleeAttack(col);
                col.GetComponent<Health>().take_damage(damage, DS, true, knockback: true, force: force);
            }
        }
    }
}
