using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    [SerializeField] private Transform Locations;
    [SerializeField] private Transform DynLocations;
    [SerializeField] private float max_dist = 500f;
    [SerializeField] private float quest_init_max_dist = 200f;

    [SerializeField] private GameObject tracker_white;
    [SerializeField] private GameObject tracker_grey;
    [SerializeField] private GameObject tracker_purple;
    [SerializeField] private GameObject tracker_gold;
    [SerializeField] private Transform trackerParent;

    private Transform Player;
    private CombatChecker CC;

    private struct VisableStorage
    {
        public VisableStorage(GameObject UI_element_in, LocationInfo LocationStoarge_in)
        {
            UI_element = UI_element_in;
            LocationStorage = LocationStoarge_in;
        }

        public GameObject UI_element { get; }

        public LocationInfo LocationStorage { get; }
    }

    private List<VisableStorage> VisableList = new List<VisableStorage>();

    void Start()
    {
        Player = GameObject.Find("Player").transform;
        CC = FindObjectOfType<CombatChecker>();

        foreach(Transform loc in Locations)
        {
            loc.GetComponent<LocationInfo>().color = Color.grey;
        }
    }


    private bool Visable_add_logic(LocationInfo locInfoPass)
    {
        Vector3 vec = locInfoPass.Return_Loc() - Player.position;

        float temp_max_dist = max_dist;

        if (locInfoPass.color == STARTUP_DECLARATIONS.goldColor)
        {
            temp_max_dist = quest_init_max_dist;
        }
        else if (locInfoPass.color == new Color(1, 0, 1, 1))
        {
            temp_max_dist = float.MaxValue;
        }
        else if(locInfoPass.color == Color.black)
        {
            return false;
        }

        if (vec.magnitude < temp_max_dist && vec.magnitude >= locInfoPass.radius)
        {
            locInfoPass.In_VisibleList = true;

            GameObject temp = null;
            if (locInfoPass.color == Color.white)
            {
                temp = Instantiate(tracker_white, trackerParent);
            }        
            else if (locInfoPass.color == Color.grey)
            {
                temp = Instantiate(tracker_grey, trackerParent);
            }
            else if (locInfoPass.color == STARTUP_DECLARATIONS.goldColor)
            {
                temp = Instantiate(tracker_gold, trackerParent);
            }
            else
            {
                temp = Instantiate(tracker_purple, trackerParent);
            }
            VisableList.Add(new VisableStorage(temp, locInfoPass));
            return true;
        }
        return false;
    }


    private void Visable_remove_logic(int i)
    {
        VisableList[i].LocationStorage.In_VisibleList = false;
        Destroy(VisableList[i].UI_element.gameObject);
        VisableList.RemoveAt(i);
    }



    // Update is called once per frame
    private void Update()
    {
        if (CC.enemies_nearby)
        {
            trackerParent.gameObject.SetActive(false);
            return;
        }
        else
        {
            trackerParent.gameObject.SetActive(true);
        }

        ////////////////////////////////////////////////////////////////////
        for (int i = 0; i < Locations.childCount; ++i)
        {
            LocationInfo locInfo = Locations.GetChild(i).GetComponent<LocationInfo>();
            if (locInfo.In_VisibleList)
            {
                continue;
            }

            if (Visable_add_logic(locInfo) && locInfo.get_CS() == LocationInfo.LocStatus.NotFound)
            {
                locInfo.SetStatus(LocationInfo.LocStatus.InRange);
            }
        }

        for (int i = 0; i < DynLocations.childCount; ++i)
        {
            LocationInfo locInfo = DynLocations.GetChild(i).GetComponent<LocationInfo>();
            if (locInfo.In_VisibleList)
            {
                continue;
            }
            Visable_add_logic(locInfo);
        }
        ////////////////////////////////////////////////////////////////////




        for (int i = VisableList.Count - 1; i >= 0; --i)
        {
            if (!VisableList[i].LocationStorage) //Dynamic Location Got removed externally
            {
                Visable_remove_logic(i);
                continue;
            }

            float temp_max_dist = max_dist;
            if (VisableList[i].LocationStorage.color == STARTUP_DECLARATIONS.goldColor) //Should be no black colored here
            {
                temp_max_dist = quest_init_max_dist;
            }
            else if (VisableList[i].LocationStorage.color == new Color(1, 0, 1, 1))
            {
                temp_max_dist = float.MaxValue;
            }

            Vector3 vec = VisableList[i].LocationStorage.Return_Loc() - Player.position;

            if (vec.magnitude > temp_max_dist)
            {
                if (VisableList[i].LocationStorage.get_CS() == LocationInfo.LocStatus.InRange)
                {
                    VisableList[i].LocationStorage.SetStatus(LocationInfo.LocStatus.NotFound);
                }
                Visable_remove_logic(i);
            }
            else if(vec.magnitude < VisableList[i].LocationStorage.radius)
            {
                if(VisableList[i].LocationStorage.get_CS() == LocationInfo.LocStatus.InRange)
                {
                    VisableList[i].LocationStorage.SetStatus(LocationInfo.LocStatus.Discovered);
                }
                Visable_remove_logic(i);
            }
            else
            {
                Vector2 modPos = new Vector2(vec.x, vec.z);

                float mag = 0.3f + (0.7f * (1 - (modPos.magnitude / temp_max_dist)));
                
                if(modPos.magnitude > 3)
                {
                    modPos.Normalize();
                    modPos *= 3;
                }


                VisableList[i].UI_element.transform.localPosition = modPos;

                float rot = Mathf.Atan2(modPos.y, modPos.x) * Mathf.Rad2Deg + 180f;
                VisableList[i].UI_element.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rot));

                VisableList[i].UI_element.transform.localScale = new Vector3(mag, mag, mag);
            }
        }
        ////////////////////////////////////////////////////////////////////ICONS



    }
}
