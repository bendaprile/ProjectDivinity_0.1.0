using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FlagBasedTaskList : QuestTask
{
    [SerializeField] List<GameObject> AnyProperFlag;
    [SerializeField] List<bool> ProperFlags_polarity;

    protected override void initialize()
    {
        Assert.IsTrue(AnyProperFlag.Count == ProperFlags_polarity.Count);
    }


    protected override void FlagCheckProperCompletion()
    {
        for(int i = 0; i < AnyProperFlag.Count; ++i)
        {
            if (ZF.CheckFlag(AnyProperFlag[i].name) == ProperFlags_polarity[i])
            {
                TaskCompletion();
            }
        }
    }
}
