using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellProjectile : MonoBehaviour
{
    public float damage;
    public float speed;
    public DamageType[] damageTypes;
    [SerializeField] private Rigidbody _rb;
    private float startTime;
    private SpellController _spellController;
    private void Start()
    {
        startTime = Time.time;
    }

    public void Initialize(SpellController spell)
    {
        _spellController = spell;
        damage = _spellController.spell.Damage;
        speed = _spellController.spell.Speed;
        damageTypes = _spellController.spell.DamageTypes;
        if (_spellController.impactEffect != null && _spellController.impactEffect.spellEffectPrefab != null) Instantiate(_spellController.impactEffect.spellEffectPrefab, transform);
        if (_spellController.projectileEffect != null && _spellController.projectileEffect.spellEffectPrefab != null) Instantiate(_spellController.projectileEffect.spellEffectPrefab, transform);
        if (_spellController.elementalEffect != null && _spellController.elementalEffect.spellEffectPrefab != null) Instantiate(_spellController.elementalEffect.spellEffectPrefab, transform);
        GetComponent<MeshRenderer>().material = _spellController.elementalEffect.projectileMat;
    }
    private void Update()
    {
        _rb.AddForce(transform.forward * speed * Time.deltaTime);
        if (Time.time - startTime > 10f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile")) return;
        if (other.CompareTag("Enemy") && _spellController.ownedByPlayer)
        {
            other.GetComponent<EnemyEntity>().GetHit(damage, damageTypes);
            Destroy(gameObject);
        }
        if (other.CompareTag("Player") && !_spellController.ownedByPlayer) {
            other.GetComponent<PlayerEntity>().GetHit(damage, damageTypes);
            Destroy(gameObject);
        }
        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
