using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class QuestObjective : MonoBehaviour
{
    [SerializeField] protected World_Setup World_Setup_Start = null; //Use this to help setup events

    private List<QuestTask> TasksList = new List<QuestTask>(); 
    private QuestTemplate questTemplate;

    private int SlaveCount = 0;
    private bool Objective_Ready_For_Completion = false; //Do not complete the objective until ALL tasks are setup 
    //ALSO, do not complete the objective more than once. This could happen if a branch and a master compelete on the same frame


    public void initialize()
    {
        questTemplate = GetComponentInParent<QuestTemplate>();

        foreach (Transform child in transform)
        {
            QuestTask QTask = child.GetComponent<QuestTask>();

            if (!QTask)
            {
                continue;
            }

            TasksList.Add(QTask);
            QTask.External_Initialize();
            if (QTask.TaskClass == TaskClassification.Slave)
            {
                SlaveCount += 1;
            }
        }

        if (World_Setup_Start)
        {
            World_Setup_Start.Setup();
        }

        Objective_Ready_For_Completion = true; //MUST BE AT END
        TaskCompletedLogic(); //Test here because it was blocked before
    }

    public void TaskCompletedLogic()
    {
        if (!Objective_Ready_For_Completion) //Do not check until all tasks are setup
        {
            return;
        }

        questTemplate.SingleTaskCompletedUpperLogic();

        GameObject Queued_Completed_Dest = null; //Will remain null if end of quest
        bool MainCondMet = false;
        int Slaves_Remaining = SlaveCount;

        for (int i = 0; i < TasksList.Count; ++i)
        {
            if(TasksList[i].TaskCurrentStatus != TaskStatus.Completed) //Only look at completed tasks
            {
                continue;
            }

            if(TasksList[i].TaskClass == TaskClassification.Hidden)
            {
                Queued_Completed_Dest = TasksList[i].TaskDest;
                MainCondMet = true;
                Slaves_Remaining = 0; //Hidden objectives ignore slaves
                break;
            }
            else if(TasksList[i].TaskClass == TaskClassification.Slave)
            {
                Slaves_Remaining -= 1;
            }
            else if (TasksList[i].TaskClass == TaskClassification.Master || TasksList[i].TaskClass == TaskClassification.Branch)
            {
                MainCondMet = true;
                Queued_Completed_Dest = TasksList[i].TaskDest;
            }
        }

        if(MainCondMet && Slaves_Remaining == 0)
        {
            Objective_Ready_For_Completion = false; //Prevent this objective from completing again
            ExternalCleanup();
            questTemplate.ObjectiveFinished(Queued_Completed_Dest);
        }
    }

    public void ExternalCleanup()
    {
        for (int i = 0; i < TasksList.Count; ++i)
        {
            TasksList[i].External_Cleanup();
        }
    }




    ///////////////////////////////////////////////////////////////////////////////////////////


    public List<(TaskStatus, string)> ReturnTasks()
    {
        List<(TaskStatus, string)> taskDescList = new List<(TaskStatus, string)>();
        for (int i = 0; i < TasksList.Count; ++i)
        {
            if(TasksList[i].TaskClass != TaskClassification.Hidden && TasksList[i].TaskClass != TaskClassification.HiddenOptional)
            {
                taskDescList.Add(TasksList[i].ReturnTaskDesc());
            }
        }
        return taskDescList;
    }

    public List<(Transform, float)> ReturnLocs()
    {
        List<(Transform, float)> taskLocList = new List<(Transform, float)>();
        for (int i = 0; i < TasksList.Count; ++i)
        {
            if (TasksList[i].TaskClass != TaskClassification.Hidden && TasksList[i].TaskCurrentStatus == TaskStatus.Open) //Show for hidden optional
            {
                taskLocList.Add(TasksList[i].ReturnTaskLoc());
            }
        }
        return taskLocList;
    }


    ///////////////////////////////////////////////////////////////////////////////////////////

    public void FlagCheckObj() //Check all
    {
        for (int i = 0; i < TasksList.Count; ++i)
        {
            TasksList[i].FlagCheckTask();
        }
    }

    public void DeathCheckObj()
    {
        for (int i = 0; i < TasksList.Count; ++i)
        {
            var PDT = TasksList[i] as PlayerDeathTask;
            if (PDT)
            {
                PDT.DeathCheckTask();
            }
        }
    }

    public void FetchCheckObj(GameObject item)
    {
        for (int i = 0; i < TasksList.Count; ++i)
        {
            var FT = TasksList[i] as FetchTask;
            if (FT)
            {
                FT.UpdateItemCount(item);
            }
        }
    }

    public void ExternalCompletionTestObj(GameObject TaskRef)
    {
        for (int i = 0; i < TasksList.Count; ++i)
        {
            TasksList[i].ExternalCompletionTest(TaskRef);
        }
    }
}
