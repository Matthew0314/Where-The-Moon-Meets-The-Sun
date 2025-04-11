using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusAilments
{
    public string Name { get; }
    public string Type { get; }
    public UnitManager Unit { get; }
    public int Attack { get; }
    public int Magic { get; }
    public int Defense { get; }                
    public int Resistance { get; }            
    public int Speed { get; }                
    public int Evasion { get; }            
    public int Luck { get; } 
    public int Movement { get; }
    public int Turns { get; set; } // will decrement at the start of the turn for enemies and players

    public StatusAilments (string n, string t, UnitManager u, int a, int m, int d, int r, int s, int e, int l, int mov, int turns) {
        Name = n;
        Type = t;
        Attack = a;
        Magic = m;
        Defense = d;
        Resistance = r;
        Speed = s;
        Evasion = e;
        Luck = l;
        Turns = turns;
        Unit = u;
        Movement = mov;
    }

    public virtual void Decrement() => Turns--;

    public virtual void StartOfTurn() {}
}


public class Poison : StatusAilments {

    public Poison(string n, string t, UnitManager u, int a, int m, int d, int r, int s, int e, int l, int mov, int turns) : base(n, t, u, a, m, d, r, s, e, l, mov, turns) {}
    public override void StartOfTurn() {
        if (Unit != null) Unit.TakeDamage(3);
    }
}
