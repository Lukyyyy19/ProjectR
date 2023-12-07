using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[Serializable]
public class PlayerInput
{
    private PlayerInputs _inputs;
    private Player _player;
    [SerializeField] private bool _attackButton;
    [Header("Input Buffering")] public Queue<InputActions> attackInputQueue = new Queue<InputActions>();
    public Queue<Inputs> InputsQueue = new Queue<Inputs>();
    private float _bufferingTime = 1f;
    [SerializeField] private float _currentBufferingTime;
    private int _maxInputBufferPermited = 2;
    private float _inputBufferingTime = 1;
    [SerializeField] private float _deltaInputBuferring;
    private bool _startInputBufferTime;
    [SerializeField] private InputActions[] _debugAttackQueue;
    [SerializeField] private Inputs[] _debugInputQueue;
    public bool attackButton => _attackButton;
    public bool jumpButton { get; private set; }
    [SerializeField]private CombosComparer _combosComparer;

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
        _combosComparer = new CombosComparer();
    }

    private bool _startBufferTimer;
    private bool xx = true;
    private bool yy = true;

    public void OnUpdate()
    {
        _startBufferTimer = InputsQueue.Count > 0;
        if (_startBufferTimer)
            _deltaInputBuferring -= Time.deltaTime;
        if (_deltaInputBuferring <= 0) InputAutomaticDequeue();
        _debugAttackQueue = attackInputQueue.ToArray();
        _debugInputQueue = InputsQueue.ToArray();
        CheckAttackInput();
        MovementInputQueue();
        var dir = attackInputQueue.Count > 0 ? Vector2.zero : _inputs.Player.Movement.ReadValue<Vector2>();
        _player.SetDirection(dir.x);
        jumpButton = _inputs.Player.Jump.WasPressedThisFrame();
        CheckJumpButton();
        CheckEastButton();
        CheckNorthButton();
    }

    private void CheckJumpButton()
    {
        if (jumpButton)
        {
            InputsQueue.Enqueue(Inputs.South);
            if (_combosComparer.IsComboCreated(InputsQueue.ToArray()))
            {
                InputsQueue.Clear();
            }
        }
    }

    private void MovementInputQueue()
    {
        var x = _inputs.Player.Movement.ReadValue<Vector2>();
        bool directionReleased = x.x == 0;
        bool yReleased = x.y == 0;
        if (directionReleased) xx = true;
        if (yReleased) yy = true;
        if (x.x < -0.1f && xx)
        {
            InputsQueue.Enqueue(Inputs.Left);
            if (_combosComparer.IsComboCreated(InputsQueue.ToArray()))
            {
                InputsQueue.Clear();
            }

            xx = false;
        }
        else if (x.x > 0.1f & xx)
        {
            InputsQueue.Enqueue(Inputs.Right);
            if (_combosComparer.IsComboCreated(InputsQueue.ToArray()))
            {
                InputsQueue.Clear();
            }

            xx = false;
        }

        else switch (x.y)
        {
            case < -0.5f when yy:
            {
                InputsQueue.Enqueue(Inputs.Down);
                if (_combosComparer.IsComboCreated(InputsQueue.ToArray()))
                {
                    InputsQueue.Clear();
                }

                yy = false;
                break;
            }
            case > 0.5f when yy:
            {
                InputsQueue.Enqueue(Inputs.Up);
                if (_combosComparer.IsComboCreated(InputsQueue.ToArray()))
                {
                    InputsQueue.Clear();
                }

                yy = false;
                break;
            }
        }
    }

    private void CheckEastButton()
    {
        if (_inputs.Player.East.WasPerformedThisFrame())
        {
            InputsQueue.Enqueue(Inputs.East);
            if (_combosComparer.IsComboCreated(InputsQueue.ToArray()))
            {
                InputsQueue.Clear();
            }
        }
    }

    private void CheckNorthButton()
    {
        if (_inputs.Player.North.WasPerformedThisFrame())
        {
            InputsQueue.Enqueue(Inputs.North);
            if (_combosComparer.IsComboCreated(InputsQueue.ToArray()))
            {
                InputsQueue.Clear();
            }
        }
    }

    private void CheckAttackInput()
    {
        if (_startBufferTimer)
        {
            _currentBufferingTime -= Time.deltaTime;
            if (_currentBufferingTime <= 0)
            {
                AttackAutomaticDequeue();
                _startBufferTimer = false;
            }
        }

        _attackButton = _inputs.Player.Attack.WasPressedThisFrame();
        if (_attackButton) InputsQueue.Enqueue(Inputs.West);
        if (_attackButton && attackInputQueue.Count < _maxInputBufferPermited)
        {
            _startBufferTimer = true;
            _currentBufferingTime = _bufferingTime;
            attackInputQueue.Enqueue(InputActions.Attack);
            if (_combosComparer.IsComboCreated(_debugInputQueue))
            {
                InputsQueue.Clear();
            }
        }
    }

    void AttackAutomaticDequeue()
    {
        if (attackInputQueue.Count > 0) attackInputQueue.Dequeue();
    }

    void InputAutomaticDequeue()
    {
        if (InputsQueue.Count > 0) InputsQueue.Dequeue();
        _deltaInputBuferring = _inputBufferingTime;
    }

    public void Movement(InputAction.CallbackContext ctx)
    {
        var dir = ctx.ReadValue<Vector2>();
        _player.SetDirection(dir.x);
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        jumpButton = ctx.ReadValueAsButton();
        InputsQueue.Enqueue(Inputs.South);
    }

    public void Attack(InputAction.CallbackContext ctx)
    {
        _attackButton = ctx.action.IsPressed();
        Debug.Log(ctx.action.WasPressedThisFrame());
    }
}