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

            string chrName = data[0];
            string dName = data[1];
            string chrDesc = data[2];
            int lev = int.Parse(data[3]);
            int HPGR = int.Parse(data[4]);
            int ATKGR = int.Parse(data[5]);
            int MAGGR = int.Parse(data[6]);
            int DEFGR = int.Parse(data[7]);
            int RESGR = int.Parse(data[8]);
            int SPDGR = int.Parse(data[9]);
            int EVAGR = int.Parse(data[10]);
            int LCKGR = int.Parse(data[11]);
            int HP = int.Parse(data[12]);
            int ATK = int.Parse(data[13]);
            int MAG = int.Parse(data[14]);
            int DEF = int.Parse(data[15]);
            int RES = int.Parse(data[16]);
            int SPD = int.Parse(data[17]);
            int EVA = int.Parse(data[18]);
            int LCK = int.Parse(data[19]);
            int MOV = int.Parse(data[20]);
            string charClass = data[21];
            int faithRank = int.Parse(data[28]);
            int magicRank = int.Parse(data[29]);

            UnitStats stats = (UnitStats)Activator.CreateInstance(unitType, chrName, dName, chrDesc, lev, HPGR, ATKGR, MAGGR, DEFGR, RESGR, SPDGR, EVAGR, LCKGR, HP, ATK, MAG, DEF, RES, SPD, EVA, LCK, MOV, charClass, faithRank, magicRank);

            // Add items and weapons
            for (int j = 0; j < 6; j++)
            {
                string itemName = data[22 + j];
                if (itemName == "NULL")
                {
                    // Debug.Log("Null Weapon");
                    break;
                }

                Weapon tempWeapon = WeaponManager.MakeWeapon(itemName);
                if (tempWeapon != null)
                {
                    stats.AddWeapon(tempWeapon);
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
