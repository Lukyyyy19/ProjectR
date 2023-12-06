using System;
using System.Collections;
using System.Collections.Generic;
using IA2;
using UnityEngine;

public abstract class Entity : MonoBehaviour,IDamageable
{
    protected enum States
    {
        None,
        Walk,
        Idle,
        Attack,
        Hurt,
        Jump,
        Fall
    }

    private EventFSM<States> _fsm;
    [Header("Components")]
    [SerializeField]protected Rigidbody2D _rb;
    protected SpriteRenderer _spriteRenderer;
    
    [Header("Movement Vars")]
    [SerializeField, Range(0, 10)] protected float _speed;
    [SerializeField]protected bool _isGrounded;
    [SerializeField]protected bool _isMoving;
    [SerializeField] protected Vector2 _dir;
    protected bool _facingRight = true;
    
    [Header("In Air")] 
    [SerializeField] protected bool _isFalling;
    [SerializeField] protected float _jumpForce;
        
    [Header("Attacking")] 
    [SerializeField]protected bool _isAttacking;
    [SerializeField] protected float _canAttackAgainTime;
    [SerializeField] protected bool _canAttackAgain = true;
    
    [Header("Checkers Vars")] 
    [SerializeField, Range(0, 10)] protected float _groundRadius;
    [SerializeField] private LayerMask _groundLayer;

    protected State<States> idle;
    protected State<States> walk;
    protected State<States> attack;
    protected State<States> hurt;
    protected State<States> jump;
    protected State<States> fall;
    [Header("States")] 
    [SerializeField] protected States _currentState;
    protected virtual void Awake()
    {
        idle = new State<States>(States.Idle);
        walk = new State<States>(States.Walk);
        attack = new State<States>(States.Attack);
        hurt = new State<States>(States.Hurt);
        jump = new State<States>(States.Jump);
        fall = new State<States>(States.Fall);
        StateConfigurer.Create(idle)
            .SetTransition(States.Walk, walk)
            .SetTransition(States.Attack,attack)
            .SetTransition(States.Hurt,hurt)
            .SetTransition(States.Jump,jump)
            .SetTransition(States.Fall,fall)
            .Done();
        StateConfigurer.Create(walk)
            .SetTransition(States.Idle, idle)
            .SetTransition(States.Attack,attack)
            .SetTransition(States.Hurt,hurt)
            .SetTransition(States.Jump,jump)
            .SetTransition(States.Fall,fall)
            .Done();
        StateConfigurer.Create(attack)
            .SetTransition(States.Idle,idle)
            .SetTransition(States.Fall,fall)
            .SetTransition(States.Attack,attack)
            .Done();
        StateConfigurer.Create(hurt)
            .SetTransition(States.Idle,idle)
            .Done();
        StateConfigurer.Create(jump)
            .SetTransition(States.Fall,fall)
            .SetTransition(States.Attack,attack)
            .Done();
        StateConfigurer.Create(fall)
            .SetTransition(States.Idle,idle)
            .SetTransition(States.Attack,attack)
            .SetTransition(States.Walk,walk)
            .Done();
        AddNewEvent();
        ConfigureEvents();
        _fsm = new EventFSM<States>(idle);
        _currentState = _fsm.Current.Name;
    }
    
    protected virtual void Update()
    {
        _fsm.Update();
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, _groundRadius,_groundLayer);
    }

    protected virtual void FixedUpdate()
    {
        _fsm.FixedUpdate();
    }

    protected virtual void LateUpdate()
    {
        _fsm.LateUpdate();
    }

    protected abstract void AddNewEvent();

    protected abstract void ConfigureEvents();
    
 protected void SendInputToFSM(States inp)
    {
        _fsm.SendInput(inp);
        _currentState = _fsm.Current.Name;
    }

    public abstract void TakeDamage(Hit hit);
    protected void Flip()
    {
        // Switch the way the player is labelled as facing
        _facingRight = !_facingRight;

        // Multiply the player's x local scale by -1
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
