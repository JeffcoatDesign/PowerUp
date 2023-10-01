using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ManaUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI manaText;
    private void OnEnable()
    {
        PlayerSpells.OnUpdatePlayerMana += UpdateMana;
    }
    private void OnDisable()
    {
        PlayerSpells.OnUpdatePlayerMana -= UpdateMana;
    }
    private void UpdateMana(float current, float max)
    {
        float quotient = current / max;
        manaText.text = quotient.ToString("");
    }
}
