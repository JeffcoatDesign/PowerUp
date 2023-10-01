using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpells : MonoBehaviour
{
    [SerializeField] SpellController _spellController;
    [SerializeField] private Transform spellTransform;
    [SerializeField] private float _maxMana = 100f;
    [SerializeField] private float _manaRegenSpeed = 0.1f;
    private float _currentMana;
    private void Awake()
    {
        _currentMana = _maxMana;
    }
    public void OnSpellAttack()
    {
        if (_currentMana > _spellController.spell.ManaCost)
        {
            _spellController.Reset();
            _spellController.Decorate();
            _currentMana -= _spellController.spell.ManaCost;
            _spellController.Cast(spellTransform);
        }
    }
    private void FixedUpdate()
    {
        _currentMana = Mathf.Clamp(_currentMana + _manaRegenSpeed, 0, _maxMana);
    }
}
