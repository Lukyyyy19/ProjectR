using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    private Animator _playerAnimator;
    private Player _player;
    private PlayerInput _playerInput;

    private bool _isGrounded;
    private float _yVelocity;
    private float _xVelocity;
    private bool _isAttacking;
    private bool _canAttack;
    private float _deltaTimeAttack;
    
    private readonly int MovementSpeed = Animator.StringToHash("MovementSpeed");
    private readonly int Attack = Animator.StringToHash("Attack");
    private readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    private readonly int Yvelocity = Animator.StringToHash("Yvelocity");
    private readonly int CanAttack = Animator.StringToHash("CanAttack");

    public float AnimationTime(int layer) => _playerAnimator.GetCurrentAnimatorStateInfo(layer).normalizedTime;

    public bool AnimationPlaying(string name)
    {
       return _playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(name);
    }
    private void Awake()
    {
        _playerAnimator = GetComponentInChildren<Animator>();
        _player = GetComponent<Player>();
        _playerInput = _player.PlayerInputs;
    }

    private void Update()
    {
        _deltaTimeAttack += Time.deltaTime;
        _isAttacking = _player.IsAttacking;
        _isGrounded = _player.IsGrounded;
        _yVelocity = _player.Yvelocity;
        _xVelocity = Mathf.Clamp(Mathf.Abs(_player.Xvelocity),0,1);
        _canAttack = _player.CanAttackAgain;
        UpdateParameters();
    }

    private void UpdateParameters()
    {
        _playerAnimator.SetBool(IsGrounded, _isGrounded);
        _playerAnimator.SetBool(CanAttack, _canAttack);
        if (!_isGrounded)
        {
            _playerAnimator.SetFloat(Yvelocity,_yVelocity);
            GroundAttack();
        }
        else
        {
            GroundAttack();
            _yVelocity = 0;
            _playerAnimator.SetFloat(MovementSpeed,_xVelocity);
            
        }
    }

    private void GroundAttack()
    {
        if (_player.IsAttacking)
        {
            _deltaTimeAttack = 0;
            _playerAnimator.SetTrigger(Attack);
        }
        else if (_deltaTimeAttack >= 0.01f)
        {
            _playerAnimator.ResetTrigger(Attack);
        }
    }

    public bool HasAnimationEnd(int layer,string name)
    {
        return AnimationTime(layer) > .9f && !_playerAnimator.IsInTransition(layer) && !AnimationPlaying(name);
    }
}
