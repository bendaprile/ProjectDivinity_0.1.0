using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Sleep : NPC_Activity
{
    private DayNightController DNC;

    private void Start()
    {
        Assert.IsFalse(Set_Duration);
        DNC = FindObjectOfType<DayNightController>();
    }

    protected override float ActivityLogic(float misc_time)
    {
        if (!DNC.isNight)
        {
            NPC_finished();
        }
        return -1;
    }
}
