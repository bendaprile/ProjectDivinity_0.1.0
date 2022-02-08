using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using TMPro;

public class InteractiveBox : InteractiveObject
{
    public string interactiveBoxName = "Chest";
    [SerializeField] protected Material[] NormalMats = null;
    [SerializeField] protected Material[] HoverMats = null;
    [SerializeField] protected MeshRenderer[] MeshesForMats = null;

    protected override void Awake()
    {
        Assert.IsTrue(NormalMats.Length == HoverMats.Length);
        Assert.IsTrue(NormalMats.Length == MeshesForMats.Length);

        base.Awake(); 
        UIControl = GameObject.Find("UI").GetComponent<UIController>();
    }

    public override void CursorOverObject()
    {
        base.CursorOverObject();
        set_item_material(true);
    }

    public override void CursorLeftObject()
    {
        base.CursorLeftObject();
        set_item_material(false);
    }

    protected virtual void set_item_material(bool active)
    {
        Material[] TempMats = active ? HoverMats : NormalMats;

        for(int i = 0; i < TempMats.Length; ++i)
        {
            MeshesForMats[i].material = TempMats[i];
        }
    }


    protected override void SuccessfullyActivated()
    {
        base.SuccessfullyActivated();
        UIControl.OpenInteractiveMenu(gameObject);
    }

    // TODO: Handle Input through InputManager and not direct key references
    protected override void ActivateLogic()
    {
        bool empty = GetComponentInChildren<ExternalItemStorage>().ReturnItems().Count <= 0;

        if(LockPickingRequirement == 0)
        {
            miscDisplay.enableDisplay("", "(F) Open" + (empty ? " (Empty)" : ""));
            if (Input.GetKeyDown(KeyCode.F))
            {
                SuccessfullyActivated();
            }
        }
        else if (playerStats.ReturnSkill(SkillsEnum.Larceny) >= STARTUP_DECLARATIONS.ReturnSkillCheck(playerStats.returnLevel(), LockPickingRequirement))
        {
            miscDisplay.enableDisplay("", "Larceny [" + playerStats.ReturnSkill(SkillsEnum.Larceny) + "/" + LockPickingRequirement + "] (F)" + (empty ? " (Empty)" : ""));
            if (Input.GetKeyDown(KeyCode.F))
            {
                SuccessfullyActivated();
            }
        }
        else if (keyToUnlock && inventory.ReturnItems_ByName(keyToUnlock).Count > 0)
        {
            miscDisplay.enableDisplay("", "Use Key: (F)" + (empty ? " (Empty)" : ""));
            if (Input.GetKeyDown(KeyCode.F))
            {
                SuccessfullyActivated();
            }
        }
        else
        {
            miscDisplay.enableDisplay("", "Larceny [" + playerStats.ReturnSkill(SkillsEnum.Larceny) + "/" + LockPickingRequirement + "]" + (empty ? " (Empty)" : ""));
        }
    }
}
