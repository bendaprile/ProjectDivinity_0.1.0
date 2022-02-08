using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilitiesController : MonoBehaviour
{
    private Ability[] abilities = new Ability[4];
    private GameObject AbilitiesBar;
    private Transform[] BarChild = new Transform[4];
    private PlayerAnimationUpdater playerAnim;
    private WeaponController weaponController;

    private bool abilityInUse = false;



    public void SlotAbility(Ability ability_in, int loc)
    {
        if (ability_in)
        {
            ability_in.gameObject.SetActive(true);
        }

        if (abilities[loc])
        {
            abilities[loc].gameObject.SetActive(false);
        }


        abilities[loc] = ability_in;
        BarChild[loc].Find("Image").GetComponent<Image>().sprite = ability_in ? ability_in.ability_sprite : null;
        BarChild[loc].Find("Image").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        BarChild[loc].transform.Find("CooldownDarken").GetComponent<Image>().fillAmount = 0;
    }

    public Ability ReturnAbility(int loc)
    {
        return abilities[loc];
    }


    private void Start()
    {
        AbilitiesBar = GameObject.Find("Abilities Bar");
        weaponController = GetComponentInChildren<WeaponController>();
        playerAnim = GameObject.Find("Player").GetComponentInChildren<PlayerAnimationUpdater>();
        int i = 0;
        foreach (Transform child in AbilitiesBar.transform.Find("TextPanel"))
        {
            BarChild[i] = child;
            ++i;
        }
    }

    private void Update()
    {
        int i = 0;
        foreach(Transform child in AbilitiesBar.transform.Find("TextPanel"))
        {
            if (abilities[i])
            {
                float cd_remaining = abilities[i].cooldown_remaining;
                float cd = abilities[i].cooldown;
                if (cd_remaining > 0)
                {
                    child.Find("Text").GetComponent<TextMeshProUGUI>().text = cd_remaining.ToString("0");
                    child.Find("CooldownDarken").GetComponent<Image>().fillAmount = cd_remaining / cd;
                }
                else
                {
                    child.Find("Text").GetComponent<TextMeshProUGUI>().text = "";
                    child.Find("CooldownDarken").GetComponent<Image>().fillAmount = 0;
                }
            }
            ++i;
        }
    }

    public void DistributeXP(int xp_amount)
    {
        foreach(Ability ability in abilities)
        {
            if (ability)
            {
                ability.add_xp(xp_amount);
            }
        }
    }

    // TODO: Handle Input through InputManager and not direct key references
    public void HandleAbilities(float time_dia)
    {
        if (!abilityInUse && playerAnim.CanUpdateAnimation()) //Not rolling
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && abilities[0])
            {
                abilities[0].AttemptAttack();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && abilities[1])
            {
                abilities[1].AttemptAttack();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) && abilities[2])
            {
                abilities[2].AttemptAttack();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4) && abilities[3])
            {
                abilities[3].AttemptAttack();
            }
        }
    }

    public void AttemptExternalAbilty(Ability ability_in)
    {
        if (!abilityInUse && playerAnim.CanUpdateAnimation()) //Not rolling
        {
            ability_in.AttemptAttack();
        }
    }

    public bool isAbilityInUse()
    {
        return abilityInUse;
    }

    public void abilityUsed(bool startingAbility)
    {
        if (startingAbility)
        {
            weaponController.enableDisable(false); //here so that it is instant unlike if it was in the animation
        }
        else
        {
            weaponController.enableDisable(true); //here so that it is instant unlike if it was in the animation
        }

        abilityInUse = startingAbility;
    }
}
