using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigun : Ranged
{
    [SerializeField] private float FireRateRamp = 0.2f;
    [SerializeField] private float FireRateDecay = 0.4f;
    [SerializeField] private float Final_TimeToAttack = 0.2f;

    public override void InitializeValues(ItemQuality ItemClass_in)
    {
        base.InitializeValues(ItemClass_in);
        Final_TimeToAttack = Mathf.RoundToInt(Final_TimeToAttack / ItemScaleNum[(int)ItemClass_in]);
        damage = Mathf.RoundToInt(damage * ItemScaleNum[(int)ItemClass_in]);
    }

    private float TimeToAttackReduction;

    private bool currently_firing = false;

    // Start is called before the first frame update
    public override void StartWeapon(bool EnemyWeapon_in = false, EnemyTemplateMaster ETM_in = null, HumanoidWeaponExpertise humanoidWeaponExpertise = HumanoidWeaponExpertise.Adept)
    {
        base.StartWeapon(EnemyWeapon_in, ETM_in, humanoidWeaponExpertise);
        Debug.Log("THIS IS INCOMPLETE, CONNECT IT TO FULLAUTO");
        CanAttack = true;
        TimeToAttackReduction = 0f;
    }

    public override void Attack()
    {
        currently_firing = true;
    }

    public override void EndSustainedAttack(bool disableCalled = false)
    {
        if(!disableCalled)
        {
            playMove.CancelFollowCursorSustained();
        }
        currently_firing = false;
    }

    protected override void Update()
    {
        if (currently_firing)
        {
            if (PTTA_timer <= TimeToAttackReduction)
            {
                CustomAttack();
            }
            else
            {
                PTTA_timer -= Time.deltaTime;
            }


            if (Final_TimeToAttack < (1 - TimeToAttackReduction))
            {
                TimeToAttackReduction += (FireRateRamp * Time.deltaTime);
            }
            playMove.SetFollowCursorSustained(1f);

        }
        else if (TimeToAttackReduction > 0)
        {
            TimeToAttackReduction -= (FireRateDecay * Time.deltaTime);
        }
    }

    private void CustomAttack()
    {
        if (CheckAmmo())
        {
            if (energy.Drain_ES(true, energyCost)) //Like this because this uses energy
            {
                Attack_helper();
                playMove.SetFollowCursorSustained(1f);
                animationUpdater.RangedAttack(rangedAnimation);
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
    }

    public override void AnimationTriggerAttack()
    {
        Debug.Log("Do something");
    }
}
