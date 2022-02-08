using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class NPC : MonoBehaviour
{
    [SerializeField] private Transform SleepingLocation;
    [SerializeField] private List<Transform> CustomActivities; 
    [SerializeField] private List<NPC_FactionsEnum> NPC_Facs;
    [SerializeField] private List<int> NPC_Facs_Percentages_input;

    private List<int> NPC_Facs_Percentages = new List<int>(); //This holdes the upperbound e.g. 25 30 30 15 => 25,55,85,100

    private HumanoidMaster HM;
    private NPC_Logic NL;
    private DayNightController DNC;

    private NPCActivityFlag ActivityFlag = NPCActivityFlag._NO_FLAG_; //Used for activity based Dia

    private Vector3 WalkDest;
    private float npcSpeed;


    private const float Speed = 3f;
    private const float SpeedVariation = .1f; // (x-SpeedVariation) to (x+SpeedVariation)


    public void Setup(List<NPC_FactionsEnum> NPC_Facs_in, List<int> NPC_Facs_Percentages_input_in)
    {
        NPC_Facs = NPC_Facs_in;
        NPC_Facs_Percentages_input = NPC_Facs_Percentages_input_in;
        SetupLogic();
    }

    private void Awake()
    {
        Assert.IsNotNull(GetComponent<Collider>());
        SetupLogic();
    }


    private void SetupLogic()
    {
        npcSpeed = Speed * (1 + (2 * SpeedVariation * (Random.value - 0.5f)));
        int total = 0;
        for(int i = 0; i < NPC_Facs_Percentages_input.Count; ++i)
        {
            total += NPC_Facs_Percentages_input[i];
            NPC_Facs_Percentages.Add(total);
        }

        if(NPC_Facs.Count != 0)
        {
            Assert.IsTrue(total == 100);
            Assert.IsTrue(CustomActivities.Count == 0);
        }
        Assert.IsTrue(NPC_Facs.Count == NPC_Facs_Percentages_input.Count);


        HM = GetComponentInParent<HumanoidMaster>();
        NL = FindObjectOfType<NPC_Logic>();
        DNC = FindObjectOfType<DayNightController>();
    }

    public void Set_ActivityFlag(NPCActivityFlag set)
    {
        ActivityFlag = set;
    }

    public string ReturnFactionDiaLine()
    {
        return NL.ReturnFactionLine(ActivityFlag, NPC_Facs, NPC_Facs_Percentages);
    }

    public string ReturnCallforHelp()
    {
        if(NPC_Facs.Count == 0)
        {
            //return "Help";
        }
        return NL.ReturnFactionCallforHelp(NPC_Facs, NPC_Facs_Percentages);
    }

    public void RandomTask()
    {
        if (HM.Return_Control_Mode() != NPC_Control_Mode.NPC_control || (NPC_Facs.Count == 0 && CustomActivities.Count == 0))
        {
            return;
        }

        NPC_Activity npcAct;
        if (SleepingLocation && DNC.isNight)
        {
            npcAct = SleepingLocation.GetComponentInChildren<NPC_Activity>();
        }
        else if(CustomActivities.Count > 0)
        {
            npcAct = CustomActivities[Random.Range(0, CustomActivities.Count)].GetComponentInChildren<NPC_Activity>();
        }
        else
        {
            npcAct = NL.FindDest(NPC_Facs, NPC_Facs_Percentages);
        }

        Walk(npcAct.transform.position);
        npcAct.UseLogic(transform);
    }

    public void Walk(Vector3 Location)
    {
        WalkDest = Location;
    }

    private void FixedUpdate()
    {
        Vector3 temp = WalkDest - transform.position;


        if (HM.Return_Control_Mode() != NPC_Control_Mode.NPC_control)
        {
            return;
        }
        HM.MoveToDest(WalkDest, false, npcSpeed);
    }
}
