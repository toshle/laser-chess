using UnityEngine;

public class Damage : EffectBase
{
    [SerializeField] int _damage;

    public override void Activate(Unit triggerUnit)
    {
        triggerUnit.Combat.TakeDamage(_damage);
    }
}
