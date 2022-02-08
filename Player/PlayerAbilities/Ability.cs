using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class Ability : MonoBehaviour
{
    [SerializeField] private DamageSource DS = DamageSource.None;
    [SerializeField] private string AbilityName = "No Name Assigned";
    [SerializeField] [TextArea(3,10)] private string Description = "";
    [SerializeField] private bool AbilityAccessible = true;

    [SerializeField] public float cooldown = 0f;
    [SerializeField] public Sprite ability_sprite = null;
    [SerializeField] protected float energyCost = 0f;
    [SerializeField] protected bool FaceCursor = false;
    [SerializeField] AbilityAnimation abilityAnimation = AbilityAnimation.ThrowArmsForward;
    public float cooldown_remaining;

    private int current_level;
    private int current_xp = 0;
    private int needed_xp;


    protected float Cast_at_this_time;
    protected Transform Player;

    protected AbilityType abilityType;
    protected EventQueue eventQueue;
    protected ActivePerks activePerks;
    protected CursorLogic cursorLogic;
    protected PlayerMovement playerMovement;
    protected PlayerAnimationUpdater animationUpdater;
    protected Energy energy;
    protected WeaponController weaponController;
    protected AbilitiesController abilitiesController;
    protected Transform PlayerProjectiles;
    protected PlayerStats playerStats;

    private bool is_setup = false;


    private const int MAX_LEVEL = 5;

    public virtual void AttemptAttack()
    {
        if (cooldown_remaining <= 0f)
        {
            if (energy.Drain_ES(true, energyCost))
            {
                if (FaceCursor)
                {
                    playerMovement.SetFollowCursor(STARTUP_DECLARATIONS.AbilityAnimationCastTime[(int)abilityAnimation]);
                }
                cooldown_remaining = cooldown;

                if (CustomBreakFunc())
                {
                    return;
                }


                Cast_at_this_time = cooldown - STARTUP_DECLARATIONS.AbilityAnimationCastTime[(int)abilityAnimation];

                AbilityAttackAnimation();
                abilitiesController.abilityUsed(true);
            }
        }
    }

    protected virtual bool CustomBreakFunc()
    {
        return false;
    }


    public void add_xp(int xp_in)
    {
        needed_xp = current_level * 2000;
        current_xp += xp_in;

        if (current_level < MAX_LEVEL && current_xp >= needed_xp)
        {
            /////
            EventData tempEvent = new EventData();
            tempEvent.Setup(EventTypeEnum.AbilityLevelUp, AbilityName);
            eventQueue.AddEvent(tempEvent);
            /////

            current_xp -= needed_xp;
            current_level += 1;
            needed_xp = current_level * 2000;
        }
    }

    public (string, Sprite, int, float) ReturnBasicStats()
    {
        float xp_out;
        if (current_level < MAX_LEVEL)
        {
            xp_out = (float)current_xp / (float)needed_xp;
        }
        else
        {
            xp_out = 1;
        }
        return (AbilityName, ability_sprite, current_level, xp_out);
    }

    public (List<(string, string)>, string) ReturnAdvStats() //Do not make virtual, call helper
    {
        List<(string, string)> tempList = new List<(string, string)>(); //Stat Name, Stat Value
        AdvStatsHelper(tempList);
        return (tempList, Description); //Stat Name, Stat Value
    }

    public void SetAccessible(bool set)
    {
        AbilityAccessible = set;
    }

    public bool ReturnAccessible()
    {
        return AbilityAccessible;
    }

    public string ReturnAbilityName()
    {
        return AbilityName;
    }

    public int ReturnLevel()
    {
        return current_level;
    }

    ////////////////////////////////////////////////////////
    private void Awake()
    {
        Assert.IsTrue(cooldown >= STARTUP_DECLARATIONS.AbilityAnimationCastTime[(int)abilityAnimation]);
        Setup();
    }

    private void Setup()
    {
        if (!is_setup)
        {
            Player = GameObject.Find("Player").transform;
            eventQueue = GameObject.Find("EventDisplay").GetComponent<EventQueue>();
            activePerks = Player.GetComponentInChildren<ActivePerks>();
            cursorLogic = GameObject.Find("Master Object").GetComponent<CursorLogic>();
            playerMovement = Player.GetComponent<PlayerMovement>();
            energy = Player.GetComponent<Energy>();
            animationUpdater = Player.GetComponentInChildren<PlayerAnimationUpdater>();
            weaponController = Player.GetComponentInChildren<WeaponController>();
            abilitiesController = Player.GetComponentInChildren<AbilitiesController>();
            PlayerProjectiles = GameObject.Find("PlayerProjectiles").transform;
            playerStats = Player.GetComponentInChildren<PlayerStats>();

            cooldown_remaining = 0f;

            //
            current_level = 1;
            current_xp = 0;
            needed_xp = 2000 * current_level;
            //

            is_setup = true;
            }
    }

    ////////////////////////////////////////////////////////
    protected virtual float AbilityEffectMult()
    {
        return ((1f + (float)current_level * 0.05f)) * playerStats.ReturnDamageMult(1, DS);
    }

    protected virtual void FixedUpdate()
    {
        Assert.IsTrue(cooldown >= Cast_at_this_time);
        if (cooldown_remaining > 0)
        {
            cooldown_remaining -= Time.fixedDeltaTime;
            if (cooldown_remaining < Cast_at_this_time)
            {
                Cast_at_this_time = -1f; //Stop Cast
                abilitiesController.abilityUsed(false);
                Attack();
            }
        }
    }

    protected virtual void AbilityAttackAnimation()
    {
        animationUpdater.AbilityAttack(abilityAnimation);
    }

    protected virtual void Attack() { }

    protected virtual void AdvStatsHelper(List<(string, string)> tempList)
    {
        tempList.Add(("Ability Name:", AbilityName));
        tempList.Add(("Cooldown:", cooldown.ToString()));
        tempList.Add(("Energy Cost:", energyCost.ToString()));

        if (current_level < MAX_LEVEL)
        {
            tempList.Add(("XP To Next Level:", (needed_xp - current_xp).ToString()));
        }
        else
        {
            tempList.Add(("Level Cap Reached:", "---"));
        }

    }
    ////////////////////////////////////////////////////////

}
