using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public partial class Character
{
    [Header("Parkour and IK settings")]
    public Transform head;
    public Transform m_LeftLeg;
    public Transform m_RightLeg;
    public Transform WallTopTransform;
    public float RayDistance;
    public LayerMask ParkourLayer;
    public Vector3 RHandOffset;
    public Vector3 LHandOffset;
    public Vector3 RLegOffset;
    public Vector3 LLegOffset;
    public ParkourHeights m_ParkourHeights;
    //Storing hitinfo and figuring the right action
    private ParkourRay m_ParkourInfo;
    public Vector3 parkouroffset;
    public Vector3 Landoffset;
    [SerializeField]
    private ParkourState lastState;
    private Vector3 lastGround;
    [SerializeField]
    private bool JumpToClimb;
    public bool OnCheckParkourStatus(Animator animator)
    {
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName(lastState.ToString()))
        {
            if (!m_Animator.isMatchingTarget)
            {
                var pose = m_ParkourInfo.HitInfo.point;
                pose -= parkouroffset;
                pose += transform.TransformDirection(Landoffset);
                body.isKinematic = true;
                m_Animator.MatchTarget(pose, new Quaternion(), AvatarTarget.RightHand, new MatchTargetWeightMask(Vector3.one, 0), animator.GetFloat("StartTarget"), animator.GetFloat("EndTarget"));
            }
        }

        if (!m_Animator.isMatchingTarget)
        {
            body.isKinematic = false;
        }

        Climb = animator.GetBool("Parkour");

        if (Climb || animator.GetCurrentAnimatorStateInfo(0).IsName("ClimbUp") || animator.GetCurrentAnimatorStateInfo(0).IsName("Vault") || animator.GetCurrentAnimatorStateInfo(0).IsName("StepUp"))
            return true;

        m_ParkourInfo = default;

        if (!(m_ParkourInfo = CheckForNearByObject()).HitInfo.collider)
            return false;

        ParkourRay();
        lastState = default;
        lastState = m_ParkourInfo.GetActionName(transform.position, m_ParkourHeights, m_IsGrounded);
        if (lastState == ParkourState.ClimbUp && (!m_PreviouslyGrounded || !m_IsGrounded) && !animator.GetCurrentAnimatorStateInfo(0).IsName("ClimbUp"))
        {
            JumpToClimb = true;
        }
        switch (lastState)
        {
            case ParkourState.ClimbUp:
                if ((Input.GetKeyDown(KeyCode.Space) || JumpToClimb) && !animator.GetCurrentAnimatorStateInfo(0).IsName("ClimbUp"))
                {
                    JumpToClimb = false;
                    var dcp = m_ParkourInfo.ClimbPoint(transform.position);
                    transform.LookAt(dcp, Vector3.up);
                    dcp = m_ParkourInfo.HitInfo.point;
                    dcp.y = m_ParkourInfo.HitInfo.collider.bounds.max.y + parkouroffset.y;
                    WallTopTransform.position = dcp;
                    WallTopTransform.rotation = transform.rotation;
                    dcp.y = transform.position.y;
                    transform.position = dcp;
                    transform.position = transform.position + transform.TransformDirection(new Vector3(0, 0, BodyZOffset));
                    RHandRayEnd = transform.TransformDirection(new Vector3(RHandRayOffset.x, 0.0f, 0.0f));
                    LHandRayEnd = transform.TransformDirection(new Vector3(LHandRayOffset.x, 0.0f, 0.0f));
                    animator.SetBool("Parkour", Climb = true);
                    animator.SetInteger("ParkourState", 3);
                    return true;
                }
                return true;
            case ParkourState.Vault:
                if (Input.GetKeyDown(KeyCode.Space) && !animator.GetCurrentAnimatorStateInfo(0).IsName("Vault"))
                {
                    var vcp = m_ParkourInfo.ClimbPoint(transform.position);
                    transform.LookAt(vcp, Vector3.up);
                    vcp.y = m_ParkourInfo.HitInfo.collider.bounds.max.y + parkouroffset.y;
                    WallTopTransform.position = vcp;
                    vcp = transform.position;
                    vcp.y = animator.GetFloat("targetOffsetY");
                    transform.position = vcp; 
                    RHandRayEnd = transform.TransformDirection(new Vector3(RHandRayOffset.x, 0.0f, 0.0f));
                    LHandRayEnd = transform.TransformDirection(new Vector3(LHandRayOffset.x, 0.0f, 0.0f));
                    animator.SetBool("Parkour", Climb = true);
                    animator.SetInteger("ParkourState", 2);
                    return true;
                }
                return true;
            case ParkourState.StepUp:
                if (Input.GetKeyDown(KeyCode.Space) && !animator.GetCurrentAnimatorStateInfo(0).IsName("StepUp"))
                {
                    var scp = m_ParkourInfo.ClimbPoint(transform.position);
                    transform.LookAt(scp, Vector3.up);
                    scp.y = m_ParkourInfo.HitInfo.collider.bounds.max.y + parkouroffset.y;
                    WallTopTransform.position = scp; 
                    RHandRayEnd = transform.TransformDirection(new Vector3(RHandRayOffset.x, 0.0f, 0.0f));
                    LHandRayEnd = transform.TransformDirection(new Vector3(LHandRayOffset.x, 0.0f, 0.0f));
                    animator.SetBool("Parkour", Climb = true);
                    animator.SetInteger("ParkourState", 1);
                }
                return true;
            case ParkourState.Slide:
                return false;
            case ParkourState.Ignore:
                return false;
            case ParkourState.none:
                return false;
        }
        return false;
    }
    public float psphereRadius;
    public float psphereDistance;
    public Vector3 psphere;

    public void OnDrawGizmos()
    {
        if (capsule)
            Gizmos.DrawWireSphere(transform.position + sphere, capsule.radius);
        var c = Gizmos.color;
        Gizmos.color = Color.red;
        if (Hinfo.collider)
            Gizmos.DrawWireSphere(Hinfo.point, 0.1f);
        Gizmos.color = c;
        Gizmos.DrawWireSphere(transform.position + psphere, psphereRadius);
    }

    RaycastHit[] hitInfos = new RaycastHit[10];
    private ParkourRay CheckForNearByObject()
    {
        int lenght;
        if ((lenght = Physics.SphereCastNonAlloc(transform.position + psphere, psphereRadius, transform.forward, hitInfos, psphereDistance, GroundIgnoreLayer)) > 0)
        {
            for (var i = 0; i < lenght; i++)
            {
                Vector3 forward = transform.forward;
                Vector3 toOther = hitInfos[i].transform.position - transform.position;

                if (Vector3.Dot(forward, toOther) > 0)
                {
                    var HitInfo = hitInfos[i];
                    Array.Clear(hitInfos, 0, hitInfos.Length);
                    return new ParkourRay() { HitInfo = HitInfo };
                }
            }
        }
        return default;
    }

    public bool debug;
    public void DebugRay(Vector3 start, Vector3 end)
    {
        if (debug)
            Debug.DrawRay(start, end, Color.green);
    }
    public void ParkourRay()
    {
        RaycastHit hitinfo;
        Vector3 offset = transform.TransformDirection(new Vector3(0, 0.2f + m_ParkourInfo.HitInfo.collider.bounds.max.y, 0.1f)) + m_ParkourInfo.ClimbPoint(transform.position);
        DebugRay(offset, -transform.up * 5);
        if (Physics.Raycast(offset, -transform.up, out hitinfo, 5, ParkourLayer))
        {
            m_ParkourInfo = new ParkourRay() { HitInfo = hitinfo };
        }
    }

    // Update is called once per frame

}

public struct ParkourRay
{
    public RaycastHit HitInfo { get; set; }
    public float distance { get { return HitInfo.distance; } }
    public Vector3 ClimbPoint(Vector3 position) => (HitInfo.collider) ? HitInfo.collider.ClosestPoint(position) : Vector3.zero;
    public ParkourState GetActionName(Vector3 position, ParkourHeights parkourHeights, bool GroundState) => parkourHeights.GetActionName(position, HitInfo.point.y, GroundState);
}
[Serializable]
public struct ParkourHeights
{
    public float IgnoreHeight;
    public float ClimbHeight;
    public float VaultHeight;
    public float StepupHeight;

    public ParkourState GetActionName(Vector3 position, float height, bool Grounded)
    {
        if (height > (position.y + IgnoreHeight))
            return ParkourState.Ignore;
        if (height > (position.y + ClimbHeight))
            return ParkourState.ClimbUp;
        if (height > (position.y + VaultHeight))
            return ParkourState.Vault;
        if (height > (position.y + StepupHeight))
            return ParkourState.StepUp;
        return ParkourState.none;
    }
}

public enum ParkourState
{
    StepUp = 5,
    Slide = 4,
    Vault = 3,
    ClimbUp = 2,
    Ignore = 1,
    none = 0
}
