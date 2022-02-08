using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 6f;
    public float runSpeedMultiplier = 1.7f;
    public float rollSpeedMultiplier = 2.5f;


    [SerializeField] private float forceConst = 50f;
    [SerializeField] private float rollEnergyCost = 40f;
    [SerializeField] private float MinSprintCost = 5f;
    [SerializeField] private float sprintEnergyCost = 0.2f;

    private Vector3 heading, forward, side;
    private MoveState moveState = MoveState.Idle;

    private ActivePerks ap;
    private Rigidbody rb;
    private PlayerAnimationUpdater animationUpdater;
    private Energy energy;
    private CursorLogic cursorLogic;

    private float heldRotation;
    private bool FollowCursor_tracking;

    private bool HoldCursorFollow = false;
    private float FollowCursorTimer = 0f;

    private float RollTimer = 0f;

    private float StandardSpeedMultiplier = 1f;

    private float MeleeForceTimer;
    private float MeleeForce;

    private WeaponController WC;
    private Collider HitboxCollider;

    // Start is called before the first frame update
    void Start()
    {
        forward = new Vector3(1/Mathf.Sqrt(2), 0, 1/Mathf.Sqrt(2));
        heldRotation = 0f;

        forward = Vector3.Normalize(forward);
        side = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
        rb = GetComponent<Rigidbody>();
        animationUpdater = GetComponentInChildren<PlayerAnimationUpdater>();
        energy = GameObject.Find("Player").GetComponent<Energy>();
        ap = GameObject.Find("Player").GetComponentInChildren<ActivePerks>();
        cursorLogic = GameObject.Find("Master Object").GetComponent<CursorLogic>();
        WC = GetComponentInChildren<WeaponController>();
        HitboxCollider = transform.Find("Hitbox").GetComponent<Collider>();
    }

    public void ChangeAngle(float angle)
    {
        forward = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), 0, Mathf.Cos(Mathf.Deg2Rad * angle));
        side = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
    }
    
    public void Move()
    {
        if (GetMoveState() == MoveState.ForceBased)
        {
            if(MeleeForceTimer > 0)
            {
                MeleeForceTimer -= Time.fixedDeltaTime;
                if(MeleeForceTimer <= 0)
                {
                    rb.AddForce(heading * MeleeForce);
                }
            }

            return;
        }

        float forceMult = forceConst;
        if (moveState == MoveState.Rolling)
        {
            forceMult = forceConst * 4; //extra force while rolling to make it more instant
        }
        
        Vector3 desired_movement = heading * GetDesiredSpeed() - rb.velocity;

        Vector3 force;
        if (desired_movement.magnitude > 1f)
        {
            desired_movement = desired_movement.normalized;
            force = forceMult * desired_movement;
        }
        else
        {
            force = forceMult * desired_movement;
        }

        rb.AddForce(new Vector3(force.x, 0, force.z));

        if (moveState == MoveState.FollowCursor)
        {
            if (!HoldCursorFollow)
            {
                FollowCursorTimer -= Time.fixedDeltaTime;
            }

            if(FollowCursor_tracking)
            {
                Vector3 diffVector = cursorLogic.ReturnPlayer2Cursor();
                heldRotation = Mathf.Atan2(diffVector.x, diffVector.z) * Mathf.Rad2Deg;
            }
        }
        else
        {
            if (heading.magnitude != 0)
            {
                heldRotation = Mathf.Atan2(heading.x, heading.z) * Mathf.Rad2Deg;
            }
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, heldRotation, 0)), 360 * Time.fixedDeltaTime);
    }

    public void PauseAimTowardCamera()
    {
        moveState = MoveState.Idle;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, Camera.main.transform.rotation.eulerAngles.y + 180f, 0)), 270 * Time.unscaledDeltaTime);
    }

    public void DialogueMenuAimTowards(Vector3 pos)
    {
        moveState = MoveState.Idle;
  
        //Player Rotation
        Vector3 direction = (pos - transform.position);
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, angle, 0)), 270 * Time.unscaledDeltaTime);
    }

    private float GetDesiredSpeed() //CALL THIS ONLY ONCE PER FRAME OR SPRINT WILL GET UPDATED TOO MANY TIMES
    {
        if (moveState == MoveState.Running)
        {
            if(StandardSpeedMultiplier < runSpeedMultiplier)
            {
                StandardSpeedMultiplier += Time.fixedDeltaTime;
            }
        }
        else
        {
            if (StandardSpeedMultiplier > 1f)
            {
                StandardSpeedMultiplier -= Time.fixedDeltaTime;
            }
        }

        switch (moveState)
        {
            case MoveState.Idle:
                return 0f;
            case MoveState.Walking:
                return baseSpeed * StandardSpeedMultiplier;
            case MoveState.Running:
                return baseSpeed * StandardSpeedMultiplier;
            case MoveState.Rolling:
                return baseSpeed * rollSpeedMultiplier;
            case MoveState.FollowCursor:
                return baseSpeed;
            default:
                return 0f;
        }
    }

    public void UpdatePlayerState()
    {
        if (GetMoveState() == MoveState.ForceBased)
        {
            return;
        }
        else if(GetMoveState() == MoveState.Rolling)
        {
            RollTimer -= Time.deltaTime;
            if(RollTimer > 0)
            {
                return;
            }
        }
        else
        {
            heading = side * Input.GetAxis("Horizontal") + forward * Input.GetAxis("Vertical");
            heading = Vector3.Normalize(new Vector3(heading.x, 0, heading.z));
        }

        if (Input.GetKeyDown(KeyCode.Space)) //Can cancel Ranged animation
        {
            if (energy.Drain_ES(false, rollEnergyCost))
            {
                Roll();
                return;
            }
        }
        else if(moveState == MoveState.FollowCursor && (FollowCursorTimer > 0 || HoldCursorFollow))
        {
            return;
        }


        // TODO: Handle for controllers
        float controlThrowX = Mathf.Abs(Input.GetAxis("Horizontal"));
        float controlThrowY = Mathf.Abs(Input.GetAxis("Vertical"));

        if (controlThrowX > 0 || controlThrowY > 0)
        {
            if (Input.GetKey(KeyCode.LeftShift) && energy.Drain_ES_Greater(false, sprintEnergyCost, (moveState != MoveState.Running), MinSprintCost))
            {
                moveState = MoveState.Running;
            }
            else
            {
                moveState = MoveState.Walking;
            }
        }
        else
        {
            moveState = MoveState.Idle;
        } 
    }

    public MoveState GetMoveState()
    {
        return moveState;
    }

    public float GetMoveSpeed()
    {
        return rb.velocity.magnitude;
    }

    public float GetRollSpeed()
    {
        return baseSpeed * rollSpeedMultiplier;
    }

    /////////////////////////////////////////////////
    public void Roll()
    {
        RollTimer = 10.5f / GetRollSpeed();
        FollowCursorTimer = 0f; //Cancel

        if (heading.magnitude == 0) //Roll correctly if no movement is pressed
        {
            float CurrentRotation = heldRotation * Mathf.Deg2Rad;
            heading = new Vector3(Mathf.Sin(CurrentRotation), 0f, Mathf.Cos(CurrentRotation));
        }

        WC.EndSustained();
        InstantRotation(false);
        ap.PlayerIsRolling();
        animationUpdater.RollAnimation();
        moveState = MoveState.Rolling; //Has to be after RollAnimation
    }

    public void MeleeAttack(bool start, float force, float force_delay = .1f)
    {
        if (start)
        {
            FollowCursorTimer = 0f; //Cancel
            moveState = MoveState.ForceBased;
            rb.velocity = Vector3.zero;

            heading = cursorLogic.ReturnPlayer2Cursor().normalized;
            heldRotation = Mathf.Atan2(heading.x, heading.z) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(new Vector3(0, heldRotation, 0));
            MeleeForceTimer = force_delay;
            MeleeForce = force;
        }
        else if(moveState == MoveState.ForceBased) //This way so it can be called multiple times
        {
            moveState = MoveState.Idle;
        }
    }

    public void SetFollowCursor(float followCursorDelay = 1f, bool tracking = false)
    {
        InstantRotation(true);
        moveState = MoveState.FollowCursor;
        FollowCursorTimer = followCursorDelay;
        FollowCursor_tracking = tracking;

        Vector3 diffVector = cursorLogic.ReturnPlayer2Cursor();
        heldRotation = Mathf.Atan2(diffVector.x, diffVector.z) * Mathf.Rad2Deg;
    }

    public void SetFollowCursorSustained(float followCursorDelay = 1f, bool tracking = false) //This delay is started after it is canceled
    {
        InstantRotation(true);
        moveState = MoveState.FollowCursor;
        FollowCursorTimer = followCursorDelay;
        FollowCursor_tracking = tracking;
        HoldCursorFollow = true;

        Vector3 diffVector = cursorLogic.ReturnPlayer2Cursor();
        heldRotation = Mathf.Atan2(diffVector.x, diffVector.z) * Mathf.Rad2Deg;
    }

    public void CancelFollowCursorSustained()
    {
        HoldCursorFollow = false;
    }

    private void InstantRotation(bool toCursor)
    {
        float RangedGlobalAngle;
        if (toCursor)
        {
            Vector3 diffVector = cursorLogic.ReturnPlayer2Cursor();
            RangedGlobalAngle = Mathf.Atan2(diffVector.x, diffVector.z) * Mathf.Rad2Deg;
        }
        else
        {
            RangedGlobalAngle = Mathf.Atan2(heading.x, heading.z) * Mathf.Rad2Deg;
        }

        transform.rotation = Quaternion.Euler(new Vector3(0, RangedGlobalAngle, 0));
    }
}
