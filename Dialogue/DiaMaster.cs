using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DiaMaster : MonoBehaviour
{
    [SerializeField] protected Transform Dest = null; //Leave blank if ending dia
    [SerializeField] protected bool DelayLogic_afterDia = false;
    [SerializeField] protected GameObject FlagRef_set;
    [SerializeField] protected bool MakeHostile;
    [SerializeField] protected World_Setup World_Setup;

    [SerializeField] protected Transform Quest = null; //null if no quest (This is a quest to give)
    [SerializeField] protected Transform Quest_CustomStarting = null; //If null then just use starting obj

    protected GameObject QuestTaskReturn; //null if no quest,

    public void GameplayLogic(bool FromQueue)
    {
        if(DelayLogic_afterDia && !FromQueue)
        {
            FindObjectOfType<DiaParent>().Delayed_GamplayLogic_Enqueue(this);
            return;
        }

        if (World_Setup)
        {
            World_Setup.Setup();
        }

        QuestsHolder QH = GameObject.Find("QuestsHolder").GetComponent<QuestsHolder>();

        if (FlagRef_set)
        {
            GameObject.Find("Master Object").GetComponentInChildren<Zone_Flags>().SetFlag(FlagRef_set);
        }

        if (MakeHostile)
        {
            GetComponentInParent<EnemyTemplateMaster>().Set_customReputation(CustomReputation.PlayerEnemy);
            GetComponentInParent<EnemyTemplateMaster>().EnableAI(false);
        }

        if (QuestTaskReturn)
        {
            QH.CheckExternalTaskCompletion(QuestTaskReturn);
        }

        if (Quest)
        {
            QH.AddQuest(Quest, Quest_CustomStarting);
            Quest = null;
            Quest_CustomStarting = null;
            GetComponentInParent<DiaRoot>().check_for_quest_merchant(); //This quest will no longer be there
        }
    }



    public bool HasQuest()
    {
        return Quest;
    }

    public void mark_dia_for_quest(GameObject quest_in)
    {
        this.GetComponentsInParent<DiaRoot>(true)[0].Modify_diaCount(true); //Annoying but GetComponent cannot find an inactive component... true makes this the case here
        QuestTaskReturn = quest_in;
    }

    public void Unmark_dia_for_quest()
    {
        this.GetComponentsInParent<DiaRoot>(true)[0].Modify_diaCount(false); //Annoying but GetComponent cannot find an inactive component... true makes this the case here
        QuestTaskReturn = null;
    }
}
