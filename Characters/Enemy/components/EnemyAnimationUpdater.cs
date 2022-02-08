using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnemyAnimationUpdater : MonoBehaviour
{
    private bool blocked = false;

    [SerializeField] private EnemyTemplateMaster ETM = null; //used for AnimationCalledFuncs

    [SerializeField] private float transitionFade = 0.25f;
    [SerializeField] private float forcetransitionFade = 0.1f;
    private string currentAnim = "idle";

    Animator animator;

    public void Setup_Logic()
    {
        animator = GetComponent<Animator>();
    }

    private void Awake()
    {
        Setup_Logic();
    }

    public bool ReturnBlocked()
    {
        return blocked;
    }

    public void PlayAnimation(string animName, bool rootMotion = false, bool force = false, bool unScaledTime = false)
    {
        if (!blocked || force)
        {
            float fadeVar;
            if (force)
            {
                fadeVar = forcetransitionFade;
            }
            else
            {
                fadeVar = transitionFade;
            }
            animator.applyRootMotion = rootMotion;

            if (unScaledTime)
            {
                animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            }
            else
            {
                animator.updateMode = AnimatorUpdateMode.Normal;
            }

            if (currentAnim != animName)
            {
                animator.CrossFadeInFixedTime(animName, fadeVar);
                currentAnim = animName;
            }
        }
    }

    //IMPORTANT Blocking animations must end with an animation over. If another blocking animation is called while one is being played, the first one will be overwritten.
    public void PlayBlockingAnimation(string animName, bool rootMotion = false) //Ends Upon AnimationOver getting called (Knockback, or self-disable)
    {
        
        animator.applyRootMotion = rootMotion;
        blocked = true;

        animator.CrossFadeInFixedTime(animName, forcetransitionFade);
        currentAnim = animName;
    }

    private void OnAnimatorMove()
    {
        if (animator.applyRootMotion)
        {
            transform.parent.position += animator.deltaPosition;
        }
    }

    public void AnimationOver() //Called to unblock an animation
    {
        blocked = false;
        animator.applyRootMotion = false;
    }

    public void AnimationCalledFunc0_inUpdater()
    {
        ETM.AnimationCalledFunc0();
    }

    public void AnimationCalledFunc1_inUpdater()
    {
        ETM.AnimationCalledFunc1();
    }

    public void AnimationCalledFunc2_inUpdater()
    {
        ETM.AnimationCalledFunc2();
    }



    ///////////////Humanoid ONLY/////////////////////////
    public void RangedAttack(RangedAnimation rangedType)
    {
        animator.SetFloat("shootNum", (float) rangedType);
        animator.SetTrigger("shootAnim");
    }

    public void MeleeAttack(MeleeAnimation meleeAnimation)
    {
        animator.SetFloat("meleeNum", (float)meleeAnimation);
        animator.SetTrigger("meleeLowerAnim"); //needed because if lower and upper are not both ready it will be set false
        animator.SetTrigger("meleeAnim");
    }

    public void Set_is2hRanged(GameObject currentWeapon) //Set on combat enter and combat end
    {
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

    public void PlayGesture(float num, bool headOnly = false) //TODO: remove commented code when this is confirmed to be working
    {
        //UnityEditor.Animations.AnimatorController ac = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;

        //int layerIndex = 1;
        //string stateName = "Gesture";
        //string transitionName = "PostGesture";

        //UnityEditor.Animations.AnimatorStateMachine sm = ac.layers[layerIndex].stateMachine;
        //UnityEditor.Animations.ChildAnimatorState[] states = sm.states;
        //foreach (UnityEditor.Animations.ChildAnimatorState s in states)
        //{
        //    if (s.state.name == stateName)
        //    {
        //        foreach (UnityEditor.Animations.AnimatorStateTransition t in s.state.transitions)
        //        {
        //            if (t.name == transitionName)
        //            {
        //                //Debug.Log(string.Format("Changing {0} duration value to {1}", transitionName, transitionTime));
        //                t.duration = transitionTime;
        //            }
        //        }
        //    }
        //}

        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.SetFloat("gestureNum", num);

        if (!headOnly)
        {
            animator.SetTrigger("upperGestureAnim");
        }
        else
        {
            animator.SetTrigger("headGestureAnim");
            //fullBody = false;
        }

        //if (fullBody)
        //{
        //    animator.applyRootMotion = true;
        //    animator.SetTrigger("lowerGestureAnim");
        //}
    }

    public void SetTransitionTrigger(DiaNpcLine.TransitionTime transitionTime)
    {
        animator.SetTrigger(transitionTime.ToString());
    }
}
