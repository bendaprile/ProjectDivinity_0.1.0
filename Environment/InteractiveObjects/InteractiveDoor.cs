using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveDoor : InteractiveObject
{
    [SerializeField] protected Material highlighted_mat = null;
    [SerializeField] bool automaticDoor = false;
    [SerializeField] AudioClip[] Sounds = null;


    private AudioSource AS;
    private DoorController[] doorControllers;

    public bool DoorOpen = false;

    InteractiveDoor()
    {

    }

    protected override void Awake()
    {
        base.Awake();

        AS = GetComponent<AudioSource>();
        doorControllers = GetComponentsInChildren<DoorController>();
    }

    public override void ChangeLockReq(SkillCheckDifficulty SCD)
    {
        base.ChangeLockReq(SCD);
        if(SCD != SkillCheckDifficulty._None_ && DoorOpen)
        {
            SuccessfullyActivated();
        }
    }

    protected override void Update()
    {
        bool keyPass = keyToUnlock ? inventory.ReturnItems_ByName(keyToUnlock).Count > 0 : true;

        if (automaticDoor && LockPickingRequirement == SkillCheckDifficulty._None_ && keyPass)
        {
            if (PIR.isTrue && !DoorOpen) //All will be the same (open)
            {
                SuccessfullyActivated();
            }
            else if (!PIR.isTrue && DoorOpen) //All will be the same (close)
            {
                SuccessfullyActivated();
            }
        }
        else
        {
            base.Update();
        }
    }


    public override void CursorOverObject()
    {
        base.CursorOverObject();
        foreach (DoorController DC in doorControllers)
        {
            DC.UpdateMesh(highlighted_mat);
        }
    }

    public override void CursorLeftObject()
    {
        base.CursorLeftObject();
        foreach (DoorController DC in doorControllers)
        {
            DC.UpdateMesh(null);
        }
    }

    protected override void SuccessfullyActivated()
    {
        base.SuccessfullyActivated();
        DoorOpen = !DoorOpen;

        if (Sounds.Length == 2)
        {
            if (DoorOpen)
            {
                AS.PlayOneShot(Sounds[0]);
            }
            else
            {
                AS.PlayOneShot(Sounds[1]);
            }
        }

        foreach (DoorController DC in doorControllers)
        {
            DC.ActivateDoor();
        }
    }


    // TODO: Handle Input through InputManager and not direct key references
    protected override void ActivateLogic()
    {
        string openCloseText = DoorOpen ? "Close" : "Open";

        if (keyToUnlock && inventory.ReturnItems_ByName(keyToUnlock).Count <= 0)
        {
            miscDisplay.enableDisplay("", "Locked: Key Required", SkillCheckStatus.Failure);
            return;
        }

        if (LockPickingRequirement == SkillCheckDifficulty._None_)
        {
            miscDisplay.enableDisplay("", "(F) " + openCloseText);
            if (Input.GetKeyDown(KeyCode.F))
            {
                SuccessfullyActivated();
            }
        }
        else if (playerStats.ReturnSkill(SkillsEnum.Larceny) >= STARTUP_DECLARATIONS.ReturnSkillCheck(playerStats.returnLevel(), LockPickingRequirement))
        {
            miscDisplay.enableDisplay("",
                "Larceny Met [" + playerStats.ReturnSkill(SkillsEnum.Larceny) + "/" + STARTUP_DECLARATIONS.ReturnSkillCheck(playerStats.returnLevel(), LockPickingRequirement) + "] (F)",
                SkillCheckStatus.Success);
            if (Input.GetKeyDown(KeyCode.F))
            {
                SuccessfullyActivated();
            }
        }
        else if (keyToUnlock && inventory.ReturnItems_ByName(keyToUnlock).Count > 0)
        {
            miscDisplay.enableDisplay("", "Use Key: (F) " + openCloseText, SkillCheckStatus.Success);
            if (Input.GetKeyDown(KeyCode.F))
            {
                SuccessfullyActivated();
            }
        }
        else
        {
            miscDisplay.enableDisplay("",
                "Larceny Not Met [" + playerStats.ReturnSkill(SkillsEnum.Larceny) + "/" + STARTUP_DECLARATIONS.ReturnSkillCheck(playerStats.returnLevel(), LockPickingRequirement) + "]",
                SkillCheckStatus.Failure);
        }
    }
}
