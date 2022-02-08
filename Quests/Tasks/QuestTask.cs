using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class QuestTask : MonoBehaviour
{
    [SerializeField] public TaskClassification TaskClass = TaskClassification.Master;
    [SerializeField] public GameObject TaskDest = null;
    public TaskStatus TaskCurrentStatus = TaskStatus.Open; //DO NOT CHANGE IN INSPECTOR

    [Space(35)]

    [SerializeField] protected string TaskDescription = "";
    [SerializeField] protected List<GameObject> AnyPosFlagFails = new List<GameObject>();

    [Space(35)]

    [SerializeField] protected Transform ExactLocation;
    [SerializeField] protected float Radius = 0;

    [Space(35)]

    [SerializeField] protected GameObject SetFlagUponCompletion = null;

    protected QuestObjective QO_parent;
    protected Zone_Flags ZF;




    public void External_Initialize()
    {
        QO_parent = transform.GetComponentInParent<QuestObjective>();
        ZF = FindObjectOfType<Zone_Flags>();
        Assert.IsTrue(TaskCurrentStatus == TaskStatus.Open);

        try
        {
            initialize();
        }
        catch
        {
            Debug.Log("INITIALIZE FAILURE");
            TaskCurrentStatus = TaskStatus.Failed;
        }


        if (TaskClass == TaskClassification.Optional || TaskClass == TaskClassification.Slave)
        {
            Assert.IsFalse(TaskDest, "Cannot have a Dest with this Task Classification");
        }
        FlagCheckTask();
    }

    public void External_Cleanup()
    {
        if (TaskCurrentStatus == TaskStatus.Open) //Cleaned otherwise
        {
            TaskCurrentStatus = TaskStatus.Failed;
            TaskCleanupLogicCatch();
        }
    }


    /////////////////////////////////////////////////////////////////////////////////





    public virtual (TaskStatus, string) ReturnTaskDesc()
    {
        return (TaskCurrentStatus, TaskDescription);
    }

    public (Transform, float) ReturnTaskLoc()
    {
        return (ExactLocation, Radius);
    }

    public void FlagCheckTask()
    {
        foreach (GameObject Flag in AnyPosFlagFails)
        {
            if (ZF.CheckFlag(Flag.name))
            {
                TaskCurrentStatus = TaskStatus.Failed;
                TaskCleanupLogicCatch(); //Must be after TaskCurrentStatus because I might check for this var
                break;
            }
        }

        FlagCheckProperCompletion();
    }



    public void ExternalCompletionTest(GameObject TaskRef)
    {
        if(TaskRef == gameObject)
        {
            TaskCompletion();
        }
    }



    /////////////////////////////////////////////////////////////////////////////////////////////////

    protected void TaskCompletion()
    {
        if (TaskCurrentStatus == TaskStatus.Open)
        {
            if (SetFlagUponCompletion)
            {
                ZF.SetFlag(SetFlagUponCompletion);
            }

            TaskCurrentStatus = TaskStatus.Completed;
            TaskCleanupLogicCatch(); //Must be after TaskCurrentStatus because I might check for this var
            QO_parent.TaskCompletedLogic();
        }
    }

    private void TaskCleanupLogicCatch()
    {
        try
        {
            TaskCleanupLogic();
        }
        catch
        {
            Debug.Log("CLEANUP FAILURE");
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////
    protected virtual void TaskCleanupLogic() //Called if completed or failed
    {

    }

    protected virtual void FlagCheckProperCompletion()
    {

    }

    protected virtual void initialize()
    {

    }

}
