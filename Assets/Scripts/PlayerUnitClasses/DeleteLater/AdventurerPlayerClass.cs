using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurerPlayerClass : BasePlayerClass
{
    public void AdventurerClass()
    {
        ClassName = "Adventurer";
        //ClassDescription = "Hello World";
        ClassType = "Base";

        Health = 50;
        Attack = 50;
        Magic = 35;
        Defense = 35;
        Resistance = 30;
        Speed = 50;
        Dexterity = 55;
        Evasion = 50;
        Luck = 50;

        Movement = 3;
    }
}
