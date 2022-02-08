using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagBasedTask : QuestTask
{
    [SerializeField] GameObject ProperFlag;
    [SerializeField] bool ProperFlags_polarity;

    protected override void FlagCheckProperCompletion()
    {
        if (ZF.CheckFlag(ProperFlag.name) == ProperFlags_polarity)
        {
            TaskCompletion();
        }
    }
}
