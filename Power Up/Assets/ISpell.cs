using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpell
{
    float ManaCost { get; }
    float Cooldown { get; }
    float Damage { get; }
    float Speed { get; }
    DamageType[] DamageTypes { get; }
}
