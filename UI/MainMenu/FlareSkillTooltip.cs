using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class FlareSkillTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float timeToDisplayStats = 0.1f;
    [SerializeField] private int skillNum = 0;

    private FlareCreationMenu flareMenu;
    private bool cursor_over = false;
    private bool stats_enabled = false;
    private float cursor_over_time = 0f;

    void Start()
    {
        flareMenu = GetComponentInParent<FlareCreationMenu>();
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
                flareMenu.EnableStatPanel(skillNum, transform.position);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cursor_over = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cursor_over = false;
        stats_enabled = false;
        flareMenu.DisableStatPanel();
        cursor_over_time = 0;
    }
}
