using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    protected Hit weaponHit;
    protected Entity weaponOwner;
    
    [SerializeField]protected Collider2D weaponCollider;
    protected ContactFilter2D _contactFilter;
    [SerializeField]protected Collider2D[] hits;
    protected List<IDamageable> damageableEntities;

    [SerializeField] protected LayerMask enemyLayerMask;
    public int damageAmount { get; protected set; }
    public float cooldownTime { get; protected set; }
    private void SetContactFilter()
    {
        _contactFilter = new ContactFilter2D
        {
            layerMask = enemyLayerMask,
            useLayerMask = true,
            useTriggers = true
        };
    }

    protected virtual void Awake()
    {
        damageableEntities = new List<IDamageable>();
        SetContactFilter();
        damageAmount = 1;
        hits = new Collider2D[10];
    }

    public abstract void Attack(Hit hit);
    
    public GameObject[] OverlappedEntities()
    {
        int num = weaponCollider.OverlapCollider(_contactFilter, hits);
        GameObject[] array = new GameObject[num];
        for (int i = 0; i < num; i++)
        {
            GameObject value = hits[i].gameObject;
            array.SetValue(value, i);
        }
        return array;
    }
    protected List<IDamageable> GetDamageableEntities()
    {
        GameObject[] array = OverlappedEntities();
        for (int j = 0; j < array.Length; j++)
            {
                IDamageable componentInParent = array[j].GetComponentInParent<IDamageable>();
                if (componentInParent != null)
                {
                    damageableEntities.Add(componentInParent);
                }
            }
        
        return damageableEntities;
    }
    protected void AttackDamageableEntities(Hit weaponHit)
    {
        if (damageableEntities != null && damageableEntities.Count > 0)
        {
            //OnHit(weaponHit);
            for (int i = 0; i < damageableEntities.Count; i++)
            {
                damageableEntities[i].TakeDamage(weaponHit);
            }
            damageableEntities.Clear();
        }
    }

    public void ClearDamageableEntities()
    {
        if (damageableEntities.Count > 0)
        {
            damageableEntities.Clear();
        }
    }
}
