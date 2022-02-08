using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : EnemyTemplateMaster
{
    [SerializeField] private float AttackCD = 2f;
    private ZombieAttack zombieAttack;

    private float zombieTimer;

    protected override void Awake()
    {
        base.Awake();
        MAX_ANGLE = Mathf.PI / 12;
        NORMAL_ROT_SPEED = 240f;
        zombieAttack = GetComponentInChildren<ZombieAttack>();
        zombieTimer = 0f;
    }

    public override void EnemyKnockback(Vector3 Force)
    {
        base.EnemyKnockback(Force);
        animationUpdater.PlayAnimation("no_attack", false, true);
    }


    protected override void AIFunc()
    {
        base.AIFunc();
        if(timer >= zombieTimer)
        {
            if (zombieAttack.CheckCast())
            {
                zombieTimer = timer + AttackCD;
                animationUpdater.PlayAnimation("attack");
            }
        }
    }

    public override void AnimationCalledFunc0() //Fires in the middle of the animation
    {
        zombieAttack.CastMechanicsForce();
    }

    protected override void Death_extraLogic()
    {
        animationUpdater.PlayAnimation("no_attack", false, true); //Stops attacks after death
    }

}
