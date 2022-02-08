using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class NPC_Logic : MonoBehaviour
{
    [SerializeField] private NPC_Faction_Data[] Total_Faction_Data = new NPC_Faction_Data[STARTUP_DECLARATIONS.NPC_FactionsCount];

    private void Start()
    {
        Assert.IsTrue(Total_Faction_Data.Length == STARTUP_DECLARATIONS.NPC_FactionsCount);
        foreach(NPC_Faction_Data iter in Total_Faction_Data)
        {
            Assert.IsNotNull(iter);
        }
    }


    public NPC_Activity FindDest(List<NPC_FactionsEnum> facs, List<int> upperBounds)
    {
        List<NPC_FactionsEnum> NPC_FacEnum = FactionPicker(facs, upperBounds); //Pick Faction
        NPC_Activity Act;

        for (int iter = 0; iter < NPC_FacEnum.Count; ++iter)
        {
            Act = Total_Faction_Data[(int)NPC_FacEnum[iter]].Return_Activity(); //Get activities
            if (Act)
            {
                return Act;
            }
        }

        Assert.IsFalse(true, "No Activity Found!");

        return null;
    }

    public string ReturnFactionLine(NPCActivityFlag npcAF, List<NPC_FactionsEnum> facs, List<int> upperBounds)
    {
        NPC_FactionsEnum NPC_FacEnum = FactionPickerSingle(facs, upperBounds); //Pick Faction
        return Total_Faction_Data[(int)NPC_FacEnum].Return_Line(npcAF);
    }

    public string ReturnFactionCallforHelp(List<NPC_FactionsEnum> facs, List<int> upperBounds)
    {
        NPC_FactionsEnum NPC_FacEnum = FactionPickerSingle(facs, upperBounds); //Pick Faction
        return Total_Faction_Data[(int)NPC_FacEnum].Return_CallforHelp();
    }

    private List<NPC_FactionsEnum> FactionPicker(List<NPC_FactionsEnum> facs, List<int> upperBounds) //Returns a scrambled list
    {
        int rand = Random.Range(0, 100);
        int iter = 0;

        List<NPC_FactionsEnum> temp_list = new List<NPC_FactionsEnum>();

        while (rand > upperBounds[iter])
        {
            iter += 1;
        }

        temp_list.Add(facs[iter]);

        for(int i = iter; i < upperBounds.Count; ++i)
        {
            temp_list.Add(facs[i]);
        }

        for (int i = 0; i < iter; ++i)
        {
            temp_list.Add(facs[i]);
        }

        return temp_list;
    }

    private NPC_FactionsEnum FactionPickerSingle(List<NPC_FactionsEnum> facs, List<int> upperBounds) //Returns 1 faction
    {
        int rand = Random.Range(0, 100);
        int iter = 0;

        while (rand > upperBounds[iter])
        {
            iter += 1;
        }

        return facs[iter];
    }
}