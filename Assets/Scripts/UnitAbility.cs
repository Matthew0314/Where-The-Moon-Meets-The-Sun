using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitAbility : MonoBehaviour
{
    public string abilityName;
    public string abilityDescription;
    public int abilityCost;
    public bool isUnique; //if true, only if this is unique to a certain unit
}
