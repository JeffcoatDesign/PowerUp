using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : Entity
{
    public delegate void PlayerHPChange(float currentHP);
    public delegate void PlayerDie();
    public static event PlayerHPChange OnPlayerHPChange;
    public static event PlayerDie OnPlayerDeath;
    private void Start()
    {
        OnPlayerHPChange?.Invoke(CurrentHP);
    }
    public override void GetHit(float amount, DamageType[] damageTypes)
    {
        base.GetHit(amount, damageTypes);
        OnPlayerHPChange?.Invoke(CurrentHP);
        if (CurrentHP <= 0) Die();
    }
    public override void Die()
    {
        OnPlayerDeath?.Invoke();
    }
}
