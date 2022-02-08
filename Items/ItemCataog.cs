using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCataog : MonoBehaviour
{
    private List<(int, GameObject, GameObject, GameObject)>[] Armor = new List<(int, GameObject, GameObject, GameObject)>[4];
    private List<(int, GameObject)>[] Weapons = new List<(int, GameObject)>[3];

    //private int[] WeaponCategoryCosts = { 30, 50, 100000000 }; //TODO Change Last one later
    //private float[] WeaponQualityCostMutiplier = { .2f, .5f, 1f, 1.5f, 2f, 5f }; //Indexed by WeaponQualiy

    //private int[] ArmorCategoryCosts = { 30, 100000000, 100000000 }; //This is the cost for all 3 pieces combined //TODO Change Last one later
    //private float[] ArmorQualityCostMultiplier = { .2f, .5f, 1f, 1.5f, 2f, 5f }; //Indexed by WeaponQualiy


    private void Awake()
    {
        Transform CataogHolder = transform.Find("CataogHolder");
        Armor[0] = CataogHolder.Find("LightArmor").GetComponent<ArmorCatHelper>().ReturnArmorSet();
        Armor[1] = CataogHolder.Find("MediumArmor").GetComponent<ArmorCatHelper>().ReturnArmorSet();
        Armor[2] = CataogHolder.Find("HeavyArmor").GetComponent<ArmorCatHelper>().ReturnArmorSet();
        Weapons[0] = CataogHolder.Find("MeleeWeapons").GetComponent<WeaponCatHelper>().ReturnWeapon();
        Weapons[1] = CataogHolder.Find("MediumWeapons").GetComponent<WeaponCatHelper>().ReturnWeapon();
        Weapons[2] = CataogHolder.Find("LongWeapons").GetComponent<WeaponCatHelper>().ReturnWeapon();
    }


    public (GameObject, GameObject, GameObject) ReturnArmorInBudget(int budget, ArmorWeightClass AWC) //Picks armor from the same group //TODO use budget
    {
        List<(int, GameObject, GameObject, GameObject)> ArmorTemp = Armor[(int)AWC];

        (GameObject, GameObject, GameObject) ArmorFinal = (Instantiate(ArmorTemp[Random.Range(0, ArmorTemp.Count)].Item2), Instantiate(ArmorTemp[Random.Range(0, ArmorTemp.Count)].Item3), Instantiate(ArmorTemp[Random.Range(0, ArmorTemp.Count)].Item4));

        return ArmorFinal; //Can be no Armor
    }

    public GameObject ReturnWeaponInBudget(int budget, int WeaponFiringDistance) //TODO use budget
    {
        List<(int, GameObject)> WeaponTemp = Weapons[WeaponFiringDistance];
        GameObject WeaponFinal = Instantiate(WeaponTemp[Random.Range(0, WeaponTemp.Count)].Item2);

        return WeaponFinal; //Can be no Weapon
    }
}
