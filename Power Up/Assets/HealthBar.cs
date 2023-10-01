using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Entity entity;
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI text;
    private void OnEnable()
    {
        if (text) text.text = entity.name;
        _slider.maxValue = entity.MaxHitpoints;
    }
    void Update()
    {
        _slider.value = entity.CurrentHP;
    }
}
