using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class DiaRoot : MonoBehaviour
{
    [SerializeField] private Transform StartingTrans = null;
    [SerializeField] private string NPC_name = "";
    [SerializeField] private Sprite QuestIcon;
    [SerializeField] private Sprite MerchantIcon;

    public bool has_quest;
    public bool is_merchant;
    public int dia_quest_objective_count;

    Dictionary<DiaNpcLine, bool> LineChecked;

    private Transform DynamicLocations;
    private GameObject QuestLocationReference;
    private GameObject MechantLocationReference;


    private void Awake()
    {
        DynamicLocations = GameObject.Find("Dynamic Locations").transform;
        Assert.IsNotNull(transform.parent.GetComponent<EnemyTemplateMaster>(), ("DiaRoot exists without an ETM above it." + transform.parent));
        //The above check is needed because saving looks for ETMs. To save diaRoots there must be an ETM parent
    }

    private void OnEnable()
    {
        check_for_quest_merchant();
    }

    public void DiaRoot_deathLogic()
    {
        if (QuestLocationReference)
        {
            Destroy(QuestLocationReference);
        }

        if (MechantLocationReference)
        {
            Destroy(MechantLocationReference);
        }
    }

    public void check_for_quest_merchant()
    {
        has_quest = false;
        is_merchant = false;

        if (StartingTrans && StartingTrans.GetComponent<DiaNpcLine>()) //Otherwise no for sure
        {
            LineChecked = new Dictionary<DiaNpcLine, bool>();
            check_for_quest_helper(StartingTrans.GetComponent<DiaNpcLine>());
        }

        DynamicLocLogic();
    }

    private void DynamicLocLogic()
    {
        if (has_quest && !QuestLocationReference)
        {
            QuestLocationReference = new GameObject("QuestLoc");
            QuestLocationReference.AddComponent<LocationInfo>();
            QuestLocationReference.GetComponent<LocationInfo>().Dynamic_Setup(QuestIcon, STARTUP_DECLARATIONS.goldColor, transform);
            QuestLocationReference.transform.parent = DynamicLocations;
        }
        else if (!has_quest && QuestLocationReference)
        {
            Destroy(QuestLocationReference);
        }

        if(is_merchant && !MechantLocationReference)
        {
            MechantLocationReference = new GameObject("MechantLoc");
            MechantLocationReference.AddComponent<LocationInfo>();
            MechantLocationReference.GetComponent<LocationInfo>().Dynamic_Setup(MerchantIcon, Color.black, transform);
            MechantLocationReference.transform.parent = DynamicLocations;
        }
        else if (!is_merchant && MechantLocationReference)
        {
            Destroy(MechantLocationReference);
        }
    }

    private void check_for_quest_helper(DiaNpcLine Line_in)
    {
        if (Line_in.HasQuest())
        {
            has_quest = true;
        }

        foreach(DiaPlayerLine DPL in Line_in.GetComponentsInChildren<DiaPlayerLine>())
        {
            if (!DPL.Check_Accessible(null))
            {
                continue;
            }

            if (DPL.HasQuest())
            {
                has_quest = true;
            }

            if(DPL.Merchant != null)
            {
                is_merchant = true;
            }

            if (DPL.return_dest(false))
            {
                DiaNpcLine tempLine = DPL.return_dest(false).GetComponent<DiaNpcLine>();

                if (!LineChecked.ContainsKey(tempLine)) //This NPCLine has not been accessed before
                {
                    LineChecked.Add(tempLine, true);
                    check_for_quest_helper(tempLine);
                }
            }
        }

        if (Line_in.return_dest())
        {
            DiaNpcLine tempLine = Line_in.return_dest().GetComponent<DiaNpcLine>();
            if (!LineChecked.ContainsKey(tempLine)) //This NPCLine has not been accessed before
            {
                LineChecked.Add(tempLine, true);
                check_for_quest_helper(tempLine);
            }
        }
    }

    public void Modify_diaCount(bool add)
    {
        if (add)
        {
            dia_quest_objective_count += 1;
        }
        else
        {
            dia_quest_objective_count -= 1;
        }
    }


    public void ModifyStarting(Transform newStart)
    {
        StartingTrans = newStart;
        check_for_quest_merchant(); //Must be after
    }

    public Transform ReturnStarting()
    {
        return StartingTrans;
    }

    public string ReturnNPC_name()
    {
        return NPC_name;
    }

    public void SetName(string name_in)
    {
        //Debug.Log(name_in);
        if (NPC_name == "")
        {
            NPC_name = name_in;
        }
    }

}
