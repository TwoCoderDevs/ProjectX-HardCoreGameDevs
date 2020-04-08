using UnityEngine;
using System.Collections;
using Unity.Entities;

public class BulletData : MonoBehaviour
{
    public Rigidbody body;
    public AudioClip hitsound;
    public float lifetime;
    [Tooltip("Meters Per Sec")]
    public float Velocity;
    [Tooltip("Kilograms")]
    public float Mass;
    public Transform hit;
    public Vector3 hitpoint;
    public Vector3 forward;
    public Vector3 start;
    public bool destroying;
    private int hitcount;
    public void Disable(float time)
    {
        if (time == 0)
            time = lifetime;
        Invoke("DisableObject", time);
    }
    public void DisableObject()
    {
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hitcount > 2)
        {
            hitcount = 0;
            gameObject.SetActive(false);
        }
        hitpoint = transform.position;
        if (body.velocity.magnitude > 10)
            body.velocity /= 5;
        var source = GetComponent<AudioSource>();
        if (!source)
            source = gameObject.AddComponent<AudioSource>();
        source.clip = hitsound;
        BulletSystem.RegisterHit(hitpoint);
        source.Play();
        hitcount++;
    }
}
