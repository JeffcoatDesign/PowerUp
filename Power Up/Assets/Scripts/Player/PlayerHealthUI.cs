using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    private void OnEnable()
    {
        PlayerEntity.OnPlayerHPChange += UpdateHealth;
    }
    private void OnDisable()
    {
        PlayerEntity.OnPlayerHPChange -= UpdateHealth;
    }
    private void UpdateHealth(float current)
    {
        healthText.text = current.ToString("");
    }
}
