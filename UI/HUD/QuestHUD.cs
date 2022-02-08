using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestHUD : MonoBehaviour
{
    [SerializeField] private Transform QuestTaskPrefab;
    private TextMeshProUGUI mainText;
    private Transform secondaryText;
    //private Image questMarkerImage;
    private Color32 grayColor = new Color32(200, 200, 200, 255);

    private QuestTemplate FocusedTemp;
    private QuestObjective CurrentObj;

    void Start()
    {
        mainText = transform.Find("MainText").GetComponent<TextMeshProUGUI>();
        secondaryText = transform.Find("SecondaryText");
        //questMarkerImage = transform.Find("Icon").GetComponent<Image>();

        DisableDisplay();
    }

    public void QuestDisplay(QuestTemplate FocusedTemp_in)
    {
        DisableDisplay();

        FocusedTemp = FocusedTemp_in;
        CurrentObj = FocusedTemp.returnActiveObjective();
        mainText.text = FocusedTemp.QuestName;
        SetupTaskDetail(CurrentObj);
        //questMarkerImage.enabled = true;
    }

    public void DisableDisplay()
    {
        mainText.text = "";
        for (int i = secondaryText.childCount - 1; i >= 0; --i)
        {
            Destroy(secondaryText.GetChild(i).gameObject);
        }
        //questMarkerImage.enabled = false;
    }

    private void SetupTaskDetail(QuestObjective tempObj)
    {
        List<(TaskStatus, string)> taskList = tempObj.ReturnTasks();
        foreach ((TaskStatus, string) task in taskList)
        {
            Transform textPrefab = Instantiate(QuestTaskPrefab, secondaryText);

            TextMeshProUGUI TMP_temp = textPrefab.Find("Text").GetComponent<TextMeshProUGUI>();
            RectTransform Rect_temp = textPrefab.GetComponent<RectTransform>();

            TMP_temp.text = task.Item2;
            int line_count = (task.Item2.Length / 60) + 1;
            Rect_temp.sizeDelta = new Vector2(Rect_temp.sizeDelta.x, Rect_temp.sizeDelta.y * line_count);


            if (task.Item1 == TaskStatus.Completed)
            {
                textPrefab.Find("Text").GetComponent<TextMeshProUGUI>().color = grayColor;
                textPrefab.Find("Bullet").GetComponent<Image>().color = grayColor;
            }
            else if (task.Item1 == TaskStatus.Failed)
            {
                textPrefab.Find("Text").GetComponent<TextMeshProUGUI>().color = Color.red;
                textPrefab.Find("Bullet").GetComponent<Image>().color = Color.red;
            }
            else
            {
                textPrefab.Find("Text").GetComponent<TextMeshProUGUI>().color = Color.white;
                textPrefab.Find("Bullet").GetComponent<Image>().color = Color.white;
            }
        }
    }

    private void FixedUpdate() //Piece of shit redundent code, but I'm too tired
    {
        if (mainText.text != "")
        {
            if(CurrentObj != FocusedTemp.returnActiveObjective())
            {
                QuestDisplay(FocusedTemp);
            }


            List<(TaskStatus, string)> taskList = (FocusedTemp.returnActiveObjective()).ReturnTasks();
            for(int i = 0; i < taskList.Count; ++i)
            {
                if (taskList[i].Item1 == TaskStatus.Completed)
                {
                    secondaryText.GetChild(i).Find("Text").GetComponent<TextMeshProUGUI>().color = Color.gray;
                    secondaryText.GetChild(i).Find("Bullet").GetComponent<Image>().color = Color.gray;
                }
                else if (taskList[i].Item1 == TaskStatus.Failed)
                {
                    secondaryText.GetChild(i).Find("Text").GetComponent<TextMeshProUGUI>().color = Color.red;
                    secondaryText.GetChild(i).Find("Bullet").GetComponent<Image>().color = Color.red;
                }
                else
                {
                    secondaryText.GetChild(i).Find("Text").GetComponent<TextMeshProUGUI>().color = Color.white;
                    secondaryText.GetChild(i).Find("Bullet").GetComponent<Image>().color = Color.white;
                }
            }
        }

    }
}
