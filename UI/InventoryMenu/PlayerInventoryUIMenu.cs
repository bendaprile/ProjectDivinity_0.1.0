using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Michsky.UI.Frost;

public class PlayerInventoryUIMenu : MonoBehaviour
{
    [SerializeField] private float TimeToDisplayStats = 1f;
    private Sprite headSlotSprite = null;
    private Sprite chestSlotSprite = null;
    private Sprite legSlotSprite = null;
    private Sprite consumableSlotSprite = null;
    private Sprite mainWeaponSlotSprite = null;
    private Sprite secondaryWeaponSlotSprite = null;

    Transform HeadSlot;
    Transform ChestSlot;
    Transform LegsSlot;
    Transform ConsumableSlot;
    Transform MainWeaponSlot;
    Transform SecondaryWeaponSlot;

    Inventory inv;
    InventoryMenuController InventoryMenu;


    private bool stats_enabled = false;
    private bool cursor_over = false;
    private float cursor_over_time = 0f;
    private int i_storage;

    private Color32 backgroundColor;
    private Color iconColor;

    void Awake()
    {
        HeadSlot = transform.Find("HeadSlot");
        ChestSlot = transform.Find("ChestSlot");
        LegsSlot = transform.Find("LegsSlot");
        ConsumableSlot = transform.Find("ConsumableSlot");
        MainWeaponSlot = transform.Find("MainWeaponSlot");
        SecondaryWeaponSlot = transform.Find("SecondaryWeaponSlot");

        headSlotSprite = HeadSlot.Find("Icon").GetComponent<Image>().sprite;
        chestSlotSprite = ChestSlot.Find("Icon").GetComponent<Image>().sprite;
        legSlotSprite = LegsSlot.Find("Icon").GetComponent<Image>().sprite;
        consumableSlotSprite = ConsumableSlot.Find("Icon").GetComponent<Image>().sprite;
        mainWeaponSlotSprite = MainWeaponSlot.Find("Icon").GetComponent<Image>().sprite;
        secondaryWeaponSlotSprite = SecondaryWeaponSlot.Find("Icon").GetComponent<Image>().sprite;

        InventoryMenu = GameObject.Find("InventoryMenu").GetComponent<InventoryMenuController>();
        inv = GameObject.Find("Player").GetComponentInChildren<Inventory>();
        backgroundColor = HeadSlot.Find("Background").GetComponent<Image>().color;
        iconColor = HeadSlot.Find("Icon").GetComponent<Image>().color;
        UpdatePanel();
    }

    public void unequipArmor(int armorType)
    {
        switch ((ArmorType)armorType)
        {
            case ArmorType.Head:
                HeadSlot.GetComponent<UIElementSound>().PlayClickSound();
                CursorLeftEquipped(0);
                break;
            case ArmorType.Chest:
                ChestSlot.GetComponent<UIElementSound>().PlayClickSound();
                CursorLeftEquipped(1);
                break;
            case ArmorType.Legs:
                LegsSlot.GetComponent<UIElementSound>().PlayClickSound();
                CursorLeftEquipped(2);
                break;
        }
        inv.UnEquipArmor((ArmorType)armorType);
        UpdatePanel();
        InventoryMenu.UpdateInventoryPanel();
    }

    public void unequipMain()
    {
        MainWeaponSlot.GetComponent<UIElementSound>().PlayClickSound();
        CursorLeftEquipped(3);
        inv.UnEquipWeapon(0);
        UpdatePanel();
        InventoryMenu.UpdateInventoryPanel();
    }

    public void unequipSecondary()
    {
        SecondaryWeaponSlot.GetComponent<UIElementSound>().PlayClickSound();
        CursorLeftEquipped(4);
        inv.UnEquipWeapon(1);
        UpdatePanel();
        InventoryMenu.UpdateInventoryPanel();
    }

    public void unequipCons()
    {
        ConsumableSlot.GetComponent<UIElementSound>().PlayClickSound();
        CursorLeftEquipped(5);
        inv.UnEquipConsumable();
        UpdatePanel();
        InventoryMenu.UpdateInventoryPanel();
    }

    public void CursorOverEquipped(int i)
    {
        cursor_over = true;
        i_storage = i;
    }

    public void CursorLeftEquipped(int i)
    {
        cursor_over = false;
        stats_enabled = false;
        InventoryMenu.DisableStatPanel();
        cursor_over_time = 0;


        if ((i_storage == 0 && inv.ReturnArmor(ArmorType.Head) == null)
            || (i_storage == 1 && inv.ReturnArmor(ArmorType.Chest) == null)
            || (i_storage == 2 && inv.ReturnArmor(ArmorType.Legs) == null)
            || (i_storage == 3 && inv.ReturnWeapon(0) == null)
            || (i_storage == 4 && inv.ReturnWeapon(1) == null)
            || (i_storage == 5 && inv.ReturnConsumable() == null)) {
            return;
        }

        if (i == 0)
        {
            HeadSlot.Find("RemoveIcon").GetComponent<Animator>().Play("Remove Out");
        }
        else if (i == 1)
        {
            ChestSlot.Find("RemoveIcon").GetComponent<Animator>().Play("Remove Out");
        }
        else if (i == 2)
        {
            LegsSlot.Find("RemoveIcon").GetComponent<Animator>().Play("Remove Out");
        }
        else if (i == 3)
        {
            MainWeaponSlot.Find("RemoveIcon").GetComponent<Animator>().Play("Remove Out");
        }
        else if (i == 4)
        {
            SecondaryWeaponSlot.Find("RemoveIcon").GetComponent<Animator>().Play("Remove Out");
        }
        else if (i == 5)
        {
            ConsumableSlot.Find("RemoveIcon").GetComponent<Animator>().Play("Remove Out");
        }
    }

    private void OnDisable()
    {
        cursor_over = false;
        stats_enabled = false;
        cursor_over_time = 0;
    }

    public void UpdatePanel()
    {
        HeadSlot.Find("Border").GetComponent<Image>().color =
            (inv.ReturnArmor(ArmorType.Head) == null) ? backgroundColor : STARTUP_DECLARATIONS.itemQualityColors[inv.ReturnArmor(ArmorType.Head).GetComponent<ItemMaster>().ReturnBasicStats().Item5];
        ChestSlot.Find("Border").GetComponent<Image>().color =
            (inv.ReturnArmor(ArmorType.Chest) == null) ? backgroundColor : STARTUP_DECLARATIONS.itemQualityColors[inv.ReturnArmor(ArmorType.Chest).GetComponent<ItemMaster>().ReturnBasicStats().Item5];
        LegsSlot.Find("Border").GetComponent<Image>().color =
            (inv.ReturnArmor(ArmorType.Legs) == null) ? backgroundColor : STARTUP_DECLARATIONS.itemQualityColors[inv.ReturnArmor(ArmorType.Legs).GetComponent<ItemMaster>().ReturnBasicStats().Item5];
        ConsumableSlot.Find("Border").GetComponent<Image>().color =
            (inv.ReturnConsumable() == null) ? backgroundColor : STARTUP_DECLARATIONS.itemQualityColors[inv.ReturnConsumable().GetComponent<ItemMaster>().ReturnBasicStats().Item5];
        MainWeaponSlot.Find("Border").GetComponent<Image>().color =
            (inv.ReturnWeapon(0) == null) ? backgroundColor : STARTUP_DECLARATIONS.itemQualityColors[inv.ReturnWeapon(0).GetComponent<ItemMaster>().ReturnBasicStats().Item5];
        SecondaryWeaponSlot.Find("Border").GetComponent<Image>().color =
            (inv.ReturnWeapon(1) == null) ? backgroundColor : STARTUP_DECLARATIONS.itemQualityColors[inv.ReturnWeapon(1).GetComponent<ItemMaster>().ReturnBasicStats().Item5];

        HeadSlot.Find("Icon").GetComponent<Image>().sprite =
            (inv.ReturnArmor(ArmorType.Head) == null) ? headSlotSprite : inv.ReturnArmor(ArmorType.Head).GetComponent<ItemMaster>().ReturnBasicStats().Item4;
        ChestSlot.Find("Icon").GetComponent<Image>().sprite =
            (inv.ReturnArmor(ArmorType.Chest) == null) ? chestSlotSprite : inv.ReturnArmor(ArmorType.Chest).GetComponent<ItemMaster>().ReturnBasicStats().Item4;
        LegsSlot.Find("Icon").GetComponent<Image>().sprite =
            (inv.ReturnArmor(ArmorType.Legs) == null) ? legSlotSprite : inv.ReturnArmor(ArmorType.Legs).GetComponent<ItemMaster>().ReturnBasicStats().Item4;
        ConsumableSlot.Find("Icon").GetComponent<Image>().sprite =
            (inv.ReturnConsumable() == null) ? consumableSlotSprite : inv.ReturnConsumable().GetComponent<ItemMaster>().ReturnBasicStats().Item4;
        MainWeaponSlot.Find("Icon").GetComponent<Image>().sprite =
            (inv.ReturnWeapon(0) == null) ? mainWeaponSlotSprite : inv.ReturnWeapon(0).GetComponent<ItemMaster>().ReturnBasicStats().Item4;
        SecondaryWeaponSlot.Find("Icon").GetComponent<Image>().sprite =
            (inv.ReturnWeapon(1) == null) ? secondaryWeaponSlotSprite : inv.ReturnWeapon(1).GetComponent<ItemMaster>().ReturnBasicStats().Item4;

        HeadSlot.Find("Icon").GetComponent<Image>().color =
           (inv.ReturnArmor(ArmorType.Head) == null) ? iconColor : Color.white;
        ChestSlot.Find("Icon").GetComponent<Image>().color =
            (inv.ReturnArmor(ArmorType.Chest) == null) ? iconColor : Color.white;
        LegsSlot.Find("Icon").GetComponent<Image>().color =
            (inv.ReturnArmor(ArmorType.Legs) == null) ? iconColor : Color.white;
        ConsumableSlot.Find("Icon").GetComponent<Image>().color =
            (inv.ReturnConsumable() == null) ? iconColor : Color.white;
        MainWeaponSlot.Find("Icon").GetComponent<Image>().color =
            (inv.ReturnWeapon(0) == null) ? iconColor : Color.white;
        SecondaryWeaponSlot.Find("Icon").GetComponent<Image>().color =
            (inv.ReturnWeapon(1) == null) ? iconColor : Color.white;
    }

    private void Update()
    {
        if (cursor_over)
        {
            cursor_over_time += Time.unscaledDeltaTime;
        }

        if (cursor_over_time >= TimeToDisplayStats)
        {
            if (!stats_enabled)
            {
                stats_enabled = true;
                if (i_storage == 0)
                {
                    if(inv.ReturnArmor(ArmorType.Head) != null)
                    {
                        var basicStats = inv.ReturnArmor(ArmorType.Head).GetComponent<ItemMaster>().ReturnBasicStats();
                        var advancedStats = inv.ReturnArmor(ArmorType.Head).GetComponent<ItemMaster>().ReturnAdvStats();
                        InventoryMenu.EnableRightStatPanel(
                            basicStats.Item3,
                            advancedStats.Item2,
                            advancedStats.Item3,
                            basicStats.Item5,
                            advancedStats.Item1);
                        HeadSlot.Find("RemoveIcon").GetComponent<Animator>().Play("Remove In");
                        HeadSlot.GetComponent<UIElementSound>().PlayHoverSound();
                    }
                }
                else if (i_storage == 1)
                {
                    if (inv.ReturnArmor(ArmorType.Chest) != null)
                    {
                        var basicStats = inv.ReturnArmor(ArmorType.Chest).GetComponent<ItemMaster>().ReturnBasicStats();
                        var advancedStats = inv.ReturnArmor(ArmorType.Chest).GetComponent<ItemMaster>().ReturnAdvStats();
                        InventoryMenu.EnableRightStatPanel(
                            basicStats.Item3,
                            advancedStats.Item2,
                            advancedStats.Item3,
                            basicStats.Item5,
                            advancedStats.Item1);
                        ChestSlot.Find("RemoveIcon").GetComponent<Animator>().Play("Remove In");
                        ChestSlot.GetComponent<UIElementSound>().PlayHoverSound();
                    }
                }
                else if (i_storage == 2)
                {
                    if (inv.ReturnArmor(ArmorType.Legs) != null)
                    {
                        var basicStats = inv.ReturnArmor(ArmorType.Legs).GetComponent<ItemMaster>().ReturnBasicStats();
                        var advancedStats = inv.ReturnArmor(ArmorType.Legs).GetComponent<ItemMaster>().ReturnAdvStats();
                        InventoryMenu.EnableRightStatPanel(
                            basicStats.Item3,
                            advancedStats.Item2,
                            advancedStats.Item3,
                            basicStats.Item5,
                            advancedStats.Item1);
                        LegsSlot.Find("RemoveIcon").GetComponent<Animator>().Play("Remove In");
                        LegsSlot.GetComponent<UIElementSound>().PlayHoverSound();
                    }
                }
                else if (i_storage == 3)
                {
                    if (inv.ReturnWeapon(0) != null)
                    {
                        var basicStats = inv.ReturnWeapon(0).GetComponent<ItemMaster>().ReturnBasicStats();
                        var advancedStats = inv.ReturnWeapon(0).GetComponent<ItemMaster>().ReturnAdvStats();
                        InventoryMenu.EnableRightStatPanel(
                            basicStats.Item3,
                            advancedStats.Item2,
                            advancedStats.Item3,
                            basicStats.Item5,
                            advancedStats.Item1);
                        MainWeaponSlot.Find("RemoveIcon").GetComponent<Animator>().Play("Remove In");
                        MainWeaponSlot.GetComponent<UIElementSound>().PlayHoverSound();
                    }
                }
                else if (i_storage == 4)
                {
                    if (inv.ReturnWeapon(1) != null)
                    {
                        var basicStats = inv.ReturnWeapon(1).GetComponent<ItemMaster>().ReturnBasicStats();
                        var advancedStats = inv.ReturnWeapon(1).GetComponent<ItemMaster>().ReturnAdvStats();
                        InventoryMenu.EnableRightStatPanel(
                            basicStats.Item3,
                            advancedStats.Item2,
                            advancedStats.Item3,
                            basicStats.Item5,
                            advancedStats.Item1);
                        SecondaryWeaponSlot.Find("RemoveIcon").GetComponent<Animator>().Play("Remove In");
                        SecondaryWeaponSlot.GetComponent<UIElementSound>().PlayHoverSound();
                    }
                }
                else if (i_storage == 5)
                {
                    if (inv.ReturnConsumable() != null)
                    {
                        var basicStats = inv.ReturnConsumable().GetComponent<ItemMaster>().ReturnBasicStats();
                        var advancedStats = inv.ReturnConsumable().GetComponent<ItemMaster>().ReturnAdvStats();
                        InventoryMenu.EnableRightStatPanel(
                            basicStats.Item3,
                            advancedStats.Item2,
                            advancedStats.Item3,
                            basicStats.Item5,
                            advancedStats.Item1);
                        ConsumableSlot.Find("RemoveIcon").GetComponent<Animator>().Play("Remove In");
                        ConsumableSlot.GetComponent<UIElementSound>().PlayHoverSound();
                    }
                }
            }
        }
    }
}
