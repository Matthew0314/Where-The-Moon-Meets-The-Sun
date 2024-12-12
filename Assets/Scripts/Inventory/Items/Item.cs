using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item
{
    public int Uses { get; protected set; }
    public bool Equipable { get; protected set; }
    public bool Usable { get; protected set; }
    public string Name { get; protected set; }

    // Constructor
    protected Item()
    {
        Name = "None";
        Uses = 0;
        Equipable = false;
        Usable = false;
    }

    protected Item(string name, int uses, bool equipable, bool usable)
    {
        Name = name;
        Uses = uses;
        Equipable = equipable;
        Usable = usable;
    }

    public virtual void Use(UnitManager user) {
        return;
    }

    public virtual void Equip(UnitManager user) {
        return;
    }

    public virtual bool CanUse(UnitManager user) {
        return Usable;
    }


}

public class Vulnerary: Item {
    // Constructor for Vulnerary
    public Vulnerary() : base("Vulnerary", 3, false, true) { }

    // Implementing the abstract Use method
    public override void Use(UnitManager user)
    {
        Uses--;

        user.HealUnit(10);

    }

    public override bool CanUse(UnitManager user) {
        if (user.getCurrentHealth() < user.getMaxHealth()) {
            return true;
        }

        return false;
    }
}
