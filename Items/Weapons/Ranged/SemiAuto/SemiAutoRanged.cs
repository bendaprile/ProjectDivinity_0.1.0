using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemiAutoRanged : Ranged
{
    public override void Attack()
    {
        if (CanAttack)
        {
            attack_buffered = false;
            if (CheckAmmo())
            {
                if(energy.Drain_ES(true, energyCost))
                {
                    Attack_helper();
                    playMove.SetFollowCursor(1f);
                    animationUpdater.RangedAttack(rangedAnimation);
                }
            }
        }
        else if(PTTA_timer < .2f)
        {
            attack_buffered = true;
        }
    }


    public override bool EnemyAttack() //Doesn't use ammo
    {
        if (CanAttack)
        {
            Attack_helper();
            enemyAnimationUpdater.RangedAttack(rangedAnimation);
            return true;
        }
        return false;
    }


    public override void AnimationTriggerAttack() //Can be overwritten
    {
        if (EnemyWeapon)
        {
            AmmoMaster AM_temp = fireProjectile(true, CustomAngle()).GetComponentInChildren<AmmoMaster>();
            AM_temp.Setup(ReturnFinalDamage(damage), DS, dt_in: DT);
            AM_temp.AdditionalNPCSetup(ETM.Return_FactionEnum(), FL, ETM.Return_customReputation());
        }
        else
        {
            fireProjectile(true, CustomAngle()).GetComponentInChildren<AmmoMaster>().Setup(ReturnFinalDamage(damage), DS, dt_in: DT);
        }
    }
}
