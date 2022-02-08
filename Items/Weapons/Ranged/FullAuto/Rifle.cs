using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : FullAutoRanged
{
    public override void InitializeValues(ItemQuality ItemClass_in)
    {
        base.InitializeValues(ItemClass_in);
        damage = Mathf.RoundToInt(damage * ItemScaleNum[(int)ItemClass_in]);
    }

    protected override float EnemyAttackLogic()
    {
        float WeaponCD;
        if (WeaponExpertise == HumanoidWeaponExpertise.Novice) //3 round burst
        {
            if (EnemyAttack_Count < 2)
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
        else if (WeaponExpertise == HumanoidWeaponExpertise.Adept) //5 round burst
        {
            if (EnemyAttack_Count < 4)
            {
                EnemyAttack_Count += 1;
                WeaponCD = PrimaryTimeToAttack * 2;
            }
            else
            {
                EnemyAttack_Count = 0;
                WeaponCD = PrimaryTimeToAttack * 12;
            }
        }
        else //8 round burst
        {
            if (EnemyAttack_Count < 7)
            {
                EnemyAttack_Count += 1;
                WeaponCD = PrimaryTimeToAttack;
            }
            else
            {
                EnemyAttack_Count = 0;
                WeaponCD = PrimaryTimeToAttack * 10;
            }
        }
        return WeaponCD;
    }
}
