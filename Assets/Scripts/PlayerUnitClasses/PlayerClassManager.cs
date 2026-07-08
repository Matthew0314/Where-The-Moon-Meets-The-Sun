using System;
using System.Collections.Generic;
using UnityEngine;
// using MyGame.Gameplay.Stats; 




public class PlayerClassManager : MonoBehaviour
{
    private static TextAsset classTextData;
    public static Dictionary<string, PlayerClass> fullClassList = new Dictionary<string, PlayerClass>();

    public static void Init()
    {
        classTextData = Resources.Load<TextAsset>("TextData/PlayerInfoCSV/PlayerClasses");
        if (classTextData != null)
        {
            ReadCSV();
        }
        else
        {
            Debug.LogError("Failed to load class text data from Resources/TextData/PlayerInfoCSV/PlayerClasses");
        }
    }

    private static void ReadCSV()
    {
        // 1. Split by lines first so we can parse row-by-row safely
        string[] lines = classTextData.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // Start at index 1 to skip the header row
        for (int i = 1; i < lines.Length; i++)
        {
            string[] row = lines[i].Split(',');

            // Edge case: skip empty lines
            if (row.Length < 16) continue;

            string cName = row[0];
            string cDesc = row[1];
            string cType = row[2];

            // 2. Assemble the lightweight stat growths struct
            ClassStatGrowths growths = new ClassStatGrowths
            {
                health     = int.Parse(row[3]),
                attack     = int.Parse(row[4]),
                magic      = int.Parse(row[5]),
                defense    = int.Parse(row[6]),
                resistance = int.Parse(row[7]),
                speed      = int.Parse(row[8]),
                evasion    = int.Parse(row[9]),
                luck       = int.Parse(row[10]),
                movement   = int.Parse(row[11])
            };

            // 3. Assemble the bitmask flags using your ClassAttributes enum
            ClassAttributes attributes = ClassAttributes.None;
            
            if (bool.Parse(row[12])) attributes |= ClassAttributes.Airborn;
            if (bool.Parse(row[13])) attributes |= ClassAttributes.Mounted;
            if (bool.Parse(row[14])) attributes |= ClassAttributes.Armored;
            if (bool.Parse(row[15])) attributes |= ClassAttributes.Whisper;

            // 4. Instantiate our updated PlayerClass with cleaner components
            PlayerClass uClass = new PlayerClass(cName, cDesc, cType, growths, attributes);

            // Avoid duplicate key crashes if Init() accidentally gets called twice
            if (!fullClassList.ContainsKey(cName))
            {
                fullClassList.Add(cName, uClass);
            }
        }
    }

    // Cleaned up using TryGetValue for slightly safer dictionary lookups
    public static PlayerClass GetUnitClass(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        return fullClassList.TryGetValue(name, out PlayerClass playerClass) ? playerClass : null;
    }
}