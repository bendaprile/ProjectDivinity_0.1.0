using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : SemiAutoRanged
{
    public override void InitializeValues(ItemQuality ItemClass_in)
    {
        base.InitializeValues(ItemClass_in);
        damage = Mathf.RoundToInt(damage * ItemScaleNum[(int)ItemClass_in]);
    }

    protected override float EnemyAttackLogic()
    {
        float WeaponCD;
        if (WeaponExpertise == HumanoidWeaponExpertise.Novice) //2 round burst
        {
            if (EnemyAttack_Count < 1)
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
        else if (WeaponExpertise == HumanoidWeaponExpertise.Adept) //3 round burst
        {
            if (EnemyAttack_Count < 2)
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
        else //5 round burst
        {
            if (EnemyAttack_Count < 4)
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

    /*
    public Pistol() //Base Pistol stats
    {
        Cost = 100;
        Weight = 3;
        ItemName = "Pistol";
        ItemQuality = ItemQuality.Common;
        DT = DamageType.Regular;
        StartingLocation = new Vector3(-0.093f, 0.033f, -0.012f);
        StartingRotation = new Vector3(-0.093f, -98.131f, -5.011f);
        StartingScale = new Vector3(1f, 1f, 1f);
        rangedType = RangedType.Ranged_1H;
        ammoCapacity = 10;
        damage = 20;
        energyCost = 5;
        reloadEnergyCost = 60;
        projectileVelocity = 50;
    }
    */
}
