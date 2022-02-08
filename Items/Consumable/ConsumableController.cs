using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConsumableController : MonoBehaviour
{
    private AbilitiesController abCont;
    private Inventory Inventory;
    private Transform Quickslot_item;
    private Image ItemImage;
    private Image cooldownFill;

    void Start()
    {
        abCont = GameObject.Find("Player").GetComponentInChildren<AbilitiesController>();
        Inventory = GameObject.Find("Player").GetComponentInChildren<Inventory>();
        Quickslot_item = GameObject.Find("Quickslot_item").transform;
        ItemImage = Quickslot_item.Find("Image").GetComponent<Image>();
        cooldownFill = Quickslot_item.Find("CooldownDarken").GetComponent<Image>();
        cooldownFill.fillAmount = 0;
    }


    void Update()
    {
        GameObject equipedItem = Inventory.ReturnConsumable();
        if(equipedItem != null)
        {
            ItemImage.sprite = equipedItem.GetComponent<Consumable>().item_sprite;
            Quickslot_item.Find("Border").GetComponent<Image>().color = STARTUP_DECLARATIONS.itemQualityColors[equipedItem.GetComponent<Consumable>().ReturnBasicStats().Item5];
            ItemImage.color = Color.white;
            float cd_remaining = equipedItem.GetComponent<Ability>().cooldown_remaining;
            float cooldown = equipedItem.GetComponent<Ability>().cooldown;
            if (cd_remaining > 0)
            {
                Quickslot_item.Find("Text").GetComponent<TextMeshProUGUI>().text = cd_remaining.ToString("0");
                cooldownFill.fillAmount = cd_remaining / cooldown;
            }
            else
            {
                Quickslot_item.Find("Text").GetComponent<TextMeshProUGUI>().text = "";
                cooldownFill.fillAmount = 0;
            }
        }
        else
        {
            ItemImage.sprite = null;
            ItemImage.color = Color.clear;
            Quickslot_item.Find("Text").GetComponent<TextMeshProUGUI>().text = "";
            cooldownFill.fillAmount = 0;
            Quickslot_item.Find("Border").GetComponent<Image>().color = new Color32(130, 90, 36, 255);
        }
    }

    // TODO: Handle Input through InputManager and not direct key references
    public void HandleConsumables()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            GameObject temp = Inventory.ReturnConsumable();
            if(temp != null)
            {
                abCont.AttemptExternalAbilty(temp.GetComponent<Ability>());
            }
        }
    }
}
