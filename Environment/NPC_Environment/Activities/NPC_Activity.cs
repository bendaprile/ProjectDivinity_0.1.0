using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class NPC_Activity : MonoBehaviour
{
    [SerializeField] protected bool Set_Duration = true;
    [SerializeField] float Activity_duration = 10f;
    [SerializeField] NPCActivityFlag npcAF = NPCActivityFlag._NO_FLAG_;

    protected enum ActNPCStatus { Empty, Migrating, Using} //If empty the rest is garbage
    protected CurrentNPC_struct CurrentNPC;
    private List<NPC_Faction_Data> npcFD = new List<NPC_Faction_Data>();


    private void Awake()
    {
        CurrentNPC = new CurrentNPC_struct();
        CurrentNPC.status = ActNPCStatus.Empty;
    }


    public void set_npcFD(NPC_Faction_Data npcFD_in)
    {
        //Debug.Log(npcFD_in);
        npcFD.Add(npcFD_in);
    }



    public void UseLogic(Transform npc)
    {
        CurrentNPC.status = ActNPCStatus.Migrating;
        CurrentNPC.npcRef = npc;

        /*
        if (npcFD.Count == 2)
        {
            Debug.Log(("Use", gameObject.name, npcFD[0], npcFD[1]));
        }
        else
        {
            Debug.Log(("Use", gameObject.name, npcFD[0]));
        }
        */

        foreach (NPC_Faction_Data nFD in npcFD)
        {
            nFD.Use_Activity_slot(this);
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if(CurrentNPC.status != ActNPCStatus.Migrating)
        {
            return;
        }

        if (other.transform == CurrentNPC.npcRef)
        {
            CurrentNPC.status = ActNPCStatus.Using;
            CurrentNPC.exitTime = Time.time + Activity_duration;
            CurrentNPC.miscTime = Time.time;

            EnteredArea();
        }
    }



    protected void NPC_finished(bool Combat_Eject = false) 
    {
        if(CurrentNPC.status == ActNPCStatus.Empty)
        {
            return;
        }

        CurrentNPC.status = ActNPCStatus.Empty;

        /*
        if(npcFD.Count == 2)
        {
            Debug.Log((gameObject.name, npcFD[0], npcFD[1]));
        }
        else
        {
            Debug.Log((gameObject.name, npcFD[0]));
        }
        */

        foreach (NPC_Faction_Data nFD in npcFD)
        {
            nFD.Free_Activity_slot(this);
        }


        if (!Combat_Eject)
        {
            CurrentNPC.npcRef.GetComponent<NPC>().RandomTask();
        }

        CurrentNPC.npcRef.GetComponent<NPC>().Set_ActivityFlag(NPCActivityFlag._NO_FLAG_);
        LeftArea();
    } 

    protected virtual void EnteredArea()
    {
        CurrentNPC.npcRef.GetComponent<NPC>().Set_ActivityFlag(npcAF);
    }

    protected virtual void LeftArea()
    {
    }

    protected virtual float ActivityLogic(float misc_time)
    {
        return -1;
    }

    protected virtual void CalledOnce()
    {

    }

    private void Dead_Missing_Cleanup() //This might not be need
    {
        if(CurrentNPC.status == ActNPCStatus.Empty)
        {
            return;
        }

        HumanoidMaster HM = CurrentNPC.npcRef.GetComponent<HumanoidMaster>();
        if (!HM || HM.Return_Control_Mode() != NPC_Control_Mode.NPC_control || HM.GetComponentInChildren<EnemyHealth>().tag == "DeadEnemy")
        {
            NPC_finished();
        }
    }

    private void FixedUpdate()
    {
        CalledOnce();
        //Dead_Missing_Cleanup();

        if(CurrentNPC.status == ActNPCStatus.Using)
        {
            if (Set_Duration && CurrentNPC.exitTime <= Time.time)
            {
                NPC_finished();
            }
            else
            {
                float newMisc = ActivityLogic(CurrentNPC.miscTime);
                if (newMisc != -1)
                {
                    CurrentNPC.miscTime = newMisc;
                }
            }
        }
    }


    protected struct CurrentNPC_struct
    {
        public ActNPCStatus status;
        public Transform npcRef;
        public float exitTime;
        public float miscTime;
    }

}
