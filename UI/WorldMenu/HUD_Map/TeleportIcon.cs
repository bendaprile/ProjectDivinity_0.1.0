using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class TeleportIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool cursor_over = false;
    private Vector3 world_loc = new Vector3();
    private BuildingControllerMaster bcMaster;
    private float cursor_over_time = 0f;
    private bool stats_enabled = false;
    private string locName = "Default Name";
    private string locDescription = "Default Description";
    private mapScript map;
    private UIController ui;

    private void Start()
    {
        map = FindObjectOfType<mapScript>();
        ui = FindObjectOfType<UIController>();
    }


    public void Setup(Transform loc, string name, string description)
    {
        Assert.IsFalse(loc.childCount > 1);
        if(loc.childCount == 1)
        {
            world_loc = loc.GetChild(0).position;
        }
        else 
        {
            world_loc = loc.position;
        }

        GetComponent<Image>().raycastTarget = true;
        bcMaster = FindObjectOfType<BuildingControllerMaster>();
        locName = name;
        locDescription = description;
    }


    private void Update()
    {
        if (cursor_over)
        {
            cursor_over_time += Time.unscaledDeltaTime;
        }

        if (cursor_over && Input.GetMouseButtonDown(0))
        {
            if (bcMaster.insideBuilding)
            {
                bcMaster.forceExitCurrentBuilding = true;
            }
            if (ui.current_UI_mode == UI_Mode.PauseMenu)
            {
                ui.Unpaused();
            }

            GameObject.Find("Player").transform.position = world_loc;
        }

        if (cursor_over_time >= STARTUP_DECLARATIONS.TIME_TO_DISPLAY_TOOLTIP)
        {
            if (!stats_enabled)
            {
                stats_enabled = true;
                map.EnableInfoPanel(locName, locDescription, transform.position);
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
        map.DisableInfoPanel();
        cursor_over_time = 0;
    }
}
