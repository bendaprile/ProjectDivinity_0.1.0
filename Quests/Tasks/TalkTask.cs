using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TalkTask : QuestTask
{
    [SerializeField] private GameObject ExactDiaLine;
    [SerializeField] Transform NewStart;
    [SerializeField] Transform EnableDiaOptionWithPostObjCleanup;

    protected override void initialize()
    {
        Assert.IsNotNull(ExactDiaLine);
        
        if (NewStart)
        {
            NewStart.GetComponentsInParent<DiaRoot>(true)[0].ModifyStarting(NewStart); //This works with inactive gameObjects
        }

        if (EnableDiaOptionWithPostObjCleanup)
        {
            EnableDiaOptionWithPostObjCleanup.gameObject.SetActive(true);
        }

        ExactDiaLine.GetComponent<DiaMaster>().mark_dia_for_quest(gameObject); //TODO CHANGE THIS
    }

    protected override void TaskCleanupLogic() //Called for Completed Obj, Failed Obj, or Failed Quest
    {
        if (EnableDiaOptionWithPostObjCleanup)
        {
            EnableDiaOptionWithPostObjCleanup.gameObject.SetActive(false);
        }

        ExactDiaLine.GetComponent<DiaMaster>().Unmark_dia_for_quest();
    }
}
