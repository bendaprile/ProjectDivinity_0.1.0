using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameCannon : SemiAutoRanged
{
    public override void InitializeValues(ItemQuality ItemClass_in)
    {
        base.InitializeValues(ItemClass_in);
        damage = Mathf.RoundToInt(damage * ItemScaleNum[(int)ItemClass_in]);
    }

    protected override float EnemyAttackLogic()
    {
        float WeaponCD;
        if (WeaponExpertise == HumanoidWeaponExpertise.Novice) //1 round burst
        {
            WeaponCD = PrimaryTimeToAttack * 8;
        }
        else if (WeaponExpertise == HumanoidWeaponExpertise.Adept) //1 round burst
        {
            WeaponCD = PrimaryTimeToAttack * 4;
        }
        else //2 round burst
        {
            if (EnemyAttack_Count < 1)
            {
                EnemyAttack_Count += 1;
                WeaponCD = PrimaryTimeToAttack;
            }
            else
            {
                EnemyAttack_Count = 0;
                WeaponCD = PrimaryTimeToAttack * 4;
            }
        }
        return WeaponCD;
    }
}
