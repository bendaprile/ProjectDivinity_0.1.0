using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ArmorCatHelper : MonoBehaviour
{
    [SerializeField] List<int> Cost = new List<int>();
    [SerializeField] List<GameObject> Head = new List<GameObject>();
    [SerializeField] List<GameObject> Chest = new List<GameObject>();
    [SerializeField] List<GameObject> Legs = new List<GameObject>();


    private List<(int, GameObject, GameObject, GameObject)> MergedData = new List<(int, GameObject, GameObject, GameObject)>();


    public List<(int, GameObject, GameObject, GameObject)> ReturnArmorSet() //Return Sorted Data
    {
        Assert.IsTrue(Head.Count == Chest.Count);
        Assert.IsTrue(Head.Count == Legs.Count);
        Assert.IsTrue(Head.Count == Cost.Count);

        for (int i = 0; i < Head.Count; ++i)
        {
            MergedData.Add((Cost[i], Head[i], Chest[i], Legs[i]));
        }

        MergedData.Sort((a, b) => a.Item1.CompareTo(b.Item1));

        return MergedData;
    }
}
