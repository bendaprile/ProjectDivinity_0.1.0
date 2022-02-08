using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class QuestTemplate : MonoBehaviour
{
    [SerializeField] private List<GameObject> Any_Flag_Fail;
    [SerializeField] private GameObject Quest_Set_Flag_Finish;

    [SerializeField] private GameObject StartingObjective;

    [SerializeField] private World_Setup Setup_Upon_Failure;
    [SerializeField] private World_Setup Setup_Upon_Completion;

    public string QuestName;
    public int xp_reward;
    [TextArea(3, 10)] public string questDescription = "";
    public int suggestedLevel = 1;
    public QuestCategory questCategory = QuestCategory.Miscellaneous;

    private GameObject ActiveObjective;
    private List<GameObject> CompletedObjectiveList = new List<GameObject>();

    private EventQueue eventQueue;
    private QuestsHolder questsHolder;
    private Zone_Flags ZF;

    EventData tempEvent = new EventData();


    private void Awake()
    {
        eventQueue = GameObject.Find("EventDisplay").GetComponent<EventQueue>();
        questsHolder = GameObject.Find("QuestsHolder").GetComponent<QuestsHolder>();
        ZF = FindObjectOfType<Zone_Flags>();

        if (QuestName.Length > 27) { Debug.LogError("Either change QuestUI or Length of this QuestName. This will not look good in UI"); }
    }

    public QuestObjective returnActiveObjective()
    {
        //Debug.Log(gameObject.name);
        return ActiveObjective.GetComponent<QuestObjective>();
    }

    public List<QuestObjective> returnCompletedObjectives()
    {
        List<QuestObjective> temp = new List<QuestObjective>();
        foreach(GameObject iter in CompletedObjectiveList)
        {
            temp.Add(iter.GetComponent<QuestObjective>());
        }
        return temp;
    }

    public List<(Transform, float)> returnActiveLocs()
    {
        return ActiveObjective.GetComponent<QuestObjective>().ReturnLocs();
    }

    public void QuestStart(GameObject ForceStart = null) //StartingObj is for Debug and Saving for now
    {
        if (ForceStart)
        {
            ActiveObjective = ForceStart;
        }
        else
        {
            ActiveObjective = StartingObjective;

            /////
            tempEvent.Setup(EventTypeEnum.QuestStarted, QuestName);
            eventQueue.AddEvent(tempEvent);
            /////
        }

        FlagCheck();
        ActiveObjective.GetComponent<QuestObjective>().initialize(); //Has to be after hidden Obj are setup or it can instantly finish (THIS WAS A REALLY ANNOYING BUG REMEMBER THIS)
    }

    public void FlagCheck()
    {
        foreach (GameObject Flag in Any_Flag_Fail)
        {
            if (Flag && ZF.CheckFlag(Flag.name)) //Entire quest failed
            {
                CompletedObjectiveList.Add(ActiveObjective); //Show the objective before failure

                /////
                tempEvent.Setup(EventTypeEnum.QuestCompleted, QuestName);
                eventQueue.AddEvent(tempEvent);
                /////

                questsHolder.FullQuestCompleted(gameObject, true);
                returnActiveObjective().ExternalCleanup();

                if (Setup_Upon_Failure)
                {
                    Setup_Upon_Failure.Setup();
                }
                break;
            }
        }
    }

    public void ObjectiveFinished(GameObject Dest)
    {
        CompletedObjectiveList.Add(ActiveObjective);

        /////
        tempEvent.Setup(EventTypeEnum.QuestObjCompleted, GetComponentInParent<QuestTemplate>().QuestName);
        eventQueue.AddEvent(tempEvent);
        /////
        ///

        if (Dest)
        {
            ActiveObjective = Dest;
            ActiveObjective.GetComponent<QuestObjective>().initialize();
            questsHolder.DynamicLocLogic(); //Needs to be after ActiveObjective is modified
        }
        else
        {
            ActiveObjective = null;

            /////
            tempEvent.Setup(EventTypeEnum.QuestCompleted, QuestName);
            eventQueue.AddEvent(tempEvent);
            /////



            questsHolder.FullQuestCompleted(gameObject);
            if (Quest_Set_Flag_Finish)
            {
                ZF.SetFlag(Quest_Set_Flag_Finish); //Has to be after
            }

            if (Setup_Upon_Completion)
            {
                Setup_Upon_Completion.Setup();
            }
        }
    }

    public void SingleTaskCompletedUpperLogic()
    {
        questsHolder.DynamicLocLogic();  //Currently does everything again (Called twice when objective is finished) (This could be task specific)
    }
}
