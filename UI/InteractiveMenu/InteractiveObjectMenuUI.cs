using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractiveObjectMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject ButtonPrefab = null;
    [SerializeField] private GameObject Name_ValuePrefab = null;
    [SerializeField] private GameObject OtherContent;
    [SerializeField] private GameObject OwnedContent;

    private Inventory inv;
    private GameObject PreviousContainer;

    private Transform ItemStatsTooltip;
    private Transform ExternalItemPanel;



    /// 
    private Transform PlayerItemPanel;
    private TextMeshProUGUI WeightVar;
    private TextMeshProUGUI NotesVar;
    /// 

    private bool panelOpen; //Catch for if the panel is opened again right after closing

    void Awake()
    {
        inv = GameObject.Find("Player").GetComponentInChildren<Inventory>();
        ItemStatsTooltip = transform.Find("ItemStatsTooltip");
        ExternalItemPanel = transform.Find("ExternalItemPanel");
        PlayerItemPanel = transform.Find("PlayerItemPanel");
        WeightVar = PlayerItemPanel.FindDeepChild("WeightVar").GetComponent< TextMeshProUGUI>();
        NotesVar = PlayerItemPanel.FindDeepChild("NotesVar").GetComponent<TextMeshProUGUI>();

        PreviousContainer = null;
        panelOpen = false;
    }

    public void Refresh(GameObject container)
    {
        PlayerPanelUpdate();

        PreviousContainer = container;
        foreach (Transform child in OtherContent.transform)
        {
            Destroy(child.gameObject);
        }
        DisableStatPanel(true);

        bool is_Merchant = container.GetComponentInChildren<ExternalItemStorage>().ReturnMerchant();

        //////////////////////////////////////////////////////
        List<GameObject> items = container.GetComponentInChildren<ExternalItemStorage>().ReturnItems(); //Reference
        for (int i = 0; i < items.Count; i++)
        { 
            GameObject temp = Instantiate(ButtonPrefab, OtherContent.transform);
            temp.GetComponent<InteractiveUIPrefab>().ExternalSetup(items[i], is_Merchant, this);
        }
        //////////////////////////////////////////////////////

        //////////////////////////////////////////////////////
        foreach(ItemTypeEnum ITE in Enum.GetValues(typeof(ItemTypeEnum)))
        {
            List<GameObject> temp_items = inv.ReturnItems(ITE);
            for (int i = 0; i < temp_items.Count; i++)
            {
                GameObject temp = Instantiate(ButtonPrefab, OwnedContent.transform);
                temp.GetComponent<InteractiveUIPrefab>().PlayerSetup(temp_items[i], is_Merchant, this);
            }
        }
        //////////////////////////////////////////////////////



        if (!panelOpen)
        {
            ExternalItemPanel.GetComponent<Animator>().Play("In");
            PlayerItemPanel.GetComponent<Animator>().Play("In");
            ExternalItemPanel.Find("Title").Find("TitleVar").GetComponent<TextMeshProUGUI>().text = container.GetComponentInChildren<ExternalItemStorage>().ReturnName();
        }

        panelOpen = true;
    }

    public void TransferButtonPressed()
    {
        Refresh(PreviousContainer);
        DisableStatPanel();
    }

    public void TransferButtonPressedReverse(GameObject item)
    {
        PreviousContainer.GetComponentInChildren<ExternalItemStorage>().StoreItem(item);
        Refresh(PreviousContainer);
        DisableStatPanel();
    }

    public void EnableStatPanel(string itemName, string description, ItemQuality itemClass, List<(string, string)> data)
    {
        DisableStatPanel();

        ItemStatsTooltip.GetComponent<Animator>().Play("In");
        ItemStatsTooltip.Find("Content").Find("ItemName").GetComponent<TextMeshProUGUI>().text = itemName.ToUpper();
        ItemStatsTooltip.Find("Content").Find("Border").GetComponent<Image>().color = STARTUP_DECLARATIONS.itemQualityColors[itemClass];
        ItemStatsTooltip.Find("Content").Find("Description").GetComponent<TextMeshProUGUI>().text = description;

        foreach ((string, string) item in data)
        {
            GameObject temp = Instantiate(Name_ValuePrefab, ItemStatsTooltip.Find("Content").Find("Stats"));
            temp.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.Item1;
            temp.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = item.Item2;
        }
    }

    public void DisableStatPanel(bool startup = false)
    {
        foreach (Transform child in ItemStatsTooltip.Find("Content").Find("Stats"))
        {
            Destroy(child.gameObject);
        }

        if (!startup)
        {
            ItemStatsTooltip.gameObject.GetComponent<Animator>().Play("Out");
        }
    }

    public void DisablePanel()
    {
        DisableStatPanel(true);
        StartCoroutine(TurnOffPanel());
    }

    private IEnumerator TurnOffPanel()
    {
        panelOpen = false;
        PlayerItemPanel.GetComponent<Animator>().Play("Out");
        ExternalItemPanel.GetComponent<Animator>().Play("Out");
        yield return new WaitForSecondsRealtime(0.3f);

        if (!panelOpen) //Catch for if the panel is opened again right after closing
        {
            gameObject.SetActive(false);
        }
    }







    ///////////////////////////////////////////////////////
    private void PlayerPanelUpdate()
    {
        WeightVar.text = "Weight " + inv.RetunWeight();
        NotesVar.text = "Notes " + inv.ReturnNotes();

        foreach (Transform child in OwnedContent.transform)
        {
            Destroy(child.gameObject);
        }
    }
}