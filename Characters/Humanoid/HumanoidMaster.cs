using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class HumanoidMaster : MonoBehaviour
{
    [SerializeField] private bool fakeNPC = false;
    [SerializeField] private HumanoidCombatClass CombatClass = HumanoidCombatClass.Classless;
    [SerializeField] private NPC_Control_Mode Control_Mode = NPC_Control_Mode.NPC_control; //If a non enemyControl is attacked, this will not change

    [SerializeField] public NPC npc = null; //This is used as a reference for other scripts

    //private NPC_Control_Mode Prev_Frame_Control_Mode = NPC_Control_Mode.NPC_control;

    private Transform selfTrans;
    private HumanoidEnemy HE;
    private EnemyWeaponController EWC;
    private EnemyAnimationUpdater EAU;
    private EnemyArmor EA;
    private InteractiveDia ID;
    private EnemyHealth EH;
    private DiaRoot DR;

    private bool AnimationOverride = false;
    private Vector3 StayLoc = new Vector3();

    public bool MovementLocked = false;

    //////////////////Key Stats
    HumanoidMovementType MovementType = HumanoidMovementType.Average;
    private HumanoidWeaponExpertise WeaponExpertise = HumanoidWeaponExpertise.Novice;
    //////////////////Key Stats


    void Awake()
    {
        HE = GetComponent<HumanoidEnemy>();
        EWC = GetComponentInChildren<EnemyWeaponController>();
        EAU = GetComponentInChildren<EnemyAnimationUpdater>();
        EA = GetComponentInChildren<EnemyArmor>();
        ID = GetComponentInChildren<InteractiveDia>();
        EH = GetComponentInChildren<EnemyHealth>();
        DR = GetComponentInChildren<DiaRoot>();
        selfTrans = transform.Find("Hitbox");
    }

    private void Start()
    {
        if (!fakeNPC)
        {
            ClassBasedStats();
        }
        else
        {
            Assert.IsFalse(EWC);
            Assert.IsFalse(EA);
            Assert.IsFalse(EH);
        }

        StartCoroutine(Delayed());
    }

    IEnumerator Delayed()
    {
        yield return new WaitForEndOfFrame();
        Set_ControlMode(Control_Mode);
    }

    public void SetupHumanoidStats(HumanoidCombatClass CombatClass_in, NPC_Control_Mode npc_mode)
    {
        Control_Mode = npc_mode;
        CombatClass = CombatClass_in;
        ClassBasedStats();
    }

    private void FixedUpdate()
    {
        Assert.IsFalse(Control_Mode == NPC_Control_Mode.Enemy_control);
        if(Return_Control_Mode() == NPC_Control_Mode.WalktoPlayer_dia)
        {
            MoveToDest(HE.player.transform.position, true, 6f);

            float distance = Vector3.Distance(HE.player.transform.position, transform.position);
            if(distance < 5)
            {
                ID.ForcedActivate();
                Set_ControlMode(NPC_Control_Mode.NPC_control);
            }
        }
        else if (Return_Control_Mode() == NPC_Control_Mode.Stay)
        {
            MoveToDest(StayLoc, false, 6f);
        }
        else if(Return_Control_Mode() == NPC_Control_Mode.NPC_control)
        {
            
        }
    }

    public void HumanoidSwitchFaction(FactionsEnum FL_in)
    {
        HE.SwitchFaction(FL_in, true);
        ClassBasedStats();
    }


    public void Set_ControlMode(NPC_Control_Mode mode)
    {
        Control_Mode = mode;
        if(mode == NPC_Control_Mode.Stay) //Do not put in control mode because this should happen only when being set
        {
            StayLoc = transform.position;
        }
        Control_Mode_Change_Logic();
    }

    public void SetupHumanoidItems(GameObject Weapon, (GameObject, GameObject, GameObject) Armor)
    {
        EWC.UpdateWeaponSlot(Weapon);
        EA.AttachArmor(Armor.Item1, Armor.Item2, Armor.Item3);
    }

    public void Start_HumanoidAttackMode()
    {
        AnimationOverride = false;
        ID.SetCombat(true);
        EAU.Set_is2hRanged(EWC.ReturnWeapon());
    }

    public void Attempt_CombatModeRevert()
    {
        EAU.Set_is2hRanged(EWC.ReturnWeapon());
        ID.SetCombat(false);
        Control_Mode_Change_Logic();
    }

    private void Control_Mode_Change_Logic() //TODO and other logic for other modes if needed
    {
        npc.RandomTask();
    }

    public void MoveToDest(Vector3 dest, bool HighPriority, float speed)
    {
        if (MovementLocked)
        {
            return;
        }
        HE.ExternalMovement(dest, HighPriority, speed);
    }

    public void ExternalAnimation(bool start = true, string anim = "")
    {
        AnimationOverride = start;
        if (start)
        {
            EAU.PlayAnimation(anim);
        }
    }

    public void SetMovementLocked(bool set)
    {
        if (set)
        {
            MoveToDest(transform.position, true, 3f); //Stop walking
        }
        MovementLocked = set;
    }

    public void DisplayCallforHelp()
    {
        ID.CombatText(npc.ReturnCallforHelp());
    }


    ////////////////////////////////////////
    public NPC_Control_Mode Return_Control_Mode()
    {
        if (HE.Return_AIenabled())
        {
            return NPC_Control_Mode.Enemy_control;
        }
        return Control_Mode;
    }

    public EnemyAnimationUpdater Return_Animation_Updater()
    {
        return EAU;
    }

    public bool return_AnimationOverride()
    {
        return AnimationOverride;
    }

    public Transform Return_selfTrans()
    {
        return selfTrans;
    }

    public HumanoidWeaponExpertise Return_WeaponExpertise()
    {
        return WeaponExpertise;
    }

    public HumanoidCombatClass Return_CombatClass()
    {
        return CombatClass;
    }

    public HumanoidMovementType Return_MovementType()
    {
        return MovementType;
    }

    public string Return_Name()
    {
        return DR.ReturnNPC_name();
    }
    public void ExtraDeathLogic()
    {
        DR.DiaRoot_deathLogic();
    }
    ////////////////////////////////////////

    private void ClassBasedStats()
    {
        int max_hp;
        int xp_val;
        HumanoidWeaponExpertise HWE_temp;
        string name = STARTUP_DECLARATIONS.FactionsEnumReverse[(int)HE.Return_FactionEnum()];

        switch (CombatClass)
        {
            case HumanoidCombatClass.Antagonist:
                HWE_temp = HumanoidWeaponExpertise.Commando;
                MovementType = HumanoidMovementType.inhuman;
                xp_val = 2000;
                max_hp = 600;
                break;
            case HumanoidCombatClass.Sharpshooter:
                name += " Sharpshooter";
                HWE_temp = HumanoidWeaponExpertise.Commando;
                MovementType = HumanoidMovementType.Average;
                xp_val = 300;
                max_hp = 150;
                break;
            case HumanoidCombatClass.Generalist:
                name += " Generalist";
                HWE_temp = HumanoidWeaponExpertise.Adept;
                MovementType = HumanoidMovementType.Agile;
                xp_val = 300;
                max_hp = 200;
                break;
            case HumanoidCombatClass.Tank:
                name += " Tank";
                HWE_temp = HumanoidWeaponExpertise.Novice;
                MovementType = HumanoidMovementType.Hindered;
                xp_val = 300;
                max_hp = 300;
                break;
            default:
                name += " Recruit";
                HWE_temp = HumanoidWeaponExpertise.Novice;
                MovementType = HumanoidMovementType.Average;
                xp_val = 100;
                max_hp = 150;
                break;
        }

        HE.Set_exp_reward(xp_val);
        EH.modify_maxHealth(max_hp);
        DR.SetName(name);
        WeaponExpertise = HWE_temp;
    }
}
