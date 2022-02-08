using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnTask : QuestTask
{
    [SerializeField] private GameObject ConnectedFetchQuest = null;
    [SerializeField] private GameObject ReturnDia = null;
    private Inventory inventory;

    private List<GameObject> itemRefs;

    protected override void initialize()
    {
        itemRefs = ConnectedFetchQuest.GetComponent<FetchTask>().ReturnItemRefs();
        inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
        ReturnDia.GetComponent<DiaMaster>().mark_dia_for_quest(gameObject); //Do later
    }

    protected override void TaskCleanupLogic()
    {
        if (TaskCurrentStatus == TaskStatus.Failed)
        {
            foreach (GameObject iter in itemRefs)
            {
                inventory.UnockItemQuest(iter);
            }
        }
        else if(TaskCurrentStatus == TaskStatus.Completed)
        {
            foreach (GameObject iter in itemRefs)
            {
                inventory.DeleteItem(iter);
            }
        }
    }
}
