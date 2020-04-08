using UnityEngine;
using System.Collections;
using Unity.Entities;

public class GunData : MonoBehaviour
{
    public Animation animator;
    public Transform Gun;
    public Transform muzzlepoint;
    public Transform bulletpoint;
    public ParticleSystem muzzleFire;
    public Transform bullet;
    private BulletData _bulletData;
    public BulletData bulletData => (_bulletData)?_bulletData :bullet.GetComponent<BulletData>();
    public AudioSource GunShoot;
    public int MagSize;
}