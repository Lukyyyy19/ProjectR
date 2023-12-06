using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSword : Weapon
{
    protected override void Awake()
    {
        cooldownTime = 0.35f;
        base.Awake();
    }

    public override void Attack(Hit swordHit)
    {
        List<IDamageable> damageableEntities = GetDamageableEntities();
        if (damageableEntities.Count < 1)
        {
            return;
        }
        AttackDamageableEntities(swordHit);
    }
}
