using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Player_Module : Virtual_Module
{
    [SerializeField] protected List<GameObject> GiveItems = new List<GameObject>();
    [SerializeField] protected int ModifyNotes = 0;

    [Space(35)]


    [SerializeField] protected bool killPlayer = false;
    [SerializeField] protected Transform MovePlayerToLoc;

    [Space(35)]

    [SerializeField] protected BuildingController BuildingToEnter;
    [SerializeField] protected int roomFloorNumber = -1;

    [SerializeField] protected bool ExitBuilding = false;

    private Inventory INV;
    private Transform Player;
    private BuildingControllerMaster BCM;
    private EventQueue EQ;

    public override void Setup()
    {
        INV = FindObjectOfType<Inventory>();
        Player = GameObject.Find("Player").transform;
        BCM = FindObjectOfType<BuildingControllerMaster>();
        EQ = FindObjectOfType<EventQueue>();
    }

    public override void Run()
    {
        foreach (GameObject obj in GiveItems)
        {
            INV.AddItem(obj);
        }

        if(GiveItems.Count > 0)
        {
            /////
            EventData tempEvent = new EventData();
            tempEvent.Setup(EventTypeEnum.ItemsReceived, "");
            EQ.AddEvent(tempEvent);
            /////
        }

        INV.AddNotes(ModifyNotes);

        if (killPlayer)
        {
            Player.GetComponentInChildren<PlayerHealth>().take_damage(1000000, DamageSource.None);
        }

        if (MovePlayerToLoc)
        {
            Player.position = MovePlayerToLoc.position;
        }

        if (BuildingToEnter)
        {
            BCM.ForceInsideBuilding(BuildingToEnter, roomFloorNumber);
        }

        if (ExitBuilding)
        {
            BCM.ForceExitBuilding();
        }
    }
}
