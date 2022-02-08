using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Michsky.UI.Frost;

public class InventoryUIPrefab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Inventory inv;
    private InventoryMenuController InventoryMenu;
    private PlayerInventoryUIMenu PIUIM;
    private Animator deleteWindow;
    private GameObject global_item;
    private UIElementSound sounds;

    private bool stats_enabled = false;
    private bool cursor_over = false;
    private float cursor_over_time = 0f;
    private string itemName;


    public void Setup(GameObject item)
    {
        global_item = item;
        inv = GameObject.Find("Player").GetComponentInChildren<Inventory>();
        InventoryMenu = GameObject.Find("InventoryMenu").GetComponent<InventoryMenuController>();
        deleteWindow = GameObject.Find("Delete Item").GetComponent<Animator>();
        sounds = GetComponent<UIElementSound>();

        ItemMaster itemProperties = item.GetComponent<ItemMaster>();
        itemName = transform.Find("ItemName").GetComponent<TextMeshProUGUI>().text = itemProperties.ReturnBasicStats().Item3.ToUpper();
        transform.Find("Preview").Find("Icon").GetComponent<Image>().sprite = itemProperties.ReturnBasicStats().Item4;
        transform.Find("QualityIndicator").GetComponent<Image>().color = GetQualityColor(itemProperties.ReturnBasicStats().Item5);
        transform.Find("Weight").Find("WeightText").GetComponent<TextMeshProUGUI>().text = itemProperties.ReturnBasicStats().Item2.ToString();
        transform.Find("Cost").Find("CostText").GetComponent<TextMeshProUGUI>().text = itemProperties.ReturnBasicStats().Item1.ToString();
        transform.Find("Description").GetComponent<TextMeshProUGUI>().text = GetItemType(itemProperties);

        PIUIM = GameObject.Find("PlayerEquippedItems").GetComponent<PlayerInventoryUIMenu>();

        if (itemProperties.LockForQuest)
        {
            transform.Find("Sub Menu").gameObject.SetActive(false);
            transform.Find("Sub Menu").gameObject.name = "Sub Menu NotLocked";
            transform.Find("Sub Menu Locked").gameObject.SetActive(true);
            transform.Find("Sub Menu Locked").gameObject.name = "Sub Menu";
            gameObject.SetActive(false);
            gameObject.SetActive(true);

            return;
        }

        if (itemProperties.ReturnItemType() == ItemTypeEnum.Armor
            || itemProperties.ReturnItemType() == ItemTypeEnum.Consumable
            || itemProperties.ReturnItemType() == ItemTypeEnum.Implant)
        {
            transform.Find("Sub Menu").gameObject.SetActive(false);
            transform.Find("Sub Menu").gameObject.name = "Sub Menu 2Equip";
            transform.Find("Sub Menu 1Equip").gameObject.SetActive(true);
            transform.Find("Sub Menu 1Equip").gameObject.name = "Sub Menu";
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
    }

    private string GetItemType(ItemMaster item)
    {
        switch(item.ReturnItemType())
        {
            case ItemTypeEnum.Weapon:
                return item.GetComponent<Weapon>().ReturnWeaponType().ToString() + " Weapon";
            case ItemTypeEnum.Armor:
                return item.GetComponent<Armor>().returnArmorType().ToString() + " Armor";
            case ItemTypeEnum.Consumable:
                Debug.LogWarning("Might want something more descriptive for Consumable description...");
                return "Consumable";
            case ItemTypeEnum.Misc:
                Debug.LogWarning("Might want something more descriptive for Misc description...");
                return "Misc";
            case ItemTypeEnum.Implant:
                Debug.LogWarning("Might want something more descriptive for Implant description...");
                return "Implant";
            default:
                Debug.LogError("Item Type Not Recognized!");
                return "";
        }
    }

    public Color32 GetQualityColor(ItemQuality itemClass)
    {
        return STARTUP_DECLARATIONS.itemQualityColors[itemClass];
    }

    public void AttemptDeleteItem()
    {
        sounds.PlayClickSound();
        deleteWindow.Play("Modal Window In");
        deleteWindow.GetComponent<DeleteItem>().SetInventoryUIPrefab(this);
    }

    public void DeleteSelectedItem()
    {
        inv.DeleteItem(global_item);
        InventoryMenu.UpdateInventoryPanel();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cursor_over = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cursor_over = false;
        stats_enabled = false;
        InventoryMenu.DisableStatPanel();
        cursor_over_time = 0;
    }

    private void OnDisable()
    {
        stats_enabled = false;
        InventoryMenu.DisableStatPanel();
    }

    public void Equip(int loc = 0)
    {
        sounds.PlayClickSound();
        ItemMaster itemProperties = global_item.GetComponent<ItemMaster>();
        if (itemProperties.ReturnItemType() == ItemTypeEnum.Weapon)
        {
            inv.EquipWeapon(global_item, loc);
        }
        else if (itemProperties.ReturnItemType() == ItemTypeEnum.Consumable)
        {
            inv.EquipConsumable(global_item);
        }
        else if(itemProperties.ReturnItemType() == ItemTypeEnum.Armor)
        {
            ArmorType armorLoc = ((Armor)itemProperties).returnArmorType();
            inv.EquipArmor(global_item, armorLoc);
        }
        else if(itemProperties.ReturnItemType() == ItemTypeEnum.Implant)
        {
            FindObjectOfType<MenuController>().SkillEnable();
        }

        PIUIM.UpdatePanel();
        InventoryMenu.UpdateInventoryPanel();
    }

    void Update()
    {
        if(cursor_over)
        {
            cursor_over_time += Time.unscaledDeltaTime;
        }

        if(cursor_over_time >= STARTUP_DECLARATIONS.TIME_TO_DISPLAY_TOOLTIP)
        {
            if (!stats_enabled)
            {
                ItemMaster itemProperties = global_item.GetComponent<ItemMaster>();
                stats_enabled = true;
                var basicStats = itemProperties.ReturnBasicStats();
                var advancedStats = itemProperties.ReturnAdvStats();
                InventoryMenu.EnableLeftStatPanel(itemProperties.ReturnItemType(), basicStats.Item3, advancedStats.Item2, advancedStats.Item3, basicStats.Item5, advancedStats.Item1, transform.position.y);
            }
        }
    }

    public string GetItemName()
    {
        return itemName;
    }
}