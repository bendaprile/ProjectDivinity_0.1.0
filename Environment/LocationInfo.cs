using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class LocationInfo : MonoBehaviour
{
    //Gold = Quest Giver
    //RedBlue = Current Quest Loc
    //(White/Grey) = Static Loc
    //Black = Merchant

    public Sprite Icon = null;
    public Color color; //Set in Compass for static locs to grey
    public bool In_VisibleList;
    public float radius = -1; //All dynamic Locations should have a radius of -1
    public bool SetupInMap = false; //Combine Compass and mapScript then get rid of this garbage
    public string locationName = "";
    
    [TextArea(3, 5)]
    public string locationDescription = "";

    public enum LocStatus { NotFound, InRange, Discovered } //InRange is only for Non-Discovered locations
    [SerializeField] private LocStatus CurrentStatus = LocStatus.NotFound; //Used for Static Locs (White/Grey) only
    private Transform DyanmicTie = null;


    public LocStatus get_CS()
    {
        return CurrentStatus;
    }

    public void SetStatus(LocStatus Stat_in, bool systemSet = false)
    {
        Assert.IsTrue(color == Color.grey || color == Color.white);

        if (Stat_in == LocStatus.Discovered)
        {
            if (!systemSet)
            {
                EventData ED = new EventData();
                ED.Setup(EventTypeEnum.LocationDiscovered, locationName);
                FindObjectOfType<EventQueue>().AddEvent(ED);
            }
            color = Color.white;
        }

        CurrentStatus = Stat_in;
    }


    public void Dynamic_Setup(Sprite Icon_in, Color color_in, Transform DynTie)
    {
        Icon = Icon_in;
        color = color_in;
        DyanmicTie = DynTie;
    }


    public Vector3 Return_Loc()
    {
        if (DyanmicTie)
        {
            return DyanmicTie.position;
        }
        else
        {
            return transform.position;
        }
    }
}
