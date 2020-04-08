using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
public partial class Character : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 Normal;
    public float Radius;
    public float Height;
    public Vector3 ColliderOffset;
    public float GroundPlaneOffset;
    private CapsuleCollider capsule;
    public Vector3 sphere;
    public float groundCheckDistance = 0.01f;
    public LayerMask GroundIgnoreLayer;
    public float sphereRadius;
    public float DeadDistance = 15f;
    public bool Climb;
    float m_CapsuleHeight;
    Vector3 m_CapsuleCenter;
    void Start()
    {
        main = Camera.main;
        camc = CameraController.cur;
        capsule = GetComponent<CapsuleCollider>();
        m_CapsuleHeight = capsule.height = Height;
        m_CapsuleCenter = capsule.center = ColliderOffset;
        capsule.radius = Radius;
        body = GetComponent<Rigidbody>();
        body.constraints = RigidbodyConstraints.FreezeRotation;
        m_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public float m_TurnAmount;
    public void Movement(Vector3 move, bool crouch, bool jump)
    {
        if (move.magnitude > 1)
            move.Normalize();
        move = transform.InverseTransformDirection(move);
        OnCheckGroundStatus();
        move = Vector3.ProjectOnPlane(move, GetGroundNormal());
        m_TurnAmount = Mathf.Atan2(move.x, move.z);
        m_ForwardAmount = move.z;


        OnCheckParkourStatus(m_Animator);
        OnAnimatorIKUpdate(m_Animator);
        if (!Climb)
        {
            if (AssignWeapon())
            {
                AimWeapon();
                Shoot();
            }
            ApplyExtraRotation();

            if (m_IsGrounded)
            {
                HandleGroundMovement(crouch, jump);
            }
            else
            {
                HandleAirboneMovement();
            }

            ScaleCapsuleForCrouching(crouch);
            PreventStandingInLowHeadroom();
            UpdateAnimator(move);
            Terrainhelper();
        }
    }



    public float SlopStartHeight;
    public float SlopDetect;
    public float SlopAngle;
    public Transform SlopTrack;
    RaycastHit Hinfo;
    private void Terrainhelper()
    {
        if (Physics.Raycast(transform.position + (Vector3.up * SlopStartHeight), transform.forward + (Vector3.up * SlopAngle), out Hinfo, SlopDetect, GroundIgnoreLayer))
        {
            if (Hinfo.normal.y == 1 && m_ForwardAmount > 0)
                transform.position = Hinfo.point;
        }
        Debug.DrawRay(transform.position + (Vector3.up * SlopStartHeight), (transform.forward + (Vector3.up * SlopAngle)) * SlopDetect, Color.red);
    }

    bool m_Crouching;
    const float k_Half = 0.5f;
    public float m_RunCycleLegOffset;
    public float m_AnimSpeedMutliplier;
    public virtual void UpdateAnimator(Vector3 move)
    {
        m_Animator.SetFloat("Forward", m_ForwardAmount);
        m_Animator.SetFloat("Turn", m_TurnAmount,0.1f,Time.deltaTime);
        m_Animator.SetBool("Crouch", m_Crouching);
        m_Animator.SetBool("OnGround",m_IsGrounded);
        if (m_IsGrounded)
        {
            m_Animator.SetFloat("Jump", body.velocity.y);
        }
        float runCycle = Mathf.Repeat(m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
        float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
        if (m_IsGrounded)
        {
            m_Animator.SetFloat("JumpLeg", jumpLeg);
        }
        if (m_IsGrounded && move.magnitude > 0)
        {
            m_Animator.speed = m_AnimSpeedMutliplier;
        }
        else
        {
            m_Animator.speed = 1;
        }
    }
    public virtual void PreventStandingInLowHeadroom()
    {
        if (!m_Crouching)
        {
            Ray crouchRay = new Ray(body.position + Vector3.up * capsule.radius * k_Half, Vector3.up);
            float crouchRayLenght = m_CapsuleHeight - capsule.radius * k_Half;
            if (Physics.SphereCast(crouchRay, capsule.radius * k_Half, crouchRayLenght))
            {
                m_Crouching = true;
            }
        }
    }
    public virtual void ScaleCapsuleForCrouching(bool crouch)
    {
        if (m_IsGrounded&&crouch)
        {
            if (m_Crouching) return;
            capsule.height = capsule.height / 2f;
            capsule.center = capsule.center / 2f;
            m_Crouching = true;
        }
        else
        {
            Ray crouchRay = new Ray(body.position + Vector3.up * capsule.radius * k_Half, Vector3.up);
            float crouchRayLenght = m_CapsuleHeight - capsule.radius * k_Half;
            if (Physics.SphereCast(crouchRay, capsule.radius * k_Half, crouchRayLenght))
            {
                m_Crouching = true;
                return;
            }
            capsule.height = m_CapsuleHeight;
            capsule.center = m_CapsuleCenter;
            m_Crouching = false;
        }
    }

    public float m_GravityMultiplier;
    public float m_JumpPower;
    public virtual void HandleGroundMovement(bool crouch, bool jump)
    {
        if (jump && !crouch && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
        {
            body.velocity = new Vector3(body.velocity.x, m_JumpPower, body.velocity.z);
            m_IsGrounded = false;
            m_Animator.applyRootMotion = false;
        }
    }
    public virtual void HandleAirboneMovement()
    {
        Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
        body.AddForce(extraGravityForce);
    }

    public float m_StationaryTurnSpeed;
    public float m_MovingTurnSpeed;
    public float m_ForwardAmount;

    public virtual void ApplyExtraRotation()
    {
        float turnspeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
        transform.Rotate(0, m_TurnAmount * turnspeed * Time.deltaTime, 0);
    }

    public virtual Vector3 GetGroundNormal()
    {
        return m_GroundNormal;
    }

    Vector3 m_GroundNormal;
    public bool m_IsGrounded { get; private set; }
    public bool panick;
    public Animator m_Animator;
    [HideInInspector]
    public Rigidbody body;
    float AirTime;

    public float falldistance;
    bool m_PreviouslyGrounded;
    public virtual void OnCheckGroundStatus()
    {
        m_PreviouslyGrounded = m_IsGrounded;
        RaycastHit hitInfo;
        if (Physics.SphereCast(transform.position + sphere, capsule.radius, Vector3.down, out hitInfo,
                               ((capsule.height / 2f) - capsule.radius) + groundCheckDistance,GroundIgnoreLayer))
        {
            m_IsGrounded = true;
            m_GroundNormal = hitInfo.normal;
            m_Animator.applyRootMotion = true;
        }
        else
        {
            m_IsGrounded = false;
            m_GroundNormal = Vector3.up;
            m_Animator.applyRootMotion = false;
            if (Climb)
                m_Animator.applyRootMotion = true;
        }

        if (m_IsGrounded)
        {

            if (AirTime > DeadDistance)
            {
                this.gameObject.SetActive(false);
            }
            AirTime = 0;
        }

        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, float.PositiveInfinity))
        {
            if (hitInfo.distance > AirTime)
                AirTime = hitInfo.distance;
        }
        /*if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping)
        {
            m_Jumping = false;
        }*/
    }
}
