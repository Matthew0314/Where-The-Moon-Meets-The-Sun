using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Limit : MonoBehaviour
{
    private int gaugeMaxCharge;
    private int gaugeCharge;

    public abstract IEnumerator StartLimit();
}

public class SecondChance : Limit {

    public override IEnumerator StartLimit() {
        yield return null;
    }
}