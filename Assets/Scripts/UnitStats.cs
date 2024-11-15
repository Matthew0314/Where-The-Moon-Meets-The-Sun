using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitStats
{
    public string UnitName {get; set;}
    public string Name {get; set;}
    public string UnitDescription {get; set;}
    public string UnitClass {get; set;}
    public string UnitType {get; set;} //Enemy or Unit
    public int Level {get; set;}
    public int Health {get; set;}               //base health
    public int Attack {get; set;}                 //base attack
    public int Magic {get; set;}                  //base magic
    public int Defense {get; set;}                //base defense
    public int Resistance {get; set;}             //base resistance to magic
    public int Speed {get; set;}                  //base speed, determines if unit attacks first
    public int Evasion {get; set;}                //base evasion, how often the unit dodges
    public int Luck {get; set;}                   //base luck, increases chance of critical
    public int EnemyID {get; set;}   

    public bool AirBorn {get; set;}                //if unit is airborn, will be ablee to pass over impassble tiles and weak to bows/air magic
    public bool Mounted {get; set;}                //if unit is mounted
    public bool Armored {get; set;}                //if unit is armored, weak to magic and other armor breaking weapons
    public bool Whisper {get; set;}                //Whisper type

    //Usually set to 0, movement is tied to class but this variable is used when items that permanantly increase mov are used
    public int Movement {get; set;} 
    public int HealthBars {get; set;}  

    public int Experience {get; set;}

    public List<Weapon> weapons;

    public UnitStats(string uName, string dName, string uDesc, int LV, int HLT, int ATK, int MG, int DEF, int RES, int SPD, int EVA, int LCK, int MOV, string uClass)
    {
        UnitName = uName;
        Name = dName;
        UnitDescription = uDesc;
        UnitClass = uClass;
        Level = LV;
        Health = HLT;
        Attack = ATK;
        Magic = MG;
        Defense = DEF;
        Resistance = RES;
        Speed = SPD;
        Evasion = EVA;
        Luck = LCK;
        Movement = MOV;

        weapons = new List<Weapon>();

    }

    public virtual void AddWeapon(Weapon weapon) {
        weapons.Add(weapon);
    }

    public virtual Weapon GetWeaponAt(int x) {
        return weapons[x];
    }

    public abstract PlayerClass getClass();
}

public class PlayerStats : UnitStats {
    private int HealthGR {get; set;}                //base health
    private int AttackGR {get; set;}                //base attack
    private int MagicGR {get; set;}                //base magic
    private int DefenseGR {get; set;}               //base defense
    private int ResistanceGR {get; set;}           //base resistance to magic
    private int SpeedGR {get; set;}                 //base speed, determines if unit attacks first
    private int EvasionGR {get; set;}               //base evasion, how often the unit dodges
    private int LuckGR {get; set;}               //base luck, increases chance of critical

    // public int Experience {get; set;}

    private PlayerClass ClassStats;


    public PlayerStats(string uName, string dName, string uDesc, int LV, int HLTGR, int ATKGR, int MGGR, int DEFGR, int RESGR, int SPDGR, int EVGR, int LCKGR, int HLT, int ATK, int MG, int DEF, int RES, int SPD, int EVA, int LCK, int MOV, string uClass) : base(uName, dName, uDesc, LV, HLT, ATK, MG, DEF, RES, SPD, EVA, LCK, MOV, uClass) {

        Experience = 0;
        HealthGR = HLTGR;
        AttackGR = ATKGR;
        MagicGR = MGGR;
        DefenseGR = DEFGR;
        ResistanceGR = RESGR;
        SpeedGR = SPDGR;
        EvasionGR = EVGR;
        LuckGR = LCKGR;
        UnitType = "Player";

        ClassStats = PlayerClassManager.GetUnitClass(uClass);

        AirBorn = ClassStats.AirBorn;
        Mounted = ClassStats.Mounted;
        Armored = ClassStats.Armored;
        Whisper = ClassStats.Whisper;

        HealthBars = 1;

    }

    public override PlayerClass getClass() {
        return ClassStats;
    }
}

public class EnemyStats : UnitStats {

    
    bool IsBoss {get; set;}

    public EnemyStats(int id, string uName, string uDesc,string uClass,int LV, int HLT,int ATK,int MG,int DEF,int RES,int SPD,int EVA,int LCK,int MOV,bool air,bool mount,bool arm,bool whisp, int hBars, bool boss) : base(uName, uName, uDesc, LV, HLT, ATK, MG, DEF, RES, SPD, EVA, LCK, MOV, uClass) {
        UnitType = "Enemy";
        AirBorn = air;
        Mounted = mount;
        Armored = arm;
        Whisper = whisp;
        HealthBars = hBars;
        EnemyID = id;
        IsBoss = boss;
    }

    public override PlayerClass getClass() {
        return null;
    }
}
