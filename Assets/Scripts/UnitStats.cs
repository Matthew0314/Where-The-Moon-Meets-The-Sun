using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class UnitStats
{
    public string UnitName { get; set; }
    public string Name { get; set; }
    public string UnitDescription { get; set; }
    public string UnitClass { get; set; }
    public string UnitType { get; set; } //Enemy or Unit
    public int Level { get; set; }
    public int Health { get; set; }               //base health
    public int CurrentHealth { get; set; }
    public int Attack { get; set; }                 //base attack
    public int Magic { get; set; }                  //base magic
    public int Defense { get; set; }                //base defense
    public int Resistance { get; set; }             //base resistance to magic
    public int Speed { get; set; }                  //base speed, determines if unit attacks first
    public int Evasion { get; set; }                //base evasion, how often the unit dodges
    public int Luck { get; set; }                   //base luck, increases chance of critical
    public int EnemyID { get; set; }

    public bool AirBorn { get; set; }                //if unit is airborn, will be ablee to pass over impassble tiles and weak to bows/air magic
    public bool Mounted { get; set; }                //if unit is mounted
    public bool Armored { get; set; }                //if unit is armored, weak to magic and other armor breaking weapons
    public bool Whisper { get; set; }                //Whisper type

    //Usually set to 0, movement is tied to class but this variable is used when items that permanantly increase mov are used
    public int Movement { get; set; }
    public int HealthBars { get; set; }

    public int Experience { get; set; }

    public List<Weapon> weapons;
    public List<Weapon> magic;
    public List<Item> items;
    public List<Faith> faith;
    public int faithRank { get; set; }
    public int magicRank { get; set; }
    public string[] faithRankList = new string[10];
    public string[] MagicRankList = new string[10];



    public UnitStats(string uName, string dName, string uDesc, int LV, int HLT, int ATK, int MG, int DEF, int RES, int SPD, int EVA, int LCK, int MOV, string uClass)
    {
        UnitName = uName;
        Name = dName;
        UnitDescription = uDesc;
        UnitClass = uClass;
        Level = LV;
        Health = HLT;
        CurrentHealth = HLT;
        Attack = ATK;
        Magic = MG;
        Defense = DEF;
        Resistance = RES;
        Speed = SPD;
        Evasion = EVA;
        Luck = LCK;
        Movement = MOV;

        weapons = new List<Weapon>();
        magic = new List<Weapon>();
        items = new List<Item>();
        faith = new List<Faith>();

    }


    public virtual void AddWeapon(Weapon weapon)
    {
        if (weapons.Count + items.Count < 6) weapons.Add(weapon);
    }


    public virtual void AddItem(Item item)
    {
        if (weapons.Count + items.Count < 6) items.Add(item);
    }

    public virtual void TakeDamage(int health)
    {
        CurrentHealth -= health;
        if (CurrentHealth < 0) { CurrentHealth = 0; }
    }

    public virtual void HealUnit(int health)
    {
        int curHlt = health + CurrentHealth;

        if (curHlt > Health) curHlt = Health;

        CurrentHealth = curHlt;
    }

    public virtual Weapon GetWeaponAt(int x) => weapons[x];
    public virtual void ResetHealth() => CurrentHealth = Health;
    public virtual Item GetItemAt(int x) => items[x];

    
    public abstract PlayerClass getClass();
    public abstract void SetFaith();
    
}

public class PlayerStats : UnitStats {
    public int HealthGR {get; set;}                //base health
    public int AttackGR {get; set;}                //base attack
    public int MagicGR {get; set;}                //base magic
    public int DefenseGR {get; set;}               //base defense
    public int ResistanceGR {get; set;}           //base resistance to magic
    public int SpeedGR {get; set;}                 //base speed, determines if unit attacks first
    public int EvasionGR {get; set;}               //base evasion, how often the unit dodges
    public int LuckGR {get; set;}               //base luck, increases chance of critical

    // public int Experience {get; set;}

    private PlayerClass ClassStats;
    


    public PlayerStats(string uName, string dName, string uDesc, int LV, int HLTGR, int ATKGR, int MGGR, int DEFGR, int RESGR, int SPDGR, int EVGR, int LCKGR, int HLT, int ATK, int MG, int DEF, int RES, int SPD, int EVA, int LCK, int MOV, string uClass, int faiRank, int magRank) : base(uName, dName, uDesc, LV, HLT, ATK, MG, DEF, RES, SPD, EVA, LCK, MOV, uClass)
    {

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
        faithRank = faiRank;
        magicRank = magRank;

    }

    public override PlayerClass getClass() {
        return ClassStats;
    }

    public override void SetFaith()
    {
        faith = new List<Faith>();
        magic = new List<Weapon>();
        for(int i = 0; i < faithRank; i++) {
            // if (faithRankList[i] != "null") {
            //     Type faithType = Type.GetType(faithRankList[i]);
            //     Faith tempFai = (Faith)Activator.CreateInstance(faithType);
            //     if (tempFai != null) {
            //         faith.Add(tempFai);
            //     }
            // }    
            if (faithRankList[i] != "null") {
                Faith tempFai = null;
                
                try {
                    Type faithType = Type.GetType(faithRankList[i]);
                    tempFai = (Faith)Activator.CreateInstance(faithType);
                    if (tempFai != null) {
                        faith.Add(tempFai);
                    }
                }
                catch (Exception)
                {
                    Weapon temp = WeaponManager.MakeWeapon(faithRankList[i]);
                    if (temp != null) {
                        magic.Add(temp);
                        // weapons.Add(temp);
                    }
                } 
            }
        }

        
        for(int i = 0; i < magicRank; i++) {
            if (MagicRankList[i] != "null") {
                Weapon temp = WeaponManager.MakeWeapon(MagicRankList[i]);
                magic.Add(temp);
            }
        }
    }
}

public class EnemyStats : UnitStats {

    
    public bool IsBoss {get; set;}

    public EnemyStats(int id, string uName, string uDesc,string uClass,int LV, int HLT,int ATK,int MG,int DEF,int RES,int SPD,int EVA,int LCK,int MOV,bool air,bool mount,bool arm,bool whisp, int hBars, bool boss) : base(uName, uName, uDesc, LV, HLT, ATK, MG, DEF, RES, SPD, EVA, LCK, MOV, uClass) {
        UnitType = "Enemy";
        AirBorn = air;
        Mounted = mount;
        Armored = arm;
        Whisper = whisp;
        HealthBars = hBars;
        EnemyID = id;
        IsBoss = boss;
        faithRank = 1;
    }

    public override PlayerClass getClass() {
        return null;
    }

    public override void SetFaith() {}
}
