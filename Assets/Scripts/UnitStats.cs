using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// namespace LTWB.UnitStats
// {
    public enum StatType { Health, Attack, Magic, Defense, Resistance, Speed, Evasion, Luck, Movement }
    public enum SkillType { Faith, Magic, Sword, Lance, Bow, Brawl }

    // Changed to classes so they are reference types—allowing direct runtime modification!
    [Serializable]
    public class CoreStats
    {
        public int health;
        public int attack;
        public int magic;
        public int defense;
        public int resistance;
        public int speed;
        public int evasion;
        public int luck;
        public int movement;

        public CoreStats() { }

        public CoreStats(int health, int attack, int magic, int defense, int resistance, int speed, int evasion, int luck, int movement)
        {
            this.health = health;
            this.attack = attack;
            this.magic = magic;
            this.defense = defense;
            this.resistance = resistance;
            this.speed = speed;
            this.evasion = evasion;
            this.luck = luck;
            this.movement = movement;
        }
    }

    [Serializable]
    public class UnitSkills
    {
        public int faith;
        public int magic;
        public int sword;
        public int lance;
        public int bow;
        public int brawl;
    }

    [Serializable]
    public class UnitStatGrowths
    {
        public int health;
        public int attack;
        public int magic;
        public int defense;
        public int resistance;
        public int speed;
        public int evasion;
        public int luck;
        public int movement; 
    }

    public abstract class UnitStats
    {
        // Identity Data
        public int UnitID { get; set; }
        public string UnitName { get; set; }
        public string Name { get; set; }
        public string UnitDescription { get; set; }
        public string UnitClass { get; protected set; }
        public string UnitType { get; protected set; }
        public int Level { get; set; }

        // Core Stats Data (Now a Class Reference)
        public CoreStats BaseStats;
        public int CurrentHealth { get; set; }
        public int HealthBars { get; set; } = 1;
        public int Experience { get; set; }
        public int SP { get; protected set; }

        // Packed Attributes
        public ClassAttributes Attributes { get; set; }

        // Inventories
        protected List<Weapon> weapons = new(6);
        protected List<Weapon> magic = new(6);
        protected List<Item> items = new(6);
        protected List<Faith> faith = new(6);

        // Deprecation placeholders
        public int faithRank { get; set; }
        public int magicRank { get; set; }
        public string[] faithRankList = new string[10];
        public string[] MagicRankList = new string[10];

        private Weapon primaryWeapon;

        protected UnitStats(int id, string uName, string dName, string desc, int level, CoreStats baseStats, string uClass)
        {
            UnitID = id;
            UnitName = uName;
            Name = dName;
            UnitDescription = desc;
            Level = level;
            BaseStats = baseStats;
            CurrentHealth = baseStats.health;
            UnitClass = uClass;
        }

        public virtual int GetStat(StatType statType)
        {
            return statType switch
            {
                StatType.Health     => BaseStats.health,
                StatType.Attack     => BaseStats.attack,
                StatType.Magic      => BaseStats.magic,
                StatType.Defense    => BaseStats.defense,
                StatType.Resistance => BaseStats.resistance,
                StatType.Speed      => BaseStats.speed,
                StatType.Evasion    => BaseStats.evasion,
                StatType.Luck       => BaseStats.luck,
                StatType.Movement   => BaseStats.movement,
                _ => 0
            };
        }

        public virtual void TakeDamage(int damage) => CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
        public virtual void HealUnit(int amount) => CurrentHealth = Mathf.Min(BaseStats.health, CurrentHealth + amount);

        public bool HasInventorySpace => (weapons.Count + items.Count) < 6;
        public virtual void AddWeapon(Weapon weapon) { if (HasInventorySpace) weapons.Add(weapon); }
        public virtual void AddItem(Item item) { if (HasInventorySpace) items.Add(item); }
        public virtual Weapon GetWeaponAt(int x) => weapons[x];

        public virtual List<Weapon> GetWeaponsList() => weapons;
        public virtual List<Weapon> GetMagicList() => magic;
        public virtual List<Item> GetItems() => items;
        public virtual List<Faith> GetFaith() => faith;

        public Weapon GetPrimaryWeapon() => primaryWeapon;
        public void SetPrimaryWeapon(Weapon weapon) => primaryWeapon = weapon;

        public virtual void FindAPrimaryWeapon()
        {
            if (weapons.Count > 0) SetPrimaryWeapon(weapons[0]);
            else if (magic.Count > 0) SetPrimaryWeapon(magic[0]);
            else SetPrimaryWeapon(null);
        }

        public virtual void AddSP(int ad) => SP += ad;
        public virtual void SubSP(int su) => SP -= su;

        public virtual ClassAttributes GetAttributes()
        {
            var classData = PlayerClassManager.GetUnitClass(UnitClass);
            return classData != null ? classData.Attributes : ClassAttributes.None;
        }

        public virtual bool HasAttribute(ClassAttributes targetFlag) => (GetAttributes() & targetFlag) == targetFlag;

        public abstract PlayerClass GetClass();

        public abstract void SetFaith();

        public virtual void ResetHealth() => CurrentHealth = BaseStats.health;


        
    }

    [System.Serializable]
    public class PlayerStats : UnitStats
    {
        public UnitStatGrowths PersonalGrowths { get; private set; }
        
        // Auto-properties that return references to our modifiable data classes
        public UnitSkills SkillLevels { get; private set; } 
        public UnitSkills SkillExperience { get; private set; } 

        public PlayerStats(int id, string uName, string dName, string desc, int level, CoreStats baseStats, UnitStatGrowths personalGrowths, string uClass) 
            : base(id, uName, dName, desc, level, baseStats, uClass)
        {
            UnitType = "Player";
            Experience = 60;
            PersonalGrowths = personalGrowths;

            // Initialize the classes cleanly
            SkillExperience = new UnitSkills();
            SkillLevels = new UnitSkills();
        }

        public void SetClass(string newClass) => UnitClass = newClass;
        public override PlayerClass GetClass() => PlayerClassManager.GetUnitClass(UnitClass);

        public int GetSkillExperience(SkillType skillType)
        {
            return skillType switch
            {
                SkillType.Faith => SkillExperience.faith,
                SkillType.Magic => SkillExperience.magic,
                SkillType.Sword => SkillExperience.sword,
                SkillType.Lance => SkillExperience.lance,
                SkillType.Bow   => SkillExperience.bow,
                SkillType.Brawl => SkillExperience.brawl,
                _ => 0
            };
        }

        // This now works perfectly without compiler restrictions because SkillExperience is a class!
        public void AddSkillExperience(SkillType skillType, int exp)
        {
            switch (skillType)
            {
                case SkillType.Faith: SkillExperience.faith += exp; break;
                case SkillType.Magic: SkillExperience.magic += exp; break;
                case SkillType.Sword: SkillExperience.sword += exp; break;
                case SkillType.Lance: SkillExperience.lance += exp; break;
                case SkillType.Bow:   SkillExperience.bow += exp; break;
                case SkillType.Brawl: SkillExperience.brawl += exp; break;
            }
        }

        public int GetGrowthRate(StatType statType)
        {
            PlayerClass currentClass = PlayerClassManager.GetUnitClass(UnitClass);
            int classGrowth = currentClass != null ? currentClass.GetGrowth(statType) : 0;

            int personalGrowth = statType switch
            {
                StatType.Health     => PersonalGrowths.health,
                StatType.Attack     => PersonalGrowths.attack,
                StatType.Magic      => PersonalGrowths.magic,
                StatType.Defense    => PersonalGrowths.defense,
                StatType.Resistance => PersonalGrowths.resistance,
                StatType.Speed      => PersonalGrowths.speed,
                StatType.Evasion    => PersonalGrowths.evasion,
                StatType.Luck       => PersonalGrowths.luck,
                _ => 0
            };

            return personalGrowth + classGrowth;
        }

        public override void SetFaith()
        {
            faith.Clear();
            magic.Clear();
            for (int i = 0; i < faithRank; i++)
            { 
                if (faithRankList[i] != "null")
                {
                    Type faithType = Type.GetType(faithRankList[i]);
                    
                    if (faithType != null) // Safer, much faster than a try-catch!
                    {
                        Faith tempFai = (Faith)Activator.CreateInstance(faithType);
                        if (tempFai != null) faith.Add(tempFai);
                    }
                    else
                    {
                        // If it's not a special Faith component class, it's a standard spell item
                        Weapon temp = WeaponManager.MakeWeapon(faithRankList[i]);
                        if (temp != null) magic.Add(temp);
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
    }

    public class EnemyStats : UnitStats
    {
        public bool IsBoss { get; set; }
        private ClassAttributes localAttributes;

        public EnemyStats(int id, string uName, string desc, string uClass, int level, CoreStats baseStats, ClassAttributes attributes, int hBars, bool boss) 
            : base(id, uName, uName, desc, level, baseStats, uClass)
        {
            UnitType = "Enemy";
            localAttributes = attributes;
            HealthBars = hBars;
            IsBoss = boss;
        }

        public override ClassAttributes GetAttributes() => localAttributes;
        public override PlayerClass GetClass() => null; // Enemies don't have a PlayerClass, so we return null here.

        public override void SetFaith() {}

        public override bool HasAttribute(ClassAttributes targetFlag) => (localAttributes & targetFlag) == targetFlag;
    }


