using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillsMenuTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] private float timeToDisplayStats = 0.1f;
    [SerializeField] private int skillNum = 0;

    private SkillMenuController skillMenu;
    private bool cursor_over = false;
    private bool stats_enabled = false;
    private float cursor_over_time = 0f;

    private Color PrevColor;

    void Start()
    {
        skillMenu = GetComponentInParent<SkillMenuController>();
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
                skillMenu.EnableStatPanel(skillNum, transform.position.y);
            }
        }
    }

    public void SetColor(Color color)
    {
        PrevColor = color;
        GetComponent<Image>().color = color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cursor_over = true;
        PrevColor = GetComponent<Image>().color;
        GetComponent<Image>().color = STARTUP_DECLARATIONS.goldColorTransparent;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cursor_over = false;
        stats_enabled = false;
        skillMenu.DisableStatPanel();
        cursor_over_time = 0;
        GetComponent<Image>().color = PrevColor;
    }
}
