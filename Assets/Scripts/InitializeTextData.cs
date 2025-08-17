using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is used so that the methods that read in the text data can be called any where without the need to copy and paste
// This may not be needed when the game is fully done
public class InitializeTextData : MonoBehaviour
{
    private static bool readData = false; 

    public static void InitData() {
        if (readData) return;
        WeaponManager.ReadCSV();
        PlayerClassManager.Init();
        UnitRosterManager.ReadCSV();
        readData = true;
    }
}
