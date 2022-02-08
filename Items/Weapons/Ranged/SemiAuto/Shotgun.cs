using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : SemiAutoRanged
{
    [SerializeField] private int ProjectileCount = 4;
    [SerializeField] private float fanAngle = 0.35f; //Both Sides Rads

    public override void InitializeValues(ItemQuality ItemClass_in)
    {
        base.InitializeValues(ItemClass_in);
        damage = Mathf.RoundToInt(damage * (1 + ItemScaleNum[(int)ItemClass_in]) / 2); //Reduced damage scaling
        ProjectileCount = Mathf.RoundToInt(ItemScaleNum[(int)ItemClass_in] * ProjectileCount);
    }

    public override void AnimationTriggerAttack()
    {
        float angle = CustomAngle();

        float fanMod = -fanAngle / 2;
        for (int i = 0; i < ProjectileCount; ++i)
        {
            float projectileAngle = angle + fanMod;

            if (EnemyWeapon)
            {
                if (i == 0)
                {
                    AmmoMaster AM_temp = fireProjectile(true, projectileAngle).GetComponentInChildren<AmmoMaster>();
                    AM_temp.Setup(ReturnFinalDamage(damage), DS, dt_in: DT);
                    AM_temp.AdditionalNPCSetup(ETM.Return_FactionEnum(), FL, ETM.Return_customReputation());
                }
                else
                {
                    AmmoMaster AM_temp = fireProjectile(false, projectileAngle).GetComponentInChildren<AmmoMaster>();
                    AM_temp.Setup(ReturnFinalDamage(damage), DS, dt_in: DT);
                    AM_temp.AdditionalNPCSetup(ETM.Return_FactionEnum(), FL, ETM.Return_customReputation());
                }
            }
            else
            {
                if (i == 0)
                {
                    fireProjectile(true, projectileAngle, true).GetComponentInChildren<AmmoMaster>().Setup(ReturnFinalDamage(damage), DS, dt_in: DT);
                }
                else
                {
                    fireProjectile(false, projectileAngle, false).GetComponentInChildren<AmmoMaster>().Setup(ReturnFinalDamage(damage), DS, dt_in: DT);
                }
            }

            fanMod += fanAngle / ProjectileCount;
        }
    }

    protected override float EnemyAttackLogic()
    {
        float WeaponCD;
        if (WeaponExpertise == HumanoidWeaponExpertise.Novice) //1 round
        {
            WeaponCD = PrimaryTimeToAttack * 8;
        }
        else if (WeaponExpertise == HumanoidWeaponExpertise.Adept) //faster
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

    protected override void AdvStatsHelper(List<(string, string)> tempList)
    {
        base.AdvStatsHelper(tempList);
        for(int i = 0; i < tempList.Count; ++i)
        {
            if(tempList[i].Item1 == "Damage:")
            {
                tempList[i] = (tempList[i].Item1, tempList[i].Item2 + " x " + ProjectileCount.ToString());
            }
        }
    }
}
