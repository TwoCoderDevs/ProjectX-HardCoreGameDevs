using UnityEngine;
using System.Collections;

public partial class Character
{
    [Header("Parkour")]
    public float BodyZOffset;
    //Hand offsets.
    public Vector3 RHandRayOffset =new Vector3(-0.5f,2.0f,0.5f);
    public Vector3 LHandRayOffset = new Vector3(0.5f, 2.0f, 0.5f);
    public Vector3 RHandRayEnd;
    public Vector3 LHandRayEnd;
    public float HandRayLenght;
    public float LegRayLenght;
    //Update Hand using ik
    public bool OnAnimatorIKUpdate(Animator animator)
    {
        RaycastHit hitinfo;
        RightHand.PositionWeight = animator.GetFloat("RHandWeight");
        LeftHand.PositionWeight = animator.GetFloat("LHandWeight");
        RightHand.RotationWeight = animator.GetFloat("RHandWeight");
        LeftHand.RotationWeight = animator.GetFloat("LHandWeight");
        RightLeg.PositionWeight = animator.GetFloat("RLegWeight");
        LeftLeg.PositionWeight = animator.GetFloat("LLegWeight");
        if (Climb)
        {
            Vector3 offset = new Vector3(0, parkouroffset.y, 0) + m_ParkourInfo.HitInfo.point;
            //Right hand hit
            var start = offset;
            var end = -transform.up + RHandRayEnd;
            DebugRay(start, end);
            if (Physics.Raycast(start, end, out hitinfo, HandRayLenght, ParkourLayer))
            {
                RightHandTarget.position = hitinfo.point - RHandOffset;
                RightHandTarget.rotation = Quaternion.FromToRotation(Vector3.up, hitinfo.normal);
                RightHandTarget.LookAt((hitinfo.point - RHandOffset) + transform.TransformDirection(0,0,0.1f), Vector3.up);
            }

            //Left hand hit
            start = offset;
            end = -transform.up + LHandRayEnd;
            DebugRay(start, end * LegRayLenght);
            if (Physics.Raycast(start, end, out hitinfo, HandRayLenght, ParkourLayer))
            {
                LeftHandTarget.position = hitinfo.point - LHandOffset;
                LeftHandTarget.rotation = Quaternion.FromToRotation(Vector3.up, hitinfo.normal);
                LeftHandTarget.LookAt((hitinfo.point - LHandOffset) + transform.TransformDirection(0, 0, 0.1f), Vector3.up);
            }

            //Right Leg hit
            start = m_RightLeg.position;
            end = transform.forward;
            DebugRay(start, end * LegRayLenght);
            if (Physics.Raycast(start, end, out hitinfo, LegRayLenght, ParkourLayer))
            {
                RightLegTarget.position = hitinfo.point - RLegOffset;
            }

            //Left Leg hit
            start = m_LeftLeg.position;
            end = transform.forward;
            DebugRay(start, end * LegRayLenght);
            if (Physics.Raycast(start, end, out hitinfo, LegRayLenght, ParkourLayer))
            {
                LeftLegTarget.position = hitinfo.point - LLegOffset;
            }

            return Climb;
        }
        else
        {
            return Climb;
        }
    }
}
