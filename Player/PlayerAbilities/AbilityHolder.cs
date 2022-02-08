using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHolder : MonoBehaviour
{
    private Ability[] AbilityArray = new Ability[9];

    private void Start()
    {
        AbilityArray[(int)AbilityHolderEnum.LigSphThr] = GetComponentInChildren<LightningSphereThrow>();
        AbilityArray[(int)AbilityHolderEnum.LigBolCas] = GetComponentInChildren<LightningBoltCast>();
        AbilityArray[(int)AbilityHolderEnum.LigBlaCas] = GetComponentInChildren<LightningBlastCast>();
        AbilityArray[(int)AbilityHolderEnum.LigMel] = GetComponentInChildren<LightningMelee>();
        AbilityArray[(int)AbilityHolderEnum.TimDia] = GetComponentInChildren<TimeDilation>();
        AbilityArray[(int)AbilityHolderEnum.ProShiCas] = GetComponentInChildren<ProjectileShieldCast>();
        AbilityArray[(int)AbilityHolderEnum.Tel] = GetComponentInChildren<Teleportation>();
        AbilityArray[(int)AbilityHolderEnum.KinRev] = GetComponentInChildren<KineticReversal>();
        AbilityArray[(int)AbilityHolderEnum.InsHea] = GetComponentInChildren<InstantHeal>();
    }

    public Ability Return_AbilityArray(AbilityHolderEnum ABE)
    {
        return AbilityArray[(int)ABE];
    }
}
