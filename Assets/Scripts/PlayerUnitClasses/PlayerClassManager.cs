using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClassManager : MonoBehaviour
{
    private PlayerClass uClass;
    public TextAsset classTextData;
    public static Dictionary<string, PlayerClass> fullClassList = new Dictionary<string, PlayerClass>();


    public void Init()
    {
        ReadCSV();
    }

    //Reads the data from the csv file and stores it in an object for each class
    //NEVER CALL THIS AFTER PROLOGUE MAP
    void ReadCSV()
    {
        string[] data = classTextData.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);

        for (int i = 16; i < data.Length - 1; i += 16)
        {
            string cName = data[i];
            string cDesc = data[i + 1];
            string cType = data[i + 2];
            int HP = int.Parse(data[i + 3]);
            int ATK = int.Parse(data[i + 4]);  
            int MAG = int.Parse(data[i + 5]);
            int DEF = int.Parse(data[i + 6]);
            int RES = int.Parse(data[i + 7]);
            int SPD = int.Parse(data[i + 8]);   
            int EVA = int.Parse(data[i + 9]);
            int LUCK = int.Parse(data[i + 10]);
            int MOVE = int.Parse(data[i + 11]);
            bool air = bool.Parse(data[i + 12]);
            bool mount = bool.Parse(data[i + 13]);
            bool armored = bool.Parse(data[i + 14]);
            bool whisp = bool.Parse(data[i + 15]);

            uClass = new PlayerClass(cName, cDesc, cType, HP, ATK, MAG, DEF, RES, SPD, EVA, LUCK, MOVE, air, mount, armored, whisp);

            Debug.Log("Class Name: " + uClass.ClassName);
            Debug.Log("Movement: " + uClass.Movement);
            Debug.Log("Whisper: " + uClass.Whisper);

            fullClassList.Add(cName, uClass);
        }
    }



    public PlayerClass GetUnitClass(string name)
    {

        if (fullClassList.ContainsKey(name))
        {

            return fullClassList[name];
        }

        return null; //Will never be called but change later
 
    }


}
