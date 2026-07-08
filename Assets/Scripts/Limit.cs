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
}

public class SecondChance : Limit {

    public override IEnumerator StartLimit() {
        yield return null;
    }

    public override bool CanUseLimit(UnitManager user) {
        return true;
    }
}