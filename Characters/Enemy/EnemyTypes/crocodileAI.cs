using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class crocodileAI : EnemyTemplateMaster
{
    [SerializeField] protected float charge_cooldown = 10f;
    [SerializeField] protected float AoE_cooldown = 10f;
    [SerializeField] protected ChargingEnemy charge_script = null;
    [SerializeField] protected PhysicalAoeEnemy AoE_script = null;
    [SerializeField] protected GameObject wedge = null;

    private float next_charge = 0;
    private float next_AoE = 0;

    protected override void Awake()
    {
        base.Awake();
        MAX_ANGLE = Mathf.PI / 12;
        NORMAL_ROT_SPEED = 60f;
    }

    protected override void AIFunc()
    {
        if(charge_script.return_BAiU())
        {
            return;
        }
        else if (AoE_script.return_BAiU()) //Stops croc movement
        {
            Assert.IsFalse(NormalMovement);
            direction = new Vector3(0f, 0f, 0f);
            mod_direction = direction;
            AgentMovementFunc(transform.position, true);
            EnemyMovement();
            return;
        }


        if ((next_AoE <= timer) && Current_Target && AoE_script.PhysicalAoe())
        {
            NormalMovement = false; //Must disable to use external animations in Physical AoE
            cc_immune = true;
            next_AoE = timer + AoE_cooldown;
        }
        else if (next_charge <= timer && Current_Target && charge_script.Charge())
        {
            NormalMovement = false;
            cc_immune = true;
            next_charge = timer + charge_cooldown;
        }
        else
        {
            base.AIFunc();
            NormalMovement = true;
            cc_immune = false;
        }
    }

    protected override void Death_extraLogic()
    {
        wedge.SetActive(false);
    }
}
