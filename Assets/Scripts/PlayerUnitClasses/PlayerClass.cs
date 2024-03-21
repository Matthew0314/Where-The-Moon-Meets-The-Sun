using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClass 
{
    //Class for all player units
    private string className;           //Name of class
    private string classDescription;    //class description
    private string classType;           //Unique, base, intermediate, advanced, master

    //Stat growths (-100 to 100)
    private int health;                 //base health
    private int attack;                 //base attack
    private int magic;                  //base magic
    private int defense;                //base defense
    private int resistance;             //base resistance to magic
    private int speed;                  //base speed, determines if unit attacks first
    //private int dexterity;              //base dexterity, how often the unit hits
    private int evasion;                //base evasion, how often the unit dodges
    private int luck;                   //base luck, increases chance of critical

    private int movement;               //movement, fixed and doesn't have a stat growth

    private bool airBorn;               //if unit is airborn, will be ablee to pass over impassble tiles and weak to bows/air magic
    private bool mounted;               //if unit is mounted
    private bool armored;               //if unit is armored, weak to magic and other armor breaking weapons
    private bool whisper;               //Whisper type

    //Will probably have to include what type of weapons can be equipped

    public PlayerClass(string cName,string cDesc,string cType,int HP,int ATK,int MAG,int DEF,int RES,int SPD,int EVA,int LUCK,int MOVE,bool air,bool mount,bool arm,bool whisp)
    {
        className = cName; 
        classDescription = cDesc; 
        classType = cType; 
        health = HP; 
        attack = ATK; 
        magic = MAG; 
        defense = DEF;
        resistance = RES;
        speed = SPD; 
        evasion = EVA;
        luck = LUCK;
        movement = MOVE; 
        airBorn = air;
        mounted = mount;
        armored = arm;
        whisper = whisp;        
    }

    public string ClassName
    {
        get { return className; }
        //set { className = value; }
    }

    public string ClassDesrciption
    {
        get { return classDescription; }
        set { classDescription = value; }
    }

    public string ClassType
    {
        get { return classType; }
        set { classType = value; }
    }

    public int Health
    {
        get { return health; }
        //set { health = value; }
    }

    public int Attack
    {
        get { return attack; }
        //set { attack = value; }
    }

    public int Magic
    {
        get { return magic; }
        //set { magic = value; }
    }

    public int Defense
    {
        get { return defense; }
        //set { defense = value; }
    }

    public int Resistance
    {
        get { return resistance; }
        //set { resistance = value; }
    }

    public int Speed
    {
        get { return speed; }
        //set { speed = value; }
    }

    /*public int Dexterity
    {
        get { return dexterity; }
        //set { dexterity = value; }
    }*/

    public int Evasion
    {
        get { return evasion; }
        //set { evasion = value; }
    }

    public int Luck
    {
        get { return luck; }
        //set { luck = value; }
    }

    public int Movement
    {
        get { return movement; }
        //set { movement = value; }
    }

    public bool AirBorn
    {
        get { return airBorn; }
        //set { airBorn = value; }
    }

    public bool Mounted
    {
        get { return mounted; }
        //set { mounted = value; }
    }

    public bool Armored
    {
        get { return armored; }
        //set { armored = value; }
    }

    public bool Whisper
    {
        get { return whisper; }
        //set { whisper = value; }
    }
}
