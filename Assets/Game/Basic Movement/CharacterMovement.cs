using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private CharacterInput ip;
    private Animator a;
    [SerializeField, Range(5f, 50f)] private float rotationSpeed = 10f;

    [Header("Foot Placement")]
    public bool enbleIK;
    [SerializeField, Range(0f, 2f)] private float heightFromGroundRaycast = 1.14f;
    [SerializeField, Range(0f, 2f)] private float raycastDownDistance = 1.5f;
    [SerializeField] private LayerMask environmentLayer;
    [SerializeField] private float pelvisOffset = 0f;
    [SerializeField, Range(0, 1)] private float pelvisMovementSpeed = 0.28f;
    [SerializeField, Range(0, 1)] private float feetIKSpeed = 0.5f;

    public string leftFootAnimCurve = "LeftFootCurve";
    public string rightFootAnimCurve = "RightFootCurve";

    public bool useFootCurves = false;
    public bool debugFootIK = false;
 
    private Vector3 rfp, lfp, rfIKp, lfIKp;
    private Quaternion lfIKr, rfIKr;
    private float lastPelvispY, lastLfpY, lastRfpY;

    // Start is called before the first frame update
    void Start()
    {
        ip = GetComponent<CharacterInput>();   
        a = GetComponent<Animator>();     
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorUpdate();  
        if(Mathf.Abs(ip.v) > 0.1f || Mathf.Abs(ip.h) > 0.1f) RotatePLayer();      
    }

    private void FixedUpdate() 
    {
        if(!enbleIK) return;
        if(!a) return;

        AdjustFootTarget(ref rfp, HumanBodyBones.RightFoot);   
        AdjustFootTarget(ref lfp, HumanBodyBones.LeftFoot);      

        FeetPositionSolver(rfp, ref rfIKp, ref rfIKr); 
        FeetPositionSolver(lfp, ref lfIKp, ref lfIKr); 
    }

    #region Animation
    void AnimatorUpdate()
    {
        a.SetFloat("Horizontal", ip.h, 0f, Time.deltaTime);
        a.SetFloat("Vertical", ip.v, 0f, Time.deltaTime);
        a.SetFloat("Turn", ip.t, 0.3f, Time.deltaTime);
    }
    #endregion

    #region PlayerRotation

    void RotatePLayer()
    {
        Vector3 dir = ip.target.position - transform.position;       
        Quaternion rot = Quaternion.LookRotation(dir);
        rot.x = 0f; rot.z = 0f;

        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * rotationSpeed); 
    }
    #endregion

    #region FootGrounding
    private void OnAnimatorIK(int layerIndex) 
    {
        if(!enbleIK || !a) return;

        MovePelvis();

        a.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
        a.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);

        if(useFootCurves)
        {
            a.SetIKRotationWeight(AvatarIKGoal.RightFoot, a.GetFloat(rightFootAnimCurve));
            a.SetIKRotationWeight(AvatarIKGoal.LeftFoot, a.GetFloat(leftFootAnimCurve));
        } 
        else
        {
            a.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
            a.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);            
        } 

        MoveFootToIKPosition(AvatarIKGoal.RightFoot, rfIKp, rfIKr, ref lastRfpY);      
        MoveFootToIKPosition(AvatarIKGoal.LeftFoot, lfIKp, lfIKr, ref lastLfpY);      
    }
    #endregion

    #region FootIKSolvers
    private void MoveFootToIKPosition(AvatarIKGoal f, Vector3 pIKh, Quaternion rIKh, ref float lastfPY)
    {
        Vector3 tIKp = a.GetIKPosition(f);

        if(pIKh != Vector3.zero)
        {
            tIKp = transform.InverseTransformPoint(tIKp);
            pIKh = transform.InverseTransformPoint(pIKh);

            float yV = Mathf.Lerp(lastfPY, pIKh.y, feetIKSpeed);
            tIKp.y += yV;

            lastfPY = yV;
            tIKp = transform.TransformPoint(tIKp);

            a.SetIKRotation(f, rIKh);
        }

        a.SetIKPosition(f, tIKp);
    }

    private void MovePelvis()
    {
        if(rfIKp == Vector3.zero || lfIKp == Vector3.zero || lastPelvispY == 0)
        {
            lastPelvispY = a.bodyPosition.y;
            return;
        }

        float lop = lfIKp.y - transform.position.y;
        float rop = rfIKp.y - transform.position.y;

        float o = (lop < rop) ? lop : rop;
        Vector3 newPP = a.bodyPosition + Vector3.up * o;
        newPP.y = Mathf.Lerp(lastPelvispY, newPP.y, pelvisMovementSpeed);
        a.bodyPosition = newPP;
        lastPelvispY = a.bodyPosition.y;
    }

    private void FeetPositionSolver(Vector3 fsp, ref Vector3 fIKp, ref Quaternion fIKr)
    {
        RaycastHit fh;

        if(debugFootIK) Debug.DrawLine(fsp, fsp + Vector3.down * (raycastDownDistance + heightFromGroundRaycast), Color.red);

        if(Physics.Raycast(fsp, Vector3.down, out fh, raycastDownDistance + heightFromGroundRaycast, environmentLayer))
        {
            fIKp = fsp;
            fIKp.y = fh.point.y + pelvisOffset;
            fIKr = Quaternion.FromToRotation(Vector3.up, fh.normal) * transform.rotation;

            return;
        }

        fIKp = Vector3.zero;
    }

    private void AdjustFootTarget(ref Vector3 fp, HumanBodyBones foot)
    {
        fp = a.GetBoneTransform(foot).position;
        fp.y = transform.position.y + heightFromGroundRaycast;
    }
    #endregion
}
