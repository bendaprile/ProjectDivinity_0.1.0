using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FetchTask : QuestTask 
{
    [SerializeField] private ItemMaster itemNeeded;
    [SerializeField] private int itemCountRequired = 1;


    private List<GameObject> itemRefs = new List<GameObject>();
    private Inventory inventory;


    protected override void initialize()
    {
        inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
        List<GameObject> temp = inventory.ReturnItems_ByName(itemNeeded);

        foreach (GameObject iter in temp)
        {
            if (itemRefs.Count < itemCountRequired)
            {
                itemRefs.Add(iter);
                inventory.LockItemQuest(iter);
            }
            else
            {
                break;
            }
        }

        if (itemRefs.Count == itemCountRequired)
        {
            TaskCompletion();
        }
    }

    public List<GameObject> ReturnItemRefs()
    {
        return itemRefs;
    }

    public void UpdateItemCount(GameObject item)
    {
        ItemMaster itemStats = item.GetComponent<ItemMaster>();

        if (itemStats.ReturnBasicStats().Item3 == itemNeeded.ReturnBasicStats().Item3 && itemRefs.Count < itemCountRequired)
        {
            itemRefs.Add(item);
            inventory.LockItemQuest(item);

            if (itemRefs.Count == itemCountRequired)
            {
                TaskCompletion();
            }
        }
    }

    public override (TaskStatus, string) ReturnTaskDesc()
    {
        string amount_completed = " (" + itemRefs.Count + "/" + itemCountRequired + ")";
        return (TaskCurrentStatus, (TaskDescription + amount_completed));
    }

    protected override void TaskCleanupLogic()
    {
        if(TaskCurrentStatus == TaskStatus.Failed) //Failed Only for this
        {
            foreach (GameObject iter in itemRefs)
            {
                inventory.UnockItemQuest(iter);
            }
        }
    }



}
