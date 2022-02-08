using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Weapon : ItemMaster
{
    protected WeaponType weaponType;
    public DamageType DT = DamageType.Regular;
    [SerializeField] protected DamageSource DS = DamageSource.FinesseBased;

    [SerializeField] public Vector3 StartingLocation = new Vector3();
    [SerializeField] public Vector3 StartingRotation = new Vector3();
    [SerializeField] public Vector3 StartingScale = new Vector3();

    [SerializeField] protected float PrimaryTimeToAttack = 1f;
    protected float PTTA_timer = 0f;
    protected bool CanAttack = true;
    protected bool attack_buffered = false;

    protected Transform player;
    protected PlayerAnimationUpdater animationUpdater;
    protected EnemyAnimationUpdater enemyAnimationUpdater;
    protected EnemyTemplateMaster ETM;
    protected FactionLogic FL;
    protected PlayerMovement playMove;
    protected Energy energy;
    protected PlayerStats stats;

    protected EnemyWeaponController enemyWeaponController;
    protected HumanoidWeaponExpertise WeaponExpertise;
    protected int EnemyAttack_Count = 0;
    protected bool EnemyWeapon = false;

    public virtual void StartWeapon(bool EnemyWeapon_in = false, EnemyTemplateMaster ETM_in = null, HumanoidWeaponExpertise humanoidWeaponExpertise = HumanoidWeaponExpertise.Adept)
    {
        EnemyWeapon = EnemyWeapon_in;
        player = GameObject.Find("Player").transform;

        if (EnemyWeapon_in)
        {
            FL = GameObject.Find("NPCs").GetComponent<FactionLogic>();
            ETM = ETM_in;
            WeaponExpertise = humanoidWeaponExpertise;
            enemyAnimationUpdater = GetComponentInParent<EnemyAnimationUpdater>();
            enemyWeaponController = GetComponentInParent<EnemyWeaponController>();
        }
        else
        {
            playMove = player.GetComponentInChildren<PlayerMovement>();
            animationUpdater = player.GetComponentInChildren<PlayerAnimationUpdater>();
            energy = player.GetComponent<Energy>();
            stats = player.GetComponent<PlayerStats>();
        }
    }

    public Weapon()
    {
        ItemType = ItemTypeEnum.Weapon;
    }

    public WeaponType ReturnWeaponType()
    {
        return weaponType;
    }


    public virtual bool EnemyAttack()
    {
        return false;
    }

    protected virtual float EnemyAttackLogic()
    {
        Assert.IsTrue(false); //Overide this
        return PrimaryTimeToAttack;
    }

    public virtual void Attack()
    {
    }

    public virtual void EndSustainedAttack(bool disableCalled = false)
    {
    }

    public virtual void Attack2() 
    {
    }

    public virtual void EndSustainedAttack2(bool disableCalled = false)
    {
    }

    protected void Attack_helper(float CustomCD = -1)
    {
        CanAttack = false;
        if (EnemyWeapon)
        {
            PTTA_timer = EnemyAttackLogic();
        }
        else if(CustomCD != -1)
        {
            PTTA_timer = CustomCD;
        }
        else
        {
            PTTA_timer = PrimaryTimeToAttack;
        }
    }

    public virtual void AnimationTriggerAttack()
    {
    }

    protected override void AdvStatsHelper(List<(string, string)> tempList)
    {
        base.AdvStatsHelper(tempList);
        tempList.Add(("Weapon Type:", STARTUP_DECLARATIONS.WeaponTypeEnumReverse[(int)weaponType]));
        tempList.Add(("Damage Type:", STARTUP_DECLARATIONS.DamageTypeEnumReverse[(int)DT]));
        tempList.Add(("Damage Source:", STARTUP_DECLARATIONS.DamageSourceEnumReverse[(int)DS]));
    }


    protected virtual void Update()
    {
        if(!CanAttack)
        {
            if (PTTA_timer <= 0)
            {
                CanAttack = true;
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
