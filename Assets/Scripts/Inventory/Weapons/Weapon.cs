using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Stores data for every weapon in the game
public abstract class Weapon 
{
    public string WeaponName {get; set;}
    public string WeaponDescription {get; set;}
    public string WeaponType {get; set;}
    public char WeaponRank {get; set;}
    public int Attack {get; set;}
    public int HitRate {get; set;}
    public int CritRate {get; set;}
    public int Weight {get; set;}
    public int Uses {get; set;}
    public int MaxUses {get; set;}
    public bool Range1 {get; set;}
    public bool Range2 {get; set;}
    public bool Range3 {get; set;}
    public int Range {get; set;}
    public float MultMounted {get; set;}
    public float MultAirBorn {get; set;}
    public float MultArmored {get; set;}
    public float MultWhisper {get; set;}
    public float MultInfantry {get; set;}
    public int NumHits {get; set;}
    public bool CanCounter {get; set;}
    public string WeaponClass {get;}

    public Queue<UnitManager> AttackingQueue {get; set;}
    public Queue<UnitManager> DefendingQueue {get; set;}
    public int AttackerCurrentHealth {get; set;}
    public int DefenderCurrentHealth {get; set;}
    bool miss;
    bool critical;

   
    private IMaps _currentMap = GameObject.Find("GridManager").GetComponent<IMaps>();
    private FindPath pathFinder = GameObject.Find("Player").GetComponent<FindPath>();

    //Constructor
    public Weapon(string name, string desc, string Wtype, char WRank, int ATK, int HitR, int crit, int wei, int use, bool R1, bool R2, bool R3, int R, float MMou, float MAB, float MArm, float MWhisp, float MInf, int nHit, bool counter, string weapClass) {
        WeaponName = name;
        WeaponDescription = desc;
        WeaponType = Wtype;
        WeaponRank = WRank;
        Attack = ATK;
        HitRate = HitR;
        CritRate = crit;
        Weight = wei;
        Uses = use;
        MaxUses = use;
        Range1 = R1;
        Range2 = R2;
        Range3 = R3;
        Range = R;
        MultMounted = MMou; 
        MultAirBorn = MAB;
        MultArmored = MArm;
        MultWhisper = MWhisp;
        MultInfantry = MInf;
        NumHits = nHit;
        CanCounter = counter;
        WeaponClass = weapClass;

        AttackingQueue = new Queue<UnitManager>();
        DefendingQueue = new Queue<UnitManager>();
        
    }

    //If theres a special ability this function will be called
    public virtual void unitAttack(Queue<UnitManager> attacking, Queue<UnitManager> defending, UnitManager defender, int attackerX, int attackerZ, int defenderX, int defenderZ) {
        
        int queueSize = attacking.Count;
        for (int i = 0; i < queueSize; i++) {
            UnitManager atk = attacking.Dequeue();
            UnitManager def = defending.Dequeue();

            int damage = atk.stats.Attack + atk.primaryWeapon.Attack - def.stats.Defense;

            float multiplier = 1;

            if (def.stats.Mounted) {
                multiplier += atk.primaryWeapon.MultMounted - 1; 
            }
            if (def.stats.AirBorn) {
                multiplier += atk.primaryWeapon.MultAirBorn - 1; 
            }
            if (def.stats.Armored) {
                multiplier += atk.primaryWeapon.MultArmored - 1; 
            }
            if (def.stats.Whisper) {
                multiplier += atk.primaryWeapon.MultWhisper - 1; 
            }

            Debug.Log("defender current health " + def.getCurrentHealth() + " " + def.stats.Health);

            damage = (int)(damage * multiplier);

            Debug.Log(atk.stats.UnitName + " Did" + damage + " damage to " + def.stats.UnitName);

            def.TakeDamage(damage);

            Debug.Log("defender current health " + def.getCurrentHealth() + " " + def.stats.Health);

            if (def.getCurrentHealth() <= 0) {

                
                if (defender.stats.UnitName == def.stats.UnitName) {
                    Debug.Log(def.stats.UnitName + "Has died");
                    _currentMap.RemoveDeadUnit(def, defenderX, defenderZ);
                    
                } else {
                    
                    _currentMap.RemoveDeadUnit(def, attackerX, attackerZ);
                }
                
                
                break;
            }
        }

    }

    public virtual int UnitAttack(UnitManager atk, UnitManager def, bool ignoreRates) {
        int damage = atk.stats.Attack + atk.primaryWeapon.Attack - def.stats.Defense;

        float multiplier = 1;

        if (def.stats.Mounted) {
            multiplier += atk.primaryWeapon.MultMounted - 1; 
        }
        if (def.stats.AirBorn) {
            multiplier += atk.primaryWeapon.MultAirBorn - 1; 
        }
        if (def.stats.Armored) {
            multiplier += atk.primaryWeapon.MultArmored - 1; 
        }
        if (def.stats.Whisper) {
            multiplier += atk.primaryWeapon.MultWhisper - 1; 
        }

        damage = (int)(damage * multiplier);

        critical = false;
        miss = false;

        if (!ignoreRates) {
            int hit = atk.primaryWeapon.HitRate + (atk.stats.Luck * 4) - def.stats.Evasion;
            int crit =  atk.primaryWeapon.CritRate + (int)(atk.stats.Luck / 2);

            int rand = Random.Range(0, 101);
            if (hit <= rand) {
                miss = true;
                Debug.Log("miss" + miss);
                return 0;
            } else if (crit >= rand) {
                critical = true;
                Debug.Log("critical" + critical);
                return damage * 3;
            }
        }

        return damage;

    }

   

    public virtual void InitiateQueues(UnitManager attacker, UnitManager defender, int attackerX, int attackerZ, int defenderX, int defenderZ) {

        AttackingQueue = new Queue<UnitManager>();
        DefendingQueue = new Queue<UnitManager>();
        
        for (int i = 0; i < attacker.primaryWeapon.NumHits ; i++) {
            Debug.Log("hi" + attacker.stats.Name);
            AttackingQueue.Enqueue(attacker);
            DefendingQueue.Enqueue(defender);
            Debug.Log("hi");
        }
        bool[,] counter;
        
        if (defender.primaryWeapon != null) {
            counter = pathFinder.CalculateAttack(defenderX, defenderZ, defender.primaryWeapon.Range, defender.primaryWeapon.Range1, defender.primaryWeapon.Range2, defender.primaryWeapon.Range3);

            if (counter[attackerX, attackerZ] && defender.primaryWeapon != null) {
                for (int i = 0; i < defender.primaryWeapon.NumHits; i++) {
                    AttackingQueue.Enqueue(defender);
                    DefendingQueue.Enqueue(attacker);
                }
            }
        }
        

        if (attacker.stats.Speed >=  defender.stats.Speed + 4) {
            for (int i = 0; i < attacker.primaryWeapon.NumHits; i++) {
                AttackingQueue.Enqueue(attacker);
                DefendingQueue.Enqueue(defender);
            }
        } else if (attacker.stats.Speed <=  defender.stats.Speed - 4 && defender.primaryWeapon != null) {
            counter = pathFinder.CalculateAttack(defenderX, defenderZ, defender.primaryWeapon.Range, defender.primaryWeapon.Range1, defender.primaryWeapon.Range2, defender.primaryWeapon.Range3);

            if (counter[attackerX, attackerZ] && defender.primaryWeapon != null) {
                for (int i = 0; i < defender.primaryWeapon.NumHits; i++) {
                    AttackingQueue.Enqueue(defender);
                    DefendingQueue.Enqueue(attacker);
                }
            }
        }
    }

    public virtual void DecrementUses() { Uses--; }

    public abstract void SpecialAbility();



    // public virtual Queue
}

//Class for all weapons without a special ability
public class NormalWeapon : Weapon
{
    public NormalWeapon(string name, string desc, string Wtype, char WRank, int ATK, int HitR, int crit, int wei, int use, bool R1, bool R2, bool R3, int R, float MMou, float MAB, float MArm, float MWhisp, float MInf, int nHit, bool counter, string weapClass) : base(name, desc, Wtype, WRank, ATK, HitR, crit, wei, use, R1, R2, R3, R, MMou, MAB, MArm, MWhisp, MInf, nHit, counter, weapClass) {}
    
    
    public override void SpecialAbility() {}
}
