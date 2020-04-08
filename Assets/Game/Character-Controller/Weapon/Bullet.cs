using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    private Rigidbody body;
    public AudioClip hitsound;
    public float lifetime = 5;
    [Tooltip("Meters Per Sec")]
    public float Velocity;
    [Tooltip("Kilograms")]
    public float Mass;
    public Transform hit;
    public Vector3 hitpoint;
    public Vector3 forward;
    public Vector3 start;
    private void Start()
    {
        body = GetComponent<Rigidbody>();
        if (!destroying)
        {
            Destroy(hit.gameObject, lifetime);
            Destroy(this, lifetime);
            destroying = true;
        }
        start = transform.position;
        transform.LookAt(forward);
        Release();
    }

    public void Release()
    {
        body.mass = Mass;
        body.velocity = forward * Velocity;
        body.centerOfMass = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(forward, hitpoint);
    }

    bool destroying = false;
    private void OnCollisionEnter(Collision collision)
    {
        hitpoint = transform.position;
        body.velocity /= 5;
        var source = gameObject.AddComponent<AudioSource>();
        source.clip = hitsound;
        var _hit = Instantiate(hit);
        _hit.position = hitpoint;
        hit.GetComponent<ParticleSystem>().Play();
        source.Play();
    }
}