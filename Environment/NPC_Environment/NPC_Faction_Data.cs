using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class NPC_Faction_Data : MonoBehaviour
{
    [SerializeField] private List<Transform> External_Activity_Locs_single = new List<Transform>();
    [SerializeField] private List<Transform> External_Activity_Locs_DoubleArray = new List<Transform>(); //Must be empty transforms that each contain Activities
    [SerializeField] private List<ActivityBasedDia> ABD = new List<ActivityBasedDia>();

    [SerializeField] [TextArea(3, 10)] private List<string> FactionDia = new List<string>();
    [SerializeField] [TextArea(3, 10)] private List<string> CallforHelp = new List<string>();

    private List<NPC_Activity> Activity_Locs_free = new List<NPC_Activity>();

    private Queue<string> FactionDiaPriority = new Queue<string>();
    private Queue<string> FactionDiaPriority50 = new Queue<string>();
    private int iter = 0;
    private int help_iter = 0;

    private void Awake()
    {
        foreach(Transform trans in External_Activity_Locs_single)
        {
            Activity_Locs_free.Add(trans.GetComponentInChildren<NPC_Activity>());
        }

        for (int i = 0; i < External_Activity_Locs_DoubleArray.Count; ++i)
        {
            for (int j = 0; j < External_Activity_Locs_DoubleArray[i].childCount; ++j)
            {
                Activity_Locs_free.Add(External_Activity_Locs_DoubleArray[i].GetChild(j).GetComponentInChildren<NPC_Activity>());
            }
        }

        foreach(NPC_Activity npcAct in Activity_Locs_free)
        {
            npcAct.set_npcFD(this);
        }
    }

    public void Free_Activity_slot(NPC_Activity Act_in)
    {
        Activity_Locs_free.Add(Act_in);
    }

    public void Use_Activity_slot(NPC_Activity Act_in) //Should have for sure
    {
        Activity_Locs_free.Remove(Act_in);
    }

    public NPC_Activity Return_Activity()
    {
        Assert.IsTrue(Activity_Locs_free.Count > 0);
        NPC_Activity temp = Activity_Locs_free[0];
        return temp; //Logic is sent from the npc (IN NPC) to the specific activity
    }



    /////////////////////
    /// DIA BELOW ///////
    /////////////////////
    public void AddDiaPri(string str)
    {
        FactionDiaPriority.Enqueue(str);
    }

    public void AddDiaPri50(string str) //50% chance of saying line
    {
        FactionDiaPriority50.Enqueue(str);
    }

    public void AddDiaRotation(string str)
    {
        FactionDia.Add(str);
    }

    public string Return_Line(NPCActivityFlag npcAF)
    {
        if (FactionDiaPriority.Count > 0)
        {
            return FactionDiaPriority.Dequeue();
        }


        if(Random.value > 0.5 && FactionDiaPriority50.Count > 0)
        {
            return FactionDiaPriority50.Dequeue();
        }
        
        
        if(npcAF != NPCActivityFlag._NO_FLAG_)
        {
            for(int i = 0; i < ABD.Count; ++i)
            {
                if(npcAF == ABD[i].npcAF)
                {
                    return ABD[i].Return_Line();
                }
            }
        }


        string final_line = FactionDia[iter];
        iter = (iter + 1) % FactionDia.Count;
        return final_line;
    }

    public string Return_CallforHelp()
    {
        if(CallforHelp.Count == 0)
        {
            return "help";
        }

        string final_line = CallforHelp[help_iter];
        help_iter = (help_iter + 1) % CallforHelp.Count;

        return final_line;
    }
}
