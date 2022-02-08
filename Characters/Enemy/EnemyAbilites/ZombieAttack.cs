using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ZombieAttack : EnemyAbility
{
    [SerializeField] float damagePerSwing = 10f;
    [SerializeField] float eject_range = 5f;
    [SerializeField] Transform thisHitbox = null;
    private List<Transform> TargetsInMeleeRange = new List<Transform>();


    protected override void Start()
    {
        base.Start();
        Assert.IsNotNull(thisHitbox);
    }

    public override void CastMechanicsForce()
    {
        for (int i = TargetsInMeleeRange.Count - 1; i >= 0; --i)
        {
            if (TargetsInMeleeRange[i].tag == "DeadEnemy")
            {
                TargetsInMeleeRange.RemoveAt(i);
            }
            else
            {
                TargetsInMeleeRange[i].GetComponent<Health>().take_damage(damagePerSwing, DamageSource.VigorBased);
            }
        }
    }

    public override bool CheckCast()
    {
        return TargetsInMeleeRange.Count > 0;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (FL.ReturnIsEnemy(ETM.Return_FactionEnum(), FactionsEnum.Player, ETM.Return_customReputation()))
            {
                TargetsInMeleeRange.Add(other.transform);
            }
        }
        else if (other.tag == "BasicEnemy")
        {
            if (FL.ReturnIsEnemy(ETM.Return_FactionEnum(), other.gameObject.GetComponentInParent<EnemyTemplateMaster>().Return_FactionEnum(), ETM.Return_customReputation()))
            {
                TargetsInMeleeRange.Add(other.transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" || (other.tag == "BasicEnemy" && other.transform != thisHitbox))
        {
            if (TargetsInMeleeRange.Contains(other.transform))
            {
                TargetsInMeleeRange.Remove(other.transform);
            }
        }
    }

    private int Out_of_range_iter = 0;
    private void FixedUpdate() //Remove enemies that are out of range, but didn't trigger the exit collider
    {
        if(TargetsInMeleeRange.Count > 0)
        {
            Out_of_range_iter %= TargetsInMeleeRange.Count;
            if((TargetsInMeleeRange[Out_of_range_iter].position - transform.position).magnitude > eject_range)
            {
                TargetsInMeleeRange.Remove(TargetsInMeleeRange[Out_of_range_iter]);
            }
            Out_of_range_iter += 1;
        }
    }
}
