using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerSpells : MonoBehaviour
{
    public delegate void UpdatePlayerMana(float playerMana, float maxMana);
    public static event UpdatePlayerMana OnUpdatePlayerMana;
    [SerializeField] private float _maxMana;
    [SerializeField] private float _staminaRegenRate;
    private float _currentMana;
    [SerializeField] private Transform playerLeftTransform;
    [SerializeField] private Transform playerRightTransform;
    public float MaxMana { get { return _maxMana; } }
    public SpellController spellController;
    private bool _isPaused = false;
    private void OnEnable()
    {
        GUI.OnGamePause += OnPause;
    }
    private void OnDisable()
    {
        GUI.OnGamePause -= OnPause;
    }
    private void Start()
    {
        _currentMana = _maxMana;
        OnUpdatePlayerMana?.Invoke(_currentMana, _maxMana);
    }
    public void OnLeftSpell (InputAction.CallbackContext ctx)
    {
        if (_isPaused) return;
        if (_currentMana > spellController.spell.ManaCost)
        {
            spellController.Reset();
            spellController.Decorate();
            _currentMana -= spellController.spell.ManaCost;
            spellController.Cast(playerLeftTransform);
        }
    }
    private void FixedUpdate()
    {
        _currentMana = Mathf.Clamp(_currentMana + _staminaRegenRate, 0, _maxMana);
        OnUpdatePlayerMana?.Invoke(_currentMana, _maxMana);
    }
    void OnPause(bool value) => _isPaused = value;
}
