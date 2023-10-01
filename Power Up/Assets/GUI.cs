using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUI : MonoBehaviour
{
    bool _isPaused = false;
    public delegate void GamePause(bool value);
    public static event GamePause OnGamePause;
    public GameObject pauseMenu;
    private float _defTimeScale;
    public SpellController playerSpellController;
    public SpellConfig[] spellConfigs;
    public SpellEffect[] spellDamageEffects;
    public SpellEffect[] spellSpeedEffects;
    public TMP_Dropdown configDrop, damageDrop, speedDrop;
    private void Start()
    {
        _defTimeScale = Time.timeScale;
        TogglePauseMenu();
        OnSetConfigType(configDrop.value);
        OnSetDamageType(damageDrop.value);
        OnSetSpeedType(speedDrop.value);
    }
    public void TogglePauseMenu ()
    {
        _isPaused = !_isPaused;
        OnGamePause?.Invoke(_isPaused);
        if (_isPaused)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0.0f;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            pauseMenu.SetActive(false);
            Time.timeScale = _defTimeScale;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
            TogglePauseMenu();
    }

    public void OnSetDamageType(int damageType)
    {
        if (damageType < spellDamageEffects.Length)
            playerSpellController.elementalEffect = spellDamageEffects[damageType];
        UpdatePlayerSpells();
    }
    public void OnSetSpeedType(int type)
    {
        if (type < spellDamageEffects.Length)
            playerSpellController.projectileEffect = spellSpeedEffects[type];
        UpdatePlayerSpells();
    }
    public void OnSetConfigType(int type)
    {
        if (type < spellConfigs.Length)
            playerSpellController.spellConfig = spellConfigs[type];
        UpdatePlayerSpells();
    }

    void UpdatePlayerSpells() {
        playerSpellController.Reset();
        playerSpellController.Decorate();
    }
}
