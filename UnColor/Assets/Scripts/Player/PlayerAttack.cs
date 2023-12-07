using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Player _player;
    private PlayerInput _playerInput;
    private Weapon _weapon;
    [SerializeField] private bool canAttack;
    [SerializeField] private float _deltaCanAttackThreshold;

    private float _timeAttack = 0.2f;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _weapon = GetComponent<Weapon>();
    }

    private void Start()
    {
        _playerInput = _player.PlayerInputs;
    }

    private void Update()
    {
        _deltaCanAttackThreshold -= Time.deltaTime;
        canAttack = _playerInput.attackInputQueue.Count > 0 &&
                    _playerInput.attackInputQueue.Peek() == InputActions.Attack &&
                    _deltaCanAttackThreshold <= 0;
        //if (_deltaCanAttackThreshold > 0) _player.IsAttacking = false;
        _player.IsAttacking = canAttack;
        _player.atacktest = _deltaCanAttackThreshold > 0;
        if (canAttack)
        {
            _playerInput.attackInputQueue.Dequeue();
            Attack(_weapon);
        }
    }

    private void Attack(Weapon weapon)
    {
        Debug.Log("Enterig attack method");
        _deltaCanAttackThreshold = weapon.cooldownTime;
        Hit hit = default(Hit);
        hit.attacker = gameObject;
        hit.damageAmount = weapon.damageAmount;
        weapon.Attack(hit);
    }
}