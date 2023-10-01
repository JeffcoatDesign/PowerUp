using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEntity : Entity
{
    public delegate void EnemyDeath();
    public delegate void ThisDeath();
    public static event EnemyDeath OnEnemyDeath;
    public event ThisDeath OnDeath;
    public override void GetHit(float amount, DamageType[] damageTypes)
    {
        base.GetHit(amount, damageTypes);
    }
    public override void Die()
    {
        OnEnemyDeath?.Invoke();
        OnDeath?.Invoke();
        base.Die();
    }
}
