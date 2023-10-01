using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : ISpell
{
    public float ManaCost { get { return _spellConfig.ManaCost; } }
    public float Cooldown { get { return _spellConfig.Cooldown; } }
    public float Damage { get { return _spellConfig.Damage; } }
    public float Speed { get { return _spellConfig.Speed; } }
    public DamageType[] DamageTypes { get { return _spellConfig.DamageTypes; } }
    private readonly SpellConfig _spellConfig;
    public Spell (SpellConfig spellConfig)
    {
        _spellConfig = spellConfig;
    }
}
