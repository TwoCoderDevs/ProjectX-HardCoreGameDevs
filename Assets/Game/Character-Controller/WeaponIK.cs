using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Character
{
    [Header("Weapons")]

    public int WeaponID;
    public WeaponHolder wholder;
    public Transform AimTarget;
    public Transform AimRot;
    public Transform rightGunIK;
    public Transform leftGunIK;
    private Camera main;
    private CameraController camc;
    public bool assigned;
    private bool aiming;
    public WeaponConfig weaponConfig;
    public void Shoot()
    {
        if (weaponConfig && Input.GetMouseButton(0) && aiming)
        {
            if (!wholder.Gun1.animator.isPlaying)
            {
                wholder.Gun1.GunShoot.Play();
                wholder.Gun1.animator.Play();
                wholder.Gun1.muzzleFire.Play();
                BulletSystem.Release(main.transform.forward, wholder.Gun1.bulletpoint.position, wholder.Gun1.bulletpoint.rotation);
            }
            if (wholder.TwoHand)
            {
                if (!wholder.Gun2.animator.isPlaying)
                {
                    wholder.Gun2.GunShoot.Play();
                    wholder.Gun2.animator.Play();
                    wholder.Gun2.muzzleFire.Play();
                    BulletSystem.Release(main.transform.forward, wholder.Gun2.bulletpoint.position, wholder.Gun2.bulletpoint.rotation);
                }
            }
        }
    }

    public bool AssignWeapon()
    {
        weaponConfig = WeaponConfigManager.GetWeapon(WeaponID);
        if (assigned)
            return true;
        if (weaponConfig)
        {
            if (tag == "Player")
                CameraController.pwConfig = weaponConfig;

            m_Animator.SetInteger("GunType", 1);
            if (wholder.TwoHand)
            {
                print(weaponConfig.gun + ":" + weaponConfig.range);
                wholder.Gun2 = Instantiate(weaponConfig.gun.gameObject).GetComponent<GunData>();
                wholder.Gun2.Gun.parent = leftGunIK;
                wholder.Gun2.Gun.localPosition = Vector3.zero;
                wholder.Gun2.Gun.localEulerAngles = Vector3.zero;
                wholder.Gun2.muzzleFire = Instantiate(weaponConfig.gunData.muzzleFire.gameObject).GetComponent<ParticleSystem>();
                wholder.Gun2.muzzleFire.transform.parent = wholder.Gun2.muzzlepoint;
            }
            wholder.Gun1 = Instantiate(weaponConfig.gun.gameObject).GetComponent<GunData>();
            wholder.Gun1.Gun.parent = rightGunIK;
            wholder.Gun1.Gun.localPosition = Vector3.zero;
            wholder.Gun1.Gun.localEulerAngles = Vector3.zero;
            wholder.Gun1.muzzleFire = Instantiate(weaponConfig.gunData.muzzleFire.gameObject).GetComponent<ParticleSystem>();
            wholder.Gun1.muzzleFire.transform.parent = wholder.Gun1.muzzlepoint;
            m_Animator.SetLayerWeight(1, 1);
            m_Animator.SetLayerWeight(3, 1);
            BulletSystem.SetupBullets(weaponConfig.gunData.bullet.GetComponent<BulletData>(), wholder.TwoHand, weaponConfig.gun.GetComponent<GunData>().MagSize);
            assigned = true;
            return true;
        }
        m_Animator.SetInteger("GunType", 0);
        LookWeight = 0;
        RightHand.PositionWeight = 0;
        LeftHand.PositionWeight = 0;
        RightHand.RotationWeight = 0;
        LeftHand.RotationWeight = 0;
        m_Animator.SetLayerWeight(1, 0);
        m_Animator.SetLayerWeight(2, 0);
        m_Animator.SetLayerWeight(3, 0);
        m_Animator.SetLayerWeight(4, 0);
        return false;
    }
    public Vector3 look;
    public void AimWeapon()
    {
        if (weaponConfig && Input.GetMouseButton(1))
        {
            if (tag == "Player")
                CameraController.aiming = aiming;
            useIK = true;
            if (m_ForwardAmount == 0)
                transform.localEulerAngles = new Vector3(0f, camc.mouseY, 0f);
            AimRot.LookAt(main.transform.forward * main.farClipPlane);
            var finalAim = AimTarget.position;
            RightHandTarget.position = finalAim + transform.TransformDirection(wholder.rightGTP);
            LeftHandTarget.position = finalAim + transform.TransformDirection(wholder.leftGTP);
            look = main.transform.forward * main.farClipPlane;
            RightHandTarget.LookAt(look);
            RightHandTarget.eulerAngles = new Vector3(RightHandTarget.eulerAngles.x, RightHandTarget.eulerAngles.y, RightHandTarget.eulerAngles.z + wholder.rightExtrarot);
            LeftHandTarget.LookAt(look);
            LeftHandTarget.eulerAngles = new Vector3(LeftHandTarget.eulerAngles.x, LeftHandTarget.eulerAngles.y, LeftHandTarget.eulerAngles.z + wholder.leftExtrarot);
            LookWeight = 1;
            RightHand.PositionWeight = 1;
            LeftHand.PositionWeight = 1;
            RightHand.RotationWeight = 1;
            LeftHand.RotationWeight = 1;
            m_Animator.SetLayerWeight(2, 1);
            m_Animator.SetLayerWeight(4, 1);
            aiming = true;
        }
        else
        {
            useIK = false;
            aiming = false;
            LookWeight = 0;
            RightHand.PositionWeight = 0;
            LeftHand.PositionWeight = 0;
            RightHand.RotationWeight = 0;
            LeftHand.RotationWeight = 0;
            m_Animator.SetLayerWeight(2, 0);
            m_Animator.SetLayerWeight(4, 0);
        }
    }
}
[Serializable]
public struct WeaponHolder
{
    //[HideInInspector]
    public GunData Gun1;
    //[HideInInspector]
    public GunData Gun2;
    public float range;
    public float speed;
    public float acuraccy;
    public float damage;
    public float rightExtrarot;
    public float leftExtrarot;
    public Vector3 rightGTP;
    public Vector3 leftGTP;
    public bool TwoHand;
}