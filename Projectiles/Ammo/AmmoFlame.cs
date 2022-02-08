using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoFlame : AmmoMaster //AOE
{
    [SerializeField] private GameObject Exp;

    protected override void OnDisable()
    {
        GameObject temp = Instantiate(Exp, transform.parent);
        temp.transform.position = transform.position;
        temp.GetComponent<ExplosionLogic>().Setup(damage, DS, dt);
        if (!Player_Fired)
        {
            temp.GetComponent<ExplosionLogic>().AdditionalNPCSetup(FacEnum, FL, CustomRep);
        }
        Destroy(temp, 5f);
    }

    protected override void DealDamage(Collider other, bool PlayerCast) //Don't Deal Damage here (Could hit a wall and we still want the AoE)
    {
        disabled = true;
        Destroy(gameObject);
    }
}
