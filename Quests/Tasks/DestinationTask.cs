using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DestinationTask : QuestTask
{
    private Collider colliderOnObject; //START DISABlED //Must be on Quest Object

    protected override void initialize()
    {
        colliderOnObject = GetComponent<Collider>();
        Assert.IsFalse(colliderOnObject.enabled); //This must start disabled
        colliderOnObject.enabled = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            TaskCompletion();
        }
    }

    protected override void TaskCleanupLogic() //Called for Completed Obj, Failed Obj, or Failed Quest
    {
        colliderOnObject.enabled = false;
    }
}
