using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractiveThing : MonoBehaviour
{
    [SerializeField] protected bool LOCK_UNTIL_QUEST = false;

    protected List<Transform> QuestIndicators = new List<Transform>();
    protected GameObject player;

    protected bool isCursorOverhead;

    protected PlayerInRadius PIR;
    protected UIController UIControl;
    protected MiscDisplay miscDisplay;


    protected GameObject QuestRef; //This should be loaded from the quest loading

    protected virtual void Awake()
    {
        foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
        {
            if (sprite.gameObject.name.Contains("QuestIndicator"))
            {
                QuestIndicators.Add(sprite.transform);
                sprite.gameObject.SetActive(false);
            }
        }

        PIR = GetComponentInChildren<PlayerInRadius>();
        player = GameObject.Find("Player");
        UIControl = GameObject.Find("UI").GetComponent<UIController>();
        miscDisplay = FindObjectOfType<MiscDisplay>();
    }

    public void SetQuest(GameObject QuestRef_in)
    {
        LOCK_UNTIL_QUEST = false;
        QuestRef = QuestRef_in;
    }

    public void ForceQuestEnd()
    {
        QuestRef = null;
    }


    public virtual void CursorOverObject()
    {
        isCursorOverhead = true;
    }

    public virtual void CursorLeftObject()
    {
        isCursorOverhead = false;
    }

    protected virtual void ActivateLogic()
    {

    }

    protected virtual void AdditionalUpdateLogic()
    {

    }

    protected void QuestActiveLogic()
    {
        if (QuestRef)
        {
            GameObject.Find("QuestsHolder").GetComponent<QuestsHolder>().CheckExternalTaskCompletion(QuestRef);
            QuestRef = null;
        }
    }

    protected virtual void Update()
    {
        if (LOCK_UNTIL_QUEST)
        {
            foreach (Transform questIndicator in QuestIndicators)
            { 
                questIndicator.gameObject.SetActive(false);
            }
            return;
        }

        AdditionalUpdateLogic();

        if ((isCursorOverhead && PIR.isTrue))
        {
            ActivateLogic();
        }

        if (QuestIndicators.Count > 0) //There might be dia without an indicator like using a computer
        {
            if (HasTask())
            {
                foreach (Transform questIndicator in QuestIndicators)
                {
                    questIndicator.gameObject.SetActive(true);
                    questIndicator.GetComponent<SpriteRenderer>().color = Color.white;
                    rotate(questIndicator);
                }
            }
            else if (StartsQuest())
            {
                foreach (Transform questIndicator in QuestIndicators)
                {
                    questIndicator.gameObject.SetActive(true);
                    questIndicator.GetComponent<SpriteRenderer>().color = STARTUP_DECLARATIONS.goldColor;
                    rotate(questIndicator);
                }
            }
            else if(IsMerchant())
            {
                foreach (Transform questIndicator in QuestIndicators)
                {
                    questIndicator.gameObject.SetActive(true);
                    questIndicator.GetComponent<SpriteRenderer>().color = Color.blue;
                    rotate(questIndicator);
                }
            }
            else
            {
                foreach (Transform questIndicator in QuestIndicators)
                {
                    questIndicator.gameObject.SetActive(false);
                }
            }
        }
    }

    protected virtual bool HasTask()
    {
        return QuestRef != null;
    }

    protected virtual bool StartsQuest()
    {
        return false;
    }

    protected virtual bool IsMerchant()
    {
        return false;
    }

    public virtual void rotate(Transform RotateObj)
    {
        RotateObj.transform.eulerAngles = new Vector3(RotateObj.transform.eulerAngles.x,  Camera.main.transform.rotation.eulerAngles.y, RotateObj.transform.eulerAngles.z);
    }
}
