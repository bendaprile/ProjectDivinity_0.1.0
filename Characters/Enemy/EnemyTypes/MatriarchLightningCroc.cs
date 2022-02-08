using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatriarchLightningCroc : LightningCrocodileAI
{
    [SerializeField] protected SphereThrowingEnemy sphere_surround_script = null;
    [SerializeField] protected float sphere_surround_cooldown = 15f;
    private float surround_sphere_next_cast;


    protected override void Awake()
    {
        base.Awake();
        surround_sphere_next_cast = 0;
    }

    public override void AnimationCalledFunc2() //Fires in the middle of the animation
    {
        sphere_surround_script.CastMechanicsForce();
    }

    protected override void AIFunc()
    {
        if (sphere_surround_script.return_BAiU())
        {
            return;
        }

        if ((surround_sphere_next_cast <= timer) && sphere_surround_script.CheckCast())
        {
            cc_immune = true;
            surround_sphere_next_cast = timer + sphere_surround_cooldown;
            sphere_next_cast = timer + sphere_cast_cooldown; //Stop casts of this right after
            lightning_next_cast = timer + lightning_cast_cooldown; //Stop casts of this right after
            return;
        }
        else
        {
            base.AIFunc(); //NOTE this is lightningCrocodile not ETM
        }
    }
}
