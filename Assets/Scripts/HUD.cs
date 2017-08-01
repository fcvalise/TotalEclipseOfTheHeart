using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour {

	public GameObject			m_walls;
	public GameObject			m_mirrors;
	public GameObject			m_player1;
	public GameObject			m_player2;
	public GameObject			m_camerasManager;
	public Font					m_font;

	private GUIStyle			m_guiStyle;
	private string				m_infos = "";

	private CamerasScript		m_camerasScript;
	private PlayerScript		m_p1Script;
	private PlayerScript		m_p2Script;
	private Light				m_light;
	private bool				m_anyKeyUp = false;

	void Start()
	{
		m_camerasScript = m_camerasManager.GetComponent<CamerasScript>();
		m_guiStyle = new GUIStyle();
		m_guiStyle.font = m_font;
		m_guiStyle.fontSize = 30;
		m_p1Script = m_player1.GetComponent<PlayerScript>();
		m_p2Script = m_player2.GetComponent<PlayerScript>();
		m_light = m_walls.GetComponent<Light>();
	}
	
	void Update()
	{
		AdditionalInput();
		UpdateLight();
	}

	void OnGUI()
	{
		m_guiStyle.normal.textColor = Color.white;
		m_guiStyle.fontSize = 30;
		m_guiStyle.alignment = TextAnchor.MiddleCenter;
		GUI.Label(new Rect(0, 0, Screen.width, Screen.height), m_infos, m_guiStyle);

		if (m_anyKeyUp)
		{
			m_guiStyle.alignment = TextAnchor.UpperCenter;
			m_guiStyle.normal.textColor = Constants.ColorBlue;
			m_guiStyle.fontSize = 50;
			GUI.Label(new Rect(0, 0, Screen.width, Screen.height), Constants.WinGameBlue.ToString(), m_guiStyle);
			m_guiStyle.alignment = TextAnchor.LowerCenter;
			m_guiStyle.normal.textColor = Constants.ColorRed;
			GUI.Label(new Rect(0, 0, Screen.width, Screen.height), Constants.WinGameRed.ToString(), m_guiStyle);
		}
	}

	void AdditionalInput()
	{
		if (m_p2Script.IsAIActive())
			m_infos = "Current AI : " + m_p2Script.GetDifficulty();
		else
			m_infos = "";
		if (Input.anyKey &&
			!Input.GetKey(m_p1Script.m_keyDown) && !Input.GetKey(m_p1Script.m_keyUp) && !Input.GetKey(m_p1Script.m_keyRight) && !Input.GetKey(m_p1Script.m_keyLeft) &&
			!Input.GetKey(m_p2Script.m_keyDown) && !Input.GetKey(m_p2Script.m_keyUp) && !Input.GetKey(m_p2Script.m_keyRight) && !Input.GetKey(m_p2Script.m_keyLeft))
		{
			m_infos = "AI Difficulty : [0-5] -- Shuffle Camera : [N] -- Mirrors : [M]";

			m_camerasScript.m_speedIntro = 10f;

			if (Input.GetKeyDown(KeyCode.M))
				m_mirrors.SetActive(!m_mirrors.activeSelf);
			if (Input.GetKeyDown(KeyCode.N))
				m_camerasScript.ShuffleCamera(true);
			if (Input.GetKeyDown(KeyCode.Alpha0))
				m_p2Script.SetAIDifficulty(0);
			if (Input.GetKeyDown(KeyCode.Alpha1))
				m_p2Script.SetAIDifficulty(1);
			if (Input.GetKeyDown(KeyCode.Alpha2))
				m_p2Script.SetAIDifficulty(2);
			if (Input.GetKeyDown(KeyCode.Alpha3))
				m_p2Script.SetAIDifficulty(3);
			if (Input.GetKeyDown(KeyCode.Alpha4))
				m_p2Script.SetAIDifficulty(4);
			if (Input.GetKeyDown(KeyCode.Alpha5))
				m_p2Script.SetAIDifficulty(5);
			m_anyKeyUp = true;
		}
		else if (m_anyKeyUp)
		{
			m_camerasScript.m_speedIntro = 1f;
			m_anyKeyUp = false;
		}
	}

	void UpdateLight()
	{
		if (m_anyKeyUp || m_p1Script.m_state == PlayerScript.State.Stun || m_p2Script.m_state == PlayerScript.State.Stun)
			m_light.intensity = 0.1f;
		else
			m_light.intensity = 4f;
	}
}
