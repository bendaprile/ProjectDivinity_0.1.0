using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerDeathTask : QuestTask
{
    public void DeathCheckTask()
    {
        SphereCollider SC = GetComponent<SphereCollider>();
        Assert.IsNotNull(SC, "No SphereCollider attached to Death Task");
        float dist = Vector3.Distance(GameObject.Find("Player").transform.position, transform.position);
        if (dist <= SC.radius)
        {
            TaskCompletion();
        }
    }
}
