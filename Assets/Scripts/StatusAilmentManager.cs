using UnityEngine;
using LTWB.GridGameplay.StatusAilments;

public class StatusAilmentManager : MonoBehaviour
{
    // A master creator function if you need string-based instantiation
    public StatusAilment CreateStatusAilment(string name, string type, UnitManager unit, int turns, StatModifiers modifiers = default)
    {
        return name switch
        {
            "Poison" => new PoisonAilment(name, type, unit, turns),
            _        => new GenericAilment(name, type, unit, turns, modifiers)
        };
    }

    // Individual helper methods are now incredibly clean
    public StatusAilment CreateAttackAilment(int attack, int turns, UnitManager unit) =>
        new GenericAilment("Attack Down", "Debuff", unit, turns, new StatModifiers { attack = attack });

    public StatusAilment CreateDefenseAilment(int defense, int turns, UnitManager unit) =>
        new GenericAilment("Defense Down", "Debuff", unit, turns, new StatModifiers { defense = defense });

    public StatusAilment CreateSpeedAilment(int speed, int turns, UnitManager unit) =>
        new GenericAilment("Speed Down", "Debuff", unit, turns, new StatModifiers { speed = speed });

    public StatusAilment CreateMovementAilment(int movement, int turns, UnitManager unit) =>
        new GenericAilment("Movement Down", "Debuff", unit, turns, new StatModifiers { movement = movement });
    
    public StatusAilment CreateEvasionAilment(int evasion, int turns, UnitManager unit) =>
        new GenericAilment("Evasion Down", "Debuff", unit, turns, new StatModifiers { evasion = evasion });

    public StatusAilment CreateLuckAilment(int luck, int turns, UnitManager unit) =>
        new GenericAilment("Luck Down", "Debuff", unit, turns, new StatModifiers { luck = luck });

    public StatusAilment CreateMagicAilment(int magic, int turns, UnitManager unit) =>
        new GenericAilment("Magic Down", "Debuff", unit, turns, new StatModifiers { magic = magic });
    
    public StatusAilment CreateResistanceAilment(int resistance, int turns, UnitManager unit) =>
        new GenericAilment("Resistance Down", "Debuff", unit, turns, new StatModifiers { resistance = resistance });

    

}
