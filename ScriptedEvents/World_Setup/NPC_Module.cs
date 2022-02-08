using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class NPC_Module : Virtual_Module
{
    [SerializeField] private Transform npc;
    [SerializeField] private npcStatus npcSetStatus;
    [SerializeField] private Transform npcDiaModifyStarting;
    [SerializeField] private bool npcControlModeBool;
    [SerializeField] private NPC_Control_Mode npcControlMode;

    [Space(35)]
    
    [SerializeField] private tri_gate npcImmortality = tri_gate.NoChange;

    [Space(35)]

    [SerializeField] private bool npcFactionSwapBool;
    [SerializeField] private FactionsEnum npcFactionSwap;

    [Space(35)]

    [SerializeField] [TextArea(3, 10)] private List<string> OverheadPri = new List<string>() { };


    public enum tri_gate { NoChange, Set, Unset };



    protected enum npcStatus { Nothing, Enable, Disable };


    public override void Setup()
    {
        Assert.IsNotNull(npc);
    }

    public override void Run()
    {
        if (npcSetStatus == npcStatus.Enable)
        {
            npc.gameObject.SetActive(true);
        }
        else if (npcSetStatus == npcStatus.Disable)
        {
            npc.gameObject.SetActive(false);
        }

        if (npcDiaModifyStarting)
        {
            npc.GetComponentInChildren<DiaRoot>().ModifyStarting(npcDiaModifyStarting);
        }

        if (npcControlModeBool)
        {
            npc.GetComponent<HumanoidMaster>().Set_ControlMode(npcControlMode);
        }

        if (npcImmortality == tri_gate.Set)
        {
            npc.GetComponentInChildren<EnemyHealth>().set_is_immortal(true);
        }
        else if(npcImmortality == tri_gate.Unset)
        {
            npc.GetComponentInChildren<EnemyHealth>().set_is_immortal(false);
        }

        if (npcFactionSwapBool)
        {
            npc.GetComponent<HumanoidMaster>().HumanoidSwitchFaction(npcFactionSwap);
        }

        for (int i = 0; i < OverheadPri.Count; ++i)
        {
            npc.GetComponentInChildren<DiaOverhead>().add_Priority_once_line(OverheadPri[i]);
        }
    }

}
