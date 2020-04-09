using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController cur;
    [Header("Input Settings : ")]
    public string mouseXName;
    public string mouseYName;
    public float mouseX;
    public float mouseY;
    public bool FlipmouseX;
    public bool FlipmouseY;

    [Header("Camera Settings : ")]
    [Range(0, 100)] public float sensitivity;
    public Transform target;
    public Vector3 offset;
    public Vector2 xRotationMinMax;
    public LayerMask detectableLayers;
    public static WeaponConfig pwConfig;
    public static Vector3 Aimpoint;
    public static bool aiming;

    // Start is called before the first frame update
    void Start()
    {
        cur = this;
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Reset()
    {
        mouseXName = "MouseX";
        mouseYName = "MouseY";
    }

    void FixedUpdate()
    {
        if (aiming && pwConfig)
        {
            CamUpdate(pwConfig.sensitivity, pwConfig.offset, pwConfig.xRotationMinMax, pwConfig.detectableLayers);

        }
        else
        {
            CamUpdate(sensitivity, offset, xRotationMinMax, detectableLayers);
        }
    }

    void CamUpdate(float sensitivity, Vector3 offset, Vector2 xRotationMinMax, LayerMask detectableLayers)
    {
        if (FlipmouseX)
            mouseX -= (Input.GetAxis(mouseYName) * Time.deltaTime) * sensitivity;
        else
            mouseX += (Input.GetAxis(mouseYName) * Time.deltaTime) * sensitivity;
        if (FlipmouseX)
            mouseY -= (Input.GetAxis(mouseXName) * Time.deltaTime) * sensitivity;
        else
            mouseX += (Input.GetAxis(mouseYName) * Time.deltaTime) * sensitivity;
        mouseX = Mathf.Clamp(mouseX, xRotationMinMax.x, xRotationMinMax.y);

        transform.eulerAngles = new Vector3(mouseX, mouseY, 0f);

        Vector3 _targetPosition = target.position - transform.forward * offset.z + transform.up * offset.y + transform.right * offset.x;
        HandleCollisions(_targetPosition, detectableLayers);

        Ray ray = new Ray(transform.position, transform.forward);
    }

    void HandleCollisions(Vector3 _originalPosition, LayerMask detectableLayers)
    {
        if (Physics.Linecast(target.position, _originalPosition, out RaycastHit hit, detectableLayers))
        {
            transform.position = hit.point + hit.normal * 0.1f;
            return;
        }
        transform.position = _originalPosition;
    }
}
