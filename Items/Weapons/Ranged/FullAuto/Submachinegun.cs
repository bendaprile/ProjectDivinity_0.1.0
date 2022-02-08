using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Submachinegun : FullAutoRanged
{
    [SerializeField] private float SpreadAngle_half = Mathf.PI / 6;
    public override void InitializeValues(ItemQuality ItemClass_in)
    {
        base.InitializeValues(ItemClass_in);
        PrimaryTimeToAttack = PrimaryTimeToAttack / ItemScaleNum[(int)ItemClass_in];
    }

    protected override float EnemyAttackLogic()
    {
        float WeaponCD;
        if (WeaponExpertise == HumanoidWeaponExpertise.Novice) //10 round burst
        {
            if (EnemyAttack_Count < 9)
            {
                EnemyAttack_Count += 1;
                WeaponCD = PrimaryTimeToAttack * 2;
            }
            else
            {
                EnemyAttack_Count = 0;
                WeaponCD = PrimaryTimeToAttack * 16;
            }
        }
        else if (WeaponExpertise == HumanoidWeaponExpertise.Adept) //18 round burst
        {
            if (EnemyAttack_Count < 17)
            {
                EnemyAttack_Count += 1;
                WeaponCD = PrimaryTimeToAttack * 2;
            }
            else
            {
                EnemyAttack_Count = 0;
                WeaponCD = PrimaryTimeToAttack * 16;
            }
        }
        else //30 round burst
        {
            if (EnemyAttack_Count < 29)
            {
                EnemyAttack_Count += 1;
                WeaponCD = PrimaryTimeToAttack;
            }
            else
            {
                EnemyAttack_Count = 0;
                WeaponCD = PrimaryTimeToAttack * 16;
            }
        }
        return WeaponCD;
    }

    protected override void PlayerCustomAttack()
    {
        if (CheckAmmo())
        {
            if (energy.Drain_ES(true, energyCost)) //Like this because this uses energy
            {
                float spread = Random.Range(-SpreadAngle_half, SpreadAngle_half);
                Attack_helper();
                playMove.SetFollowCursorSustained(1f);
                animationUpdater.RangedAttack(rangedAnimation);
                fireProjectile(true, CustomAngle() + spread).GetComponentInChildren<AmmoMaster>().Setup(ReturnFinalDamage(damage), DS, dt_in: DT);
            }
        }
    }

    protected override void EnemyCustomAttack()
    {
        float spread = Random.Range(-SpreadAngle_half, SpreadAngle_half);
        Attack_helper();
        enemyAnimationUpdater.RangedAttack(rangedAnimation);
        AmmoMaster AM_temp = fireProjectile(true, CustomAngle() + spread).GetComponentInChildren<AmmoMaster>();
        AM_temp.Setup(ReturnFinalDamage(damage), DS, dt_in: DT);
        AM_temp.AdditionalNPCSetup(ETM.Return_FactionEnum(), FL, ETM.Return_customReputation());
    }
}
