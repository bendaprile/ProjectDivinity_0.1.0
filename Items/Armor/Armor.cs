using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA;
using UMA.CharacterSystem;

public class Armor : ItemMaster
{
    [SerializeField] private ArmorType armorType = ArmorType.Chest;
    [SerializeField] private ArmorWeightClass armorWeightClass = ArmorWeightClass.Medium;
    [SerializeField] private float armor = 0f;
    [SerializeField] private float plating = 0f;
    [SerializeField] private UMATextRecipe wardrobeRecipe = null;
    [SerializeField] private UMATextRecipe wardrobeRecipeHandsFeet = null;

    public Armor()
    {
        ItemType = ItemTypeEnum.Armor;
    }

    public float returnArmor()
    {
        return armor;
    }

    public float returnPlating()
    {
        return plating;
    }

    public ArmorType returnArmorType()
    {
        return armorType;
    }

    public float returnEEnergyRegenModifier()
    {
        if(armorWeightClass == ArmorWeightClass.Light)
        {
            return -0.05f;
        }
        else if(armorWeightClass == ArmorWeightClass.Medium)
        {
            return -0.1f;
        }
        else
        {
            return -0.2f;
        }
    }

    public (UMATextRecipe, UMATextRecipe) GetWardrobePiece()
    {
        return (wardrobeRecipe, wardrobeRecipeHandsFeet);
    }

    protected override void AdvStatsHelper(List<(string, string)> tempList)
    {
        base.AdvStatsHelper(tempList);
        tempList.Add(("Armor Type:", STARTUP_DECLARATIONS.ArmorTypeEnumReverse[(int)armorType]));
        tempList.Add(("Item Weight Class:", STARTUP_DECLARATIONS.ArmorWeightEnumReverse[(int)armorWeightClass]));
        tempList.Add(("Armor Value:", armor.ToString()));
        tempList.Add(("Plating Value:", plating.ToString()));
        tempList.Add(("Expanded Energy Regen:", (returnEEnergyRegenModifier() * 100).ToString() + "%"));
    }
}