using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BulletSystem
{
    static List<BulletData> bullets = new List<BulletData>();
    static ParticleSystem[] hits = new ParticleSystem[6];
    static int magSize;
    public static bool TwoGuns;
    public static void SetupBullets(BulletData bulletData,bool TwoGuns, int magSize)
    {
        for (int i = 0; i < magSize; i++)
        {
            bullets.Add(Object.Instantiate(bulletData.gameObject).GetComponent<BulletData>());
            if (TwoGuns)
                bullets.Add(Object.Instantiate(bulletData.gameObject).GetComponent<BulletData>());
        }

        for (int i = 0; i < hits.Length; i++)
        {
            hits[i] = Object.Instantiate(bulletData.hit.gameObject).GetComponent<ParticleSystem>();
        }
    }
    public static void Release(Vector3 forward,Vector3 position,Quaternion rotation)
    {
        foreach (var bullet in bullets) {
            if (!bullet.gameObject.activeSelf)
            {
                if (!bullet.body)
                    bullet.body = bullet.GetComponent<Rigidbody>();
                bullet.transform.position = position;
                bullet.transform.rotation = rotation;
                bullet.start = bullet.transform.position;
                bullet.transform.LookAt(forward);
                bullet.body.mass = bullet.Mass;
                bullet.body.velocity = forward * bullet.Velocity;
                bullet.body.centerOfMass = Vector3.zero;
                bullet.gameObject.SetActive(true);
                bullet.Disable(0f);
                break;
            }
            else
            {
                if(bullet == bullets.LastOrDefault())
                {
                    if (!bullets[0].body)
                        bullets[0].body = bullets[0].GetComponent<Rigidbody>();
                    bullets[0].transform.position = position;
                    bullets[0].transform.rotation = rotation;
                    bullets[0].start = bullets[0].transform.position;
                    bullets[0].transform.LookAt(forward);
                    bullets[0].body.mass = bullets[0].Mass;
                    bullets[0].body.velocity = forward * bullets[0].Velocity;
                    bullets[0].body.centerOfMass = Vector3.zero;
                    bullets[0].gameObject.SetActive(true);
                    bullets[0].Disable(0f);
                    break;
                }
            }
        }
    }

    public static void RegisterHit(Vector3 position)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            if (!hits[i].isPlaying && !hits[i].isEmitting)
            {
                hits[i].transform.position = position;
                hits[i].Play();
                break;
            }
            else
            {
                if (i == hits.Length)
                {
                    hits[0].transform.position = position;
                    hits[0].Play();
                    break;
                }
            }
        }
    }



    public static void Detect()
    {
        foreach (var bullet in bullets)
        {
            if (bullet.gameObject.activeSelf)
            {
                Ray ray = new Ray(bullet.transform.position, Vector3.forward);
                if(Physics.SphereCast(ray, 0.1f, 0.2f))
                {

                }
            }
        }
    }
}