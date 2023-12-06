using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : Entity
{
    private int hitCount;
    protected override void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        base.Awake();
    }

    protected override void AddNewEvent()
    {
        throw new System.NotImplementedException();
    }

    protected override void ConfigureEvents()
    {
        throw new System.NotImplementedException();
    }

    public override void TakeDamage(Hit hit)
    {
        hitCount++;
        StartCoroutine(nameof(DamagedCorutine));
    }

    IEnumerator DamagedCorutine()
    {
        for (int i = 0; i < 3; i++)
        {
            _spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.03f);
            _spriteRenderer.color = Color.red;
        }
    }
}