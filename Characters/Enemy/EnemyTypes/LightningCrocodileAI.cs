using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LightningCrocodileAI : EnemyTemplateMaster
{
    [SerializeField] protected float sphere_cast_cooldown = 10f;
    [SerializeField] protected float lightning_cast_cooldown = 10f;
    [SerializeField] protected SphereThrowingEnemy sphere_script = null;
    [SerializeField] protected LightningCastingEnemy lightning_script = null;
    

    protected float sphere_next_cast;
    protected float lightning_next_cast;


    protected override void Awake()
    {
        base.Awake();
        MAX_ANGLE = Mathf.PI / 12;
        NORMAL_ROT_SPEED = 120f;
        sphere_next_cast = 0;
        lightning_next_cast = 0;
    }

    protected override void EnemyBasicAnimation()
    {
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float RotationSpeed = 360 * rB.velocity.magnitude / (rB.velocity.magnitude * rB.velocity.magnitude + 10); //Can only rotate at medium speeds
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, angle, 0)), RotationSpeed * Time.fixedDeltaTime);
        animationUpdater.PlayAnimation("Blend Tree");
    }

    protected override void AIFunc()
    {
        if (sphere_script.return_BAiU() || lightning_script.return_BAiU())
        {
            return;
        }


        if ((sphere_next_cast <= timer) && sphere_script.CheckCast())
        {
            cc_immune = true;
            sphere_next_cast = timer + sphere_cast_cooldown;
        }
        else if (lightning_next_cast <= timer && lightning_script.CastMechanics())
        {
            cc_immune = true;
            lightning_next_cast = timer + lightning_cast_cooldown;
        }
        else
        {
            cc_immune = false;
            base.AIFunc();
        }
    }

    public override void AnimationCalledFunc0() //Fires in the middle of the animation
    {
        sphere_script.CastMechanicsForce();
    }

    public override void AnimationCalledFunc1() //Fires in the middle of the animation
    {
        lightning_script.CastMechanicsForce();
    }
}
