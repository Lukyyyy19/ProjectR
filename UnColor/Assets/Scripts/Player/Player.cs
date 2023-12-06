using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    [SerializeField]private PlayerInput _playerInput;
    private Weapon _playerWeapon;
    private AnimatorController _animatorController;
    public bool IsGrounded => _isGrounded;
    public bool IsMoving => _isMoving;
    public bool IsAttacking
    {
        get { return _isAttacking; }
        set { _isAttacking = value; }
    }

    [SerializeField]public bool atacktest;
    public bool CanAttackAgain => _canAttackAgain;
    public float Yvelocity => _rb.velocity.y;
    public float Xvelocity => _dir.x;
    public PlayerInput PlayerInputs => _playerInput;
    protected override void Awake()
    {
        _playerWeapon = GetComponent<Weapon>();
        _animatorController = GetComponent<AnimatorController>();
        _canAttackAgainTime = _playerWeapon.cooldownTime;
        base.Awake();
        _playerInput = new PlayerInput(this);
    }

    protected override void Update()
    {
        base.Update();
        _playerInput.OnUpdate();
        _isMoving = _dir.x > 0.1f || _dir.x < -0.1;
        _isFalling = _rb.velocity.y < 0;
        CheckJump(_playerInput.jumpButton);
        //Debug.Log(_statesQueue.Count);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    protected override void AddNewEvent()
    {
    }

    protected override void ConfigureEvents()
    {
        //idle.OnEnter  += x => { _isAttacking = false; };
        idle.OnUpdate += () =>
        {
            if (_isMoving && _isGrounded) SendInputToFSM(States.Walk);
            else if(_isFalling && !_isGrounded)SendInputToFSM(States.Fall);
        };
        walk.OnUpdate += () =>
        {
            if (!_isMoving && _isGrounded) SendInputToFSM(States.Idle);
            else if(_isFalling && !_isGrounded)SendInputToFSM(States.Fall);
        };
        walk.OnFixedUpdate += Move;

        jump.OnEnter += x=> Jump();
        jump.OnUpdate += () =>
        {
            
            if (_rb.velocity.y < 0)
            {
                SendInputToFSM(States.Fall);
            }
        };
        jump.OnFixedUpdate += Move;

        fall.OnUpdate += () =>
        {
            if (_isGrounded)
            {
                SendInputToFSM(States.Idle);
            }

            //if (_isMoving && _isGrounded) SendInputToFSM(States.Walk);
        };
        fall.OnFixedUpdate += Move;

    }

    #region Actions

    private void Move()
    {
        if (atacktest)
        {
            _dir.x = 0;
        }
        _dir.y = _rb.velocity.y;
        _rb.velocity = _dir;
        if (_dir.x > 0.1f && !_facingRight)
        {
            Flip();
        }
        else if (_dir.x < -0.1f && _facingRight)
        {
            Flip();
        }
    }

    private void Jump()
    {
        _rb.AddForce(Vector2.up*_jumpForce,ForceMode2D.Impulse);
    }

   

    #endregion
    

    #region Inputs

    public void SetDirection(float x)
    {
        _dir.x = Mathf.Clamp(x*_speed,-_speed,_speed) ;
    }

    public void CheckJump(bool jump)
    {
        // _statesQueue.Enqueue(States.Jump);
        if (jump && _isGrounded)
        {
            SendInputToFSM(States.Jump);
        }
    }

    #endregion

    public override void TakeDamage(Hit hit)
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position,transform.up*-1*_groundRadius);
    }
}