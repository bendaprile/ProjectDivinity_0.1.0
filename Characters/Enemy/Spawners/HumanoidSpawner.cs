using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class HumanoidSpawner : Spawner
{
    private enum enemyquality { Swarm, Group, Elite } //Used for derived classes
    [SerializeField] private enemyquality EnemyQuality = enemyquality.Swarm; //max_enemy will override the number

    [Space(50)]

    [SerializeField] private List<NPC_FactionsEnum> Npc;
    [SerializeField] private List<int> NpcPercentages;
    [SerializeField] private NPC_Control_Mode ControlMode = NPC_Control_Mode.Stay;

    private ItemCataog itemCataog;
    private int GearPoints; //Total points for the group
    private int[] PointsPerEnemy;
    HumanoidMaster HM;


    public override void External_Initalize(bool force_replenish)
    {
        Assert.IsTrue(enemyPool.Count == 1);
        itemCataog = GameObject.Find("Master Object").GetComponent<ItemCataog>();
        base.External_Initalize(force_replenish); //Needs to be last
    }

    protected override void AdditionalSetup(int i, Transform child)
    {
        HumanoidCombatClass HCC;
        if (EnemyQuality == enemyquality.Swarm)
        {
            HCC = PickClass(false, child);
        }
        else if (EnemyQuality == enemyquality.Group)
        {
            HCC = PickClass(i % 3 == 2, child); // 1/3
        }
        else
        {
            HCC = PickClass(true, child);
        }
        HM = child.GetComponent<HumanoidMaster>();

        SetupEnemyItems(HCC, child);
        SetupNPC(child);
    }

    ////////////////////////////////////////////////////////////////
    protected override void SetMaxEnemy()
    {
        if(max_enemy.Count == 0)
        {
            return;
        }

        switch (EnemyQuality)
        {
            case enemyquality.Swarm:
                max_enemy.Add(3 + PS.returnLevel() / 5); //All classless
                break;
            case enemyquality.Group:
                max_enemy.Add(2 + PS.returnLevel() / 10); // 1/3 has a class
                break;
            case enemyquality.Elite:
                max_enemy.Add(1 + PS.returnLevel() / 15); // All Classes
                break;
        }
    }


    private HumanoidCombatClass PickClass(bool SpecialClass, Transform child)
    {
        HumanoidCombatClass CombatClass = HumanoidCombatClass.Classless; //No class
        HumanoidMaster HM = child.GetComponent<HumanoidMaster>();

        if (SpecialClass)
        {
            CombatClass = (HumanoidCombatClass)Random.Range(1, 4);
        }

        HM.SetupHumanoidStats(CombatClass, ControlMode);
        return CombatClass;
    }


    private void SetupEnemyItems(HumanoidCombatClass HAC, Transform child)
    {
        int firing_distance;
        ArmorWeightClass AWC;
        if (HAC == HumanoidCombatClass.Sharpshooter)
        {
            firing_distance = 2;
            AWC = ArmorWeightClass.Light;
        }
        else if(HAC == HumanoidCombatClass.Generalist)
        {
            firing_distance = Random.Range(0, 3); //Any
            AWC = ArmorWeightClass.Medium;
        }
        else if (HAC == HumanoidCombatClass.Tank)
        {
            firing_distance = Random.Range(0, 2); //Melee or Medium
            AWC = ArmorWeightClass.Heavy;
        }
        else
        {
            firing_distance = Random.Range(0, 3); //Any
            AWC = ArmorWeightClass.Light;
        }

        GameObject TempWeapon = itemCataog.ReturnWeaponInBudget(10, firing_distance); //TODO base the points off somthing
        (GameObject, GameObject, GameObject) TempArmor = itemCataog.ReturnArmorInBudget(10, AWC); //TODO base the points off somthing

        if (TempArmor.Item1)
        {
            (TempArmor.Item1).transform.parent = child.Find("Hitbox");
        }

        if (TempArmor.Item2)
        {
            (TempArmor.Item2).transform.parent = child.Find("Hitbox");
        }

        if (TempArmor.Item3)
        {
            (TempArmor.Item3).transform.parent = child.Find("Hitbox");
        }

        if (TempWeapon)
        {
            TempWeapon.transform.parent = child.Find("Body");
        }

        HM.SetupHumanoidItems(TempWeapon, TempArmor);
    }

    private void SetupNPC(Transform child)
    {
        child.Find("Body").GetComponent<NPC>().Setup(Npc, NpcPercentages);

    }
}
