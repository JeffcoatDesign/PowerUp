using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpellDecorator : ISpell
{
    private readonly ISpell _decoratedSpell;
    private readonly SpellEffect _spellEffect;

    public SpellDecorator(ISpell decoratedSpell, SpellEffect spellEffect)
    {
        _decoratedSpell = decoratedSpell;
        _spellEffect = spellEffect;
    }

    public float ManaCost { get { return _decoratedSpell.ManaCost + _spellEffect.ManaCost; } }
    public float Cooldown { get { return _decoratedSpell.Cooldown + _spellEffect.Cooldown; } }
    public float Damage { get { return _decoratedSpell.Damage + _spellEffect.Damage; } }
    public float Speed { get { return _decoratedSpell.Speed + _spellEffect.Speed; } }
    public DamageType[] DamageTypes { get {
            List<DamageType> damageTypes = new();
            damageTypes.AddRange(_decoratedSpell.DamageTypes);
            damageTypes.AddRange(_spellEffect.DamageTypes);
            return damageTypes.ToArray(); } }
}
