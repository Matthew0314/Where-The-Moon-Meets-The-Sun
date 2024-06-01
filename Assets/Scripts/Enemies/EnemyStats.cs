/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats 
{
    private string unitName;
    private string unitDescription;
    private string unitClass;
    private int level;

    private int health;                 //base health
    private int attack;                 //base attack
    private int magic;                  //base magic
    private int defense;                //base defense
    private int resistance;             //base resistance to magic
    private int speed;                  //base speed, determines if unit attacks first
    private int evasion;                //base evasion, how often the unit dodges
    private int luck;                   //base luck, increases chance of critical
    private int movement;               //Movement

    private bool airBorn;               //if unit is airborn, will be ablee to pass over impassble tiles and weak to bows/air magic
    private bool mounted;               //if unit is mounted
    private bool armored;               //if unit is armored, weak to magic and other armor breaking weapons
    private bool whisper;               //Whisper type

    private int healthBars;

     private List<Weapon> weapons;

    public EnemyStats(string uName,string uDesc,string uType,int lev, int HP,int ATK,int MAG,int DEF,int RES,int SPD,int EVA,int LUCK,int MOVE,bool air,bool mount,bool arm,bool whisp, int hBars)
    {
        unitName = uName; 
        unitDescription = uDesc; 
        unitClass = uType; 
        level = lev;
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
        healthBars = hBars;     

        weapons = new List<Weapon>();
    }

     public string UnitName
    {
        get { return unitName; }
        // set { unitName = value; }
    }

    public string UnitDesrciption
    {
        get { return unitDescription; }
        // set { unitDescription = value; }
    }

    public string UnitClass
    {
        get { return unitClass; }
        // set { unitClass = value; }
    }

    public int Level
    {
        get { return level; }
        //set { level = value; }
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

    public int HealthBars
    {
        get { return healthBars; }
        set { healthBars = value; }
    }

    public void AddWeapon(Weapon weapon) {
        weapons.Add(weapon);
    }
}*/
