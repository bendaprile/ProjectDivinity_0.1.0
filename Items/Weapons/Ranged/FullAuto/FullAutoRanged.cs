using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullAutoRanged : Ranged //DOES NOT USE ANIMATION TRIGGER //TODO might want to add delay before projectile
{
    private bool currently_firing = false;

    public override void Attack()
    {
        currently_firing = true;
    }

    public override void EndSustainedAttack(bool disableCalled = false)
    {
        if (!disableCalled)
        {
            playMove.CancelFollowCursorSustained();
        }
        currently_firing = false;
    }

    public override bool EnemyAttack() //Doesn't use ammo
    {
        if (CanAttack)
        {
            EnemyCustomAttack();
            return true;
        }
        return false;
    }

    protected override void Update()
    {
        if (!CanAttack)
        {
            if (PTTA_timer <= 0)
            {
                CanAttack = true;
            }
            else
            {
                PTTA_timer -= Time.deltaTime;
            }
        }

        if (!EnemyWeapon)
        {
            if(currently_firing && CanAttack)
            {
                PlayerCustomAttack();
            }
        }
    }

    protected virtual void PlayerCustomAttack()
    {
        if (CheckAmmo())
        {
            if (energy.Drain_ES(true, energyCost)) //Like this because this uses energy
            {
                Attack_helper();
                playMove.SetFollowCursorSustained(1f);
                animationUpdater.RangedAttack(rangedAnimation);
                fireProjectile(true, CustomAngle()).GetComponentInChildren<AmmoMaster>().Setup(ReturnFinalDamage(damage), DS, dt_in: DT);
            }
        }
    }

    protected virtual void EnemyCustomAttack()
    {
        Attack_helper();
        enemyAnimationUpdater.RangedAttack(rangedAnimation);
        AmmoMaster AM_temp = fireProjectile(true, CustomAngle()).GetComponentInChildren<AmmoMaster>();
        AM_temp.Setup(ReturnFinalDamage(damage), DS, dt_in: DT);
        AM_temp.AdditionalNPCSetup(ETM.Return_FactionEnum(), FL, ETM.Return_customReputation());
    }
}
