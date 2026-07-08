using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public struct ClassStatGrowths 
{
    public int health;
    public int attack;
    public int magic;
    public int defense;
    public int resistance;
    public int speed;
    public int evasion;
    public int luck;
    public int movement; //fixed, doesn't have a stat growth
}

[Flags] // Can store a max of 8 attributes at the moment
public enum ClassAttributes : byte
{
    None     = 0,
    Airborn  = 1 << 0,
    Mounted  = 1 << 1,
    Armored  = 1 << 2,
    Whisper  = 1 << 3
}

public class PlayerClass 
{
    public string ClassName { get; private set; }
    public string ClassDescription { get; set; }
    public string ClassType { get; set; } // Unique, base, intermediate, etc.

    public ClassStatGrowths Growths { get; private set; }
    public ClassAttributes Attributes { get; private set; }

    // Clean, streamlined constructor
    public PlayerClass(string name, string desc, string type, ClassStatGrowths growths, ClassAttributes attributes)
    {
        ClassName = name;
        ClassDescription = desc;
        ClassType = type;
        Growths = growths;
        Attributes = attributes;
    }

    public bool HassAttribute(ClassAttributes attribute) => (Attributes & attribute) == attribute;

    public int GetGrowth(StatType statType)
    {
        return statType switch
        {
            StatType.Health     => Growths.health,
            StatType.Attack     => Growths.attack,
            StatType.Magic      => Growths.magic,
            StatType.Defense    => Growths.defense,
            StatType.Resistance => Growths.resistance,
            StatType.Speed      => Growths.speed,
            StatType.Evasion    => Growths.evasion,
            StatType.Luck       => Growths.luck,
            StatType.Movement   => Growths.movement, // Returns 0 or fixed class movement base
            _ => 0
        };
    }

}
