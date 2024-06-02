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

    public Queue<UnitManager> AttackingQueue {get; set;}
    public Queue<UnitManager> DefendingQueue {get; set;}

    //Constructor
    public Weapon(string name, string desc, string Wtype, char WRank, int ATK, int HitR, int crit, int wei, int use, bool R1, bool R2, bool R3, int R, float MMou, float MAB, float MArm, float MWhisp, float MInf, int nHit, bool counter) {
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
    }

    //If theres a special ability this function will be called
    public virtual void unitAttack(GameObject attacker, GameObject defender) {}

    public virtual void InitiateQueues(UnitManager attacker, UnitManager defender) {
        for (int i = 0; i < attacker.primaryWeapon.NumHits; i++) {
            AttackingQueue.Enqueue(attacker);
            DefendingQueue.Enqueue(defender);
        }

        if (attacker.primaryWeapon.Range > 3) {
            if (defender.primaryWeapon.Range >= attacker.primaryWeapon.Range) {
                
            }
        } else {

        }
    }

    public abstract void SpecialAbility();


    // public virtual Queue
}

//Class for all weapons without a special ability
public class NormalWeapon : Weapon
{
    public NormalWeapon(string name, string desc, string Wtype, char WRank, int ATK, int HitR, int crit, int wei, int use, bool R1, bool R2, bool R3, int R, float MMou, float MAB, float MArm, float MWhisp, float MInf, int nHit, bool counter) : base(name, desc, Wtype, WRank, ATK, HitR, crit, wei, use, R1, R2, R3, R, MMou, MAB, MArm, MWhisp, MInf, nHit, counter) {}
    
    public override void unitAttack(GameObject attacker, GameObject defender) {}
    public override void SpecialAbility() {}
}
