using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Make costs protected so subclasses can access them
public class CombatArts : Weapon
{
    protected int APCost = 0;
    protected int CPCost = 0;
    protected int durCost = 0;

    // Optional: description field
    // protected string description = "";
    public CombatArts(string name, string desc, string Wtype, char WRank, int ATK, int HitR, int crit, int wei, int use, bool R1, bool R2, bool R3, int R, float MMou, float MAB, float MArm, float MWhisp, float MInf, int nHit, bool counter, bool useMagic, string weapClass)
        : base(name, desc, Wtype, WRank, ATK, HitR, crit, 0, 0, R1, R2, R3, R, MMou, MAB, MArm, MWhisp, MInf, nHit, counter, useMagic, weapClass)
    {
        // Constructor
    }
}

public class CA_BaneOfWispers : CombatArts
{
    // Constructor
    public CA_BaneOfWispers() 
        : base("Bane of Whispers", "Effective against Whispers", "Sword", 'C', 5, 0, 10, 0, 10, true, false, false, 1, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1, true, false, "Physical")
    {
        APCost = 3;
        CPCost = 2;
        durCost = 3;
    }
}
