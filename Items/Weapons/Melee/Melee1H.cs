using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee1H : Weapon
{
    [SerializeField] public float energyCost = 20f;
    [SerializeField] public float heavyEnergyCost = 40f;
    [SerializeField] public float comboTimeout = 1f;

    [SerializeField] private float basicAttackForce = 400f;
    [SerializeField] private float heavyAttackForce = 800f;

    [SerializeField] public float basicAttackDamage = 30f;
    [SerializeField] public float heavyAttackDamage = 60f;

    MeleeContact MC;

    [SerializeField] List<MeleeAnimation> animNames = new List<MeleeAnimation>(new MeleeAnimation[] {
        MeleeAnimation.StandingAttackDownward,
        MeleeAnimation.StandingAttack360High});

    [SerializeField] MeleeAnimation heavyAnim = MeleeAnimation.JumpAttack;

    private bool LightCleanedUp = true;
    private bool isHeavy;
    private int attackAnimIndex = 0;
    private float comboTimer = 10f;


    public override void InitializeValues(ItemQuality ItemClass_in)
    {
        base.InitializeValues(ItemClass_in);
        basicAttackDamage = Mathf.Round(ItemScaleNum[(int)ItemClass_in] * basicAttackDamage);
        heavyAttackDamage = Mathf.Round(ItemScaleNum[(int)ItemClass_in] * heavyAttackDamage);
    }

    public override void StartWeapon(bool EnemyWeapon_in = false, EnemyTemplateMaster ETM_in = null, HumanoidWeaponExpertise humanoidWeaponExpertise = HumanoidWeaponExpertise.Adept)
    {
        base.StartWeapon(EnemyWeapon_in, ETM_in, humanoidWeaponExpertise);
        if (!EnemyWeapon_in)
        {
            MC = player.GetComponentInChildren<MeleeContact>();
            energy = player.GetComponentInChildren<Energy>();
        }
    }

    public Melee1H()
    {
        weaponType = WeaponType.Melee_1H;
    }

    private void FixedUpdate()
    {
        ComboTimer();
    }

    void ComboTimer()
    {
        if (CanAttack)
        {
            comboTimer += Time.fixedDeltaTime;
        }

        if (comboTimer > comboTimeout)
        {
            attackAnimIndex = 0;
        }
    }


    protected override float EnemyAttackLogic()
    {
        float WeaponCD;
        if (WeaponExpertise == HumanoidWeaponExpertise.Novice)
            WeaponCD = PrimaryTimeToAttack * 4;
        else if (WeaponExpertise == HumanoidWeaponExpertise.Adept)
            WeaponCD = PrimaryTimeToAttack * 2;
        else
            WeaponCD = PrimaryTimeToAttack;

        return WeaponCD;
    }


    public override bool EnemyAttack()
    {
        if (CanAttack)
        {
            Attack_helper();
            isHeavy = false;
            CanAttack = false;
            enemyAnimationUpdater.MeleeAttack(animNames[attackAnimIndex]);
            return true;
        }
        return false;
    }



    /**
     * Method is called from WeaponController and handles a light attack
     * 
     * @return Whether or not the attack succeeded
     */
    public override void Attack()
    {
        if (CanAttack)
        {
            attack_buffered = false;
            if(energy.Drain_ES(true, energyCost))
            {
                LightCleanedUp = false;
                Attack_helper();
                isHeavy = false;
                CanAttack = false;
                playMove.MeleeAttack(true, basicAttackForce);
                animationUpdater.MeleeAttack(animNames[attackAnimIndex]);
            }
        }
        else if(PTTA_timer < .2f)
        {
            attack_buffered = true;
        }
    }

    /**
     * Method is called from WeaponController and handles a heavy attack
     * 
     * @return Whether or not the attack succeeded
     */
    public override void Attack2()
    {
        if (CanAttack)
        {
            if(energy.Drain_ES(true, heavyEnergyCost))
            {
                Attack_helper();
                isHeavy = true;
                CanAttack = false;
                playMove.MeleeAttack(true, heavyAttackForce);
                animationUpdater.MeleeAttack(heavyAnim);
            }
        }
    }

    public override void AnimationTriggerAttack()
    {
        if(EnemyWeapon)
        {
            foreach(Transform child in enemyWeaponController.ReturnTiMR())
            {
                if (child)
                {
                    child.GetComponent<Health>().take_damage(basicAttackDamage, DS);
                }
            }
        }
        else
        {
            if (isHeavy)
            {
                MC.MeleeContactFunc(DS, true, heavyAttackForce, stats.ReturnDamageMult(heavyAttackDamage, DS));
            }
            else
            {
                MC.MeleeContactFunc(DS, false, basicAttackForce, stats.ReturnDamageMult(basicAttackDamage, DS));
            }
        }
    }

    private void MeleeCleanUp()
    {
        if (EnemyWeapon)
        {
            return;
        }
        comboTimer = 0;
        animationUpdater.DisableRootMotion();
        playMove.MeleeAttack(false, 0);

        attackAnimIndex++;
        if (attackAnimIndex >= animNames.Count)
        {
            attackAnimIndex = 0;
        }
    }

    private void OnDisable()
    {
        MeleeCleanUp();
    }

    protected override void AdvStatsHelper(List<(string, string)> tempList)
    {
        base.AdvStatsHelper(tempList);
        tempList.Add(("Basic Energy Cost:", energyCost.ToString()));
        tempList.Add(("Basic Damage:", basicAttackDamage.ToString()));
        tempList.Add(("Heavy Energy Cost:", heavyEnergyCost.ToString()));
        tempList.Add(("Heavy Damage:", heavyAttackDamage.ToString()));
    }

    protected override void Update()
    {
        if (!CanAttack)
        {
            if(!isHeavy && !LightCleanedUp && PTTA_timer < PrimaryTimeToAttack / 2) //Reset early for normals (can be called multiple times)
            {
                LightCleanedUp = true;
                MeleeCleanUp();
            }

            if (PTTA_timer <= 0)
            {
                CanAttack = true;
                if (isHeavy)
                {
                    MeleeCleanUp();
                }

                if (attack_buffered)
                {
                    Attack();
                }
            }
            else
            {
                PTTA_timer -= Time.deltaTime;
            }
        }
    }
}
