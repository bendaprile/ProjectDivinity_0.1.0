using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationUpdater : MonoBehaviour
{
    public bool postAnimation = false;

    [SerializeField] float transitionFade = 0.25f;
    [SerializeField] private UIController uiController = null;
    string currentAnim = "idle";

    MoveDir moveDirection;
    public float moveSpeed = 0f;

    GameObject currentWeapon;

    // Stored references
    Animator animator;
    PlayerMovement playerMovement;
    Direction direction;
    WeaponController weaponController;


    //////////////////////////
    public void RangedAttack(RangedAnimation rangedType, bool WeaponChargeAttack = false)
    {
        animator.SetFloat("shootNum", (float)rangedType);
        animator.SetTrigger("shootAnim");
        animator.SetBool("WeaponChargeAttack", WeaponChargeAttack);
    }

    public void ReloadAnimation()
    {
        animator.SetTrigger("reloadAnim");
    }

    public void ChargeAttackOver()
    {
        animator.SetBool("WeaponChargeAttack", false);
    }

    public void AbilityAttack(AbilityAnimation abilityAnimation)
    {
        animator.SetFloat("abilityNum", (float)abilityAnimation);
        //if (animator.GetFloat("abilityNum") > 7.5) { animator.applyRootMotion = true; }
        animator.SetTrigger("abilityLowerAnim"); //needed because if lower and upper are not both ready it will be set false
        animator.SetTrigger("abilityAnim");
    }

    public void MeleeAttack(MeleeAnimation meleeAnimation)
    {
        animator.applyRootMotion = true;
        animator.SetFloat("meleeNum", (float)meleeAnimation);
        animator.SetTrigger("meleeLowerAnim"); //needed because if lower and upper are not both ready it will be set false
        animator.SetTrigger("meleeAnim");
    }

    public void PlayAnimation(string animName, bool rootMotion = false)
    {
        if (currentAnim != animName)
        {
            animator.applyRootMotion = rootMotion;

            animator.CrossFadeInFixedTime(animName, transitionFade);
            currentAnim = animName;
        }
    }

    public void UpdateAnimation()
    {
        UpdateMoveSpeed();

        currentWeapon = weaponController.getCurrentWeapon();

        if (currentWeapon && currentWeapon.GetComponent<Ranged>())
        {
            RangedAnimation rangedAnimation = currentWeapon.GetComponent<Ranged>().rangedAnimation;
            animator.SetBool("is2hRanged", rangedAnimation == RangedAnimation.Ranged_2H);
        }
        else
        {
            animator.SetBool("is2hRanged", false);
        }

        if (CanUpdateAnimation())
        {
            UpdateMoveAnimation();
        }
    }

    public void UpdateAnimationPauseMenu()
    {
        UpdateMoveSpeed();
        currentWeapon = weaponController.getCurrentWeapon();

        if (currentWeapon && currentWeapon.GetComponent<Ranged>())
        {
            RangedAnimation rangedAnimation = currentWeapon.GetComponent<Ranged>().rangedAnimation;
            animator.SetBool("is2hRanged", rangedAnimation == RangedAnimation.Ranged_2H);
        }
        else
        {
            animator.SetBool("is2hRanged", false);
        }
    }

    //////////////////////////


    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        direction = GetComponent<Direction>();
        weaponController = GetComponent<WeaponController>();
    }

    private void UpdateMoveAnimation()
    {
        if (playerMovement.GetMoveSpeed() > .1f)
        {
            if (playerMovement.GetMoveState() == MoveState.FollowCursor)
            {
                moveDirection = direction.GetMoveDirection();
                switch (moveDirection)
                {
                    case MoveDir.Forward:
                        PlayAnimation("walk_jog_f");
                        break;
                    case MoveDir.Right:
                        PlayAnimation("jog_r_1h");
                        break;
                    case MoveDir.Left:
                        PlayAnimation("jog_l_1h");
                        break;
                    case MoveDir.Backward:
                        PlayAnimation("walk_jog_b");
                        break;
                }
            }
            else
            {
                PlayAnimation("walk_jog_f");
            }
        }
        else
        {
            PlayAnimation("idle");
        }
    }

    public void RollAnimation()
    {
        PlayAnimation("roll_f");
    }

    public void DisableRootMotion()
    {
        animator.applyRootMotion = false;
    }

    public void OnAnimatorMove()
    {
        if (animator.applyRootMotion)
        {
            transform.parent.position += animator.deltaPosition;
        }
    }

    public bool CanUpdateAnimation()
    {
        return !(playerMovement.GetMoveState() == MoveState.Rolling)
            && !(playerMovement.GetMoveState() == MoveState.ForceBased)
            && uiController.current_UI_mode == UI_Mode.Normal;
    }


    private void UpdateMoveSpeed()
    {
        if(playerMovement.GetMoveState() == MoveState.Rolling)
        {
            animator.SetFloat("MoveSpeed", playerMovement.GetRollSpeed());
        }
        else
        {
            animator.SetFloat("MoveSpeed", playerMovement.GetMoveSpeed());
        }
    }

    public void SetUpdateMode(AnimatorUpdateMode updateMode)
    {
        animator.updateMode = updateMode;
    }
}
