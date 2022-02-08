using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class WorldMenuController : MonoBehaviour
{
    [SerializeField] private UIController uiController = null;
    [SerializeField] private Transform mapRect = null;
    [SerializeField] private Transform QuestViewInfo = null;
    [SerializeField] private Transform DetailViewInfo = null;

    [SerializeField] private RectTransform QuestUIPrefab = null;
    [SerializeField] private RectTransform QuestTaskPrefab = null;
    [SerializeField] private RectTransform SpacerUIPrefab = null;
    [SerializeField] private GameObject QuestCategoryHeader = null;
    [SerializeField] private Transform questDetailPrefab = null;

    private bool First_run = true;
    //private List<GameObject> tempList = new List<GameObject>();
    private QuestsHolder QuestsHolder;
    private Dictionary<QuestCategory, Transform> questCategories = new Dictionary<QuestCategory, Transform>();

    public void GetDetails(QuestTemplate QT_in)
    {
        foreach (Transform iter in DetailViewInfo)
        {
            Destroy(iter.gameObject);
        }

        SetupComplex(QT_in);

        if (QT_in.questCategory != QuestCategory.Completed)
        {
            SetupTaskDetail(QT_in.returnActiveObjective());
        }

        Instantiate(SpacerUIPrefab, DetailViewInfo);

        List<QuestObjective> tempObjList = QT_in.returnCompletedObjectives();

        for (int i = tempObjList.Count - 1; i >= 0; i--)
        {
            SetupTaskDetail(tempObjList[i]);
            Instantiate(SpacerUIPrefab, DetailViewInfo);
        }
    }

    public void QuestSetFocus(QuestTemplate QT_in)
    {
        if (QT_in.questCategory == QuestCategory.Completed)
        {
            return;
        }

        QuestsHolder.QuestSetFocus(QT_in.gameObject);
        UpdateFocusUI(QuestsHolder.ReturnFocus());
    }

    private void SetupComplex(QuestTemplate tempQuest)
    {
        Transform tempQuestDetail = Instantiate(questDetailPrefab, DetailViewInfo);
        tempQuestDetail.Find("Content").Find("QuestName").GetComponent<TextMeshProUGUI>().text = tempQuest.QuestName;
        tempQuestDetail.Find("Content").Find("Level").Find("Text").GetComponent<TextMeshProUGUI>().text = "LEVEL " + tempQuest.suggestedLevel.ToString();
        tempQuestDetail.Find("Content").Find("XP").Find("Text").GetComponent<TextMeshProUGUI>().text = "XP REWARD: " + tempQuest.xp_reward.ToString();
        tempQuestDetail.Find("Content").Find("Description").GetComponent<TextMeshProUGUI>().text = tempQuest.questDescription;
        DetailViewInfo.GetComponent<Animator>().Play("In");
    }

    public void LeaveDetailFocus()
    {
        DetailViewInfo.GetComponent<Animator>().Play("Out");
    }

    private void SetupTaskDetail(QuestObjective tempObj)
    {
        List<(TaskStatus, string)> taskList = tempObj.ReturnTasks();
        foreach ((TaskStatus, string) task in taskList)
        {
            Transform textPrefab = Instantiate(QuestTaskPrefab, DetailViewInfo);

            TextMeshProUGUI TMP_temp = textPrefab.Find("Text").GetComponent<TextMeshProUGUI>();
            RectTransform Rect_temp = textPrefab.GetComponent<RectTransform>();

            TMP_temp.text = task.Item2;

            int line_count = (task.Item2.Length / 60) + 1;

            Rect_temp.sizeDelta = new Vector2(Rect_temp.sizeDelta.x, Rect_temp.sizeDelta.y * line_count);

            if (task.Item1 == TaskStatus.Completed)
            {
                textPrefab.Find("Text").GetComponent<TextMeshProUGUI>().color = STARTUP_DECLARATIONS.checkSuccessColor;
                textPrefab.Find("Bullet").GetComponent<Image>().color = STARTUP_DECLARATIONS.checkSuccessColor;
            }
            else if(task.Item1 == TaskStatus.Failed)
            {
                textPrefab.Find("Text").GetComponent<TextMeshProUGUI>().color = STARTUP_DECLARATIONS.checkFailColor;
                textPrefab.Find("Bullet").GetComponent<Image>().color = STARTUP_DECLARATIONS.checkFailColor;
            }
            else
            {
                textPrefab.Find("Text").GetComponent<TextMeshProUGUI>().color = Color.white;
                textPrefab.Find("Bullet").GetComponent<Image>().color = Color.white;
            }
        }
    }

    TextMeshProUGUI fdf;

    private void OnEnable()
    {
        if (First_run)
        {
            first_run_func();
        }
        InstantiateQuestCategories();
        SetupQuests();

        if (QuestsHolder.ReturnFocus())
        {
            GetDetails(QuestsHolder.ReturnFocus().GetComponent<QuestTemplate>());
        }

        mapRect.localPosition = new Vector3(1437.5f, 582.5f);
    }

    private void InstantiateQuestCategories()
    {
        foreach(QuestCategory questCategory in Enum.GetValues(typeof(QuestCategory)))
        {
            GameObject temp = Instantiate(QuestCategoryHeader, QuestViewInfo);
            temp.GetComponentInChildren<TextMeshProUGUI>().text = questCategory.ToString() + " Quests";
            questCategories.Add(questCategory, temp.transform);
        }
    }

    private void SetupQuests()
    {
        foreach (GameObject quest in QuestsHolder.ReturnActiveQuests())
        {
            Transform tempPrefab = Instantiate(QuestUIPrefab, questCategories[quest.GetComponent<QuestTemplate>().questCategory]);
            tempPrefab.GetComponent<QuestNameUIPrefab>().Setup(quest);
        }
        foreach (GameObject quest in QuestsHolder.ReturnCompletedQuests())
        {
            Transform tempPrefab = Instantiate(QuestUIPrefab, questCategories[QuestCategory.Completed]);
            tempPrefab.GetComponent<QuestNameUIPrefab>().Setup(quest);
        }
        foreach (Transform transform in questCategories.Values)
        {
            if (transform.GetComponentsInChildren<QuestNameUIPrefab>().Length == 0)
            {
                transform.gameObject.SetActive(false);
            }
        }
        UpdateFocusUI(QuestsHolder.ReturnFocus());
    }

    private void UpdateFocusUI(GameObject temp)
    {
        foreach(Transform child in QuestViewInfo)
        {
            foreach(QuestNameUIPrefab quest in GetComponentsInChildren<QuestNameUIPrefab>())
            {
                quest.CheckFocus(temp);
            }
        }
    }

    private void first_run_func()
    {
        QuestsHolder = GameObject.Find("QuestsHolder").GetComponent<QuestsHolder>();
        First_run = false;
    }

    private void OnDisable()
    {
        uiController.ReturnMapLocation(mapRect);
        questCategories.Clear();
        //tempList.Clear();

        foreach (Transform iter in QuestViewInfo)
        {
            Destroy(iter.gameObject);
        }

        foreach (Transform iter in DetailViewInfo)
        {
            Destroy(iter.gameObject);
        }
    }
}
