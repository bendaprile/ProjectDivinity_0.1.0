using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestsHolder : MonoBehaviour
{
    [SerializeField] private Sprite QuestObjectLocSprite;

    private List<GameObject> CompletedQuests = new List<GameObject>();
    private List<GameObject> ActiveQuests = new List<GameObject>();
    private GameObject FocusedQuestReference; //INCLUSIVE WITH ACTIVE QUEST

    private EventQueue eventQueue;
    private PlayerStats playerStats;
    private QuestHUD QHud;

    private List<GameObject> LocRefs = new List<GameObject>();

    void Start()
    {
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
        eventQueue = GameObject.Find("EventDisplay").GetComponent<EventQueue>();
        QHud = GameObject.Find("QuestDisplay").GetComponent<QuestHUD>();
    }

    private bool CheckIfQuestActivated(string QuestAttempt)
    {
        foreach(GameObject iter in CompletedQuests)
        {
            if(iter.GetComponent<QuestTemplate>().QuestName == QuestAttempt)
            {
                return true;
            }
        }

        foreach (GameObject iter in ActiveQuests)
        {
            if (iter.GetComponent<QuestTemplate>().QuestName == QuestAttempt)
            {
                return true;
            }
        }

        return false;
    }

    public void AddQuest(Transform Quest, Transform CustomStarting)
    {
        if (CheckIfQuestActivated(Quest.GetComponent<QuestTemplate>().name)) //Leave if the player already has this quest
        {
            return;
        }

        QuestTemplate tempQuest = Quest.GetComponent<QuestTemplate>();
        Quest.parent = transform;
        ActiveQuests.Add(Quest.gameObject);
        tempQuest.QuestStart(CustomStarting ? CustomStarting.gameObject :  null);

        if (FocusedQuestReference == null)
        {
            QuestSetFocus(Quest.gameObject);
        }
    }

    public void LoadQuest(Transform Quest, Transform Obj) //LOADING ONLY
    {
        QuestTemplate tempQuest = Quest.GetComponent<QuestTemplate>();
        Quest.parent = transform;

        if (tempQuest.questCategory == QuestCategory.Completed)
        {
            CompletedQuests.Add(Quest.gameObject);
        }
        else
        {
            ActiveQuests.Add(Quest.gameObject);
            tempQuest.QuestStart(Obj.gameObject);
        }
    }

    public List<GameObject> ReturnActiveQuests()
    {
        return ActiveQuests;
    }

    public List<GameObject> ReturnCompletedQuests()
    {
        return CompletedQuests;
    }

    public void QuestSetFocus(GameObject quest_in)
    {
        if(quest_in == FocusedQuestReference)
        {
            FocusedQuestReference = null;
            QHud.DisableDisplay();
        }
        else
        {
            FocusedQuestReference = quest_in;
            QuestTemplate FocusedTemp = FocusedQuestReference.GetComponent<QuestTemplate>();
            QHud.QuestDisplay(FocusedTemp);
        }
        DynamicLocLogic();
    }

    public void DynamicLocLogic() //Used by map and compass
    {
        for (int i = LocRefs.Count - 1; i >= 0; --i) //Clean
        {
            Destroy(LocRefs[i]);
            LocRefs.RemoveAt(i);
        }

        if (!FocusedQuestReference)
        {
            return;
        }

        Transform DynamicLocations = GameObject.Find("Dynamic Locations").transform;
        QuestTemplate tempScipt = FocusedQuestReference.GetComponent<QuestTemplate>();

        foreach ((Transform, float) loc in tempScipt.returnActiveLocs())
        {
            if(loc.Item1 == null)
            {
                continue;
            }
            GameObject temp = new GameObject();
            temp.AddComponent<LocationInfo>();
            temp.GetComponent<LocationInfo>().Dynamic_Setup(QuestObjectLocSprite, new Color (1, 0, 1, 1), loc.Item1);
            temp.GetComponent<LocationInfo>().radius = loc.Item2;
            temp.transform.parent = DynamicLocations;

            LocRefs.Add(temp);
        }
    }

    public GameObject ReturnFocus()
    {
        return FocusedQuestReference;
    }

    public void FullQuestCompleted(GameObject UniqueObject, bool failed = false)
    {
        int temp_loc = 0;
        foreach(GameObject iter in ActiveQuests)
        {
            if (iter == UniqueObject)
            {
                QuestTemplate QuestiterScript = UniqueObject.GetComponent<QuestTemplate>();
                CompletedQuests.Add(iter);
                ActiveQuests.Remove(iter);
                QuestiterScript.questCategory = QuestCategory.Completed;

                if (FocusedQuestReference == UniqueObject)
                {
                    FocusedQuestReference = null;
                    QHud.DisableDisplay();
                }

                if (!failed)
                {
                    playerStats.AddEXP(QuestiterScript.xp_reward);
                }    
                break;
            }
            temp_loc += 1;
        }
        DynamicLocLogic();
    }












    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



    public void CheckPlayerDeath()
    {
        for (int i = ActiveQuests.Count - 1; i >= 0; i--) //A quest could be completed
        {
            QuestTemplate TempActiveObj = ActiveQuests[i].GetComponent<QuestTemplate>();
            TempActiveObj.returnActiveObjective().DeathCheckObj();
        }
    }

    public void CheckFlags()
    {
        for (int i = ActiveQuests.Count - 1; i >= 0; i--) //A quest could be completed
        {
            QuestTemplate TempActiveObj = ActiveQuests[i].GetComponent<QuestTemplate>();
            TempActiveObj.FlagCheck(); //Check the Template too
            TempActiveObj.returnActiveObjective().FlagCheckObj();
        }
    }

    public void CheckFetchObjectives(GameObject item)
    {
        for(int i = ActiveQuests.Count - 1; i >= 0; i--) //A quest could be completed
        {
            QuestObjective TempActiveObj = ActiveQuests[i].GetComponent<QuestTemplate>().returnActiveObjective();
            TempActiveObj.FetchCheckObj(item);
        }
    }

    public void CheckExternalTaskCompletion(GameObject TaskRef) //Many types of quests
    {
        for (int i = ActiveQuests.Count - 1; i >= 0; i--) //A quest could be completed
        {
            QuestObjective TempActiveObj = ActiveQuests[i].GetComponent<QuestTemplate>().returnActiveObjective();
            TempActiveObj.ExternalCompletionTestObj(TaskRef);
        }
    }
}
