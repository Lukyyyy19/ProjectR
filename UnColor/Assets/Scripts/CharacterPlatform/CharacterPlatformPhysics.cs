using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlatformCharacterPhysics
{
	public const float k_MaxTimeToSolveTimeToReachSpeed = 5f;

	[SerializeField]
	private Vector3 m_pos;

	[SerializeField]
	private Vector3 m_vel;

	[SerializeField]
	private Vector3 m_acc;

	[SerializeField]
	private Vector3 m_gravity = 9.8f * Vector3.down;

	[SerializeField]
	private float m_gravityScale = 1f;

	[SerializeField]
	private float m_terminalVel;

	[SerializeField]
	private float m_maxHSpeed;

	[SerializeField]
	private Vector3 m_drag;

	private Vector3 m_prevPos;

	public Vector3 Position
	{
		get
		{
			return m_pos;
		}
		set
		{
			m_pos = value;
		}
	}

	public Vector3 Velocity
	{
		get
		{
			return m_vel;
		}
		set
		{
			m_vel = value;
		}
	}

	public Vector3 Acceleration
	{
		get
		{
			return m_acc;
		}
		set
		{
			m_acc = value;
		}
	}

	public Vector3 Gravity
	{
		get
		{
			return m_gravity;
		}
		set
		{
			m_gravity = value;
		}
	}

	public float GravityScale
	{
		get
		{
			return m_gravityScale;
		}
		set
		{
			m_gravityScale = Mathf.Max(0f, value);
		}
	}

	public float TerminalVelocity
	{
		get
		{
			return m_terminalVel;
		}
		set
		{
			m_terminalVel = value;
		}
	}

	public float MaxHSpeed
	{
		get
		{
			return m_maxHSpeed;
		}
		set
		{
			m_maxHSpeed = value;
		}
	}

	public Vector3 Drag
	{
		get
		{
			return m_drag;
		}
		set
		{
			m_drag = value;
		}
	}

	public Vector3 DeltaDisp => m_pos - m_prevPos;

	public float HSpeed
	{
		get
		{
			return m_vel.x;
		}
		set
		{
			m_vel.x = value;
		}
	}

	public float VSpeed
	{
		get
		{
			return m_vel.y;
		}
		set
		{
			m_vel.y = value;
		}
	}

	public float HDrag
	{
		get
		{
			return m_drag.x;
		}
		set
		{
			m_drag.x = value;
		}
	}

	public PlatformCharacterPhysics()
	{
	}

	public PlatformCharacterPhysics(PlatformCharacterPhysics other)
	{
		m_pos = other.m_pos;
		m_vel = other.m_vel;
		m_acc = other.m_acc;
		m_gravity = other.m_gravity;
		m_gravityScale = other.m_gravityScale;
		m_terminalVel = other.m_terminalVel;
		m_maxHSpeed = other.m_maxHSpeed;
		m_drag = other.m_drag;
	}

	public void AddAcceleration(Vector3 acc)
	{
		m_acc += acc;
	}

	public void UpdatePhysics(float timeDt)
	{
		m_acc += m_gravity * m_gravityScale;
		if (m_terminalVel > 0f && m_vel.y < 0f - m_terminalVel)
		{
			m_vel.y = 0f - m_terminalVel;
		}
		if (m_maxHSpeed >= 0f)
		{
			m_vel.x = Mathf.Clamp(m_vel.x, 0f - m_maxHSpeed, m_maxHSpeed);
		}
		m_prevPos = m_pos;
		Vector3 vector = m_vel * timeDt + 0.5f * m_acc * timeDt * timeDt;
		float num = m_maxHSpeed * timeDt;
		vector.x = Mathf.Clamp(vector.x, 0f - num, num);
		m_pos += vector;
		m_vel += m_acc * timeDt;
		if (m_terminalVel > 0f && m_vel.y < 0f - m_terminalVel)
		{
			m_vel.y = 0f - m_terminalVel;
		}
		if (m_maxHSpeed >= 0f)
		{
			m_vel.x = Mathf.Clamp(m_vel.x, 0f - m_maxHSpeed, m_maxHSpeed);
		}
		m_vel.x *= Mathf.Clamp01(1f - m_drag.x * timeDt);
		m_vel.y *= Mathf.Clamp01(1f - m_drag.y * timeDt);
		m_acc = Vector3.zero;
	}

	public float SolveTimeToPosY(float posY)
	{
		float num = m_gravity.y * m_gravityScale;
		float num2 = Mathf.Sqrt(m_vel.y * m_vel.y - 2f * num * (m_pos.y - posY));
		return (0f - m_vel.y - num2) / num;
	}

	public float SolveMaxJumpHeight(float jumpSpeed, float jumpAcc = 0f, float jumpAccTime = 0f)
	{
		float num = m_gravity.y * m_gravityScale;
		if (jumpAccTime > 0f)
		{
			float num2 = jumpAccTime % Time.fixedDeltaTime;
			if (num2 > 1E-05f)
			{
				jumpAccTime += Time.fixedDeltaTime;
				jumpAccTime -= num2;
			}
		}
		float num3 = jumpAcc + num;
		float num4 = (0f - jumpSpeed) / num3;
		if (num4 <= jumpAccTime)
		{
			return jumpSpeed * num4 + 0.5f * num3 * num4 * num4;
		}
		float num5 = jumpSpeed * jumpAccTime + 0.5f * num3 * jumpAccTime * jumpAccTime;
		float num6 = jumpSpeed + num3 * jumpAccTime;
		num4 = (0f - num6) / num;
		return num5 + (num6 * num4 + 0.5f * num * num4 * num4);
	}

	public float SolveJumpSpeedToReachHeight(float height)
	{
		float num = m_gravity.y * m_gravityScale;
		return Mathf.Sqrt(-2f * num * height);
	}

	public float SolveJumpAccToReachHeight(float height, float jumpSpeed, float jumpAccTime)
	{
		float num = m_gravity.y * m_gravityScale;
		if (jumpAccTime > 0f)
		{
			float num2 = jumpAccTime % Time.fixedDeltaTime;
			if (num2 > 1E-05f)
			{
				jumpAccTime += Time.fixedDeltaTime;
				jumpAccTime -= num2;
			}
		}
		float num3 = jumpAccTime;
		float num4 = (0f - num3) * num3;
		float num5 = -2f * jumpSpeed * num3 + num * num3 * num3;
		float num6 = 2f * num * (jumpSpeed * num3 - height) - jumpSpeed * jumpSpeed;
		float num7 = Mathf.Sqrt(num5 * num5 - 4f * num4 * num6);
		float num8 = (0f - num5 - num7) / (2f * num4);
		float num9 = (0f - (jumpSpeed + num8 * num3)) / num;
		if (num9 <= 0f)
		{
			num8 = (0f - jumpSpeed * jumpSpeed) / (2f * height);
		}
		return num8 - num;
	}

	public float SolveMaxSpeedWithAccAndDrag(float acc, float drag)
	{
		return (1f - drag * Time.fixedDeltaTime) * acc * Time.fixedDeltaTime / (drag * Time.fixedDeltaTime);
	}

	public float SolveMapHorizontalSpeedWithAcc(float acc)
	{
		return SolveMaxSpeedWithAccAndDrag(acc, m_drag.x);
	}

	public float SolveTimeToReachSpeed(float speed, float acc, float drag, float precision = 0.01f)
	{
		float num = drag * Time.fixedDeltaTime;
		if (num == 1f)
		{
			return 0f;
		}
		speed = Mathf.Abs(speed);
		acc = Mathf.Abs(acc);
		float num2 = SolveMaxSpeedWithAccAndDrag(acc, drag);
		if (speed > num2)
		{
			return float.PositiveInfinity;
		}
		float num3 = 0f;
		float num4 = 0f;
		while (speed - num4 > precision)
		{
			num3 += Time.fixedDeltaTime;
			if (num3 >= 5f)
			{
				return 5f;
			}
			num4 += acc * Time.fixedDeltaTime;
			num4 *= 1f - drag * Time.fixedDeltaTime;
		}
		return num3;
	}

	public void ResetPreviousPosition(Vector3 newPosition)
	{
		Position = newPosition;
		m_prevPos = Position;
	}
}
