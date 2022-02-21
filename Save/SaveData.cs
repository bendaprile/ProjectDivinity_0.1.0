using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UMA.CharacterSystem;
using UMA;

[System.Serializable]
public struct NewGameSave
{
    public int[] skills;
    public int[] aptitude;
}


public class SaveData : MonoBehaviour
{
    [SerializeField] private bool Initialize_Default = true;

    [SerializeField] private List<GameObject> GeneralInv = new List<GameObject>();
    [SerializeField] private GameObject[] EquippedWeapons = new GameObject[2];
    [SerializeField] private GameObject EquippedConsumable;
    [SerializeField] private GameObject[] EquippedArmor = new GameObject[3];
    [SerializeField] private GameObject[] SlottedAbilities_FROMHOLDER = new GameObject[4];

    [SerializeField] private Transform CurrentObj;

    Transform player;
    Inventory inv;
    AbilitiesController AC;
    QuestsHolder QH;
    AssetEnumeration AE;
    Zone_Flags ZF;
    private DynamicCharacterAvatar avatar;

    private Transform env;
    private Transform NPCs;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        inv = player.GetComponentInChildren<Inventory>();
        AC = player.GetComponentInChildren<AbilitiesController>();
        QH = FindObjectOfType<QuestsHolder>();
        AE = GetComponentInChildren<AssetEnumeration>();
        ZF = FindObjectOfType<Zone_Flags>();
        avatar = player.GetComponentInChildren<DynamicCharacterAvatar>();

        env = GameObject.Find("Environment").transform;
        NPCs = GameObject.Find("NPCs").transform;

        if (!Initialize_Default)
        {
            StartCoroutine("Load");
        }
    }

    public void SaveAndExit()
    {
        Save();
        Application.Quit();
    }

    private void Save()
    {
        SaveHelper CurrentState = new SaveHelper();

        CurrentState.SaveItems(inv.DataTransfer());

        ImplantController[] impCont = FindObjectsOfType<ImplantController>(true);

        foreach (ImplantController imp in impCont)
        {
            CurrentState.SaveImplants(imp.return_implants());
        }


        CurrentState.SaveFlags(ZF.Return_Set_Flags());


        nonSpawnedNPCs.Clear();
        nonSpawnedFinder(env); //Must run before InventorySaveNPCs
        nonSpawnedFinder(NPCs);

        List<GameObject> nonSpawnedNPC_List = new List<GameObject>();
        foreach (KeyValuePair<string, GameObject> pair in nonSpawnedNPCs)
        {
            //Debug.Log(pair.Value);
            nonSpawnedNPC_List.Add(pair.Value);
        }
        CurrentState.SaveNPCs(nonSpawnedNPC_List);

        List<QuestTemplate> QT_List = new List<QuestTemplate>();
        foreach (Transform QT in QH.transform)
        {
            QT_List.Add(QT.GetComponent<QuestTemplate>());
        }

        string QuestFocusName = "";
        if (QH.ReturnFocus() != null)
        {
            QuestFocusName = QH.ReturnFocus().name;
        }
        CurrentState.SaveQuests(QT_List, QuestFocusName);
        CurrentState.MiscSave();

        string destination = Application.persistentDataPath + "/save.dat";
        Debug.Log("Saving to: " + destination);
        FileStream file;

        if (File.Exists(destination))
        {
            file = File.OpenWrite(destination);
        }
        else
        {
            file = File.Create(destination);
        }

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, CurrentState);
        file.Close();
    }

    public void NewGameStart()
    {
        string destination = Application.persistentDataPath + "/temp.dat";
        FileStream file = File.OpenRead(destination);
        BinaryFormatter bf = new BinaryFormatter();
        NewGameSave tempData = (NewGameSave)bf.Deserialize(file);

        PlayerStats ps = FindObjectOfType<PlayerStats>();
        for(int i = 0; i < 3; ++i)
        {
            ps.SetAptitude((AptitudeEnum)i, tempData.aptitude[i]);
        }

        for (int i = 0; i < 7; ++i)
        {
            ps.ModifySkill((SkillsEnum)i, tempData.skills[i]);
        }

        StartCoroutine(PlayerUMALoad());
    }

    public void LoadFile()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        Debug.Log("Loading from: " + destination);
        FileStream file = File.OpenRead(destination);
        BinaryFormatter bf = new BinaryFormatter();
        SaveHelper save = (SaveHelper)bf.Deserialize(file);

        StartCoroutine(PlayerUMALoad());
        FlagLoad(save.FlagLoad());
        ItemLoad(save.InventoryLoad()); //Maybe move ItemLoad to AssetEnumeration
        ImplantLoad(save.ImplantLoad());
        NpcLoad(save.npcLoad()); //Must be before Quest load (quest load can modify npcs that would overwise be disabled)
        QuestLoad(save.questLoad());
        save.MiscLoad();
    }

    private void FlagLoad(List<string> flags)
    {
        foreach(string flag in flags)
        {
            ZF.SetFlag_string(flag);
        }
    }

    private void ItemLoad(List<ITEM_SAVE_DATA> ISD_List)
    {
        inv.Clear();
        for (int i = 0; i < ISD_List.Count; ++i) //Armor //Equip
        {
            GameObject item = AE.returnItem(ISD_List[i].itemName);
            ItemMaster IM = item.GetComponent<ItemMaster>();
            IM.InitializeValues(ISD_List[i].itemQuality);
            inv.AddItem(item);

            if(ISD_List[i].itemLoc != null) //Equip item in array if initialized
            {
                if (IM.ReturnItemType() == ItemTypeEnum.Armor)
                {
                    inv.EquipArmor(item, (ArmorType)ISD_List[i].itemLoc[0]);
                }
                else if (IM.ReturnItemType() == ItemTypeEnum.Consumable)
                {
                    inv.EquipConsumable(item);
                }
                else if (IM.ReturnItemType() == ItemTypeEnum.Weapon)
                {
                    inv.EquipWeapon(item, ISD_List[i].itemLoc[0]);
                }
            }

        }
    }

    private void ImplantLoad(List<IMPLANT_SAVE_DATA> IMP_List)
    {
        ImplantController[] impCont = FindObjectsOfType<ImplantController>(true);
        for (int i = 0; i < impCont.Length; ++i) //This should be in the same order as it was saved
        {
            impCont[i].first_start_func();
            for (int j = 0; j < IMP_List.Count; ++j) //I go through this 3 times with is pretty terrible.
            {
                GameObject item = AE.returnItem(IMP_List[j].itemName);
                if (item.GetComponent<ImplantStats>().AptitudeType == impCont[i].Type)
                {
                    item.SetActive(true);
                    item.transform.parent = impCont[i].transform.Find("Implants");
                    ImplantPrefab IP = item.GetComponent<ImplantPrefab>();
                    IP.Setup(item.GetComponent<ImplantStats>());
                    for (int r = 0; r < IMP_List[j].rot; ++r)
                    {
                        IP.rotation();
                    }
                    item.GetComponent<RectTransform>().position = new Vector3(IMP_List[j].xy[0], IMP_List[j].xy[1], 0);
                }
                else
                {
                    Destroy(item);
                }
            }
            impCont[i].FinalizeImplants(true);
        }
    }

    private void NpcLoad(List<NPC_SAVE_DATA> NSD_List)
    {
        nonSpawnedNPCs.Clear();
        nonSpawnedFinder(env);
        nonSpawnedFinder(NPCs);

        foreach (NPC_SAVE_DATA NSD in NSD_List)
        {
            if (!nonSpawnedNPCs.ContainsKey(NSD.gameObject_name))
            {
                Debug.LogError(NSD.gameObject_name);
            }

            GameObject npc_inWorld = nonSpawnedNPCs[NSD.gameObject_name];

            npc_inWorld.SetActive(NSD.enabled);
            npc_inWorld.transform.position = new Vector3(NSD.pos[0], NSD.pos[1], NSD.pos[2]);
            npc_inWorld.GetComponent<EnemyTemplateMaster>().SwitchFaction(NSD.faction, false);

            DiaRoot DR = npc_inWorld.GetComponentInChildren<DiaRoot>();
            if (DR)
            {
                DiaTranslate diaTranslate = new DiaTranslate();
                //Debug.Log(NSD.starting_dia);
                //Debug.Log(NSD.dia_enabled);
                diaTranslate.DiaDecompress(DR, NSD.starting_dia, NSD.dia_enabled);
            }

            nonSpawnedNPCs.Remove(NSD.gameObject_name);
        }

        foreach (KeyValuePair<string, GameObject> pair in nonSpawnedNPCs) //These NPCs were not saved, so they are assumed to be dead
        {
            Destroy(pair.Value);
        }
    }

    private void QuestLoad((List<QUEST_SAVE_DATA>, string) QSD_List)
    {
        questHeads.Clear();
        questHeadsFinder(env);

        foreach (QUEST_SAVE_DATA QSD in QSD_List.Item1)
        {
            GameObject q_temp = questHeads[QSD.gameObject_name];
            q_temp.GetComponent<QuestTemplate>().questCategory = QSD.questCategory;
            QuestObjective[] Qobjs = q_temp.GetComponentsInChildren<QuestObjective>(true); //Task saving

            Transform loadObj = null;
            if (QSD.questCategory != QuestCategory.Completed)
            {
                loadObj = Qobjs[QSD.currentObjective].transform;
            }

            QH.LoadQuest(q_temp.transform, loadObj);

            for (int i = 0; i < Qobjs.Length; ++i)
            {
                QuestTask[] QTs = Qobjs[i].GetComponentsInChildren<QuestTask>(true);
                for (int j = 0; j < QTs.Length; ++j)
                {
                    QTs[j].TaskCurrentStatus = QSD.taskStatus[i][j];
                }
            }
        }

        if(QSD_List.Item2 != "")
        {
            QH.QuestSetFocus(questHeads[QSD_List.Item2]);
        }
    }


    Dictionary<string, GameObject> nonSpawnedNPCs = new Dictionary<string, GameObject>();
    private void nonSpawnedFinder(Transform trans) //MUST CLEAN nonSpawnedNPCs EVERYTIME BEFORE USE!!
    {
        EnemyTemplateMaster etm = trans.GetComponent<EnemyTemplateMaster>();
        if (etm)
        {
            if (!etm.Return_wasSpawned())
            {
                Assert.IsFalse(nonSpawnedNPCs.ContainsKey(etm.name), "TWO NONSPAWNED NPCS SHARE THE GAMEOBJECT NAME (" + etm.name + ")");
                nonSpawnedNPCs.Add(etm.name, trans.gameObject);
            }
            return;
        }


        foreach(Transform child in trans)
        {
            nonSpawnedFinder(child);
        }
    }

    Dictionary<string, GameObject> questHeads = new Dictionary<string, GameObject>();
    private void questHeadsFinder(Transform trans) //MUST CLEAN questHeads EVERYTIME BEFORE USE!!
    {
        QuestTemplate qt = trans.GetComponent<QuestTemplate>();
        if (qt)
        {
            Assert.IsFalse(questHeads.ContainsKey(qt.name), "TWO QUESTS SHARE THE GAMEOBJECT NAME (" + qt.name + ")");
            questHeads.Add(qt.name, trans.gameObject);
            return;
        }

        foreach (Transform child in trans)
        {
            questHeadsFinder(child);
        }
    }

    private IEnumerator PlayerUMALoad()
    {
        avatar.ClearSlots();
        if (File.Exists(Application.persistentDataPath + "/umaData.txt"))
        {
            yield return new WaitUntil(() => avatar.GetComponent<UMAData>() != null);
            yield return new WaitUntil(() => avatar.GetComponent<UMAData>().isOfficiallyCreated == true);
            avatar.LoadFromRecipeString(File.ReadAllText(Application.persistentDataPath + "/umaData.txt"));
            avatar.UpdateColors(true);
        }
    }

    ////////////////////////////////////////////////////////////////
    /// In-editor Load below ///////////////////////////////////////
    //////////////////////////////////////////////////////////////// 

    IEnumerator Load()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        StartCoroutine(PlayerUMALoad());
        SetupPlayer();
    }

    private void SetupPlayer()
    {
        foreach (GameObject gmeObj in GeneralInv) //General items
        {
            if (gmeObj)
            {
                inv.AddItem(Instantiate(gmeObj));
            }
        }

        GameObject temp;
        for(int i = 0; i < 2; ++i) //Weapons //Equip
        {
            if (EquippedWeapons[i])
            {
                temp = Instantiate(EquippedWeapons[i]);
                inv.AddItem(temp);
                inv.EquipWeapon(temp, i);
            }
        }

        if (EquippedConsumable)
        {
            temp = Instantiate(EquippedConsumable);//Consumables //Equip
            inv.AddItem(temp);
            inv.EquipConsumable(temp);
        }

        for (int i = 0; i < 3; ++i) //Armor //Equip
        {
            if (EquippedArmor[i])
            {
                temp = Instantiate(EquippedArmor[i]);
                inv.AddItem(temp);
                inv.EquipArmor(temp, (ArmorType)i);
            }
        }

        for (int i = 0; i < 4; ++i) //Abilities //Equip
        {
            if (SlottedAbilities_FROMHOLDER[i])
            {
                Ability abil = SlottedAbilities_FROMHOLDER[i].GetComponent<Ability>();
                AC.SlotAbility(abil, i);
            }
        }

        if (CurrentObj)
        {
            QH.LoadQuest(CurrentObj.parent, CurrentObj);
        }
    }
}
