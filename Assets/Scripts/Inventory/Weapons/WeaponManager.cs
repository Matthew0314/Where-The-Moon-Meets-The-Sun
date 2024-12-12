using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] TextAsset weaponTextData;
    private static Dictionary<string, Weapon> Weapons = new Dictionary<string, Weapon>();

    //Reads in data from the Weapons CSV file
    public void ReadCSV() {
        string[] data = weaponTextData.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);

        for(int i = 23; i < data.Length - 1; i += 23) {
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

            //Calles CreateWeapon to determine which type to store in Weapon weapon
            Weapon weapon = CreateWeapon(weaponClass, name, description, type, tomeType, rank, attack, hit, crit, weight, uses, range1, range2, range3, range, MultMounted, MultAirBorn, MultArmored, MultWhisper, MultInfantry, numHits, canCounter);

            //Stores in a dictionary with the name of the wepaon as the key
            Weapons[name] = weapon;
        }
    }

    Weapon CreateWeapon(string weaponClass, string name, string description, string type, string tomeType, char rank, int attack, int hit, int crit, int weight, int uses, bool range1, bool range2, bool range3, int range, float MultMounted, float MultAirBorn, float MultArmored, float MultWhisper,float MultInfantry, int numHits, bool canCounter) {

        //Determines which child class to instantiate it ass
        Type weaponType = Type.GetType(weaponClass);

        //Returns the new child object
        return (Weapon)Activator.CreateInstance(weaponType, name, description, type, rank, attack, hit, crit, weight, uses, range1, range2, range3, range, MultMounted, MultAirBorn, MultArmored, MultWhisper, MultInfantry, numHits, canCounter, weaponClass);
    }

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

        return (Weapon)Activator.CreateInstance(weaponType, temp.WeaponName, temp.WeaponDescription, temp.WeaponType, temp.WeaponRank, temp.Attack, temp.HitRate, temp.CritRate, temp.Weight, temp.MaxUses, temp.Range1, temp.Range2, temp.Range3, temp.Range, temp.MultMounted, temp.MultAirBorn, temp.MultArmored, temp.MultWhisper, temp.MultInfantry, temp.NumHits, temp.CanCounter, temp.WeaponClass);
    }
}
