using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


///General Damage
public enum DamageType { Regular }

public enum DamageSource { None, VigorBased, CerebralBased, FinesseBased }
///General Damage

///Misc
public enum MoveDir { Forward, Left, Right, Backward }
///Misc

///Player
public enum AptitudeEnum { Vigor, Cerebral, Finesse }

public enum SkillsEnum { Larceny, Science, Medicine, Speech, Survival, Gadgeteer, NightOwl }

public enum AttributesEnum
{
    max_energy, energy_regen, max_overheat, cooldown_rate, max_health, health_regen, armor, vigor_mult, cerebral_mult, finesse_mult
}

public enum ExclusiveActivePerksEnum { RollReload, LightningSphereTurret, InstantTele, ProjShieldReflection }

public enum MoveState { Idle, Walking, Running, Rolling, ForceBased, FollowCursor }
public enum EventTypeEnum { QuestObjCompleted, QuestCompleted, QuestFailed, QuestStarted, LevelUp, AbilityLevelUp, ItemsReceived, LocationDiscovered }
///Player

///Items
public enum ItemTypeEnum { Misc, Armor, Weapon, Consumable, Implant }
public enum WeaponType { Melee_1H, Melee_2H, Ranged }
public enum ArmorWeightClass { Light, Medium, Heavy }
public enum ArmorType { Head, Chest, Legs }
public enum ConsumableType { Restoring, Grenade }
public enum ItemQuality { Damaged, Flawed, Common, Rare, Flawless, Legendary }
public enum ItemTier { Basic, Advanced, Experimental }
public enum RangedAnimation { Ranged_1H, Ranged_2H, Minigun, Launcher }
public enum MeleeAnimation { StandingAttackDownward, StandingAttack360High, JumpAttack }
///Items

///Abilites
public enum AbilityType { Lightning, Spacetime, Exoskeleton, Regeneration, Faction }
public enum AbilityHolderEnum
{
    LigSphThr, LigBolCas, LigBlaCas, LigMel,
    TimDia, ProShiCas, Tel, KinRev,

    InsHea
}
public enum AbilityAnimation
{
    ThrowArmsForward, Battlecry, ThrowArmForward, UnderhandThrowArmUp, ExplodeArmsOutward, SwingBackThrowArmsForward,
    HoldHandsForward, OverhandThrow, BendDownThrowArmsForward_FB, TwistSlamDown_FB
}
///Abilites

///Quest

public enum TaskClassification { Master, Slave, Branch, Optional, Hidden, HiddenOptional }

//Master    (Has dest, can be only 1)
//Slave     (No dest, must be completed to go to master dest)
//Branch    (Has dest, can be completed instead of master)
//Optional  (No dest, does not need to be completed)
//Hidden    (Has dest, not shown in gui, can be used to similarly to previous objective fail)
//HiddenOptional (This is only used for setup, no UI is changed and no quest logic is affected)

public enum TaskStatus { Open, Completed, Failed }

public enum ObjectiveType { NOTHING, Fetch, GenericKill, SpawnerKill, Talk }
public enum QuestCategory { Main, Side, Miscellaneous, Completed }
///Quest


///NPC/Humanoid
public enum HumanoidLogicMode { none, Kiting, Kiting_reposition, Charge_in, Flee }
public enum HumanoidCombatClass { Classless, Sharpshooter, Generalist, Tank, Antagonist }
public enum HumanoidWeaponExpertise { Novice, Adept, Commando }
public enum HumanoidMovementType { Hindered, Average, Agile, inhuman }
public enum NPC_Control_Mode { WalktoPlayer_dia, Stay, NPC_control, Enemy_control }

public enum NPCActivityFlag { _NO_FLAG_, 
    City_ArenaSpectator, City_Bench,
                        } //Used for activity based Dia

public enum NPC_FactionsEnum
{
    City_Rich, City_Poor,
    Plantation_Farmer, Wasteland_Scavengers, City_Guard,
    Ascended, Lightning_Cult
}
///NPC/Humanoid

public enum StartTypeEnum { DEBUG, NEW_GAME, LOAD }
public enum CustomReputation { PlayerEnemy, Standard, PlayerAlly }
public enum EnemyCategory { Mechanical, Creature, Human}
public enum FactionsEnum{ Neutral, Player, Rogue, Feral, AntiPlayer, B, Scavengers, Plantation, MidwayCityCivilian, FacelessReapers, Ascended, LightningCult }
public enum Zones { Wasteland, Storm, Tundra, Jungle, Midway }
public enum UI_Mode { Normal, PauseMenu, DiaMenu, InteractiveMenu} //IF MODIFIED change "Check_if_Escapable" in UIController
public enum SkillCheckStatus { NoCheck, Success, Failure }

public enum SkillCheckDifficulty { _None_, Novice, Simple, Adept, Expert, Master, _Exalted_ }

public static class STARTUP_DECLARATIONS
{
    public const int NPC_FactionsCount = 7;
    public const int FactionCount = 12;
    public const int AllyNumber = 1800;
    public const int EnemyNumber = 200;
    public const int HumanoidDeathFactionChange = -500;
    public const int HumanoidInjuryFactionChange = -100;

    public const int Number_of_ExclusiveActivePerks = 4;
    public const int Number_of_Attributes = 10;
    public const int Number_of_Skills = 7;

    public const int Skill_Points_per_Level = 5;
    public const float TIME_TO_DISPLAY_TOOLTIP = 0.1f;

    public static string[] FactionsEnumReverse = new string[FactionCount] { "Neutral", "Ally", "Rogue", "Feral", "AntiPlayer", "B", "Scavengers", "Plantation", "Midway City Civilian", "Faceless Reapers", "Ascended", "Lightning Cult" };

    public static string[] NPC_FactionsEnumReverse = new string[NPC_FactionsCount] { "City Rich", "City Poor", "Farmer", "Scavengers", "City Guard", "Ascended", "Lightning Cult" };

    public static string[] SkillEnumReverse = new string[7] { "Larceny", "Science", "Medicine", "Speech", "Survival", "Gadgeteer", "Night Owl" };

    public static string[] ArmorTypeEnumReverse = new string[3] { "Head", "Chest", "Legs"};
    public static string[] ArmorWeightEnumReverse = new string[3] { "Light", "Medium", "Heavy" };
    public static string[] ItemClassEnumReverse = new string[6] { "Damaged", "Flawed", "Common", "Rare", "Flawless", "Legendary" };
    public static string[] WeaponTypeEnumReverse = new string[3] { "Melee 1-handed", "Melee 2-handed", "Ranged"};
    public static string[] DamageTypeEnumReverse = new string[3] { "True" , "Piercing", "Regular" };
    public static string[] DamageSourceEnumReverse = new string[4] { "None", "Vigor Based", "Cerebral Based", "Finesse Based" };

    public static Color32 goldColor = new Color32(255, 199, 0, 255);
    public static Color32 goldColorTransparent = new Color32(255, 199, 0, 100);
    public static Color32 checkSuccessColor = new Color32(160, 255, 110, 255);
    public static Color32 checkFailColor = new Color32(255, 110, 110, 255);
    public static Color32 vigorColor = new Color32(255, 0, 0, 255);
    public static Color32 cerebralColor = new Color32(0, 169, 255, 255);
    public static Color32 finesseColor = new Color32(98, 0, 255, 255);

    public static float[] AbilityAnimationCastTime = new float[10] { .8f, .5f, .5f, .5f, .5f, .5f, .5f, .5f, .5f, .5f };

    public static Dictionary<ItemQuality, Color32> itemQualityColors = new Dictionary<ItemQuality, Color32>
    {
        { ItemQuality.Damaged, new Color32(159, 159, 159, 255) },
        { ItemQuality.Flawed, new Color32(112, 255, 0, 255) },
        { ItemQuality.Common, new Color32(0, 131, 255, 255) },
        { ItemQuality.Rare, new Color32(170, 0, 255, 255) },
        { ItemQuality.Flawless, new Color32(255, 192, 0, 255) },
        { ItemQuality.Legendary, new Color32(0, 255, 255, 255) }
    };

    public static (int, int)[] screenResolutions = new (int, int)[5]
    {
        (1280, 720),
        (1920, 1080),
        (2560, 1440),
        (2560, 1080),
        (3440, 1440)
    };

    public static (string, string)[] skills_AptitudesDescriptions = new (string, string)[Number_of_Skills + 3]
    {
        ////////////// GENERAL SKILLS
        ("Larceny", "Larceny allows you to unlock doors and other locked objects. It also reduces your detection radius."),
        ("Science", "Science allows you to hack terminals and other systems. It also gives you a slight damage bonus to mechanical enemies."),
        ("Medicine", "Medicine allows you to heal others given the right situation. It also increases healing from consumables."),
        ("Speech", "Speech allows you to persuade or intimidate others. It also reduces prices with merchants."),
        ("Survival", "Survival increases your passive health recovery. It also gives you extra armor at low health."),
        ("Gadgeteer", "Gadgeteer reduces the cooldown of your consumable slot."),
        ("Night Owl", "Night Owl grants better senses during the night. It also gives you a slight damage bonus during the night."),

        /////////////// Attributes SKILLS
        ("Vigor", "Vigor determines your power and fortitude. A high vigor apptitude will let you excel with heavy slow attacks or high defense."),
        ("Cerebral", "Cerebral determines your intellegence and creativety. A high cerebral apptitude will help you outwit your enemies in or before combat."),
        ("Finesse ", "Finesse helps your quick attacks and agility. A high finesse apptitude will let you rapidly attack your enemies while avoiding thier attacks."),
    };


    public static (string, string)[] AttributeDescriptions = new (string, string)[Number_of_Attributes]
    {
        ("Energy", "The maximum amount of energy that you can have at anypoint. Most abilities can only use energy."),
        ("Energy Regen", "The rate at which your energy is restored naturally."),
        ("Overheat Capacity", "Using abilities and weapons fills the overheat bar. At 50% -> 90% you use 50% increased energy. At 90% -> 100% you use 100% increased energy"),
        ("Cooldown Rate", "The rate at which your Overheat decreases. This rate is a proportion of your Overheat Capacity."),
        ("Health", "The maximum amount of damage you take before dying. Armor can reduce the amount of damage you take."),
        ("Health Regen", "The rate at which you natrually restore health."),
        ("Armor", "Reduces the amount of damage you take by a percentage. The damage reduction is calculated by (Armor) / (Armor + 100). e.g. 50 armor => 50 / 150 => 33.33% reduction"),

        ("Vigor Damage Multiplier", "Increases your damage with vigor based attack. Vigor attacks are best for breaking plating."),
        ("Cerebral Damage Multiplier", "Increases your damage with cerebral based. Cerebral attacks are decent for breaking both plating and shields."),
        ("Finesse Damage Multiplier", "Increases your damage with finesse based attacks. Finesse attacks are best for breaking shields."),
    };

    public static string ParagraphBreak()
    {
        return Environment.NewLine + Environment.NewLine;
    }

    public static int ReturnSkillCheck(int level, SkillCheckDifficulty type)
    {
        if(type == SkillCheckDifficulty.Novice)
        {
            return 10 + (level / 2);
        }
        else if (type == SkillCheckDifficulty.Simple)
        {
            return 12 + 2 * (level / 2);
        }
        else if (type == SkillCheckDifficulty.Adept)
        {
            return 14 + 3 * (level / 2);
        }
        else if (type == SkillCheckDifficulty.Expert)
        {
            return 16 + 4 * (level / 2);
        }
        else if (type == SkillCheckDifficulty.Master)
        {
            return 20 + 5 * (level / 2);
        }
        else if (type == SkillCheckDifficulty._Exalted_)
        {
            return 90;
        }
        else
        {
            return 0;
        }
    }
}
