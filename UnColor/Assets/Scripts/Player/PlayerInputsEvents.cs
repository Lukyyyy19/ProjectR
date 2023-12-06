using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInputsEvents
{
    private PlayerInputs _playerInputs;
    private PlayerInput _playerInput;
    private CharacterPlatformController _characterPlatformController;
    private Player _player;
    public PlayerInputsEvents(PlayerInput playerInput)
    {
        _playerInput = playerInput;
        Awake();
    }
    private void Awake()
    {
        _playerInputs = new PlayerInputs();
        _playerInputs.Enable();
        _playerInputs.Player.Movement.performed += _playerInput.Movement;
        _playerInputs.Player.Movement.canceled += _playerInput.Movement;
        _playerInputs.Player.Jump.started += _playerInput.Jump;
        _playerInputs.Player.Jump.canceled += _playerInput.Jump;
        _playerInputs.Player.Attack.started += _playerInput.Attack;
       // _playerInputs.Player.Attack.performed += _playerInput.Attack;
        _playerInputs.Player.Attack.canceled += _playerInput.Attack;
    }
    
}
