using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellController : MonoBehaviour
{
    public SpellConfig spellConfig;
    public SpellEffect elementalEffect;
    public SpellEffect projectileEffect;
    public SpellEffect impactEffect;
    public bool isCasting;
    public bool isDecorated;
    public ISpell spell;
    public bool ownedByPlayer;
    void Start()
    {
        spell = new Spell(spellConfig);
    }
    public void Cast (Transform castLocationTransform)
    {
        if (isCasting || !isDecorated) return;
        SpellProjectile spellProjectile = Instantiate(spellConfig.spellProjPrefab).GetComponent<SpellProjectile>();
        spellProjectile.transform.position = castLocationTransform.position;
        spellProjectile.transform.rotation = castLocationTransform.rotation;
        spellProjectile.Initialize(this);
        StartCoroutine(CastSpell());
    }

    IEnumerator CastSpell()
    {
        isCasting = true;
        yield return new WaitForSeconds(spell.Cooldown);
        isCasting = false;
    }
    public void Reset()
    {
        spell = new Spell(spellConfig);
        isDecorated = false;
    }
    public void Decorate()
    {
        if (elementalEffect && !projectileEffect && !impactEffect)
            spell = new SpellDecorator(spell, elementalEffect);
        if (elementalEffect && projectileEffect && !impactEffect)
            spell = new SpellDecorator(new SpellDecorator(spell, elementalEffect), projectileEffect);
        if (elementalEffect && !projectileEffect && impactEffect)
            spell = new SpellDecorator(new SpellDecorator(spell, elementalEffect), impactEffect);
        if (elementalEffect && projectileEffect && impactEffect)
            spell = new SpellDecorator(new SpellDecorator(new SpellDecorator(spell, elementalEffect), projectileEffect), impactEffect);
        isDecorated = true;
    }
}
