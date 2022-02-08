using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class mapScript : MonoBehaviour
{
    [SerializeField] private float angleoffset = -45f; // x or y size
    [SerializeField] private float mapsize = 0; // x or y size
    [SerializeField] private float Worldmapsize = 0; //One edge
    [SerializeField] private Vector2 TopPoint = new Vector2(); //X then Z
    [SerializeField] private Transform TeleportLocs;
    [SerializeField] private Transform DynamicLocs;


    [SerializeField] private Transform ObjParent = null;
    [SerializeField] private GameObject Circle = null;
    [SerializeField] private GameObject Point = null;

    [SerializeField] private GameObject TeleportTemplate = null;

    private float length;
    private Vector2 MapCenter;
    private Transform playerLoc;
    private RectTransform playerIconUI;
    private Transform LocInfoTooltip;

    private bool first_start = true;
    private List<GameObject> DynamicUI = new List<GameObject>();
    private List<GameObject> TempStaticUI = new List<GameObject>(); //This is used for static locations that could still change



    private void OnEnable()
    {
        if (first_start)
        {
            First_Start();
            first_start = false;
        }
    }

    private void  First_Start()
    {
        playerLoc = GameObject.Find("Player").transform;
        playerIconUI = transform.Find("FullMapHolder").Find("PlayerLocation").GetComponent<RectTransform>();

        length = Worldmapsize / 2;
        MapCenter = new Vector2(TopPoint.x - length, TopPoint.y - length);
        LocInfoTooltip = transform.Find("FullMapHolder").Find("LocInfoTooltip");
    }

    private Vector2 WorldSpace_to_MapSpace(Vector2 loc)
    {
        Vector2 startingLoc = new Vector2(loc.x - MapCenter.x, loc.y - MapCenter.y);
        startingLoc *= (mapsize / (Worldmapsize));

        Vector2 rotatedObjloc;
        rotatedObjloc.x = (startingLoc.x - startingLoc.y) / 2;
        rotatedObjloc.y = (startingLoc.x + startingLoc.y) / 2;

        return rotatedObjloc;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 currentloc = WorldSpace_to_MapSpace(new Vector2(playerLoc.position.x, playerLoc.position.z));
        playerIconUI.localPosition = new Vector3(currentloc.x, currentloc.y, 0);
        playerIconUI.localEulerAngles = new Vector3(0f, 0f, -(playerLoc.localEulerAngles.y + angleoffset));

        DynamicLocsSetup(); //Very poor big O
        TeleLocsSetup(); //Very poor big O
    }


    public void TeleLocsSetup()
    {
        for (int i = TempStaticUI.Count - 1; i >= 0; --i)
        {
            Destroy(TempStaticUI[i]);
            TempStaticUI.RemoveAt(i);
        }

        foreach (Transform tele in TeleportLocs)
        {
            if(tele.GetComponent<LocationInfo>().get_CS() == LocationInfo.LocStatus.NotFound)
            {
                continue;
            }
            else if (tele.GetComponent<LocationInfo>().SetupInMap)
            {
                continue;
            }

            LocationInfo LI = tele.GetComponent<LocationInfo>();

            Vector2 rotatedObjloc = WorldSpace_to_MapSpace(new Vector2(LI.Return_Loc().x, LI.Return_Loc().z));

            GameObject temp = Instantiate(TeleportTemplate, ObjParent);

            temp.GetComponent<Image>().sprite = tele.GetComponent<LocationInfo>().Icon;
            temp.GetComponent<Image>().color = tele.GetComponent<LocationInfo>().color;
            temp.GetComponent<RectTransform>().localPosition = new Vector3(rotatedObjloc.x, rotatedObjloc.y, 0f);
            

            if (tele.GetComponent<LocationInfo>().get_CS() == LocationInfo.LocStatus.Discovered)
            {
                tele.GetComponent<LocationInfo>().SetupInMap = true;
                temp.GetComponent<TeleportIcon>().Setup(tele, tele.GetComponent<LocationInfo>().locationName, tele.GetComponent<LocationInfo>().locationDescription); //Do not put in TempStaticUI so it isn't deleted. This should happen once
            }
            else
            {
                TempStaticUI.Add(temp); 
            }
        }
    }


    private void DynamicLocsSetup()
    {
        for(int i = DynamicUI.Count - 1; i >= 0; --i)
        {
            Destroy(DynamicUI[i]);
            DynamicUI.RemoveAt(i);
        }


        foreach (Transform tele in DynamicLocs)
        {
            LocationInfo LI = tele.GetComponent<LocationInfo>();

            if(LI.color == STARTUP_DECLARATIONS.goldColor && !LI.In_VisibleList)
            {
                continue;
            }

            Vector2 rotatedObjloc = WorldSpace_to_MapSpace(new Vector2(LI.Return_Loc().x, LI.Return_Loc().z));

            GameObject temp = Instantiate(TeleportTemplate, ObjParent);

            temp.GetComponent<Image>().sprite = tele.GetComponent<LocationInfo>().Icon;
            temp.GetComponent<Image>().color = tele.GetComponent<LocationInfo>().color;

            //DO NOT SETUP, Because cannot tele to
            temp.GetComponent<RectTransform>().localPosition = new Vector3(rotatedObjloc.x, rotatedObjloc.y, 0f);

            DynamicUI.Add(temp);
        }
    }

    public void EnableInfoPanel(string name, string description, Vector3 pos)
    {
        LocInfoTooltip.gameObject.SetActive(true);
        Vector3 tooltipPos = new Vector3(pos.x, pos.y -150f, LocInfoTooltip.position.z);
        LocInfoTooltip.position = tooltipPos;
        LocInfoTooltip.GetComponent<Animator>().Play("In");
        LocInfoTooltip.Find("Content").Find("Name").GetComponent<TextMeshProUGUI>().text = name.ToUpper();
        LocInfoTooltip.Find("Content").Find("Description").GetComponent<TextMeshProUGUI>().text = description;
    }

    public void DisableInfoPanel()
    {
        if (LocInfoTooltip.gameObject.activeInHierarchy)
        {
            LocInfoTooltip.gameObject.GetComponent<Animator>().Play("Out");
        }
    }
}
