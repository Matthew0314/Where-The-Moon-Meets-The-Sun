using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// using System.Diagnostics;

public class WeaponManager : MonoBehaviour
{
    private static TextAsset weaponTextData;
    private static Dictionary<string, Weapon> Weapons = new Dictionary<string, Weapon>();

    //Reads in data from the Weapons CSV file
    public static void ReadCSV() {
        weaponTextData = Resources.Load<TextAsset>("TextData/InventoryCSV/WeaponsCSV");
        string[] data = weaponTextData.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);

        for(int i = 25; i < data.Length - 1; i += 25) {
            //Stores data
            string weaponClass = data[i];
            string name = data[i + 1];
            string description = data[i + 2];
            string type = data[i + 3];
            string tomeType = data[i + 4];
            char rank = char.Parse(data[i + 5]);
            int attack = int.Parse(data[i + 6]);
            int hit = int.Parse(data[i + 7]);
            int crit = int.Parse(data[i + 8]);
            int weight = int.Parse(data[i + 9]);
            int uses = int.Parse(data[i + 10]);
            bool range1 = bool.Parse(data[i + 11]);
            bool range2 = bool.Parse(data[i + 12]);
            bool range3 = bool.Parse(data[i + 13]);
            int range = int.Parse(data[i + 14]);
            float MultMounted = float.Parse(data[i + 15]);
            float MultAirBorn = float.Parse(data[i + 16]);
            float MultArmored = float.Parse(data[i + 17]);
            float MultWhisper = float.Parse(data[i + 18]);
            float MultInfantry = float.Parse(data[i + 19]);
            int numHits = int.Parse(data[i + 20]);
            bool canCounter = bool.Parse(data[i + 21]);
            bool useMagic = bool.Parse(data[i + 22]);
            string animationType = data[i + 23];

            Type weaponType = Type.GetType(weaponClass);

            //Calles CreateWeapon to determine which type to store in Weapon weapon
            Weapon weapon = (Weapon)Activator.CreateInstance(
                weaponType,
                name, description, type, rank,
                attack, hit, crit, weight, uses,
                range1, range2, range3, range,
                MultMounted, MultAirBorn, MultArmored, MultWhisper, MultInfantry,
                numHits, canCounter, useMagic,
                animationType, weaponClass
            );

            //Stores in a dictionary with the name of the wepaon as the key
            Weapons[name] = weapon;

            Debug.Log($"Loaded weapon: {name} of type {weaponClass}");
        }
    }

    // public static void ReadCSV() {
    //     weaponTextData = Resources.Load<TextAsset>("TextData/InventoryCSV/WeaponsCSV");

    //     string[] lines = weaponTextData.text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

    //     foreach (string line in lines)
    //     {
    //         if (string.IsNullOrWhiteSpace(line))
    //             continue;

    //         string[] data = line.Split('\t'); // <-- IMPORTANT: your file is TAB separated, not comma

    //         string firstCell = data[0].Trim().Trim('\uFEFF');

    //         if (firstCell == "Class")
    //             continue;

    //         Debug.Log("Processing: " + data[1]); // weapon name

    //         string weaponClass = data[0];
    //         string name = data[1];
    //         string description = data[2];
    //         string type = data[3];
    //         string tomeType = data[4];
    //         char rank = char.Parse(data[5]);
    //         int attack = int.Parse(data[6]);
    //         int hit = int.Parse(data[7]);
    //         int crit = int.Parse(data[8]);
    //         int weight = int.Parse(data[9]);
    //         int uses = int.Parse(data[10]);
    //         bool range1 = bool.Parse(data[11]);
    //         bool range2 = bool.Parse(data[12]);
    //         bool range3 = bool.Parse(data[13]);
    //         int range = int.Parse(data[14]);
    //         float MultMounted = float.Parse(data[15]);
    //         float MultAirBorn = float.Parse(data[16]);
    //         float MultArmored = float.Parse(data[17]);
    //         float MultWhisper = float.Parse(data[18]);
    //         float MultInfantry = float.Parse(data[19]);
    //         int numHits = int.Parse(data[20]);
    //         bool canCounter = bool.Parse(data[21]);
    //         bool useMagic = bool.Parse(data[22]);
    //         string animationType = data[23];

    //         Type weaponType = typeof(NormalWeapon); // safer than Type.GetType

    //         Weapon weapon = (Weapon)Activator.CreateInstance(
    //             weaponType,
    //             name, description, type, tomeType, rank,
    //             attack, hit, crit, weight, uses,
    //             range1, range2, range3, range,
    //             MultMounted, MultAirBorn, MultArmored, MultWhisper, MultInfantry,
    //             numHits, canCounter, useMagic, animationType, weaponClass
    //         );

    //         Weapons[name] = weapon;
    //     }


    // }

    public static Weapon GetWeaponData(string WeaponName) {
        Weapon weapon;
        if (Weapons.TryGetValue(WeaponName, out weapon)) {
            return weapon; // Key exists, return the value
        }
        return null;
    }

    public static Weapon MakeWeapon(string WeaponName) {
        Weapon temp = GetWeaponData(WeaponName);

        if (temp == null) { return null; }

        Type weaponType = Type.GetType(temp.WeaponClass);

        return (Weapon)Activator.CreateInstance(weaponType, temp.WeaponName, temp.WeaponDescription, temp.WeaponType, temp.WeaponRank, temp.Attack, temp.HitRate, temp.CritRate, temp.Weight, temp.MaxUses, temp.Range1, temp.Range2, temp.Range3, temp.Range, temp.MultMounted, temp.MultAirBorn, temp.MultArmored, temp.MultWhisper, temp.MultInfantry, temp.NumHits, temp.CanCounter, temp.UseMagic, temp.AnimationType, temp.WeaponClass);
    }
}
