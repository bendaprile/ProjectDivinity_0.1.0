using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Michsky.UI.Frost;

public class InteractiveUIPrefab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI HoverText;


    private InteractiveObjectMenuUI IOM;
    private GameObject item;
    private ItemMaster item_specs;

    private Inventory inv;


    private bool PlayerItem;


    private bool stats_enabled = false;
    private bool cursor_over = false;
    private float cursor_over_time = 0f;

    private int item_price;

    private bool CanAfford()
    {
        int PlayerNotes = FindObjectOfType<Inventory>().ReturnNotes();

        return PlayerNotes >= item_price;
    }


    public void ExternalSetup(GameObject item_in, bool is_Merchant, InteractiveObjectMenuUI IOM_in)
    {
        PlayerItem = false;

        Setup(item_in, is_Merchant, IOM_in);

        if (is_Merchant)
        {
            item_price = item_specs.ReturnBasicStats().Item1;
            if (CanAfford())
            {
                HoverText.text = "Purchase " + item_price.ToString();
            }
            else
            {
                HoverText.text = "Cannot Afford";
            }
        }
        else
        {
            item_price = 0;
            HoverText.text = "Take";
        }
    }

    public void PlayerSetup(GameObject item_in, bool is_Merchant, InteractiveObjectMenuUI IOM_in)
    {
        PlayerItem = true;
        Setup(item_in, is_Merchant, IOM_in);

        if (is_Merchant)
        {
            item_price = item_specs.ReturnBasicStats().Item1;
            HoverText.text = "Sell " + item_price.ToString();
        }
        else
        {
            item_price = 0;
            HoverText.text = "Store";
        }
    }

    private void Setup(GameObject item_in, bool is_Merchant, InteractiveObjectMenuUI IOM_in)
    {
        inv = FindObjectOfType<Inventory>();
        IOM = IOM_in;
        item = item_in;
        item_specs = item_in.GetComponent<ItemMaster>();

        transform.Find("ItemName").GetComponent<TextMeshProUGUI>().text = item_specs.ReturnBasicStats().Item3.ToUpper();
        transform.Find("Preview").Find("Icon").GetComponent<Image>().sprite = item_specs.ReturnBasicStats().Item4;
        transform.Find("QualityIndicator").GetComponent<Image>().color = GetQualityColor(item_specs.ReturnBasicStats().Item5);
        transform.Find("Weight").Find("WeightText").GetComponent<TextMeshProUGUI>().text = item_specs.ReturnBasicStats().Item2.ToString();
        transform.Find("Cost").Find("CostText").GetComponent<TextMeshProUGUI>().text = item_specs.ReturnBasicStats().Item1.ToString();
        transform.Find("Description").GetComponent<TextMeshProUGUI>().text = GetItemType(item_specs);


        if(is_Merchant)
        {
            item_price = item_specs.ReturnBasicStats().Item1;
            if (CanAfford())
            {
                HoverText.text = "Purchase " + item_price.ToString();
            }
            else
            {
                HoverText.text = "Cannot Afford";
            }
        }
        else
        {
            item_price = 0;
            HoverText.text = "Take";
        }
    }

    private Color32 GetQualityColor(ItemQuality itemClass)
    {
        return STARTUP_DECLARATIONS.itemQualityColors[itemClass];
    }

    private string GetItemType(ItemMaster item)
    {
        switch (item.ReturnItemType())
        {
            case ItemTypeEnum.Weapon:
                return item.GetComponent<Weapon>().ReturnWeaponType().ToString() + " Weapon";
            case ItemTypeEnum.Armor:
                return item.GetComponent<Armor>().returnArmorType().ToString() + " Armor";
            case ItemTypeEnum.Consumable:
                return "Consumable";
            case ItemTypeEnum.Misc:
                return "Misc";
            case ItemTypeEnum.Implant:
                return "Implant";
            default:
                Debug.LogError("Item Type Not Recognized!");
                return "";
        }
    }

    public void ButtonPressedTransfer()
    {
        if (PlayerItem)
        {
            inv.DeleteItem(item, false);
            inv.AddNotes(item_price);
            GetComponent<UIElementSound>().PlayClickSound();
            IOM.TransferButtonPressedReverse(item); //Must be last so notes are updated properly
        }
        else
        {
            if (FindObjectOfType<Inventory>().Attempt_NoteRemoval(item_price))
            {
                inv.AddItem(item);
                GetComponent<UIElementSound>().PlayClickSound();
                IOM.TransferButtonPressed(); //Must be last so notes are updated properly
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
        IOM.DisableStatPanel();
        cursor_over_time = 0;
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
                IOM.EnableStatPanel(item_specs.ReturnBasicStats().Item3, item_specs.ReturnAdvStats().Item2, item_specs.ReturnBasicStats().Item5, item_specs.ReturnAdvStats().Item1);
            }
        }
    }
}