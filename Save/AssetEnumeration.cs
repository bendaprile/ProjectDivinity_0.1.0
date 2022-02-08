using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AssetEnumeration : MonoBehaviour
{
    Dictionary<string, GameObject> Asset2Int = new Dictionary<string, GameObject>();
    List<GameObject>[] Items_by_Tiers = new List<GameObject>[3];


    public GameObject returnItem(string str)
    {
        return Instantiate(Asset2Int[str]);
    }

    public GameObject returnItem_fromTier(ItemTier it)
    {
        return Instantiate(Items_by_Tiers[(int)it][Random.Range(0, Items_by_Tiers[(int)it].Count)]);
    }

    public List<GameObject> returnAllItems()
    {
        List<GameObject> Assets = new List<GameObject>();
        foreach (string k in Asset2Int.Keys)
        {
            GameObject itemTemp = Instantiate(Asset2Int[k]);
            Assets.Add(itemTemp);
        }
        return Assets;
    }

    void Awake()
    {
        AssetSetupRec(transform);
        for(int i = 0; i < 3; ++i)
        {
            Items_by_Tiers[i] = new List<GameObject>();
        }

        foreach(string key in Asset2Int.Keys)
        {
            if (Asset2Int[key].GetComponent<ItemMaster>().ReturnItemType() == ItemTypeEnum.Misc)
            {
                continue; //Don't add these
            }

            Items_by_Tiers[(int)Asset2Int[key].GetComponent<ItemMaster>().itemTier].Add(Asset2Int[key]);
        }
    }

    private void AssetSetupRec(Transform trans)
    {
        ItemMaster item = trans.GetComponent<ItemMaster>();
        if (item)
        {
            Assert.IsFalse(Asset2Int.ContainsKey(item.ReturnItemName()), "TWO ITEMS SHARE THE ITEM NAME (" + item.ReturnItemName() + ")");
            Assert.IsTrue(item.ReturnItemClass() == ItemQuality.Common, "ITEM (" + item.ReturnItemName() + ") MUST BE COMMON");
            Asset2Int.Add(item.ReturnItemName(), trans.gameObject);
        }
        else
        {
            foreach (Transform child in trans)
            {
                AssetSetupRec(child);
            }
        }
    }
}
