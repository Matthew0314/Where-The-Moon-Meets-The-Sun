using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using System.Diagnostics;
//using System.Diagnostics;

public class UnitRosterManager : MonoBehaviour
{
    [SerializeField] TextAsset statTextData;
    [SerializeField] TextAsset faithTextData;
    [SerializeField] TextAsset magicTextData;
    private static Dictionary<string, UnitStats> fullRoster = new Dictionary<string, UnitStats>();
    // public static Dictionary<string, UnitStats> playableRoster = new Dictionary<string, UnitStats>();
    // private static Dictionary<string, UnitStats> MapRoster = new Dictionary<string, UnitStats>();
    private static List<UnitStats> playableList = new List<UnitStats>();
    private List<UnitStats> mapList = new List<UnitStats>();
 





    //Reads the data from the csv file and constructs a new UnitStats object
    // ! MAKE SURE TO NEVER CALL THIS AFTER THE FIRST MAP
    public void ReadCSV() {
        string[] data = statTextData.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        string[] faithData = faithTextData.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        string[] magicData = magicTextData.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        Type unitType = Type.GetType("PlayerStats");
        int faithInd = 0;
        
        for (int i = 31; i < data.Length - 1; i += 31) {
            faithInd += 11;
            string chrName = data[i];
            string dName = data[i + 1];
            string chrDesc = data[i + 2];
            int lev = int.Parse(data[i + 3]);
            int HPGR = int.Parse(data[i + 4]);
            int ATKGR = int.Parse(data[i + 5]);
            int MAGGR = int.Parse(data[i + 6]);
            int DEFGR = int.Parse(data[i + 7]);
            int RESGR = int.Parse(data[i + 8]);
            int SPDGR = int.Parse(data[i + 9]);
            int EVAGR = int.Parse(data[i + 10]);
            int LCKGR = int.Parse(data[i + 11]);
            int HP = int.Parse(data[i + 12]);
            int ATK = int.Parse(data[i + 13]);
            int MAG = int.Parse(data[i + 14]);
            int DEF = int.Parse(data[i + 15]);
            int RES = int.Parse(data[i + 16]);
            int SPD = int.Parse(data[i + 17]);
            int EVA = int.Parse(data[i + 18]);
            int LCK = int.Parse(data[i + 19]);
            int MOV = int.Parse(data[i + 20]);
            string charClass = data[i + 21];
            string item1 = data[i + 22];
            string item2 = data[i + 23];
            string item3 = data[i + 24];
            string item4 = data[i + 25];
            string item5 = data[i + 26];
            string item6 = data[i + 27];
            int faithRank = int.Parse(data[i + 28]);
            int magicRank = int.Parse(data[i + 29]);

            Debug.Log(chrName);
            Debug.Log(chrDesc);
            Debug.Log(lev);
            Debug.Log("MOV: " + MOV);

            
            UnitStats stats = (UnitStats)Activator.CreateInstance(unitType, chrName, dName, chrDesc, lev, HPGR, ATKGR, MAGGR, DEFGR, RESGR, SPDGR, EVAGR, LCKGR, HP, ATK, MAG, DEF, RES, SPD, EVA, LCK, MOV, charClass, faithRank, magicRank);
            // stats = new UnitStats(chrName, chrDesc, lev, HPGR, ATKGR, MAGGR, DEFGR, RESGR, SPDGR, EVAGR, LCKGR, HP, ATK, MAG, DEF, RES, SPD, EVA, LCK, MOV, charClass);
           
            Debug.Log("Name: " + stats.UnitName);
            Debug.Log("Player Class: " + stats.UnitClass);

            for (int j = 0; j < 6; j++) {
                if (data[i + 22 + j] == "NULL") {
                    Debug.Log("Null Weapon");
                    break;
                }

                Weapon tempWeapon = WeaponManager.MakeWeapon(data[i + 22 + j]);
                if (tempWeapon != null) {
                    stats.AddWeapon(tempWeapon);
                    Debug.Log("Added " + tempWeapon.WeaponName);
                } else {
                    Type itemType = Type.GetType(data[i + 22 + j]);
                    Item tempItem = (Item)Activator.CreateInstance(itemType);
                    if (tempItem != null) {
                        stats.AddItem(tempItem);
                    }
                }
                
            }

            int ind = 0;

            for (int j = faithInd; j < faithInd + 10; j++) {
                stats.MagicRankList[ind] = magicData[j+1];
                ind++;
            }

            ind = 0;

            for(int j = faithInd; j < faithInd + 10; j++) {
                Debug.Log("Faith Created " + faithData[j+1]);
                stats.faithRankList[ind] = faithData[j+1];
                ind++;
            }


           
            




            fullRoster.Add(chrName, stats);
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
    public void AddPlayableUnit(string Name)
    {
        if (fullRoster.ContainsKey(Name))
        {
            // playableRoster.Add(Name, fullRoster[Name]);
            playableList.Add(fullRoster[Name]);
        }
    }

    //Initlizes which units are going to be put on the map
    public void InitMapUnit(int len)
    {
        if (len < playableList.Count)
        {
            for (int i = 0; i < len; i++)
            {
                UnitStats stat = playableList[i];
                //MapRoster.Add(stat.UnitName, stat);
                mapList.Add(playableList[i]);
            }
        }
        else
        {
            for (int i = 0; i < playableList.Count; i++)
            {
                UnitStats stat = playableList[i];
                //MapRoster.Add(stat.UnitName, stat);
                mapList.Add(playableList[i]);
            }
        }

    }


    public void AddMapUnit(string unitName) {//
        if (fullRoster.ContainsKey(unitName))
        {
            mapList.Add(fullRoster[unitName]);
        }
    }

    public void RemoveMapUnit(string unitName) {//
        if (mapList.Contains(fullRoster[unitName])) {
            mapList.Remove(fullRoster[unitName]);
        }
    }

    public List<UnitStats> getMapUnits() { return  mapList; }

    public int GetNumUnits() { return playableList.Count; }

    public void setFaithSpells() {
        foreach (UnitStats un in playableList) {
            un.SetFaith();
        }
    }

}
