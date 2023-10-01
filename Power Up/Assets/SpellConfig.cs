using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewSpellConfig", menuName = "Spell/Config", order = 0)]

public class SpellConfig : ScriptableObject, ISpell
{
    [SerializeField] private float _manaCost;
    [SerializeField] private float _cooldown;
    [SerializeField] private float _damage;
    [SerializeField] private float _speed;
    [SerializeField] private DamageType[] _elements;
    public string spellName;
    public GameObject spellProjPrefab;
    public string spellDescription;
    public float ManaCost { get { return _manaCost; } }
    public float Cooldown { get { return _cooldown; } }
    public float Damage { get { return _damage; } }
    public float Speed { get { return _speed; } }
    public DamageType[] DamageTypes { get { return _elements; } }
}
