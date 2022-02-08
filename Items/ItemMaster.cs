using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ItemMaster : MonoBehaviour
{
    [SerializeField] private int Cost = 0;
    [SerializeField] private int Weight = 0;
    [SerializeField] private string ItemName = "NoNameAssigned";
    [SerializeField] [TextArea(3, 10)] private string Description = "";
    [SerializeField] [TextArea(3, 10)] private string Special = "";
    [SerializeField] private ItemQuality itemClass = ItemQuality.Common;
    [SerializeField] public ItemTier itemTier = ItemTier.Basic;
    [SerializeField] public Sprite item_sprite = null;

    public bool LockForQuest = false;

    protected ItemTypeEnum ItemType;
    protected float[] ItemScaleNum = new float[] {.6f, .8f, 1.0f, 1.25f, 1.5f, 2f};


    public virtual void InitializeValues(ItemQuality ItemClass_in)
    {
        //Debug.Log(itemClass);
        if(itemClass != ItemQuality.Common)
        {
            Debug.Log("Already Initialized, must be common");
            Assert.IsTrue(false);
            return;
        }
        itemClass = ItemClass_in;
    }

    public string ReturnItemName()
    {
        return ItemName;
    }

    public ItemQuality ReturnItemClass()
    {
        return itemClass;
    }

    public ItemTypeEnum ReturnItemType()
    {
        return ItemType;
    }

    public (int, int, string, Sprite, ItemQuality) ReturnBasicStats() //Do not make virtual
    {
        return (Cost, Weight, ItemName, item_sprite, itemClass);
    }

    public (List<(string, string)>, string, string) ReturnAdvStats() //Do not make virtual, call helper
    {
        List<(string, string)>  tempList = new List<(string, string)>(); //Stat Name, Stat Value
        AdvStatsHelper(tempList);
        return (tempList, Description, Special); //Stat Name, Stat Value
    }

    protected virtual void AdvStatsHelper(List<(string, string)> tempList)
    {
        tempList.Add(("Item Name:", ItemName));
        tempList.Add(("Item Quality:", STARTUP_DECLARATIONS.ItemClassEnumReverse[(int)itemClass]));
    }
}
