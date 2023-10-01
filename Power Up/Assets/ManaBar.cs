using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    private void OnEnable()
    {
        PlayerSpells.OnUpdatePlayerMana += UpdateMana;
    }
    void UpdateMana(float mana, float max)
    {
        _slider.maxValue = max;
        _slider.value = mana;
    }
}
