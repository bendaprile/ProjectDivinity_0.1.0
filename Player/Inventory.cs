using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA;
using UMA.CharacterSystem;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int MaxWeight = 100;
    [SerializeField] private DynamicCharacterAvatar playerAvatar;

    private int CurrentWeight = 0;
    private int CurrentNotes = 0;

    private WeaponController weaponController;
    private QuestsHolder questsHolder;

    private Transform[] StorageRefs = new Transform[5];
    private Dictionary<string, List<GameObject>>[] InventoryStorage = new Dictionary<string, List<GameObject>>[5]; //Type, name, (list of items)

    //These are mutally exclusive with InventoryStorage //However their parent is still Storage
    private GameObject[] EquippedWeapons = new GameObject[2]; //Item NOT enabled, this happens when the weapon is readied
    private GameObject EquippedConsumable; //Item enabled if here
    private GameObject[] EquippedArmor = new GameObject[3]; //Item enabled if here

    private PlayerStats playerStats;

    public (List<GameObject>, GameObject[], GameObject, GameObject[]) DataTransfer()
    {
        List <GameObject> tempList = new List<GameObject>();
        for(int i = 0; i < 5; ++i)
        {
            foreach (KeyValuePair<string, List<GameObject>> tinyList in InventoryStorage[i])
            {
                tempList.AddRange(tinyList.Value);
            }
        }

        return (tempList, EquippedWeapons, EquippedConsumable, EquippedArmor);
    }

    public void Clear()
    {
        for(int i = 0; i < 2; ++i)
        {
            UnEquipWeapon(i);
        }
        UnEquipConsumable();
        foreach(ArmorType iter in System.Enum.GetValues(typeof(ArmorType)))
        {
            UnEquipArmor(iter);
        }
        foreach (ItemTypeEnum iter in System.Enum.GetValues(typeof(ItemTypeEnum)))
        {
            foreach(GameObject iter2 in ReturnItems(iter))
            {
                DeleteItem(iter2);
            }
        }
    }


    //InventoryStorage////////////////////////////////
    public bool AddItem(GameObject item, bool ignoreWeight = false)
    {
        ItemMaster tempMaster = item.GetComponent<ItemMaster>();
        int itemSubArray = (int)tempMaster.ReturnItemType();
        item.SetActive(false);

        if (!ignoreWeight)
        {
            int newWeight = CurrentWeight + tempMaster.ReturnBasicStats().Item2;
            if (newWeight > MaxWeight)
            {
                Debug.Log("OVER MAX WEIGHT");
                return false;
            }
            CurrentWeight = newWeight;
        }

        item.transform.SetParent(StorageRefs[itemSubArray]);
        string dictKey = tempMaster.ReturnItemName();
        if (!InventoryStorage[itemSubArray].ContainsKey(dictKey))
        {
            InventoryStorage[itemSubArray].Add(dictKey, new List<GameObject>());
        }
        InventoryStorage[itemSubArray][tempMaster.ReturnItemName()].Add(item);
        questsHolder.CheckFetchObjectives(item);
 
        return true;
    }

    public void DeleteItem(GameObject item, bool destroy = true)
    {
        ItemMaster tempMaster = item.GetComponent<ItemMaster>();
        int itemSubArray = (int)tempMaster.ReturnItemType();

        CurrentWeight -= tempMaster.ReturnBasicStats().Item2;
        InventoryStorage[itemSubArray][tempMaster.ReturnItemName()].Remove(item); //Destroy doesn't get rid of this reference

        if (destroy)
        {
            Destroy(item);
        }
    }

    public List<GameObject> ReturnItems(ItemTypeEnum type)
    {
        List<GameObject> TempList = new List<GameObject>();
        Dictionary<string, List<GameObject>> TempDict = InventoryStorage[(int)type];

        foreach (KeyValuePair<string, List<GameObject>> shortList in TempDict)
        {
            TempList.AddRange(shortList.Value);
        }
        return TempList;
    }

    public List<GameObject> ReturnItems_ByName(ItemMaster WantedItem)
    {
        ItemTypeEnum type = WantedItem.ReturnItemType();
        Dictionary<string, List<GameObject>> tempDict = InventoryStorage[(int)type];

        string dictKey = WantedItem.ReturnItemName();
        if (!tempDict.ContainsKey(dictKey))
        {
            InventoryStorage[(int)type].Add(dictKey, new List<GameObject>());
        }
        return tempDict[WantedItem.ReturnItemName()];
    }


    public void LockItemQuest(GameObject item)
    {
        item.GetComponent<ItemMaster>().LockForQuest = true;
    }

    public void UnockItemQuest(GameObject item)
    {
        item.GetComponent<ItemMaster>().LockForQuest = false;
    }
    //InventoryStorage////////////////////////////////


    //EquipedWeapons//////////////////////////////////
    public void EquipWeapon(GameObject item, int equipped_loc)
    {
        ItemMaster tempMaster = item.GetComponent<ItemMaster>();
        int itemSubArray = (int)tempMaster.ReturnItemType();

        UnEquipWeapon(equipped_loc);

        EquippedWeapons[equipped_loc] = item;
        InventoryStorage[itemSubArray][tempMaster.ReturnItemName()].Remove(item);

        item.GetComponent<Weapon>().StartWeapon();
        weaponController.RefreshWeapons();
    }

    public void UnEquipWeapon(int equppied_location)
    {
        if (EquippedWeapons[equppied_location])
        {
            AddItem(EquippedWeapons[equppied_location], true);
            EquippedWeapons[equppied_location] = null;
            weaponController.RefreshWeapons();
        }
    }

    public GameObject ReturnWeapon(int equipped_loc)
    {
        return EquippedWeapons[equipped_loc];
    }
    //EquipedWeapons//////////////////////////////////


    //EquipedConsumable//////////////////////////////////
    public void EquipConsumable(GameObject item)
    {
        ItemMaster tempMaster = item.GetComponent<ItemMaster>();
        int itemSubArray = (int)tempMaster.ReturnItemType();

        UnEquipConsumable();

        item.SetActive(true);
        EquippedConsumable = item;
        InventoryStorage[itemSubArray][tempMaster.ReturnItemName()].Remove(item);
    }

    public void UnEquipConsumable()
    {
        if(EquippedConsumable)
        {
            EquippedConsumable.SetActive(false);
            AddItem(EquippedConsumable, true);
            EquippedConsumable = null;
        }
    }

    public GameObject ReturnConsumable()
    {
        return EquippedConsumable;
    }
    //EquipedConsumable//////////////////////////////////


    //EquipedArmor///////////////////////////////////////
    public void EquipArmor(GameObject item, ArmorType armorType) //MUST UNEQUIP BEFORE EQUIPPING IF NOT NULL
    {
        ItemMaster tempMaster = item.GetComponent<ItemMaster>();
        int itemSubArray = (int)tempMaster.ReturnItemType();
        Armor armorScript = item.GetComponent<Armor>();

        UnEquipArmor(armorType);

        EquippedArmor[(int)armorType] = item;
        InventoryStorage[itemSubArray][tempMaster.ReturnItemName()].Remove(item);

        playerStats.AddAttributeEffect(AttributesEnum.armor, STARTUP_DECLARATIONS.ArmorTypeEnumReverse[(int)armorType], true, armorScript.returnArmor()); //second entry is name e.g. "Head"
        playerStats.AddAttributeEffect(AttributesEnum.energy_regen, STARTUP_DECLARATIONS.ArmorTypeEnumReverse[(int)armorType], false, armorScript.returnEEnergyRegenModifier());

        // UMA Component
        playerAvatar.SetSlot(armorScript.GetWardrobePiece().Item1);
        if (armorScript.GetWardrobePiece().Item2) { playerAvatar.SetSlot(armorScript.GetWardrobePiece().Item2); }
        playerAvatar.BuildCharacter();
    }

    public void UnEquipArmor(ArmorType armorType) //MUST UNEQUIP BEFORE EQUIPPING IF NOT NULL
    {
        if (EquippedArmor[(int)armorType])
        {
            AddItem(EquippedArmor[(int)armorType], true);
            EquippedArmor[(int)armorType] = null;

            playerStats.RemoveAttributeEffect(AttributesEnum.armor, STARTUP_DECLARATIONS.ArmorTypeEnumReverse[(int)armorType], true); //second entry is name e.g. "Head"
            playerStats.RemoveAttributeEffect(AttributesEnum.energy_regen, STARTUP_DECLARATIONS.ArmorTypeEnumReverse[(int)armorType], false); //second entry is name e.g. "Head"

            // UMA Component
            string currentSlot = armorType.ToString();
            if (currentSlot == "Head") { currentSlot = "Helmet"; }
            playerAvatar.ClearSlot(currentSlot);

            if (currentSlot == "Chest")
            {
                playerAvatar.ClearSlot("Hands");
            }
            if (currentSlot == "Legs")
            {
                playerAvatar.ClearSlot("Feet");
            }

            playerAvatar.BuildCharacter();
        }
    }

    public GameObject ReturnArmor(ArmorType armorType)
    {
        return EquippedArmor[(int)armorType];
    }
    //EquipedArmor///////////////////////////////////////


    //Notes/Weight////////////////////////////////
    public int RetunWeight()
    {
        return CurrentWeight;
    }

    public int ReturnNotes()
    {
        return CurrentNotes;
    }

    public int RetunMaxWeight()
    {
        return MaxWeight;
    }

    public void AddNotes(int amount)
    {
        CurrentNotes += amount;
    }

    public bool Attempt_NoteRemoval(int amount)
    {
        if(amount <= CurrentNotes)
        {
            CurrentNotes -= amount;
            return true;
        }
        return false;
    }
    //Notes/Weight////////////////////////////////

    private void Start() 
    {
        for(int i = 0; i < 5; ++i)
        {
            InventoryStorage[i] = new Dictionary<string, List<GameObject>>();
        }

        StorageRefs[(int)ItemTypeEnum.Misc] = transform.Find("MiscStorage");
        StorageRefs[(int)ItemTypeEnum.Armor] = transform.Find("ArmorStorage");
        StorageRefs[(int)ItemTypeEnum.Weapon] = transform.Find("WeaponStorage");
        StorageRefs[(int)ItemTypeEnum.Consumable] = transform.Find("ConsumableStorage");
        StorageRefs[(int)ItemTypeEnum.Implant] = transform.Find("ImplantStorage");

        CurrentWeight = 0;
        EquippedConsumable = null;

        for (int j = 0; j < EquippedArmor.Length; ++j)
        {
            EquippedArmor[j] = null;
        }

        for (int j = 0; j < EquippedWeapons.Length; ++j)
        {
            EquippedWeapons[j] = null;
        }


        weaponController = GameObject.Find("Player").transform.Find("Body").GetComponent<WeaponController>();
        questsHolder = GameObject.Find("QuestsHolder").GetComponent<QuestsHolder>();
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
    }
}
