using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Plasma : Ranged
{
    [SerializeField] private float MaxChargeTime = 2f;
    [SerializeField] private float UpfrontCharge = 0.2f;

    private bool charging = false;
    private float charge_time = 0;

    public override void InitializeValues(ItemQuality ItemClass_in)
    {
        base.InitializeValues(ItemClass_in);
        damage = Mathf.RoundToInt(damage * ItemScaleNum[(int)ItemClass_in]);
    }

    public override void Attack()
    {
        if (CanAttack && CheckAmmo()) //TODO play some other animation, not shooting, just charging
        {
            playMove.SetFollowCursorSustained(1f, true);
            animationUpdater.RangedAttack(rangedAnimation, true);
            charging = true;
            transform.Find("ChargeUpSound").GetComponent<AudioSource>().Play();
        }
    }

    public override void EndSustainedAttack(bool disableCalled = false)
    {
        if (charging)
        {
            charging = false;
            animationUpdater.ChargeAttackOver();
            CanAttack = false;

            if(!disableCalled)
            {
                playMove.CancelFollowCursorSustained();
            }

            float charge_ratio = UpfrontCharge + ((1 - UpfrontCharge) * charge_time / MaxChargeTime);

            float final_size = (2 * charge_ratio);
            int final_dam = (int)(ReturnFinalDamage(damage * charge_ratio));
            fireProjectile(true, CustomAngle()).GetComponent<AmmoPlasma>().PlasmaSetup(final_dam, final_size, false);
            charge_time = 0;
            transform.Find("ChargeUpSound").GetComponent<AudioSource>().Stop();
        }
    }

    protected override void Update()
    {
        base.Update();
        if (charging)
        {
            Assert.IsFalse(EnemyWeapon);
            if (charge_time < MaxChargeTime)
            {
                charge_time += Time.deltaTime;

                if (!energy.Drain_ES(true, energyCost * Time.deltaTime / MaxChargeTime))
                {
                    EndSustainedAttack();
                }
            }

            transform.Find("ChargeUpSound").GetComponent<AudioSource>().pitch = 2 * charge_time / MaxChargeTime;
        }
    }

    protected override void AdvStatsHelper(List<(string, string)> tempList)
    {
        base.AdvStatsHelper(tempList);
        for (int i = 0; i < tempList.Count; ++i)
        {
            if (tempList[i].Item1 == "Damage:")
            {
                tempList[i] = (tempList[i].Item1, (damage * UpfrontCharge).ToString() + " - " + damage.ToString());
            }
        }
    }
}
