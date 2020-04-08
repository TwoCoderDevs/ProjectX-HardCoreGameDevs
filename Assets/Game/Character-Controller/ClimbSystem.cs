using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbSystem : StateMachineBehaviour
{
    public string ParameterName;
    public int ParameterValue;
    public string ExitParameterName;
    public bool ExitParameterValue;
    public string AlternativeExitParameterName;
    public bool AlternativeExitParameterValue;
    public bool HasNextState;
    public bool HasExitState;
    public bool AlternativeExit;
    private CharInput input;
    private Rigidbody body;
    public bool DisableRootMotion;
    public bool EnableKinematic;
    public bool OnExitEnableRootMotion;
    public bool OnExitDisableKinematic;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!input)
            input = animator.GetComponent<CharInput>();
        if (!body)
            body = animator.GetComponent<Rigidbody>();
        if (DisableRootMotion)
            animator.applyRootMotion = false;
        if (EnableKinematic)
            body.isKinematic = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.isMatchingTarget)
            body.isKinematic = true;
        if (HasExitState && input.m_Jump)
        {
            animator.SetBool("ExitParameterName", ExitParameterValue);
            if (HasNextState)
                animator.SetInteger(ParameterName, ParameterValue);
        }
        else if (HasNextState && input.m_Jump)
            animator.SetInteger(ParameterName, ParameterValue);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.isMatchingTarget)
        {
            animator.InterruptMatchTarget();
        }
        if (!HasNextState && !AlternativeExit)
            animator.SetBool("Parkour", false);
        if (AlternativeExit)
        {
            animator.SetBool(AlternativeExitParameterName, AlternativeExitParameterValue);
        }

        if (OnExitEnableRootMotion)
            animator.applyRootMotion = true;
        if (OnExitDisableKinematic)
            body.isKinematic = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
