using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class WeaponCatHelper : MonoBehaviour
{
    [SerializeField] List<int> Cost = new List<int>();
    [SerializeField] List<GameObject> Weapon = new List<GameObject>();

    private List<(int, GameObject)> MergedData = new List<(int, GameObject)>();


    public List<(int, GameObject)> ReturnWeapon() //Return Sorted Data
    {
        Assert.IsTrue(Cost.Count == Weapon.Count);

        for (int i = 0; i < Cost.Count; ++i)
        {
            MergedData.Add((Cost[i], Weapon[i]));
        }

        MergedData.Sort((a, b) => a.Item1.CompareTo(b.Item1));

        return MergedData;
    }
}
