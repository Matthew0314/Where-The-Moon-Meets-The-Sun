using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
// using LTWB.UnitStats;

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

        if (statTextData == null || faithTextData == null || magicTextData == null)
        {
            Debug.LogError("Failed to load one or more player CSV data files from Resources.");
            return;
        }

        string[] lines = statTextData.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string[] faithLines = faithTextData.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string[] magicLines = magicTextData.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        int lineIndex = 0;

        // Skip(1) bypasses the header line safely
        foreach (string line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] data = line.Trim().Split(',');
            int index = 0;

            // 1. Core Meta Data
            int unitID = int.Parse(data[index++]);
            string chrName = data[index++];
            Debug.Log("Adding " + chrName);

            string dName = data[index++];
            string chrDesc = data[index++];
            int lev = int.Parse(data[index++]);

            // 2. Personal Growth Rates Struct Assignment
            UnitStatGrowths personalGrowths = new UnitStatGrowths
            {
                health     = int.Parse(data[index++]),
                attack     = int.Parse(data[index++]),
                magic      = int.Parse(data[index++]),
                defense    = int.Parse(data[index++]),
                resistance = int.Parse(data[index++]),
                speed      = int.Parse(data[index++]),
                evasion    = int.Parse(data[index++]),
                luck       = int.Parse(data[index++])
            };

            // 3. Base Core Stats Struct Assignment
            CoreStats baseStats = new CoreStats
            {
                health     = int.Parse(data[index++]),
                attack     = int.Parse(data[index++]),
                magic      = int.Parse(data[index++]),
                defense    = int.Parse(data[index++]),
                resistance = int.Parse(data[index++]),
                speed      = int.Parse(data[index++]),
                evasion    = int.Parse(data[index++]),
                luck       = int.Parse(data[index++]),
                movement   = int.Parse(data[index++])
            };

            string charClass = data[index++];
            
            

            // Clean direct initialization—completely skipping costly Activator methods!
            PlayerStats stats = new PlayerStats(unitID, chrName, dName, chrDesc, lev, baseStats, personalGrowths, charClass);
            

            // 5. Weapon and Item Parsing (Uses the uniform index counter to find the inventory slots)
            for (int j = 0; j < 6; j++)
            {
                string itemName = data[index++]; // Reads through items dynamically based on file position
                if (itemName == "NULL") continue;

                Weapon tempWeapon = WeaponManager.MakeWeapon(itemName);
                if (tempWeapon != null)
                {
                    stats.AddWeapon(tempWeapon);
                }
                else
                {
                    // Simple factory method replacement alternative for generic inventory tools
                    Type itemType = Type.GetType(itemName); 
                    if (itemType != null)
                    {
                        Item tempItem = (Item)Activator.CreateInstance(itemType);
                        stats.AddItem(tempItem);
                    }
                }
            }

            // 4. Safely handle the rank offset indexes
            int faithRank = int.Parse(data[index++]);
            int magicRank = int.Parse(data[index++]);

            stats.faithRank = faithRank;
            stats.magicRank = magicRank;

            // 6. Parallel Magic and Faith List Line Matrix Mapping
            if (lineIndex + 1 < magicLines.Length && lineIndex + 1 < faithLines.Length)
            {
                string[] magicFields = magicLines[lineIndex + 1].Trim().Split(','); 
                string[] faithFields = faithLines[lineIndex + 1].Trim().Split(',');

                for (int i = 1; i <= 10 && i < magicFields.Length && i < faithFields.Length; i++)
                {
                    stats.MagicRankList[i - 1] = magicFields[i];
                    stats.faithRankList[i - 1] = faithFields[i];
                }
            }

            stats.FindAPrimaryWeapon();
            stats.SetFaith(); // Triggers internal spell database lists parsing

            if (!fullRoster.ContainsKey(chrName))
            {
                fullRoster.Add(chrName, stats);
            }
            
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
