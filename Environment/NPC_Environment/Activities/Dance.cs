using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dance : NPC_Activity
{
    protected override void EnteredArea()
    {
        base.EnteredArea();
        CurrentNPC.npcRef.GetComponentInParent<HumanoidMaster>().ExternalAnimation(anim: "dance");
    }

    protected override void LeftArea()
    {
        CurrentNPC.npcRef.GetComponentInParent<HumanoidMaster>().ExternalAnimation(false);
    }
}
