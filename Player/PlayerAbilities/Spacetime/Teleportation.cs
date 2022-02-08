using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportation : Ability
{
    [SerializeField] private float Max_tele_distance = 10f;
    Teleportation()
    {
        abilityType = AbilityType.Spacetime;
    }

    protected override float AbilityEffectMult() //TODO
    {
        float Mult = base.AbilityEffectMult();
        return Mult;
    }

    protected override bool CustomBreakFunc()
    {
        if (activePerks.InstantTele())
        {
            Attack();
            return true;
        }
        return false;
    }

    protected override void Attack()
    {
        Vector3 curVec = cursorLogic.ReturnPlayer2Cursor();
        curVec = new Vector3(curVec.x, curVec.y + 1.4f, curVec.z); //Player's body offset +.1f
        Vector3 curVec_norm = curVec.normalized;

        float f_max_dist = Max_tele_distance * AbilityEffectMult();

        if (curVec.magnitude > f_max_dist)
        {
            curVec = curVec_norm * f_max_dist;
        }

        int layerMask = LayerMask.GetMask("Terrain", "Obstacles");

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, curVec, out hit, curVec.magnitude, layerMask))
        {
            Player.position += curVec_norm * hit.distance;
        }
        else
        {
            Player.position += curVec;
        }
    }
}
