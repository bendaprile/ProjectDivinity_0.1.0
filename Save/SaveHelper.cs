using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveHelper
{
    private List<string> flagSave = new List<string>();
    private List<ITEM_SAVE_DATA> itemSave = new List<ITEM_SAVE_DATA>();
    private List<IMPLANT_SAVE_DATA> implantSave = new List<IMPLANT_SAVE_DATA>(); //This is only for used implants
    private List<NPC_SAVE_DATA> npcSave = new List<NPC_SAVE_DATA>();
    private List<QUEST_SAVE_DATA> questSave = new List<QUEST_SAVE_DATA>();
    private string questfocusSave = "";
    private List<SPAWNER_SAVE_DATA> spawnerSave = new List<SPAWNER_SAVE_DATA>();
    private List<INTERACTIVE_OBJECT_SAVE_DATA> interactiveObjectSave = new List<INTERACTIVE_OBJECT_SAVE_DATA>();

    private float[][] repMatrix;
    private float timeOfDay;
    private List<bool> cagEnabled;
    private List<LocationInfo.LocStatus> locDiscovedMatrix = new List<LocationInfo.LocStatus>();

    public void SaveItems((List<GameObject> GenInv_in, GameObject[] EquippedWeapons_in, GameObject EquippedConsumable_in, GameObject[] EquippedArmor_in) inv)
    {
        for(int i = 0; i < inv.EquippedWeapons_in.Length; ++i)
        {
            if (inv.EquippedWeapons_in[i])
            {
                ItemConv(inv.EquippedWeapons_in[i], new int[1] { i });
            }
        }
        if (inv.EquippedConsumable_in)
        {
            ItemConv(inv.EquippedConsumable_in, new int[1] { -1 });
        }
        for (int i = 0; i < inv.EquippedArmor_in.Length; ++i)
        {
            if (inv.EquippedArmor_in[i])
            {
                ItemConv(inv.EquippedArmor_in[i], new int[1] { (int)inv.EquippedArmor_in[i].GetComponent<Armor>().returnArmorType() });
            }
        }
        foreach (GameObject temp in inv.GenInv_in)
        {
            ItemConv(temp);
        }
    }

    public void SaveImplants(List<GameObject> implants_in) //This is only for used implants
    {
        for (int i = 0; i < implants_in.Count; ++i)
        {
            ItemMaster IM = implants_in[i].GetComponent<ItemMaster>();
            RectTransform impTran = implants_in[i].GetComponent<RectTransform>();

            IMPLANT_SAVE_DATA ISD = new IMPLANT_SAVE_DATA();
            ISD.itemName = IM.ReturnItemName();
            ISD.xy = new float[2] { impTran.position.x, impTran.position.y};
            Debug.Log(impTran.position);
            ISD.rot = implants_in[i].GetComponent<ImplantPrefab>().return_rot();
            implantSave.Add(ISD);
        }
    }

    public void SaveNPCs(List<GameObject> nonSpawned)
    {
        foreach (GameObject temp in nonSpawned)
        {
            if (temp.GetComponentInChildren<Health>() && temp.GetComponentInChildren<Health>().gameObject.tag == "DeadEnemy")
            {
                continue; //Don't save recently dead enemies
            }

            NPC_SAVE_DATA NSD = new NPC_SAVE_DATA();
            NSD.gameObject_name = temp.name;
            NSD.enabled = temp.activeSelf;
            NSD.pos = new float[3] { temp.transform.position.x, temp.transform.position.y, temp.transform.position.z };
            NSD.faction = temp.GetComponent<EnemyTemplateMaster>().Return_FactionEnum();

            DiaRoot DR = temp.GetComponentInChildren<DiaRoot>();
            if (DR) //Only for humanoids
            {
                DiaTranslate diaTranslate = new DiaTranslate();
                int startingDia;
                List<bool> enList;
                diaTranslate.DiaCompress(DR, out startingDia, out enList);
                NSD.starting_dia = startingDia;
                NSD.dia_enabled = enList;
            }

            npcSave.Add(NSD);
        }
    }

    public void SaveQuests(List<QuestTemplate> QT_List, string quest_focus)
    {
        foreach (QuestTemplate temp in QT_List)
        {
            QUEST_SAVE_DATA QSD = new QUEST_SAVE_DATA();
            QSD.gameObject_name = temp.gameObject.name;
            QSD.questCategory = temp.questCategory;
            int quest_obj = -1;

            QSD.taskStatus = new List<List<TaskStatus>>();
            QuestObjective[] Qobjs = temp.GetComponentsInChildren<QuestObjective>(true);
            for (int i = 0; i < Qobjs.Length; ++i)
            {
                QSD.taskStatus.Add(new List<TaskStatus>());
                QuestTask[] QTs = Qobjs[i].GetComponentsInChildren<QuestTask>(true);
                foreach (QuestTask q_iter in QTs)
                {
                    QSD.taskStatus[i].Add(q_iter.TaskCurrentStatus);
                }
            }


            if (temp.questCategory != QuestCategory.Completed)
            {
                //All the children should be active (But just in case)
                for (int i = 0; i < Qobjs.Length; ++i)
                {
                    if (Qobjs[i].gameObject == temp.returnActiveObjective().gameObject)
                    {
                        quest_obj = i;
                        break;
                    }
                }
            }

            QSD.currentObjective = quest_obj;
            questSave.Add(QSD);
        }

        questfocusSave = quest_focus;
    }

    public void SaveFlags(List<string> flags_in)
    {
        flagSave = flags_in;
    }

    public void MiscSave()
    {
        Spawner[] spawners = MonoBehaviour.FindObjectsOfType<Spawner>(true);
        for(int i = 0; i < spawners.Length; ++i)
        {
            SPAWNER_SAVE_DATA SSD = new SPAWNER_SAVE_DATA();
            SSD.spawnerEnabled = spawners[i].dataDump().Item1;
            SSD.current_enemies = spawners[i].dataDump().Item2;
            spawnerSave.Add(SSD);
        }

        AudioOverrideCollider[] CAG = MonoBehaviour.FindObjectsOfType<AudioOverrideCollider>(true);
        cagEnabled = new List<bool>();
        for (int i = 0; i < CAG.Length; ++i)
        {
            cagEnabled.Add(CAG[i].gameObject.activeSelf);
        }

        InteractiveObject[] IO = MonoBehaviour.FindObjectsOfType<InteractiveObject>(true);
        for (int i = 0; i < IO.Length; ++i)
        {
            INTERACTIVE_OBJECT_SAVE_DATA IOSD = new INTERACTIVE_OBJECT_SAVE_DATA();
            IOSD.LockPickingRequirement = IO[i].LockPickingRequirement;
            IOSD.SetupOnceLogic = IO[i].SetupOnceLogic;

            ExternalItemStorage EIS = IO[i].GetComponentInChildren<ExternalItemStorage>(true);
            if (EIS)
            {
                ItemMaster[] IMs = EIS.GetComponentsInChildren<ItemMaster>(true);
                IOSD.itemName = new List<string>();
                IOSD.itemQuality = new List<ItemQuality>();
                foreach (ItemMaster IM in IMs)
                {
                    IOSD.itemName.Add(IM.ReturnItemName());
                    IOSD.itemQuality.Add(IM.ReturnItemClass());
                }
            }
            interactiveObjectSave.Add(IOSD);
        }

        repMatrix = MonoBehaviour.FindObjectOfType<FactionLogic>().return_fullMatrix();
        timeOfDay = MonoBehaviour.FindObjectOfType<DayNightController>().CurrentTime;
        foreach (Transform loc in GameObject.Find("Static Locations UNIQUE").transform)
        {
            locDiscovedMatrix.Add(loc.GetComponent<LocationInfo>().get_CS());
        }
    }

    private void ItemConv(GameObject item, int[] loc = null) //if loc isn't null then the item is equipped
    {
        ItemMaster IM = item.GetComponent<ItemMaster>();
        ITEM_SAVE_DATA ITD = new ITEM_SAVE_DATA();
        ITD.itemName = IM.ReturnItemName();
        ITD.itemQuality = IM.ReturnItemClass();
        ITD.itemLoc = loc;
        itemSave.Add(ITD);
    }

    public List<string> FlagLoad()
    {
        return flagSave;
    }

    public void MiscLoad()
    {
        Spawner[] spawners = MonoBehaviour.FindObjectsOfType<Spawner>(true);
        for (int i = 0; i < spawners.Length; ++i)
        {
            spawners[i].External_Load(spawnerSave[i].spawnerEnabled, spawnerSave[i].current_enemies);
        }

        AudioOverrideCollider[] CAG = MonoBehaviour.FindObjectsOfType<AudioOverrideCollider>(true);
        for (int i = 0; i < CAG.Length; ++i)
        {
            CAG[i].gameObject.SetActive(cagEnabled[i]);
        }

        InteractiveObject[] IO = MonoBehaviour.FindObjectsOfType<InteractiveObject>(true);
        for (int i = 0; i < IO.Length; ++i)
        {
            IO[i].LockPickingRequirement = interactiveObjectSave[i].LockPickingRequirement;
            IO[i].SetupOnceLogic = interactiveObjectSave[i].SetupOnceLogic;

            ExternalItemStorage EIS = IO[i].GetComponentInChildren<ExternalItemStorage>(true);
            if (EIS)
            {
                foreach (Transform child in EIS.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }

                AssetEnumeration AE = MonoBehaviour.FindObjectOfType<AssetEnumeration>(true);
                for (int j = 0; j < interactiveObjectSave[i].itemName.Count; ++j)
                {
                    GameObject item = AE.returnItem(interactiveObjectSave[i].itemName[j]);
                    ItemMaster IM = item.GetComponent<ItemMaster>();
                    IM.InitializeValues(interactiveObjectSave[i].itemQuality[j]);
                    item.transform.parent = EIS.transform;
                }
            }
        }

        MonoBehaviour.FindObjectOfType<FactionLogic>().set_fullMatrix(repMatrix);
        MonoBehaviour.FindObjectOfType<DayNightController>().CurrentTime = timeOfDay;
        int iter = 0;
        foreach(Transform loc in GameObject.Find("Static Locations UNIQUE").transform)
        {
            loc.GetComponent<LocationInfo>().SetStatus(locDiscovedMatrix[iter], true);
            iter += 1;
        }
    }

    public List<ITEM_SAVE_DATA> InventoryLoad()
    {
        return itemSave;
    }

    public List<IMPLANT_SAVE_DATA> ImplantLoad()
    {
        return implantSave;
    }

    public List<NPC_SAVE_DATA> npcLoad()
    {
        return npcSave;
    }

    public (List<QUEST_SAVE_DATA>, string) questLoad()
    {
        return (questSave, questfocusSave);
    }
}

[System.Serializable]
public struct ITEM_SAVE_DATA
{
    public string itemName;
    public ItemQuality itemQuality;
    public int[] itemLoc; //Weapons,Cons,Armor (single var)
}

[System.Serializable]
public struct NPC_SAVE_DATA
{
    public string gameObject_name;
    public bool enabled;
    public float[] pos;
    public FactionsEnum faction;
    public int starting_dia;
    public List<bool> dia_enabled;
}

[System.Serializable]
public struct QUEST_SAVE_DATA
{
    public string gameObject_name;
    public int currentObjective;
    public List<List<TaskStatus>> taskStatus; // Quest Tasks for each Objective
    public QuestCategory questCategory;
}

[System.Serializable]
public struct IMPLANT_SAVE_DATA
{
    public string itemName;
    public float[] xy;
    public int rot;
}

[System.Serializable]
public struct SPAWNER_SAVE_DATA
{
    public bool spawnerEnabled;
    public float current_enemies; //Can be a parial enemy
}

[System.Serializable]
public struct INTERACTIVE_OBJECT_SAVE_DATA
{
    public SkillCheckDifficulty LockPickingRequirement;
    public bool SetupOnceLogic;

    public List<string> itemName; //Only For Boxes
    public List<ItemQuality> itemQuality; //Only For Boxes
}