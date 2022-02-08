using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Michsky.UI.Frost;

public class PlayerAbilityUIMenu : MonoBehaviour
{
    [SerializeField] private Sprite DefaultIcon = null;

    private bool stats_enabled = false;
    private bool cursor_over = false;
    private float cursor_over_time = 0f;
    private AbilitiesController AC;
    private AbilityMenuController AMC;
    private Transform[] AbilitySlots = new Transform[4];
    private Ability hoverAbility;
    private float hoverAbilityPosX;

    // Start is called before the first frame update
    void Awake()
    {
        AC = GameObject.Find("Player").GetComponentInChildren<AbilitiesController>();
        AMC = GetComponentInParent<AbilityMenuController>();

        AbilitySlots[0] = transform.Find("Ability0");
        AbilitySlots[1] = transform.Find("Ability1");
        AbilitySlots[2] = transform.Find("Ability2");
        AbilitySlots[3] = transform.Find("Ability3");
    }

    private void OnDisable()
    {
        cursor_over = false;
        stats_enabled = false;
        cursor_over_time = 0;
    }

    public void UnslotAbility(int loc)
    {
        AC.SlotAbility(null, loc);
        AMC.UpdateAbilityPanel();
    }

    public void UnslotAbilityAnim(int loc)
    {
        if (AC.ReturnAbility(loc))
        {
            AbilitySlots[loc].GetComponent<UIElementSound>().PlayClickSound();
            AbilitySlots[loc].Find("RemoveIcon").GetComponent<Animator>().Play("Remove Out");
        }
        cursor_over = false;
        stats_enabled = false;
        cursor_over_time = 0;
        AMC.DisableStatPanel();
        AC.SlotAbility(null, loc);
        AMC.UpdateAbilityPanel();
    }

    public void HoverAbility(int loc)
    {
        if (AC.ReturnAbility(loc))
        {
            hoverAbility = AC.ReturnAbility(loc);
            hoverAbilityPosX = AbilitySlots[loc].transform.position.x;
            cursor_over = true;
            AbilitySlots[loc].GetComponent<UIElementSound>().PlayHoverSound();
            AbilitySlots[loc].Find("RemoveIcon").GetComponent<Animator>().Play("Remove In");
        }
    }

    public void LeaveHoverAbility(int loc)
    {
        if (AC.ReturnAbility(loc))
        {
            cursor_over = false;
            stats_enabled = false;
            cursor_over_time = 0;
            AMC.DisableStatPanel();
            AbilitySlots[loc].Find("RemoveIcon").GetComponent<Animator>().Play("Remove Out");
        }
    }

    private void Update()
    {
        for(int i = 0; i < 4; ++i)
        {
            AbilitySlots[i].Find("Icon").GetComponent<Image>().sprite =
            (AC.ReturnAbility(i) ? AC.ReturnAbility(i).ReturnBasicStats().Item2 : DefaultIcon);

            AbilitySlots[i].Find("Icon").GetComponent<Image>().color = AC.ReturnAbility(i) ? Color.white : Color.clear;
        }

        if (cursor_over)
        {
            cursor_over_time += Time.unscaledDeltaTime;
        }

        if (cursor_over_time >= STARTUP_DECLARATIONS.TIME_TO_DISPLAY_TOOLTIP)
        {
            if (!stats_enabled)
            {

                stats_enabled = true;
                var basicStats = hoverAbility.ReturnBasicStats();
                var advancedStats = hoverAbility.ReturnAdvStats();

                AMC.EnableBottomStatPanel(basicStats.Item1, advancedStats.Item2, hoverAbility.ReturnAdvStats().Item1, hoverAbilityPosX);
            }
        }
    }
}
