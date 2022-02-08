using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidEnemy : EnemyTemplateMaster
{
    [SerializeField] protected EnemyWeaponController EWC = null;
    [SerializeField] protected HumanoidMaster HM = null;

    Combat_Master CM;
    bool In_Combat_System;
    HumanoidLogicMode humanoidLogicMode = HumanoidLogicMode.none;

    [Range(0.0f, 1.0f)] private float RollRandomness = .5f; //1.0 means up to no cd or double


    protected const float callForHelpDelay = 5f;

    ///
    protected const float baseChargeSpeed = 9f;
    protected const float baseKiteBackSpeed = 4f;
    protected const float baseKiteForwardSpeed = 7f;
    protected const float baseWalkSpeed = 3f;
    protected const float baseRollSpeed = 15f;
    ///


    private float n_RollTime;
    private float n_CallForHelpTime;

    protected bool faceTarget = false;
    protected Vector3 vec_diff;
    protected float current_distance;
    private float[] SpeedMultArray = new float[4] { .8f, 1f, 1.2f, 2.5f };
    private float[] RollTimeArray = new float[4] { float.MaxValue, 20f, 5f, 3f };
    private float rolling_timer;

    private int Unregistered_Deaths_Witnessed = 0;

    private float RollRandomnessFunc()
    {
        return 1 + ((2 * Random.value - 1) * RollRandomness);
    }

    private float SpeedMult()
    {
        return SpeedMultArray[(int)HM.Return_MovementType()];
    }

    private float GetRollSpeed()
    {
        return baseRollSpeed * SpeedMult();
    }

    protected override bool CustomFixedFunc()
    {
        if (rolling_timer > 0)
        {
            rolling_timer -= Time.deltaTime;
            animationUpdater.PlayAnimation("roll_f");
            animator.SetFloat("MoveSpeed", GetRollSpeed());
            direction = agent.nextPosition - transform.position;
            EnemyMovement();
            return true;
        }

        return false;
    }

    public void ExternalMovement(Vector3 loc_in, bool HighPriority, float speed = 3f)
    {
        agent.speed = speed * SpeedMult();
        AgentMovementFunc(loc_in, HighPriority);
    }

    public void ExternalCombatMovement(Vector3 loc_in)
    {
        AgentMovementFunc(loc_in, true);
    }

    public void Eject_From_Combat_System()
    {
        In_Combat_System = false;
    }

    private void Roll()
    {
        if (n_RollTime <= Time.time)
        {
            rolling_timer = 10.5f / GetRollSpeed();
            agent.speed = GetRollSpeed();
            n_RollTime = (RollTimeArray[(int)HM.Return_MovementType()] * RollRandomnessFunc()) + Time.time;
        }
    }

    private void CallForHelp()
    {
        if (Time.time >= n_CallForHelpTime)
        {
            (bool, bool) Tested_AllyFound = FL.RallyAlliesLoS(factionEnum, transform, CustomRep);
            if (Tested_AllyFound.Item1) //Can ask for help this frame
            {
                n_CallForHelpTime = Time.time + callForHelpDelay;
                if (Tested_AllyFound.Item2)
                {
                    HM.DisplayCallforHelp();
                }
            }
        }
    }

    public float Return_Distance_Pref()
    {
        if(humanoidLogicMode == HumanoidLogicMode.Kiting || humanoidLogicMode == HumanoidLogicMode.Kiting_reposition)
        {
            int kite_radius = 0;
            switch (HM.Return_CombatClass())
            {
                case HumanoidCombatClass.Antagonist:
                    kite_radius = 25;
                    break;
                case HumanoidCombatClass.Classless:
                    kite_radius = 10;
                    break;
                case HumanoidCombatClass.Sharpshooter:
                    kite_radius = 20;
                    break;
                case HumanoidCombatClass.Generalist:
                    kite_radius = 15;
                    break;
                case HumanoidCombatClass.Tank:
                    kite_radius = 10;
                    break;
            }
            return kite_radius;
        }
        return 0f;
    }

    protected override void AIFunc()
    {
        humanoidLogicMode = EWC.Attack();
        vec_diff = new Vector3(Return_Current_Target().position.x - transform.position.x, 0f, Return_Current_Target().position.z - transform.position.z);
        current_distance = vec_diff.magnitude;

        CallForHelp();

        if (humanoidLogicMode == HumanoidLogicMode.Flee)
        {
            Flee(); //Var agent.speed
        }
        else if (humanoidLogicMode == HumanoidLogicMode.Charge_in)
        {
            agent.speed = baseChargeSpeed * SpeedMult();
            Charge();
        }
        else if (humanoidLogicMode == HumanoidLogicMode.Kiting || humanoidLogicMode == HumanoidLogicMode.Kiting_reposition)
        {
            agent.speed = baseKiteForwardSpeed * SpeedMult();
            faceTarget = humanoidLogicMode == HumanoidLogicMode.Kiting;
            Kite(); //Var agent.speed
        }
    }

    protected override void AIEnable_extraLogic(bool Enabled_by_Rally)
    {
        //DO NOT ENABLE ALLIES directly here, do with below 
        n_CallForHelpTime = Time.time;

        In_Combat_System = false;

        n_RollTime = (RollTimeArray[(int)HM.Return_MovementType()] * RollRandomnessFunc()) + Time.time;
        HM.Start_HumanoidAttackMode();
    }

    protected override void EnemyBasicAnimation()
    {
        if (HM.return_AnimationOverride()) //Used for activities
        {
            return;
        }

        if (!Current_Target)
        {
            faceTarget = false;
        }

        if (rB.velocity.magnitude > 1)
        {
            if (faceTarget)
            {
                MoveDir moveDirection = GetMoveDirection();
                switch (moveDirection)
                {
                    case MoveDir.Forward:
                        animationUpdater.PlayAnimation("walk_jog_f");
                        break;
                    case MoveDir.Right:
                        animationUpdater.PlayAnimation("jog_r_1h");
                        break;
                    case MoveDir.Left:
                        animationUpdater.PlayAnimation("jog_l_1h");
                        break;
                    case MoveDir.Backward:
                        animationUpdater.PlayAnimation("walk_jog_b");
                        break;
                }
            }
            else
            {
                animationUpdater.PlayAnimation("walk_jog_f");
            }
        }
        else
        {
            animationUpdater.PlayAnimation("idle");
        }

        float angle;
        if (faceTarget)
        {
            Vector3 custom_direction = Current_Target.transform.position - transform.position;
            angle = Mathf.Atan2(custom_direction.x, custom_direction.z) * Mathf.Rad2Deg;
        }
        else
        {
            angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, angle, 0)), 120 * Time.fixedDeltaTime);
    }



    protected void Kite()
    {
        if (!In_Combat_System)
        {
            if (Current_Target.tag == "Player")
            {
                In_Combat_System = true;
                CM.Nearest_Ranged_Position(this);
            }
            else
            {
                if (humanoidLogicMode == HumanoidLogicMode.Kiting_reposition || current_distance > Return_Distance_Pref())
                {
                    AgentMovementFunc(Return_Current_Target().position, true);
                }
                else
                {
                    AgentMovementFunc(transform.position, true);
                }
            }
        }
    }

    protected void Charge()
    {
        if(current_distance < 5)
        {
            faceTarget = true; //Face target in melee range
        }
        else
        {
            faceTarget = false;
            Roll();
        }

        AgentMovementFunc(Return_Current_Target().position, true);
    }

    private int flee_distace = 40;
    protected void Flee()
    {
        if (current_distance < (flee_distace)) //Too close
        {
            agent.speed = 7f * SpeedMult();
            AgentMovementFunc(transform.position - vec_diff, true); //Run away
        }
        else
        {
            agent.speed = 0f;
        }
    }

    private float CalculateFacingDirection()
    {
        Vector3 v3Pos = Return_Current_Target().transform.position - transform.position;

        float angle = Mathf.Atan2(v3Pos.x, v3Pos.z) * Mathf.Rad2Deg;

        if (angle < 0)
        {
            angle += 360f;
        }
        return angle;
    }

    protected override void AIDisableFunc()
    {
        HM.Attempt_CombatModeRevert();

        int final_rep_mod = Unregistered_Deaths_Witnessed * STARTUP_DECLARATIONS.HumanoidDeathFactionChange;
        if (PlayerPercentDamage >= expDamageThreshold)
        {
            final_rep_mod += STARTUP_DECLARATIONS.HumanoidInjuryFactionChange;
        }

        FL.Modify_Reputation(factionEnum, FactionsEnum.Player, final_rep_mod);
        CustomRep = CustomReputation.Standard;
    }

    private float CalculateMovingDirection()
    {
        Vector2 direction = new Vector2(GetComponent<Rigidbody>().velocity.x, GetComponent<Rigidbody>().velocity.z);
        Vector2 normDir = direction.normalized;
        float angle = -(Vector2.SignedAngle(Vector2.up, normDir));
        if (angle < 0)
        {
            angle += 360f;
        }
        return angle;
    }

    private MoveDir GetMoveDirection()
    {
        MoveDir moveDirection = MoveDir.Forward;

        float movingAngle = CalculateMovingDirection();
        float facingAngle = CalculateFacingDirection();

        float angleDifferenceAbs = Mathf.Abs(facingAngle - movingAngle);
        float angleDifference = facingAngle - movingAngle;

        //Debug.Log(movingAngle);
        //Debug.Log(facingAngle);

        if (angleDifferenceAbs < 45 || angleDifferenceAbs > 315f)
        {
            moveDirection = MoveDir.Forward;
        }
        else if (angleDifferenceAbs < 135f || angleDifferenceAbs > 225f)
        {

            if (angleDifference > 180f || (angleDifference <= 0f && angleDifference >= -180f))
            {
                moveDirection = MoveDir.Right;
            }
            else if (angleDifference < -180f || (angleDifference > 0f && angleDifference <= 180f))
            {
                moveDirection = MoveDir.Left;
            }
            else
            {
                Debug.LogError("Move Direction is Broken");
            }
        }
        else
        {
            moveDirection = MoveDir.Backward;
        }

        return moveDirection;
    }

    protected override void Death_extraLogic()
    {
        animationUpdater.PlayAnimation("No Motion", false, true); //Stops attacks after death
        transform.Find("Body").GetComponent<Collider>().enabled = false;

        HM.ExtraDeathLogic();


        Transform Witness = FL.ReturnAlly_withinDistance(factionEnum, transform, 40);

        if(PlayerPercentDamage > expDamageThreshold)
        {
            Unregistered_Deaths_Witnessed += 1;
        }

        if (Witness)
        {
            HumanoidEnemy Wit_ETM = Witness.GetComponent<HumanoidEnemy>();
            if (Wit_ETM)
            {
                Wit_ETM.Witness_Death(Unregistered_Deaths_Witnessed);
                //Wit_ETM.EnableAI(false);
            }
        }
    }

    public void Witness_Death(int Deaths_Passed)
    {
        Unregistered_Deaths_Witnessed += Deaths_Passed;
    }

    public override void EnemyKnockback(Vector3 Force)
    {
        animationUpdater.PlayAnimation("No Motion", false, true); //Stops attacks
        base.EnemyKnockback(Force);
    }

    protected override void Awake()
    {
        base.Awake();
        CM = FindObjectOfType<Combat_Master>();
    }
}
