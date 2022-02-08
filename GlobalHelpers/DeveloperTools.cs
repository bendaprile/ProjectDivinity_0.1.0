 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

public class DeveloperTools : MonoBehaviour
{
    Transform playerTrans;
    [SerializeField] TextMeshProUGUI tmText = null;
    private void Assertions()
    {
        Assert.IsTrue(STARTUP_DECLARATIONS.FactionCount == System.Enum.GetValues(typeof(FactionsEnum)).Length);
        Assert.IsTrue(STARTUP_DECLARATIONS.FactionCount == STARTUP_DECLARATIONS.FactionsEnumReverse.Length);

        Assert.IsTrue(STARTUP_DECLARATIONS.NPC_FactionsCount == System.Enum.GetValues(typeof(NPC_FactionsEnum)).Length);
        Assert.IsTrue(STARTUP_DECLARATIONS.NPC_FactionsCount == STARTUP_DECLARATIONS.NPC_FactionsEnumReverse.Length);

        Transform NPCs = GameObject.Find("NPCs").transform;
        foreach(FactionsEnum FE in System.Enum.GetValues(typeof(FactionsEnum)))
        {
            string NPC_name = STARTUP_DECLARATIONS.FactionsEnumReverse[(int)FE];
            Assert.IsNotNull(NPCs.Find(NPC_name), "Must add [" + NPC_name + "] to `NPCs` GameObject");
        }

        UniqueChecker(GameObject.Find("Environment").transform);
        //////////////////////////////////////////////////////////////////////////////////
    }

    Dictionary<string, bool> UniqueTrans = new Dictionary<string, bool>();
    private void UniqueChecker(Transform tran)
    {
        string lower_name = tran.name.ToLower();
        if (lower_name.Contains("unique"))
        {
            Assert.IsFalse(UniqueTrans.ContainsKey(tran.name)); //Keep as upper and lower
            UniqueTrans.Add(tran.name, true);
        }

        foreach(Transform tr in tran)
        {
            UniqueChecker(tr);
        }
    }
    
    private void Start()
    {
        playerTrans = GameObject.Find("Player").transform;
        Assertions();
    }
    // Update is called once per frame
    void Update()
    {
        UpdateText();

        if (Input.GetKeyDown(KeyCode.F1))
        {
            UpdatePlayerRunSpeed();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            UpdateDayNight();
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            PlayerStats playerStats = FindObjectOfType<PlayerStats>();
            for (int i = 0; i < 7; i++)
            {
                playerStats.ModifySkill((SkillsEnum)i, 100);
            }
        }

        // Auto Discover all locations
        if (Input.GetKeyDown(KeyCode.F4))
        {
            foreach(LocationInfo LI in GameObject.Find("Static Locations UNIQUE").GetComponentsInChildren<LocationInfo>())
            {
                LI.SetStatus(LocationInfo.LocStatus.Discovered, true);
            }
        }

        // Spawn random event
        if (Input.GetKeyDown(KeyCode.F5))
        {
            FindObjectOfType<RandomEventMaster>().RandomEventLogic();
        }

        // Equip all items
        if (Input.GetKeyDown(KeyCode.F6))
        {
            Inventory inv = FindObjectOfType<Inventory>();
            List<GameObject> items = FindObjectOfType<AssetEnumeration>().returnAllItems();
            foreach(GameObject item in items)
            {
                inv.AddItem(item);
            }
        }

        // Add 1000 xp
        if (Input.GetKeyDown(KeyCode.F7))
        {
            PlayerStats playerStats = FindObjectOfType<PlayerStats>();
            playerStats.AddEXP(1000);
        }

        // Kill the player
        if (Input.GetKeyDown(KeyCode.F8))
        {
            FindObjectOfType<PlayerMaster>().PlayerDeath(true);
        }
    }

    void UpdatePlayerRunSpeed()
    {
        PlayerMovement pm = playerTrans.GetComponent<PlayerMovement>();

        if (pm.runSpeedMultiplier < 2.5f)
        {
            pm.runSpeedMultiplier = 5f;
        }
        else
        {
            pm.runSpeedMultiplier = 1.7f;
        }
    }

    void UpdateDayNight()
    {
        DayNightController dn = FindObjectOfType<DayNightController>();

        if (!dn.isNight)
        {
            dn.CurrentTime = 24;
        }
        else
        {
            dn.CurrentTime = 12;
        }
    }

    void UpdateText()
    {
        tmText.text = "";
        string currentRun = "Normal";
        if (playerTrans.GetComponent<PlayerMovement>().runSpeedMultiplier > 2f) { currentRun = "Fast"; }
        tmText.text = tmText.text + "\n" + "F1: Toggle Player Run Speed (Currently " + currentRun + ")";
        tmText.text = tmText.text + "\n" + "F2: Toggle Day/Night";
        tmText.text = tmText.text + "\n" + "F3: Max Skills (Not Reversable)";
        tmText.text = tmText.text + "\n" + "F4: Reveal All Locations (Not Reversable)";
        tmText.text = tmText.text + "\n" + "F5: Spawn Random Event";
        tmText.text = tmText.text + "\n" + "F6: Add All Items";
        tmText.text = tmText.text + "\n" + "F7: Add xp";
        tmText.text = tmText.text + "\n" + "F8: Suicidal thoughts";
    }
}
