using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtrocityAI : EnemyTemplateMaster
{
    [SerializeField] private float primaryAttackCD = 2f;
    [SerializeField] private float jumpAttackCD = 5f;
    private ZombieAttack primaryAttack;
    private JumpAttack jumpAttack;
    private float primaryTimer;
    private float jumpTimer;

    protected override void Awake()
    {
        base.Awake();
        primaryAttack = GetComponentInChildren<ZombieAttack>();
        jumpAttack = GetComponentInChildren<JumpAttack>();
        primaryTimer = 0f;
        jumpTimer = 0f;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        animationUpdater = GetComponentInChildren<EnemyAnimationUpdater>(); //This will not be found yet
        animationUpdater.Setup_Logic(); //This is needed because awake will not be called yet
        animationUpdater.PlayBlockingAnimation("Sit_Out_Of_ Ground");
    }

    protected override void AIFunc()
    {
        if (jumpAttack.return_BAiU())
        {
            return;
        }

        if (timer >= primaryTimer && primaryAttack.CheckCast())
        {
            NormalMovement = true;
            cc_immune = false;
            primaryTimer = timer + primaryAttackCD;
            animationUpdater.PlayBlockingAnimation("primary_attack"); //Needs to be blocking, because not seperate top and bottom animators
            return;
        }
        else if(timer >= jumpTimer && jumpAttack.CheckCast())
        {
            NormalMovement = false; //Must disable to use external animations in Jump
            cc_immune = true;
            jumpTimer = timer + jumpAttackCD;
            return;
        }
        else
        {
            NormalMovement = true;
            cc_immune = false;
            base.AIFunc();
        }
    }

    public override void AnimationCalledFunc0() //Fires in the middle of the animation
    {
        primaryAttack.CastMechanicsForce();
    }
}
