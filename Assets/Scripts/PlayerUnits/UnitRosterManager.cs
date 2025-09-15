using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

// Holds how much experience is needed to obtain each skill level
public struct SkillLevelData
{
    public int level;           
    public int experienceNeeded; 

    public SkillLevelData(int level, int experienceNeeded)
    {
        this.level = level;
        this.experienceNeeded = experienceNeeded;
    }
}

public class UnitRosterManager : MonoBehaviour
{
    private static TextAsset statTextData;
    private static TextAsset faithTextData;
    private static TextAsset magicTextData;
    private static Dictionary<string, UnitStats> fullRoster = new Dictionary<string, UnitStats>();
    private static List<UnitStats> playableList = new List<UnitStats>();
    private static SkillLevelData[] skillLevels = new SkillLevelData[10];


    private void Awake() {
        // skillLevels[0] = new SkillLevelData(1, 0);
        // skillLevels[1] = new SkillLevelData(2, 40);
        // skillLevels[2] = new SkillLevelData(3, 90);
        // skillLevels[3] = new SkillLevelData(4, 150);
        // skillLevels[4] = new SkillLevelData(5, 400);
        // skillLevels[5] = new SkillLevelData(6, 1100);
        // skillLevels[6] = new SkillLevelData(7, 1500);
        // skillLevels[7] = new SkillLevelData(8, 2000);
        // skillLevels[8] = new SkillLevelData(9, 2800);
        // skillLevels[9] = new SkillLevelData(10, 3500);
        skillLevels[0] = new SkillLevelData(1, 0);
        skillLevels[1] = new SkillLevelData(2, 40);
        skillLevels[2] = new SkillLevelData(3, 130);
        skillLevels[3] = new SkillLevelData(4, 280);
        skillLevels[4] = new SkillLevelData(5, 680);
        skillLevels[5] = new SkillLevelData(6, 1780);
        skillLevels[6] = new SkillLevelData(7, 3280);
        skillLevels[7] = new SkillLevelData(8, 5280);
        skillLevels[8] = new SkillLevelData(9, 8080);
        skillLevels[9] = new SkillLevelData(10, 11580);
    }

    public static void ReadCSV()
    {
        statTextData = Resources.Load<TextAsset>("TextData/PlayerInfoCSV/RosterStats");
        faithTextData = Resources.Load<TextAsset>("TextData/PlayerInfoCSV/FaithList");
        magicTextData = Resources.Load<TextAsset>("TextData/PlayerInfoCSV/MagicList");

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
                    break;

                Weapon tempWeapon = WeaponManager.MakeWeapon(itemName);

                if (tempWeapon != null)
                    stats.AddWeapon(tempWeapon);
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
                }
            }

            stats.FindAPrimaryWeapon();

            fullRoster.Add(chrName, stats);
            lineIndex++;
        }
    }

    /// <summary>
    /// Gets the amount of experience required to reach the next level
    /// given the unit's current experience.
    /// </summary>
    // Get current level from total experience
    public static int GetCurrentLevel(int currentExp)
    {
        for (int i = skillLevels.Length - 1; i >= 0; i--)
        {
            if (currentExp >= skillLevels[i].experienceNeeded)
                return skillLevels[i].level;
        }
        return 1;
    }

    // Get total experience required to reach a given level
    public static int GetTotalExpForLevel(int level)
    {
        var data = System.Array.Find(skillLevels, s => s.level == level);
        return data.experienceNeeded;
    }

    // Get experience required to go from (level) → (level+1)
    public static int GetExpBetweenLevels(int level)
    {
        if (level < 1 || level >= skillLevels.Length)
            return 0;

        int currentLevelExp = skillLevels[level - 1].experienceNeeded;
        int nextLevelExp = skillLevels[level].experienceNeeded;

        return nextLevelExp - currentLevelExp;
    }

    // Get exp still needed until the next level from currentExp
    public static int GetExpToNextLevel(int currentExp)
    {
        int currentLevel = GetCurrentLevel(currentExp);

        if (currentLevel >= skillLevels[skillLevels.Length - 1].level)
            return 0; // maxed out

        int nextLevelExp = GetTotalExpForLevel(currentLevel + 1);
        return nextLevelExp - currentExp;
    }

    //Writes the new UnitStats object into a dictionally using the name of the character as a key
    void WriteFullRoster(string Name, UnitStats stat) => fullRoster.Add(Name, stat);

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
    public static UnitStats GetPlayableUnit(string name) => playableList.Find(unit => unit.UnitName == name);

    public void setFaithSpells()
    {
        foreach (UnitStats un in playableList)
        {
            un.SetFaith();
        }
    }

    // Resets the Health of all units
    public static void HealUnits() {
        foreach (UnitStats unit in playableList)
        {
            unit.ResetHealth();
        }
    }

}
