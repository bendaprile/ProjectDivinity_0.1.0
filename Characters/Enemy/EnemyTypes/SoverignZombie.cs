using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoverignZombie : ZombieAI
{
    [SerializeField] Light bodyLight;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        MAX_ANGLE = Mathf.PI / 6;
        NORMAL_ROT_SPEED = 720f;
        animator.SetFloat("MoveSet", .5f);
    }

    protected override void Death_extraLogic()
    {
        bodyLight.enabled = false;
        base.Death_extraLogic();
    }

    public override void EnemyKnockback(Vector3 Force)
    {
        if (cc_immune || !knockbackable)
        {
            return;
        }
        animationUpdater.PlayBlockingAnimation("quick_take_damage", true); //Knockback duration determined by animation length
        animationUpdater.PlayAnimation("no_attack", false, true);
        rB.velocity = new Vector3(0f, 0f, 0f);
        rB.AddForce(Force);
    }

    private void Update()
    {
        float red_level = health.ReturnCurrentHealthProportion();
        bodyLight.color = new Color(1, red_level, red_level);
    }
}
