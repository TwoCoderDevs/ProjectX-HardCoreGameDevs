using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class GameCamera : MonoBehaviour
{
    public enum UpdateType
    {
        FixedUpdate,
        LateUpdate,
        Update
    }
    public Transform target;
    public UpdateType update;
    [Range(0,1)]public float smoothSpeed = 0.125f;
    public Vector3 offset;

    private void Update()
    {
        if (update == UpdateType.Update)
            Target();
    }

    private void FixedUpdate()
    {
        if (update == UpdateType.FixedUpdate)
            Target();
    }

    private void LateUpdate()
    {
        if (update == UpdateType.LateUpdate)
            Target();
    }

    public void Target()
    {
        Vector3 desiredPosition = target.position+offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
