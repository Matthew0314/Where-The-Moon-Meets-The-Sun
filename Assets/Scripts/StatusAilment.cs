using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTWB.GridGameplay.StatusAilments {
    [System.Serializable]
    public struct StatModifiers
    {
        public int attack;
        public int magic;
        public int defense;
        public int resistance;
        public int speed;
        public int evasion;
        public int luck;
        public int movement;
    }

    public abstract class StatusAilment
    {
        public string Name { get; }
        public string Type { get; } 
        public UnitManager Unit { get; }
        public StatModifiers Modifiers { get; }
        public int Turns { get; set; }

        protected StatusAilment(string name, string type, UnitManager unit, int turns, StatModifiers modifiers = default)
        {
            Name = name;
            Type = type;
            Unit = unit;
            Turns = turns;
            Modifiers = modifiers;
        }

        public virtual void Decrement() => Turns--;

        public virtual IEnumerator StartOfTurn()
        {
            yield return null;
        }
    }

    // Replaced NormalAliment with a generic Debuff/Buff class
    public class GenericAilment : StatusAilment
    {
        public GenericAilment(string name, string type, UnitManager unit, int turns, StatModifiers modifiers) 
            : base(name, type, unit, turns, modifiers) { }
    }

    public class PoisonAilment : StatusAilment
    {
        private readonly int damagePerTurn = 3;

        public PoisonAilment(string name, string type, UnitManager unit, int turns) 
            : base(name, type, unit, turns) { }

        public override IEnumerator StartOfTurn()
        {
            if (Unit != null)
            {
                int currentHp = Unit.GetCurrentHealth();
                Unit.TakeDamage(Mathf.Min(currentHp - 1, damagePerTurn));
            }
            yield return null;
        }
    }

    public class FreezeAilment : StatusAilment
    {
        public FreezeAilment(string name, string type, UnitManager unit, int turns) 
            : base(name, type, unit, turns) { }

        public override IEnumerator StartOfTurn()
        {
            //Basically you need to get the turn manager and tell it to remove the character from the list.
            yield return null;
        }
    }
}
