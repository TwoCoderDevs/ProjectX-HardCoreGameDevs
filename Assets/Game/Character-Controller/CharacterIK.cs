using UnityEngine;
using System;
using System.Collections;

public partial class Character
{
    [Header("IKRigSetUp")]

    public bool useIK = true;
    
    public Transform LookTarget;
    public Transform BodyTarget;

    public Transform RightHandTarget;
    public Transform LeftHandTarget;

    public Transform RightLegTarget;
    public Transform LeftLegTarget;

    public Transform RightElbowHintTarget;
    public Transform LeftElbowHintTarget;

    public Transform RightKneeHintTarget;
    public Transform LeftKneeHintTarget;

    private PosRotWeightSliders RightHand;
    private PosRotWeightSliders LeftHand;
    private PosRotWeightSliders RightLeg;
    private PosRotWeightSliders LeftLeg;
    private float RightElbowHintWeight;
    private float LeftElbowHintWeight;
    private float RightKneeHintWeight;
    private float LeftKneeHintWeight;
    public float LookWeight;
    private void OnAnimatorIK(int layerIndex)
    {
        if (useIK)
        {
            m_Animator.SetLookAtWeight(LookWeight,0.5f,1,1,1);
            m_Animator.SetLookAtPosition(LookTarget.position);

            m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, LeftHand.PositionWeight);
            m_Animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandTarget.position);

            m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, RightHand.PositionWeight);
            m_Animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandTarget.position);

            m_Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, LeftHand.RotationWeight);
            m_Animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandTarget.rotation);

            m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, RightHand.RotationWeight);
            m_Animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandTarget.rotation);

            m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, LeftLeg.PositionWeight);
            m_Animator.SetIKPosition(AvatarIKGoal.LeftFoot, LeftLegTarget.position);

            m_Animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, RightLeg.PositionWeight);
            m_Animator.SetIKPosition(AvatarIKGoal.RightFoot, RightLegTarget.position);

            m_Animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, LeftLeg.RotationWeight);
            m_Animator.SetIKRotation(AvatarIKGoal.LeftFoot, LeftLegTarget.rotation);

            m_Animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, RightLeg.RotationWeight);
            m_Animator.SetIKRotation(AvatarIKGoal.RightFoot, RightLegTarget.rotation);

            //Hint
            m_Animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, LeftElbowHintWeight);
            m_Animator.SetIKHintPosition(AvatarIKHint.LeftElbow, LeftElbowHintTarget.position);

            m_Animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, RightElbowHintWeight);
            m_Animator.SetIKHintPosition(AvatarIKHint.RightElbow, RightElbowHintTarget.position);

            m_Animator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, LeftKneeHintWeight);
            m_Animator.SetIKHintPosition(AvatarIKHint.LeftKnee, LeftKneeHintTarget.position);

            m_Animator.SetIKHintPositionWeight(AvatarIKHint.RightKnee, RightKneeHintWeight);
            m_Animator.SetIKHintPosition(AvatarIKHint.RightKnee, RightKneeHintTarget.position);
        }
    }
}
[Serializable]
public struct PosRotWeightSliders
{
    [Range(0, 1)] public float PositionWeight;
    [Range(0, 1)] public float RotationWeight;
}
