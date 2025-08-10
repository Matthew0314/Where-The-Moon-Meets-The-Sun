using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class UnitRosterManager : MonoBehaviour
{
    [SerializeField] TextAsset statTextData;
    [SerializeField] TextAsset faithTextData;
    [SerializeField] TextAsset magicTextData;
    private static Dictionary<string, UnitStats> fullRoster = new Dictionary<string, UnitStats>();
    private static List<UnitStats> playableList = new List<UnitStats>();

    public void ReadCSV()
    {
        string[] lines = statTextData.text.Trim().Split('\n');
        string[] faithLines = faithTextData.text.Trim().Split('\n');
        string[] magicLines = magicTextData.text.Trim().Split('\n');
        Type unitType = Type.GetType("PlayerStats");

        int lineIndex = 0;

        foreach (string line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] data = line.Trim().Split(',');

            int index = 0;


            int unitID = int.Parse(data[index++]);
            string chrName = data[index++];
            Debug.Log("adding " + chrName);

            string dName = data[index++];
            string chrDesc = data[index++];
            int lev = int.Parse(data[index++]);
            int HPGR = int.Parse(data[index++]);
            int ATKGR = int.Parse(data[index++]);
            int MAGGR = int.Parse(data[index++]);
            int DEFGR = int.Parse(data[index++]);
            int RESGR = int.Parse(data[index++]);
            int SPDGR = int.Parse(data[index++]);
            int EVAGR = int.Parse(data[index++]);
            int LCKGR = int.Parse(data[index++]);
            int HP = int.Parse(data[index++]);
            int ATK = int.Parse(data[index++]);
            int MAG = int.Parse(data[index++]);
            int DEF = int.Parse(data[index++]);
            int RES = int.Parse(data[index++]);
            int SPD = int.Parse(data[index++]);
            int EVA = int.Parse(data[index++]);
            int LCK = int.Parse(data[index++]);
            int MOV = int.Parse(data[index++]);
            string charClass = data[index++];
            int faithRank = int.Parse(data[index++ + 6]);
            int magicRank = int.Parse(data[index++ + 6]);

            UnitStats stats = (UnitStats)Activator.CreateInstance(unitType, unitID, chrName, dName, chrDesc, lev, HPGR, ATKGR, MAGGR, DEFGR, RESGR, SPDGR, EVAGR, LCKGR, HP, ATK, MAG, DEF, RES, SPD, EVA, LCK, MOV, charClass, faithRank, magicRank);

            // Add items and weapons
            for (int j = 0; j < 6; j++)
            {
                string itemName = data[23 + j];
                if (itemName == "NULL")
                {
                    // Debug.Log("Null Weapon");
                    break;
                }

                Weapon tempWeapon = WeaponManager.MakeWeapon(itemName);
                if (tempWeapon != null)
                {
                    stats.AddWeapon(tempWeapon);

                    if (j == 0) stats.SetPrimaryWeapon(tempWeapon);
                    // Debug.Log("Added " + tempWeapon.WeaponName);
                }
                else
                {
                    Type itemType = Type.GetType(itemName);
                    if (itemType != null)
                    {
                        Item tempItem = (Item)Activator.CreateInstance(itemType);
                        stats.AddItem(tempItem);
                    }
                }
            }

            // Faith and Magic parsing using corresponding lines
            if (lineIndex + 1 < magicLines.Length && lineIndex + 1 < faithLines.Length)
            {
                string[] magicFields = magicLines[lineIndex + 1].Trim().Split(','); // Skip header
                string[] faithFields = faithLines[lineIndex + 1].Trim().Split(',');

                for (int i = 1; i <= 10 && i < magicFields.Length && i < faithFields.Length; i++)
                {
                    stats.MagicRankList[i - 1] = magicFields[i];
                    stats.faithRankList[i - 1] = faithFields[i];
                    // Debug.Log("Faith Created " + faithFields[i]);
                }
                Debug.LogError(chrName);
                // if (stats.GetPrimaryWeapon() == null && stats.magic[0] != null) stats.SetPrimaryWeapon(stats.magic[0]);
            }

            fullRoster.Add(chrName, stats);
            lineIndex++;
        }
    }

    //Writes the new UnitStats object into a dictionally using the name of the character as a key
    void WriteFullRoster(string Name, UnitStats stat)
    {
        fullRoster.Add(Name, stat);

    }

    //Returns the object based on the character's name
    public static UnitStats GetUnitStats(string Name)
    {
        if (fullRoster.ContainsKey(Name))
        {
            return fullRoster[Name];
        }

        return null; //Will never be called but change later
    }


    //Adds unit from the full roster to the playable roster when needed
    public static void AddPlayableUnit(string Name)
    {
        if (fullRoster.ContainsKey(Name))
        {
            // playableRoster.Add(Name, fullRoster[Name]);
            playableList.Add(fullRoster[Name]);
        }
    }

    public static void RemovePlayableUnit(string name) {
        if (fullRoster.ContainsKey(name))
        {
            playableList.Remove(fullRoster[name]);
        }
    }

    public static List<UnitStats> GetPlayableUnits() => playableList;

    public static UnitStats GetPlayableUnit(string name)
    {
        return playableList.Find(unit => unit.UnitName == name);
    }
    public int GetNumUnits() { return playableList.Count; }

    public void setFaithSpells()
    {
        foreach (UnitStats un in playableList)
        {
            un.SetFaith();
        }
    }

    // Resets the Health of all units
    public static void HealUnits()
    {
        // foreach (UnitStats unit in fullRoster.Values)
        // {
        //     unit.ResetHealth();
        // }
        foreach (UnitStats unit in playableList)
        {
            unit.ResetHealth();
        }
    }

}
