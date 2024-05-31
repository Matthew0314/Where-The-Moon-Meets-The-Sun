using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Holds each units stats
public class UnitStats
{
    private string unitName;
    private string unitDescription;
    private string unitClass;
    private int level;
    private int experience;

    //Stat growths (-100 to 100), can't be modified once object is created
    private int healthGR;                 //base health
    private int attackGR;                 //base attack
    private int magicGR;                  //base magic
    private int defenseGR;                //base defense
    private int resistanceGR;             //base resistance to magic
    private int speedGR;                  //base speed, determines if unit attacks first
    private int evasionGR;                //base evasion, how often the unit dodges
    private int luckGR;                   //base luck, increases chance of critical

    private int health;                 //base health
    private int attack;                 //base attack
    private int magic;                  //base magic
    private int defense;                //base defense
    private int resistance;             //base resistance to magic
    private int speed;                  //base speed, determines if unit attacks first
    private int evasion;                //base evasion, how often the unit dodges
    private int luck;                   //base luck, increases chance of critical

    //Usually set to 0, movement is tied to class but this variable is used when items that permanantly increase mov are used
    private int movement;

    private List<Weapon> weapons;

    //need to add weapon types


    public UnitStats(string uName, string uDesc, int LV, int HLTGR, int ATKGR, int MGGR, int DEFGR, int RESGR, int SPDGR, int EVGR, int LCKGR, int HLT, int ATK, int MG, int DEF, int RES, int SPD, int EVA, int LCK, int MOV, string uClass)
    {
        unitName = uName;
        unitDescription = uDesc;
        unitClass = uClass;
        level = LV;
        experience = 0;
        healthGR = HLTGR;
        attackGR = ATKGR;
        magicGR = MGGR;
        defenseGR = DEFGR;
        resistanceGR = RESGR;
        speedGR = SPDGR;
        evasionGR = EVGR;
        luckGR = LCKGR;
        health = HLT;
        attack = ATK;
        magic = MG;
        defense = DEF;
        resistance = RES;
        speed = SPD;
        evasion = EVA;
        luck = LCK;
        movement = MOV;

        weapons = new List<Weapon>();

    }


    public string UnitClass
    {
        get { return unitClass; }
        set { unitClass = value; }
    }

    public string UnitName
    {
        get { return unitName; }
        set { unitName = value; }
    }

    public string UnitDescription
    {
        get { return unitDescription; }
        set { unitDescription = value; }
    }

    public int Level
    {
        get { return level; }
        set { level = value; }
    }

    public int Experience
    {
        get { return experience; }
        set { experience = value; }
    }

    //Only Getters because we can't change growth rates
    public int HealthGR
    {
        get { return healthGR; }
        //set { healthGR = value; }
    }

    public int AttackGR
    {
        get { return attackGR; }
        //set { attackGR = value; }
    }

    public int MagicGR
    {
        get { return magicGR; }
        //set { magicGR = value; }
    }

    public int DefenseGR
    {
        get { return defenseGR; }
        //set { defenseGR = value; }
    }

    public int ResistanceGR
    {
        get { return resistanceGR; }
        //set { resistanceGR = value; }
    }

    public int SpeedGR
    {
        get { return speedGR; }
        //set { speedGR = value; }
    }

    public int EvasionGR
    {
        get { return evasionGR; }
        //set { evasionGR = value; }
    }

    public int LuckGR
    {
        get { return luckGR; }
        //set { luckGR = value; }
    }


    public int Health
    {
        get { return health; }
        set { health = value; }
    }

    public int Attack
    {
        get { return attack; }
        set { attack = value; }
    }

    public int Magic
    {
        get { return magic; }
        set { magic = value; }
    }

    public int Defense
    {
        get { return defense; }
        set { defense = value; }
    }

    public int Resistance
    {
        get { return resistance; }
        set { resistance = value; }
    }

    public int Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    public int Evasion
    {
        get { return evasion; }
        set { evasion = value; }
    }

    public int Luck
    {
        get { return luck; }
        set { luck = value; }
    }


    public int Movement
    {
        get { return movement; }
        set { movement = value; }
    }

    public void AddWeapon(Weapon weapon) {
        weapons.Add(weapon);
    }
}

