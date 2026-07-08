using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Limit : MonoBehaviour
{
    private int gaugeMaxCharge;
    private int gaugeCharge;

    public abstract IEnumerator StartLimit();

    public abstract bool CanUseLimit(UnitManager user);

    public abstract IEnumerator PassiveEffect(UnitManager user);
}

public class SecondChance : Limit {

    public override IEnumerator StartLimit() {
        yield return null;
    }

    public override bool CanUseLimit(UnitManager user) {
        return true;
    }

    public override IEnumerator PassiveEffect(UnitManager user) {
        yield break;
    }
}

// <summary>
// This limit gives a random buff to the player each turn.
// When the limit is activated, it will give all the buffs to the player, a random buff to player units nearby, and a random debuff to enemy units nearby.
// </summary>
// public class StarFall : Limit {


// }