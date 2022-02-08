using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImplantStats : ItemMaster
{
    [SerializeField] public AptitudeEnum AptitudeType;

    [SerializeField] private bool SkillLock = false;
    [SerializeField] private SkillsEnum SkillLockType = SkillsEnum.Larceny;
    [SerializeField] private int SkillLockValue = 0;

    [SerializeField] private bool AbilityLevelLock = false;
    [SerializeField] private AbilityHolderEnum AbilityExact = AbilityHolderEnum.InsHea;
    [SerializeField] private int RequiredLevel = 0;

    [SerializeField] public bool AttributeModifier = false;
    [SerializeField] public AttributesEnum attributeName = AttributesEnum.max_health;
    [SerializeField] public bool isAdd = true;
    [SerializeField] public float value = 0f;

    [SerializeField] private bool ExclusiveActivePerks = false;
    [SerializeField] private ExclusiveActivePerksEnum ExclusiveActivePerks_type = ExclusiveActivePerksEnum.RollReload;
    [SerializeField] private List<float> PerkValues = new List<float>();



    protected override void AdvStatsHelper(List<(string, string)> tempList)
    {
        base.AdvStatsHelper(tempList);
        if (SkillLock)
        {
            tempList.Add((STARTUP_DECLARATIONS.SkillEnumReverse[(int)SkillLockType] + " Requirement:", SkillLockValue.ToString()));
        }

        if (AbilityLevelLock)
        {
            tempList.Add((GameObject.Find("AbilityHolder").GetComponent<AbilityHolder>().Return_AbilityArray(AbilityExact).ReturnAbilityName() + " Level Requirement:", RequiredLevel.ToString()));
        }

        if (AttributeModifier)
        {
            tempList.Add((STARTUP_DECLARATIONS.AttributeDescriptions[(int)attributeName].Item1 + " Increase:", value.ToString() + (isAdd ? "" : "%")));
        }
    }

    public ImplantStats()
    {
        ItemType = ItemTypeEnum.Implant;
    }

    public bool CheckLocked()
    {
        PlayerStats PlayStats = GameObject.Find("Player").GetComponent<PlayerStats>();

        if (SkillLock && PlayStats.ReturnSkill(SkillLockType) < SkillLockValue)
        {
            return true;
        }

        if(AbilityLevelLock)
        {
            if (GameObject.Find("AbilityHolder").GetComponent<AbilityHolder>().Return_AbilityArray(AbilityExact).ReturnLevel() < RequiredLevel)
            {
                return true;
            }
        }
        return false;
    }


    public (bool, ExclusiveActivePerksEnum, List<float>) ReturnEAP()
    {
        return (ExclusiveActivePerks, ExclusiveActivePerks_type, PerkValues);
    }
}
