using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivePerks : MonoBehaviour
{
    private (bool, List<float>)[] ImplantExclusiveActiveArray = new (bool, List<float>)[STARTUP_DECLARATIONS.Number_of_ExclusiveActivePerks]; //Only 1 of each type can be enabled, don't stack

    public void AddImplantPerk(ExclusiveActivePerksEnum Perk_in, List<float> stats)
    {
        ImplantExclusiveActiveArray[(int)Perk_in] = (true, stats);
    }

    public void ResetImplantPerks()
    {
        for(int i = 0; i < STARTUP_DECLARATIONS.Number_of_ExclusiveActivePerks; i++)
        {
            ImplantExclusiveActiveArray[i] = (false, new List<float>());
        }
    }

    //////////////////////////////////////////////////////////
    public void PlayerIsRolling()
    {
        if (ImplantExclusiveActiveArray[(int)ExclusiveActivePerksEnum.RollReload].Item1) //Value doesn't matter
        {
            weaponController.Reload(true);
        }
    }
    //////////////////////////////////////////////////////////

    public bool InstantTele()
    {
        return ImplantExclusiveActiveArray[(int)ExclusiveActivePerksEnum.InstantTele].Item1;
    }

    public bool ProjShieldReflection()
    {
        return ImplantExclusiveActiveArray[(int)ExclusiveActivePerksEnum.ProjShieldReflection].Item1;
    }

    public float LightningSphereCast() //(Duration of turret, -1 means no turret)
    {
        if (ImplantExclusiveActiveArray[(int)ExclusiveActivePerksEnum.LightningSphereTurret].Item1)
        {
            return ImplantExclusiveActiveArray[(int)ExclusiveActivePerksEnum.LightningSphereTurret].Item2[0];
        }
        else
        {
            return -1f;
        }
    }


    private WeaponController weaponController;

    private void Start()
    {
        weaponController = GameObject.Find("Player").GetComponentInChildren<WeaponController>();
    }
}
