using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.AI;

public class Sit : NPC_Activity
{
    [SerializeField] private Transform Object_Trans;
    [SerializeField] private Transform Sit_Loc;

    private Collider col;
    private NavMeshObstacle NMO;

    private void Start()
    {
        col = Object_Trans.GetComponent<Collider>();
        NMO = Object_Trans.GetComponent<NavMeshObstacle>();
        Assert.IsNotNull(col);
        Assert.IsNotNull(NMO);
        Assert.IsNotNull(Sit_Loc);
    }

    protected override void EnteredArea()
    {
        base.EnteredArea();
        CurrentNPC.npcRef.GetComponent<NPC>().Walk(Sit_Loc.position);
    }

    protected override void LeftArea()
    {
        CurrentNPC.npcRef.GetComponentInParent<HumanoidMaster>().ExternalAnimation(false);
    }

    protected override void CalledOnce()
    {
        col.enabled = CurrentNPC.status != ActNPCStatus.Using;
        NMO.enabled = CurrentNPC.status != ActNPCStatus.Using;
    }

    protected override float ActivityLogic(float misc_time)
    {       
        if (Vector2.Distance(new Vector2(CurrentNPC.npcRef.transform.position.x, CurrentNPC.npcRef.transform.position.z), new Vector2(Sit_Loc.position.x, Sit_Loc.position.z)) < 0.25f)
        {
            CurrentNPC.npcRef.GetComponentInParent<HumanoidMaster>().ExternalAnimation(anim: "sit");
            CurrentNPC.npcRef.parent.rotation = Sit_Loc.rotation;
        }

        return -1;
    }
}
