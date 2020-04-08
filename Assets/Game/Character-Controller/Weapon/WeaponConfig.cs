using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Data", menuName = "Weapon Cofig")]
public class WeaponConfig : ScriptableObject
{
    public int ID;
    public string WeaponName { get { return name; } set { name = value; } }
    public Transform gun;
    public GunData gunData;
    public float range;
    public float speed;
    public float acuraccy;
    public float damage;
    public float rightExtrarot;
    public float leftExtrarot;
    public Vector3 rightGTP;
    public Vector3 leftGTP;

    [Header("Camera Settings : ")]
    [Range(0, 100)] public float sensitivity;
    public Vector3 offset;
    public Vector2 xRotationMinMax;
    public LayerMask detectableLayers;
    public static WeaponConfig pwConfig;
}