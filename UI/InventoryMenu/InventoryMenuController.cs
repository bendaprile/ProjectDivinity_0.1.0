using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UMA;
using UMA.CharacterSystem;

public class InventoryMenuController : MonoBehaviour
{
    [SerializeField] private InventoryUIPrefab ItemUIPrefab = null;
    [SerializeField] private GameObject Name_ValuePrefab = null;
    [SerializeField] private GameObject InventoryPanelContent = null;

    private GameObject InventoryNavMenu;

    //Animators
    private Animator weaponsButton;
    private Animator armorButton;
    private Animator consumableButton;
    private Animator miscButton;
    private Animator implantsButton;

    private TextMeshProUGUI LWeight, LNotes;

    private Transform ItemStatsTooltipLeft;
    private Transform ItemStatsTooltipRight;

    private Inventory InventoryScript;

    private ItemTypeEnum last_category = ItemTypeEnum.Weapon;

    private bool First_run = true;
    private bool leftStatEnabled;
    private bool rightStatEnabled;

    private void OnEnable()
    {
        if (First_run)
        {
            first_run_func();
        }

        MassDisable(true);
        DestoryInventoryPanel();
        leftStatEnabled = false;
        rightStatEnabled = false;

        switch (last_category)
        {
            case ItemTypeEnum.Weapon:
                WeaponsEnable(true);
                break;
            case ItemTypeEnum.Armor:
                ArmorEnable(true);
                break;
            case ItemTypeEnum.Consumable:
                ConsumableEnable(true);
                break;
            case ItemTypeEnum.Misc:
                MiscEnable(true);
                break;
            case ItemTypeEnum.Implant:
                ImplantsEnable(true);
                break;
        }
    }

    private void first_run_func()
    {
        LWeight = GameObject.Find("LWeight").GetComponent<TextMeshProUGUI>();
        LNotes = GameObject.Find("LNotes").GetComponent<TextMeshProUGUI>();
        ItemStatsTooltipLeft = transform.Find("Content").Find("ItemStatsTooltip_Left");
        ItemStatsTooltipRight = transform.Find("Content").Find("ItemStatsTooltip_Right");
        InventoryScript = GameObject.Find("Player").GetComponentInChildren<Inventory>();
        InventoryNavMenu = transform.Find("Content").Find("InventoryNavMenu").gameObject;
        weaponsButton = InventoryNavMenu.transform.Find("ButtonList").Find("WeaponsButton").GetComponent<Animator>();
        armorButton = InventoryNavMenu.transform.Find("ButtonList").Find("ArmorButton").GetComponent<Animator>();
        consumableButton = InventoryNavMenu.transform.Find("ButtonList").Find("ConsumableButton").GetComponent<Animator>();
        miscButton = InventoryNavMenu.transform.Find("ButtonList").Find("MiscButton").GetComponent<Animator>();
        implantsButton = InventoryNavMenu.transform.Find("ButtonList").Find("ImplantsButton").GetComponent<Animator>();
        First_run = false;
        StartCoroutine(ExecuteAfterTime(0.01f));
    }


    public void ConsumableEnable(bool overrideCheck = false)
    {
        if (last_category == ItemTypeEnum.Consumable && !overrideCheck) { return; }

        MassDisable();
        last_category = ItemTypeEnum.Consumable;
        UpdateInventoryPanel();
        consumableButton.Play("Hover to Pressed");
    }

    public void ArmorEnable(bool overrideCheck = false)
    {
        if (last_category == ItemTypeEnum.Armor && !overrideCheck) { return; }

        MassDisable();
        last_category = ItemTypeEnum.Armor;
        UpdateInventoryPanel();
        armorButton.Play("Hover to Pressed");
    }

    public void WeaponsEnable(bool overrideCheck = false)
    {
        if (last_category == ItemTypeEnum.Weapon && !overrideCheck) { return; }

        MassDisable();
        last_category = ItemTypeEnum.Weapon;
        UpdateInventoryPanel();
        weaponsButton.Play("Hover to Pressed");
    }

    public void MiscEnable(bool overrideCheck = false)
    {
        if (last_category == ItemTypeEnum.Misc && !overrideCheck) { return; }

        MassDisable();
        last_category = ItemTypeEnum.Misc;
        UpdateInventoryPanel();
        miscButton.Play("Hover to Pressed");
    }

    public void ImplantsEnable(bool overrideCheck = false)
    {
        if (last_category == ItemTypeEnum.Implant && !overrideCheck) { return; }

        MassDisable();
        last_category = ItemTypeEnum.Implant;
        UpdateInventoryPanel();
        implantsButton.Play("Hover to Pressed");
    }

    private void MassDisable(bool resetAll = false)
    {
        ResetButtons(resetAll);
        DisableStatPanel(true);
    }

    private void ResetButtons(bool resetAll)
    {
        if (resetAll)
        {
            weaponsButton.Play("Normal");
            armorButton.Play("Normal");
            consumableButton.Play("Normal");
            miscButton.Play("Normal");
            implantsButton.Play("Normal");
            return;
        }

        switch (last_category)
        {
            case ItemTypeEnum.Weapon:
                weaponsButton.Play("Pressed to Normal");
                break;
            case ItemTypeEnum.Armor:
                armorButton.Play("Pressed to Normal");
                break;
            case ItemTypeEnum.Consumable:
                consumableButton.Play("Pressed to Normal");
                break;
            case ItemTypeEnum.Misc:
                miscButton.Play("Pressed to Normal");
                break;
            case ItemTypeEnum.Implant:
                implantsButton.Play("Pressed to Normal");
                break;
        }
    }

    public void UpdateInventoryPanel()
    {
        DestoryInventoryPanel();
        PopulateInventoryPanel(last_category);
    }

    public void EnableLeftStatPanel(ItemTypeEnum itemType, string itemName, string description, string special, ItemQuality itemClass, List<(string, string)> data, float posY)
    {
        DisableStatPanel();
        leftStatEnabled = true;

        if (CompareItemIfEquipped(itemType)) { EnableRightStatPanel(itemName, description, special, itemClass, data); }

        ItemStatsTooltipLeft.gameObject.SetActive(true);
        ItemStatsTooltipLeft.GetComponent<Animator>().Play("In");

        ItemStatsTooltipLeft.position = new Vector3(ItemStatsTooltipLeft.position.x, posY - 50f, ItemStatsTooltipLeft.position.z);

        if (ItemStatsTooltipLeft.localPosition.y < -150f)
        {
            ItemStatsTooltipLeft.localPosition = new Vector3(ItemStatsTooltipLeft.localPosition.x, -200f, ItemStatsTooltipLeft.localPosition.z);
        }

        ItemStatsTooltipLeft.Find("Content").Find("ItemName").GetComponent<TextMeshProUGUI>().text = itemName.ToUpper();
        ItemStatsTooltipLeft.Find("Content").Find("Border").GetComponent<Image>().color = STARTUP_DECLARATIONS.itemQualityColors[itemClass];
        ItemStatsTooltipLeft.Find("Content").Find("Description").GetComponent<TextMeshProUGUI>().text = description;
        ItemStatsTooltipLeft.Find("Content").Find("Special").GetComponent<TextMeshProUGUI>().text = special;

        foreach ((string, string) item in data)
        {
            GameObject temp = Instantiate(Name_ValuePrefab, ItemStatsTooltipLeft.Find("Content").Find("Stats"));
            temp.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.Item1;
            temp.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = item.Item2;
        }
    }

    private bool CompareItemIfEquipped(ItemTypeEnum itemType)
    {
        // TODO: Compare Items. This is complicated so do it later maybe
        return false;
    }

    public void EnableRightStatPanel(string itemName, string description, string special, ItemQuality itemClass, List<(string, string)> data, bool disablePanel = true)
    {
        if (disablePanel) { DisableStatPanel(); }
        rightStatEnabled = true;

        ItemStatsTooltipRight.gameObject.SetActive(true);
        ItemStatsTooltipRight.GetComponent<Animator>().Play("In");

        Vector3 tooltipPos = new Vector3(ItemStatsTooltipRight.position.x, Input.mousePosition.y, ItemStatsTooltipRight.position.z);
        ItemStatsTooltipRight.position = tooltipPos;

        ItemStatsTooltipRight.Find("Content").Find("ItemName").GetComponent<TextMeshProUGUI>().text = itemName.ToUpper();
        ItemStatsTooltipRight.Find("Content").Find("Border").GetComponent<Image>().color = STARTUP_DECLARATIONS.itemQualityColors[itemClass];
        ItemStatsTooltipRight.Find("Content").Find("Description").GetComponent<TextMeshProUGUI>().text = description;
        ItemStatsTooltipLeft.Find("Content").Find("Special").GetComponent<TextMeshProUGUI>().text = special;

        foreach ((string, string) item in data)
        {
            GameObject temp = Instantiate(Name_ValuePrefab, ItemStatsTooltipRight.Find("Content").Find("Stats"));
            temp.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.Item1;
            temp.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = item.Item2;
        }
    }

    public void DisableStatPanel(bool startup = false)
    {
        foreach (Transform child in ItemStatsTooltipLeft.Find("Content").Find("Stats"))
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in ItemStatsTooltipRight.Find("Content").Find("Stats"))
        {
            Destroy(child.gameObject);
        }

        if (!startup && ItemStatsTooltipLeft.gameObject.activeInHierarchy && leftStatEnabled)
        {
            ItemStatsTooltipLeft.gameObject.GetComponent<Animator>().Play("Out");
            leftStatEnabled = false;
        }
        if (!startup && ItemStatsTooltipRight.gameObject.activeInHierarchy && rightStatEnabled)
        {
            ItemStatsTooltipRight.gameObject.GetComponent<Animator>().Play("Out");
            rightStatEnabled = false;
        }
    }

    private void DestoryInventoryPanel()
    {
        foreach (Transform item in InventoryPanelContent.transform)
        {
            Destroy(item.gameObject);
        }
    }

    private void PopulateInventoryPanel (ItemTypeEnum type)
    {
        List<GameObject> items = InventoryScript.ReturnItems(type);

        foreach (GameObject item in items)
        {
            InventoryUIPrefab UIFab = Instantiate(ItemUIPrefab, InventoryPanelContent.transform);
            UIFab.Setup(item);
        }
    }

    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        InventoryNavMenu.transform.Find("ButtonList").gameObject.SetActive(false);
        InventoryNavMenu.transform.Find("ButtonList").gameObject.SetActive(true);

        switch (last_category)
        {
            case ItemTypeEnum.Weapon:
                WeaponsEnable(true);
                break;
            case ItemTypeEnum.Armor:
                ArmorEnable(true);
                break;
            case ItemTypeEnum.Consumable:
                ConsumableEnable(true);
                break;
            case ItemTypeEnum.Misc:
                MiscEnable(true);
                break;
            case ItemTypeEnum.Implant:
                ImplantsEnable(true);
                break;
        }
    }

    private void Update()
    {
        LWeight.text = InventoryScript.RetunWeight().ToString();
        LNotes.text = InventoryScript.ReturnNotes().ToString();
    }
}
