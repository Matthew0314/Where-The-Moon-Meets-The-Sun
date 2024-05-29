using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Weapon(string name, string desc, string Wtype, char WRank, int ATK, int HitR, int crit, int wei, int use, bool R1, bool R2, bool R3, int R, float MMou, float MAB, float MArm, float MWhisp, float MInf) {
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
    }

    public abstract void SpecialAbility();
}

public class NormalWeapon : Weapon
{
    public NormalWeapon(string name, string desc, string Wtype, char WRank, int ATK, int HitR, int crit, int wei, int use, bool R1, bool R2, bool R3, int R, float MMou, float MAB, float MArm, float MWhisp, float MInf) : base(name, desc, Wtype, WRank, ATK, HitR, crit, wei, use, R1, R2, R3, R, MMou, MAB, MArm, MWhisp, MInf) {}
    
    public override void SpecialAbility() {}
}
