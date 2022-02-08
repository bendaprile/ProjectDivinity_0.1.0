using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class World_Setup : MonoBehaviour
{
    [SerializeField] [TextArea(3, 10)] private string COMMENT = "";

    [Space(50)]

    [SerializeField] protected List<GameObject> ObjectsToEnable = new List<GameObject>();
    [SerializeField] protected List<GameObject> ObjectsToDisable = new List<GameObject>();


    [Space(50)]


    [SerializeField] protected PlayerMovementSet SetMovement = PlayerMovementSet.Nothing;

    [Space(50)]

    [SerializeField] protected List<Virtual_Module> ModulesToRun;


    protected enum PlayerMovementSet { Nothing, Enable, Disable };
    protected NonDiegeticController NDC;
    protected Inventory INV;


    protected virtual void Start()
    {
        foreach (Virtual_Module VM in ModulesToRun)
        {
            VM.Setup();
        }
    }


    public virtual void Setup()
    {
        foreach(GameObject obj in ObjectsToEnable)
        {
            obj.SetActive(true);

            DiaRoot DR = obj.GetComponentInParent<DiaRoot>();
            if (DR != null)
            {
                DR.check_for_quest_merchant();
            }
        }

        foreach (GameObject obj in ObjectsToDisable)
        {
            obj.SetActive(false);

            DiaRoot DR = obj.GetComponentInParent<DiaRoot>();
            if (DR != null)
            {
                DR.check_for_quest_merchant();
            }
        }

        if (SetMovement == PlayerMovementSet.Enable)
        {
            FindObjectOfType<PlayerMaster>().Set_PlayerControl(true);
        }
        else if(SetMovement == PlayerMovementSet.Disable)
        {
            FindObjectOfType<PlayerMaster>().Set_PlayerControl(false);
        }

        foreach(Virtual_Module VM in ModulesToRun)
        {
            VM.Run();
        }
    }
}
