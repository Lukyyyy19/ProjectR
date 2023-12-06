using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPlatformController : MonoBehaviour
{
    private PlatformCharacterPhysics m_platformPhysics = new PlatformCharacterPhysics();

    private bool _isGrounded;

    [Header("Jump")]
    private float _jumpingSpeed = 2f;
    private float _ghostJump;
    private float _ghostJumpDelay;
    [Header("Moving")] 
    private float _horizontalSpeed = 5f;

    private Vector2 _dir;
    
    
    private ControllerStates _currentState;
    private ControllerStates _prevState;
    private Vector3 m_instantVelocity;
    private Vector3 m_prevPos;


    private bool CanGhostJump => _ghostJump < _ghostJumpDelay;
    public bool IsGrounded => _isGrounded;
    void Update()
    {
        if (IsGrounded)
        {
            _ghostJump = 0f;
        }
        else
        {
            _ghostJump += Time.deltaTime;
        }

        if (CheckControllerState(ControllerStates.Jump) && (IsGrounded || CanGhostJump))
        {
            _ghostJump = _ghostJumpDelay;
            m_platformPhysics.VSpeed = _jumpingSpeed;
            _isGrounded = false;
        }
        m_platformPhysics.AddAcceleration(Vector2.right * (_horizontalSpeed*_dir.x));
        m_instantVelocity = (base.transform.position - m_prevPos) / Time.deltaTime;
        m_prevPos = base.transform.position;
        m_platformPhysics.Position = base.transform.position;
        m_platformPhysics.UpdatePhysics(Time.deltaTime);
       // m_groundDist = _CalculateGroundDist();
       // bool isGrounded = m_isGrounded;
       // m_isGrounded = m_smartCollider.enabled && m_smartCollider.IsGrounded() && m_platformPhysics.DeltaDisp.y <= 0f;
        base.transform.position = base.transform.position + base.transform.rotation * (m_platformPhysics.Position - base.transform.position);
    }

    public void UpdateDirection(Vector2 dir)
    {
        _dir = dir;
    }
    public bool CheckControllerState(ControllerStates state)
    {
        return(_prevState & state)!=0;
    }
}
