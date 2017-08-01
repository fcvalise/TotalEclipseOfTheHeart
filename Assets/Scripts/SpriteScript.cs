using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Renderer))]
public class SpriteScript : MonoBehaviour {

	public enum Sprite
	{
		BackDash,
		FrontDash,
		FrontIdle,
		FrontRun,
		FrontRunEscape,
		SideBackIdle,
		SideBackRun,
		SideDash,
		SideFrontIdle,
		SideFrontRun,
		FrontFall,
		SideBackFall,
		None
	}

	public Texture[]						m_backDash;
	public Texture[]						m_frontDash;
	public Texture[]						m_frontIdle;
	public Texture[]						m_frontRun;
	public Texture[]						m_frontRunEscape;
	public Texture[]						m_sideBackIdle;
	public Texture[]						m_sideBackRun;
	public Texture[]						m_sideDash;
	public Texture[]						m_sideFrontIdle;
	public Texture[]						m_sideFrontRun;
	public Texture[]						m_frontFall;
	public Texture[]						m_sideBackFall;
	public Texture[]						m_none;

	public AudioClip						m_moveSound1;
	public AudioClip						m_moveSound2;
	public AudioClip						m_dashSound;

	private AudioSource 					m_source;

	private Renderer						m_renderer;
	private Dictionary<Sprite, Texture[]>	m_animations = new Dictionary<Sprite, Texture[]>();

	private Sprite							m_current;
	private Sprite							m_next;

	private int								m_index = 0;

	private float							m_timeFrame = 0.1f;
	private float							m_timeFrameMax = 0.1f;

	private bool							m_isLoop = true;

	void Start()
	{
		m_renderer = GetComponent<Renderer>();
		m_source = GetComponent<AudioSource>();

		m_current = Sprite.FrontRun;

		m_animations[Sprite.BackDash] = m_backDash;
		m_animations[Sprite.FrontDash] = m_frontDash;
		m_animations[Sprite.FrontIdle] = m_frontIdle;
		m_animations[Sprite.FrontRunEscape] = m_frontRunEscape;
		m_animations[Sprite.FrontRun] = m_frontRun;
		m_animations[Sprite.SideBackIdle] = m_sideBackIdle;
		m_animations[Sprite.SideBackRun] = m_sideBackRun;
		m_animations[Sprite.SideDash] = m_sideDash;
		m_animations[Sprite.SideFrontIdle] = m_sideFrontIdle;
		m_animations[Sprite.SideFrontRun] = m_sideFrontRun;
		m_animations[Sprite.FrontFall] = m_frontFall;
		m_animations[Sprite.SideBackFall] = m_sideBackFall;
		m_animations[Sprite.None] = m_none;
	}

	void Update()
	{
		m_timeFrame += Time.deltaTime;

		if (m_next != m_current)
		{
			m_current = m_next;
			m_index = 0;
			m_timeFrame = m_timeFrameMax;
		}

		if (m_timeFrame >= m_timeFrameMax)
		{
			if (m_index < m_animations[m_current].Length)
			{
				PlaySound();
				m_renderer.material.mainTexture = m_animations[m_current][m_index];
				m_index++;
				m_timeFrame = 0f;
			}
			else if (m_isLoop)
				m_index = 0;
		}
	}

	void PlaySound()
	{
		switch (m_current)
		{
			case Sprite.FrontRun:
			{
				if (m_index == 2)
					m_source.PlayOneShot(m_moveSound1, 1f);
				if (m_index == 6)
					m_source.PlayOneShot(m_moveSound2, 1f);
				break;
			}
			case Sprite.FrontIdle:
			{
				if (m_index == 0)
					m_source.PlayOneShot(m_moveSound1, 1f);
				break;
			}
			case Sprite.FrontDash:
			{
				if (m_index == 0)
					m_source.PlayOneShot(m_dashSound , 1f);
				break;
			}
		}
	}

	public void SetOpacity(float opacity)
	{
		Color color = m_renderer.material.color;
		color.a = opacity;
		m_renderer.material.color = color;
	}

	public void PlayNext(Sprite sprite, bool isLoop = true)
	{
		m_next = sprite;
		m_isLoop = isLoop;
	}

	public void ReverseSprite(bool reverse)
	{
		if (reverse)
		{
			m_renderer.material.SetTextureScale("_MainTex", new Vector2(-1,1));
			m_renderer.material.SetTextureOffset("_MainTex", new Vector2(1, 0));
		}
		else
		{
			m_renderer.material.SetTextureScale("_MainTex", new Vector2(1,1));
			m_renderer.material.SetTextureOffset("_MainTex", new Vector2(0, 0));
		}
	}
}
