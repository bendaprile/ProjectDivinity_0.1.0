using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class StatUIPrefab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float timeToDisplayStats = 0.1f;
    TextMeshProUGUI StatName;
    TextMeshProUGUI StatValue;
    StatMenuController STC;

    string FinalValueHolder;
    List<(string, float)> AddDerivHolder;
    List<(string, float)> MultDerivHolder;
    (string, string) DescHolder;
    private bool cursor_over = false;
    private bool stats_enabled = false;
    private float cursor_over_time = 0f;

    public void Setup(string finalValue, List<(string, float)> Add_in, List<(string, float)> Mult_in, (string, string) desc)
    {
        STC = GetComponentInParent<StatMenuController>();
        StatName = transform.Find("StatName").GetComponent<TextMeshProUGUI>();
        StatValue = transform.Find("StatValue").GetComponent<TextMeshProUGUI>();

        StatName.text = desc.Item1;
        StatValue.text = finalValue;

        FinalValueHolder = finalValue;
        AddDerivHolder = Add_in;
        MultDerivHolder = Mult_in;
        DescHolder = desc;
    }

    void Update()
    {
        if (cursor_over)
        {
            cursor_over_time += Time.unscaledDeltaTime;
        }

        if (cursor_over_time >= timeToDisplayStats)
        {
            if (!stats_enabled)
            {
                stats_enabled = true;
                STC.EnableStatsInfoPanel(FinalValueHolder, AddDerivHolder, MultDerivHolder, DescHolder, transform.position.y);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cursor_over = true;
        GetComponent<Image>().color = STARTUP_DECLARATIONS.goldColorTransparent;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cursor_over = false;
        stats_enabled = false;
        STC.DisableStatPanel();
        cursor_over_time = 0;
        GetComponent<Image>().color = new Color32(0, 0, 0, 0);
    }
}
