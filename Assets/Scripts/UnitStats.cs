using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class UnitStats
{
    public string UnitName { get; set; }
    public int UnitID { get; set; }
    public string Name { get; set; }
    public string UnitDescription { get; set; }
    public string UnitClass { get; protected set; }
    public string UnitType { get; protected set; } //Enemy or Unit
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
    public bool AirBorn { get; set; }                //if unit is airborn, will be ablee to pass over impassble tiles and weak to bows/air magic
    public bool Mounted { get; set; }                //if unit is mounted
    public bool Armored { get; set; }                //if unit is armored, weak to magic and other armor breaking weapons
    public bool Whisper { get; set; }                //Whisper type

    //Usually set to 0, movement is tied to class but this variable is used when items that permanantly increase mov are used
    public int Movement { get; set; }
    public int HealthBars { get; set; }

    public int Experience { get; set; }
    public int SP { get; protected set; }

    private Weapon primaryWeapon;

    protected List<Weapon> weapons;
    protected List<Weapon> magic;
    protected List<Item> items;
    protected List<Faith> faith;
    public int faithRank { get; set; }
    public int magicRank { get; set; }
    public string[] faithRankList = new string[10];
    public string[] MagicRankList = new string[10];

    public List<UnitAbility> abilities;



    public UnitStats(int uID, string uName, string dName, string uDesc, int LV, int HLT, int ATK, int MG, int DEF, int RES, int SPD, int EVA, int LCK, int MOV, string uClass)
    {
        UnitID = uID;
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
        SP = 0;

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
    // public virtual Weapon GetWeaponAt(int x) => (x >= 0 && x < weapons.Count) ? weapons[x] : null;

    public virtual void ResetHealth() => CurrentHealth = Health;
    public virtual Item GetItemAt(int x) => items[x];


    public abstract PlayerClass GetClass();
    public abstract void SetFaith();

    public virtual void SetPrimaryWeapon(Weapon weap)
    {
        primaryWeapon = weap;

        // if (primaryWeapon == null) return;

        // temp.Remove(primaryWeapon);
        // temp.Insert(0, primaryWeapon);

    }

    public virtual void FindAPrimaryWeapon()
    {
        if (weapons.Count >= 1)
        {
            SetPrimaryWeapon(weapons[0]);
            return;
        }

        if (magic.Count >= 1)
        {
            SetPrimaryWeapon(magic[0]);
            return;
        }

        SetPrimaryWeapon(null);


    }

    public virtual Weapon GetPrimaryWeapon() => primaryWeapon;
    public virtual void AddSP(int ad) => SP += ad;
    public virtual void SubSP(int su) => SP -= su;
    public virtual int GetSkillExperience(string skillType) => 0;

    public virtual List<Weapon> GetWeaponsList() => weapons;
    public virtual List<Weapon> GetMagicList() => magic;
    public virtual List<Item> GetItems() => items;
    public virtual List<Faith> GetFaith() => faith;
    
}

[System.Serializable]
public class PlayerStats : UnitStats
{
    public int HealthGR { get; set; }                //base health
    public int AttackGR { get; set; }                //base attack
    public int MagicGR { get; set; }                //base magic
    public int DefenseGR { get; set; }               //base defense
    public int ResistanceGR { get; set; }           //base resistance to magic
    public int SpeedGR { get; set; }                 //base speed, determines if unit attacks first
    public int EvasionGR { get; set; }               //base evasion, how often the unit dodges
    public int LuckGR { get; set; }               //base luck, increases chance of critical
    public int AbilityPoints { get; protected set; }

    // public int Experience {get; set;}

    // private PlayerClass ClassStats;

    public Dictionary<string, int> SkillExperience { get; protected set; }



    public PlayerStats(int uID, string uName, string dName, string uDesc, int LV, int HLTGR, int ATKGR, int MGGR, int DEFGR, int RESGR, int SPDGR, int EVGR, int LCKGR, int HLT, int ATK, int MG, int DEF, int RES, int SPD, int EVA, int LCK, int MOV, string uClass, int faiRank, int magRank) : base(uID, uName, dName, uDesc, LV, HLT, ATK, MG, DEF, RES, SPD, EVA, LCK, MOV, uClass)
    {

        Experience = 60;
        HealthGR = HLTGR;
        AttackGR = ATKGR;
        MagicGR = MGGR;
        DefenseGR = DEFGR;
        ResistanceGR = RESGR;
        SpeedGR = SPDGR;
        EvasionGR = EVGR;
        LuckGR = LCKGR;
        UnitType = "Player";

        // ClassStats = PlayerClassManager.GetUnitClass(uClass);

        AirBorn = GetClass().AirBorn;
        Mounted = GetClass().Mounted;
        Armored = GetClass().Armored;
        Whisper = GetClass().Whisper;

        HealthBars = 1;
        faithRank = faiRank;
        magicRank = magRank;

        SkillExperience = new Dictionary<string, int>();

        SkillExperience.Add("Sword", 0);
        SkillExperience.Add("Lance", 0);
        SkillExperience.Add("Bow", 0);
        SkillExperience.Add("Magic", 0);
        SkillExperience.Add("Faith", 0);
        SkillExperience.Add("Brawl", 0);

    }

    public override PlayerClass GetClass()
    {
        return PlayerClassManager.GetUnitClass(UnitClass);
    }

    public void SetClass(String cla)
    {
        // PlayerClass temp = PlayerClassManager.GetUnitClass(cla);
        // if (temp == null) return;

        // ClassStats = temp;
        UnitClass = cla;
        AirBorn = GetClass().AirBorn;
        Mounted = GetClass().Mounted;
        Armored = GetClass().Armored;
        Whisper = GetClass().Whisper;
    }

    public override void SetFaith()
    {
        faith = new List<Faith>();
        magic = new List<Weapon>();
        for (int i = 0; i < faithRank; i++)
        {
            // if (faithRankList[i] != "null") {
            //     Type faithType = Type.GetType(faithRankList[i]);
            //     Faith tempFai = (Faith)Activator.CreateInstance(faithType);
            //     if (tempFai != null) {
            //         faith.Add(tempFai);
            //     }
            // }    
            if (faithRankList[i] != "null")
            {
                Faith tempFai = null;

                try
                {
                    Type faithType = Type.GetType(faithRankList[i]);
                    tempFai = (Faith)Activator.CreateInstance(faithType);
                    if (tempFai != null)
                    {
                        faith.Add(tempFai);
                    }
                }
                catch (Exception)
                {
                    Weapon temp = WeaponManager.MakeWeapon(faithRankList[i]);
                    if (temp != null)
                    {
                        magic.Add(temp);
                        // weapons.Add(temp);
                    }
                }
            }
        }


        for (int i = 0; i < magicRank; i++)
        {
            if (MagicRankList[i] != "null")
            {
                Weapon temp = WeaponManager.MakeWeapon(MagicRankList[i]);
                magic.Add(temp);
            }
        }
    }



    public override int GetSkillExperience(string skillType)
    {
        if (string.IsNullOrEmpty(skillType))
        {
            Debug.LogWarning("GetSkillExperience called with a null or empty skillType.");
            return 0;
        }

        if (SkillExperience == null)
        {
            Debug.LogError("SkillExperience dictionary is null. Make sure it is initialized.");
            return 0;
        }

        if (SkillExperience.TryGetValue(skillType, out int exp))
        {
            return exp;
        }
        else
        {
            Debug.LogWarning($"Skill '{skillType}' not found in SkillExperience dictionary. Returning 0.");
            return 0;
        }
    }
    
    public void AddSkillExperience(string skillType, int exp)
    {
        if (string.IsNullOrEmpty(skillType))
        {
            Debug.LogWarning("AddSkillExperience called with a null or empty skillType.");
            return;
        }

        if (SkillExperience == null)
        {
            Debug.LogError("SkillExperience dictionary is null. Make sure it is initialized.");
            return;
        }

        if (SkillExperience.ContainsKey(skillType))
        {
            SkillExperience[skillType] += exp;
            Debug.Log($"Added {exp} experience to skill '{skillType}'. New total: {SkillExperience[skillType]}");
        }
        else
        {
            Debug.LogWarning($"Skill '{skillType}' not found in SkillExperience dictionary. Cannot add experience.");
        }
    }


    
}

public class EnemyStats : UnitStats {

    
    public bool IsBoss {get; set;}

    public EnemyStats(int id, string uName, string uDesc,string uClass,int LV, int HLT,int ATK,int MG,int DEF,int RES,int SPD,int EVA,int LCK,int MOV,bool air,bool mount,bool arm,bool whisp, int hBars, bool boss) : base(id, uName, uName, uDesc, LV, HLT, ATK, MG, DEF, RES, SPD, EVA, LCK, MOV, uClass) {
        UnitType = "Enemy";
        AirBorn = air;
        Mounted = mount;
        Armored = arm;
        Whisper = whisp;
        HealthBars = hBars;
        // EnemyID = id;
        IsBoss = boss;
        faithRank = 1;
    }

    public override PlayerClass GetClass() {
        return null;
    }

    public override void SetFaith() {}
}
