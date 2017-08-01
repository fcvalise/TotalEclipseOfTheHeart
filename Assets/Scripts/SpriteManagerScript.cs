using UnityEngine;
using System.Collections;

public class SpriteManagerScript : MonoBehaviour {

	public GameObject			m_player;
	public GameObject			m_frontSprite;
	public GameObject			m_backSprite;
	public GameObject			m_rightSprite;
	public GameObject			m_leftSprite;

	private PlayerScript		m_playerScript;
	private SpriteScript		m_frontScript;
	private SpriteScript		m_backScript;
	private SpriteScript		m_rightScript;
	private SpriteScript		m_leftScript;

	private PlayerScript.State	m_lastState;

	void Start()
	{
		m_playerScript = m_player.GetComponent<PlayerScript>();
		m_frontScript = m_frontSprite.GetComponent<SpriteScript>();
		m_backScript = m_backSprite.GetComponent<SpriteScript>();
		m_rightScript = m_rightSprite.GetComponent<SpriteScript>();
		m_leftScript = m_leftSprite.GetComponent<SpriteScript>();

		m_lastState = m_playerScript.m_state;

		m_frontScript.PlayNext(SpriteScript.Sprite.FrontIdle, false);
		m_backScript.PlayNext(SpriteScript.Sprite.SideBackIdle, false);
		m_rightScript.PlayNext(SpriteScript.Sprite.SideFrontIdle, false);
		m_leftScript.PlayNext(SpriteScript.Sprite.SideBackIdle, false);
	}

	void Update()
	{
		UpdateState();
	}

	void UpdateState()
	{
		switch (m_playerScript.m_state)
		{
			case PlayerScript.State.Idle:
			{
				m_frontScript.PlayNext(SpriteScript.Sprite.FrontIdle, false);
				m_backScript.PlayNext(SpriteScript.Sprite.SideBackIdle, false);

				if (m_lastState == PlayerScript.State.Right)
				{
					m_rightScript.PlayNext(SpriteScript.Sprite.SideFrontIdle, false);
					m_leftScript.PlayNext(SpriteScript.Sprite.SideBackIdle, false);
				}
				if (m_lastState == PlayerScript.State.Left)
				{
					m_rightScript.PlayNext(SpriteScript.Sprite.SideBackIdle, false);
					m_leftScript.PlayNext(SpriteScript.Sprite.SideFrontIdle, false);
				}
				break;
			}
			case PlayerScript.State.Right:
			{
				if (m_playerScript.m_speedBoost)
				{
					m_frontScript.PlayNext(SpriteScript.Sprite.FrontRunEscape);
					m_backScript.PlayNext(SpriteScript.Sprite.FrontRunEscape);
				}
				else
				{
					m_frontScript.PlayNext(SpriteScript.Sprite.FrontRun);
					m_backScript.PlayNext(SpriteScript.Sprite.FrontRun);
				}

				m_rightScript.PlayNext(SpriteScript.Sprite.SideFrontRun);
				m_leftScript.PlayNext(SpriteScript.Sprite.SideBackRun);

				m_frontScript.ReverseSprite(false);
				m_backScript.ReverseSprite(true);
				m_rightScript.ReverseSprite(false);
				m_leftScript.ReverseSprite(false);

				m_lastState = PlayerScript.State.Right;
				break;
			}
			case PlayerScript.State.Left:
			{
				if (m_playerScript.m_speedBoost)
				{
					m_frontScript.PlayNext(SpriteScript.Sprite.FrontRunEscape);
					m_backScript.PlayNext(SpriteScript.Sprite.FrontRunEscape);
				}
				else
				{
					m_frontScript.PlayNext(SpriteScript.Sprite.FrontRun);
					m_backScript.PlayNext(SpriteScript.Sprite.FrontRun);
				}
				
				m_rightScript.PlayNext(SpriteScript.Sprite.SideBackRun);
				m_leftScript.PlayNext(SpriteScript.Sprite.SideFrontRun);

				m_frontScript.ReverseSprite(true);
				m_backScript.ReverseSprite(false);
				m_rightScript.ReverseSprite(true);
				m_leftScript.ReverseSprite(true);

				m_lastState = PlayerScript.State.Left;
				break;
			}
			case PlayerScript.State.Jump:
			{
				m_frontScript.PlayNext(SpriteScript.Sprite.FrontDash, true);
				m_backScript.PlayNext(SpriteScript.Sprite.BackDash, true);
				m_rightScript.PlayNext(SpriteScript.Sprite.SideDash, true);
				m_leftScript.PlayNext(SpriteScript.Sprite.SideDash, true);

				m_frontScript.ReverseSprite(false);
				m_backScript.ReverseSprite(false);
				m_rightScript.ReverseSprite(true);
				m_leftScript.ReverseSprite(false);
				break;
			}
			case PlayerScript.State.Stun:
			{
				m_frontScript.PlayNext(SpriteScript.Sprite.FrontFall, false);
				m_backScript.PlayNext(SpriteScript.Sprite.SideBackFall, false);
				if (m_lastState == PlayerScript.State.Right)
				{
					m_rightScript.PlayNext(SpriteScript.Sprite.SideBackFall, false);
					m_leftScript.PlayNext(SpriteScript.Sprite.FrontFall, false);
				}
				else
				{
					m_rightScript.PlayNext(SpriteScript.Sprite.FrontFall, false);
					m_leftScript.PlayNext(SpriteScript.Sprite.SideBackFall, false);
				}
				break;
			}
		}
	}

	public void SetMoveOpacity(float coefMove1, float coefMove2)
	{
		m_backScript.SetOpacity(Constants.MinOpacity);

		if (m_playerScript.m_state == PlayerScript.State.Left)
		{
			if (coefMove2 == 0f)
			{
				m_rightScript.SetOpacity(1f); //SideBackRun
				m_leftScript.SetOpacity(Mathf.Lerp(1f, Constants.MinOpacity, coefMove1)); //SideFrontRun
			}
			else
			{
				m_leftScript.SetOpacity(1f); //SideFrontRun
				m_rightScript.SetOpacity(Mathf.Lerp(Constants.MinOpacity, 1f, coefMove2)); //SideBackRun
			}
		}
		else if (m_playerScript.m_state == PlayerScript.State.Right)
		{
			if (coefMove2 == 0f)
			{
				m_leftScript.SetOpacity(1f); //SideBackRun
				m_rightScript.SetOpacity(Mathf.Lerp(1f, Constants.MinOpacity, coefMove1)); //SideFrontRun
			}
			else
			{
				m_rightScript.SetOpacity(1f); //SideFrontRun
				m_leftScript.SetOpacity(Mathf.Lerp(Constants.MinOpacity, 1f, coefMove2)); //SideBackRun
			}
		}
	}

	public void SetJumpOpacity(float coefJump)
	{
		m_frontScript.SetOpacity(Mathf.Lerp(1f, Constants.MinOpacity, coefJump));
		m_backScript.SetOpacity(Mathf.Lerp(Constants.MinOpacity, 1f, coefJump));
		if (coefJump == 1)
		{
			m_frontScript.SetOpacity(1f);
			m_backScript.SetOpacity(Constants.MinOpacity);
		}
	}
}
