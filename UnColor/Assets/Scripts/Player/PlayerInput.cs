using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;
[Serializable]
public class PlayerInput
{
    private PlayerInputs _inputs;
    private Player _player;
    [SerializeField] private bool _attackButton;
    [Header("Input Buffering")]
    public Queue<InputActions> inputsQueue = new Queue<InputActions>();
    private float _bufferingTime = 1f;
    [SerializeField]private float _currentBufferingTime;
    private int _maxInputBufferPermited = 2;
    [SerializeField] private InputActions[] _debugQueue;
    public bool attackButton => _attackButton;
    public bool jumpButton { get; private set; }
    public PlayerInput(Player player)
    {
        _player = player;
        Init();
    }

    void Init()
    {
        _inputs = new PlayerInputs();
        _inputs.Enable();
        _currentBufferingTime = _bufferingTime;
    }

    private bool _startBufferTimer;
    public void OnUpdate()
    {
        CheckAttackInput();
        var dir =inputsQueue.Count>0? Vector2.zero: _inputs.Player.Movement.ReadValue<Vector2>();
        _player.SetDirection(dir.x);
        jumpButton = _inputs.Player.Jump.WasPressedThisFrame();
        _debugQueue = inputsQueue.ToArray();
    }

    private void CheckAttackInput()
    {
        if (_startBufferTimer)
        {
            _currentBufferingTime -= Time.deltaTime;
            if (_currentBufferingTime <= 0)
            {
                AutomaticDequeue();
                _startBufferTimer = false;
            }
        }

        _attackButton = _inputs.Player.Attack.WasPressedThisFrame();
        if (_attackButton && inputsQueue.Count < _maxInputBufferPermited)
        {
            _startBufferTimer = true;
            _currentBufferingTime = _bufferingTime;
            inputsQueue.Enqueue(InputActions.Attack);
        }
    }

    void AutomaticDequeue()
    {
        if(inputsQueue.Count>0) inputsQueue.Dequeue();
    }
    public void Movement(InputAction.CallbackContext ctx)
    {
        var dir = ctx.ReadValue<Vector2>();
        _player.SetDirection(dir.x);
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
       jumpButton = ctx.ReadValueAsButton();
    }

    public void Attack(InputAction.CallbackContext ctx)
    {
        _attackButton = ctx.action.IsPressed();
        Debug.Log(ctx.action.WasPressedThisFrame());
    }

}
