using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] ItemQuality SpawnerQuality;
    [SerializeField] bool BasicItems;
    [SerializeField] bool AdvancedItems;
    [SerializeField] bool ExperimentalItems;


    AssetEnumeration AS;
    private float[][] chanceMatrix = new  float [6][];

    void Start()
    {
        chanceMatrix[0] = new float[6] { .6f, .35f, .05f, 0, 0, 0 };
        chanceMatrix[1] = new float[6] { 0, .6f, .35f, .05f, 0, 0 };
        chanceMatrix[2] = new float[6] { 0, 0, .6f, .35f, .05f, 0 };
        chanceMatrix[3] = new float[6] { 0, 0, 0, .6f, .35f, .05f };
        chanceMatrix[4] = new float[6] { 0, 0, 0, 0, .95f, .05f };
        chanceMatrix[5] = new float[6] { 0, 0, 0, 0, 0, 1f };

        float[] selectedMatrix = chanceMatrix[(int)SpawnerQuality];
        float genValue = Random.value;

        int quality = 0;
        while(genValue > selectedMatrix[quality])
        {
            genValue -= selectedMatrix[quality];
            quality += 1;
        }

        AS = FindObjectOfType<AssetEnumeration>();
        List<int> itemTierList = new List<int>();
        if (BasicItems)
        {
            itemTierList.Add(0);
        }

        if (AdvancedItems)
        {
            itemTierList.Add(1);
        }

        if (ExperimentalItems)
        {
            itemTierList.Add(2);
        }

        ItemTier it = (ItemTier)itemTierList[Random.Range(0, itemTierList.Count)];

        GameObject item = AS.returnItem_fromTier(it);
        item.transform.parent = transform.parent;
        item.GetComponent<ItemMaster>().InitializeValues((ItemQuality)quality);
        Destroy(gameObject);
    }
}
