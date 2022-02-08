using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ImplantUIPrefab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float timeToDisplayStats = 0.1f;
    private Inventory inv;
    private ImplantUIHolder iUIh;
    private SkillMenuController skillMenu;

    private GameObject implant;
    private bool inv_is_parent;

    private Animator deleteWindow;
    private string itemName;
    private ImplantStats implantStats;

    private bool cursor_over = false;
    private bool stats_enabled = false;
    private float cursor_over_time = 0f;

    public void Setup(bool inv_is_parent_in, GameObject implant_in, ImplantUIHolder iUIh_in, Inventory inv_in)
    {
        inv_is_parent = inv_is_parent_in;
        inv = inv_in;
        iUIh = iUIh_in;
        implant = implant_in;

        implantStats = implant.GetComponent<ImplantStats>();
        ImplantPrefab temp2 = implant.GetComponent<ImplantPrefab>();

        itemName = transform.Find("ItemName").GetComponent<TextMeshProUGUI>().text = implantStats.ReturnBasicStats().Item3;
        transform.Find("Width").Find("Value").GetComponent<TextMeshProUGUI>().text = temp2.sizeX.ToString();
        transform.Find("Height").Find("Value").GetComponent<TextMeshProUGUI>().text = temp2.sizeY.ToString();
        transform.Find("QualityIndicator").GetComponent<Image>().color = STARTUP_DECLARATIONS.itemQualityColors[implantStats.ReturnBasicStats().Item5];
        transform.Find("Preview").Find("Icon").GetComponent<Image>().sprite = implantStats.ReturnBasicStats().Item4;

        deleteWindow = GameObject.Find("DeleteImplant").GetComponent<Animator>();
        skillMenu = FindObjectOfType<SkillMenuController>();

        if (implantStats.LockForQuest)
        {
            LockedLogic();
        }
        else if (implantStats.CheckLocked())
        {
            transform.Find("Sub Menu Locked").GetComponentInChildren<TextMeshProUGUI>().text = "REQUIREMENTS NOT MET";
            LockedLogic();
        }
    }

    private void LockedLogic()
    {
        transform.Find("Sub Menu").gameObject.SetActive(false);
        transform.Find("Sub Menu").gameObject.name = "Sub Menu NotLocked";
        transform.Find("Sub Menu Locked").gameObject.SetActive(true);
        transform.Find("Sub Menu Locked").gameObject.name = "Sub Menu";
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    public void Select()
    {
        ImplantController impCont;

        if (implantStats.AptitudeType == AptitudeEnum.Vigor)
        {
            impCont = GameObject.Find("VigorImplantScreen").GetComponent<ImplantController>();
        }
        else if (implantStats.AptitudeType == AptitudeEnum.Cerebral)
        {
            impCont = GameObject.Find("CerebralImplantScreen").GetComponent<ImplantController>();
        }
        else //AptitudeEnum.Finesse
        {
            impCont = GameObject.Find("FinesseImplantScreen").GetComponent<ImplantController>();
        }

        Transform parent = impCont.transform.Find("Implants");

        if (inv_is_parent)
        {
            inv.DeleteItem(implant, false);
        }

        implant.transform.SetParent(parent);
        impCont.AddReference(implant);

        implant.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        implant.GetComponent<ImplantPrefab>().Setup(implantStats);
        iUIh.Refresh();
    }

    public void AttemptDeleteImplant()
    {
        deleteWindow.Play("Modal Window In");
        deleteWindow.GetComponent<DeleteImplant>().SetImplantUIPrefab(this);
    }

    public string GetName()
    {
        return itemName;
    }

    public void DeleteSelectedItem()
    {
        inv.DeleteItem(implant);
        iUIh.Refresh();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cursor_over = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cursor_over = false;
        stats_enabled = false;
        skillMenu.DisableImplantStatPanel();
        cursor_over_time = 0;
    }

    private void OnDisable()
    {
        if (stats_enabled)
        {
            stats_enabled = false;
            skillMenu.DisableImplantStatPanel();
        }
    }

    void Update()
    {
        if (cursor_over)
        {
            cursor_over_time += Time.unscaledDeltaTime;
        }

        if (cursor_over_time >= timeToDisplayStats)
        {
            if (!stats_enabled)
            {
                stats_enabled = true;
                var basicStats = implantStats.ReturnBasicStats();
                var advancedStats = implantStats.ReturnAdvStats();
                skillMenu.EnableImplantStatPanel(basicStats.Item3, advancedStats.Item2, basicStats.Item5, advancedStats.Item1, transform.position, true);
            }
        }
    }
}
