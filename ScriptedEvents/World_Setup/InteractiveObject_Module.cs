using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class InteractiveObject_Module : Virtual_Module
{
    [SerializeField] List<InteractiveObject> InterObjects;
    [SerializeField] List<bool> Change_Locks;
    [SerializeField] List<SkillCheckDifficulty> SkillCheck;

    public override void Setup()
    {
        Assert.IsTrue(InterObjects.Count == Change_Locks.Count || Change_Locks.Count == 0);
        Assert.IsTrue(Change_Locks.Count == SkillCheck.Count);
    }

    public override void Run()
    {
        for(int i = 0; i < InterObjects.Count; i++)
        {
            if(Change_Locks.Count != 0 && Change_Locks[i])
            {
                InterObjects[i].ChangeLockReq(SkillCheck[i]);
            }
        }
    }
}
