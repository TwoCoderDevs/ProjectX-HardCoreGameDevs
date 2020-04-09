using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class WeaponConfigManager : MonoBehaviour
{
    public static Dictionary<int, WeaponConfig> weapons;
    public List<WeaponConfig> Weapons = new List<WeaponConfig>();

    public void OnEnable()
    {
        if (weapons == null)
        {
            weapons = new Dictionary<int, WeaponConfig>();
        }
        foreach (var Weapon in Weapons)
        {
            AddWeapon(Weapon.ID, Weapon);
        }
    }

    public static WeaponConfig GetWeapon(int id)
    {
        if (weapons.ContainsKey(id))
           return weapons[id];
        return null;
    }

    public static void AddWeapon(int ID, WeaponConfig Weapon)
    {
        if (!weapons.ContainsKey(ID) && !weapons.ContainsValue(Weapon))
            weapons[ID] = Weapon;
    }

    public static void Save(WeaponConfig weaponConfig)
    {
        if(!weaponConfig)
        {
            var loc = EditorUtility.SaveFilePanel("Create New Weapon Config", Application.dataPath, "weapon", ".asset");
            AssetDatabase.CreateAsset(weaponConfig, loc);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}