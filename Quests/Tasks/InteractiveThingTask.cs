using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveThingTask : QuestTask
{
    [SerializeField] private InteractiveThing InteractiveThing;

    protected override void initialize()
    {
        InteractiveThing.SetQuest(gameObject);
    }

    protected override void TaskCleanupLogic() //Called if completed or failed
    {
        InteractiveThing.ForceQuestEnd();
    }
}
