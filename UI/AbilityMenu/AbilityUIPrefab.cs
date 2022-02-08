using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Michsky.UI.Frost;

public class AbilityUIPrefab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool stats_enabled = false;
    private bool cursor_over = false;
    private float cursor_over_time = 0f;
    private string abilityName;

    private AbilitiesController AC;
    private Ability global_abilityProperties;
    private AbilityMenuController AMC;
    private PlayerAbilityUIMenu unslotAbility;
    private UIElementSound sounds;

    private int abilitySlot = 0;


    public void Setup(Ability abilityProperties)
    {
        global_abilityProperties = abilityProperties;
        AMC = GameObject.Find("AbilitiesMenu").GetComponent<AbilityMenuController>();
        AC = GameObject.Find("Player").GetComponentInChildren<AbilitiesController>();
        unslotAbility = AMC.GetComponentInChildren<PlayerAbilityUIMenu>();
        sounds = GetComponent<UIElementSound>();

        abilityName = transform.Find("AbilityName").GetComponent<TextMeshProUGUI>().text = abilityProperties.ReturnBasicStats().Item1.ToUpper();
        transform.Find("Preview").Find("Icon").GetComponent<Image>().sprite = abilityProperties.ReturnBasicStats().Item2;
        transform.Find("AbilityLevel").Find("Level").GetComponent<TextMeshProUGUI>().text = abilityProperties.ReturnBasicStats().Item3.ToString();
        transform.Find("ExpBar").Find("Progress").GetComponent<Image>().fillAmount = abilityProperties.ReturnBasicStats().Item4;

        if (!abilityProperties.ReturnAccessible())
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
            transform.Find("Unknown").gameObject.SetActive(true);
        }
        else if (test_if_used())
        {
            transform.Find("Sub Menu").gameObject.SetActive(false);
            transform.Find("Sub Menu").gameObject.name = "Sub Menu Equip";
            transform.Find("Sub Menu Unequip").gameObject.SetActive(true);
            transform.Find("Sub Menu Unequip").gameObject.name = "Sub Menu";
            gameObject.SetActive(false);
            gameObject.SetActive(true);
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
        AMC.DisableStatPanel();
        cursor_over_time = 0;
    }

    private void OnDisable()
    {
        stats_enabled = false;
        AMC.DisableStatPanel();
    }

    public void Slot(int loc = 0)
    {
        sounds.PlayClickSound();
        AC.SlotAbility(global_abilityProperties, loc);
        AMC.UpdateAbilityPanel();
    }

    public void UnslotAbility()
    {
        sounds.PlayClickSound();
        unslotAbility.UnslotAbility(abilitySlot);
    }

    void Update()
    {

        if (cursor_over)
        {
            cursor_over_time += Time.unscaledDeltaTime;
        }

        if (cursor_over_time >= STARTUP_DECLARATIONS.TIME_TO_DISPLAY_TOOLTIP)
        {
            if (!stats_enabled)
            {
                stats_enabled = true;
                var basicStats = global_abilityProperties.ReturnBasicStats();
                var advancedStats = global_abilityProperties.ReturnAdvStats();

                if (global_abilityProperties.ReturnAccessible())
                {
                    AMC.EnableLeftStatPanel(basicStats.Item1, advancedStats.Item2, global_abilityProperties.ReturnAdvStats().Item1, transform.position.y);
                }
                else
                {
                    AMC.EnableLeftStatPanel("???", "", new List<(string, string)>(), transform.position.y);
                }
            }
        }
    }

    private bool test_if_used()
    {
        for (int i = 0; i < 4; ++i)
        {
            if (AC.ReturnAbility(i) == global_abilityProperties)
            {
                abilitySlot = i;
                return true;
            }
        }
        return false;
    }

    public string GetAbilityName()
    {
        return abilityName;
    }
}
