using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolatileZombieAI : ZombieAI
{
    private PhysicalAoeEnemy POE;

    protected override void Awake()
    {
        base.Awake();
        POE = GetComponentInChildren<PhysicalAoeEnemy>();
    }

    protected override void Death_extraLogic()
    {
        POE.PhysicalAoe_force();
        base.Death_extraLogic();
    }
}
