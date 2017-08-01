using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class BlueRedWinScript : MonoBehaviour {

	public AudioClip		m_sound;
	public Font				m_font;

	private AudioSource		m_source;
	private float			m_timer = 2f;
	private GUIStyle		m_guiStyle;

	void Start ()
	{
		m_source = GetComponent<AudioSource>();
		m_source.PlayOneShot(m_sound, 1f);
		m_guiStyle = new GUIStyle();
		m_guiStyle.font = m_font;
		m_guiStyle.fontSize = 30;
	}

	void OnGUI()
	{
		m_guiStyle.alignment = TextAnchor.UpperLeft;
		m_guiStyle.normal.textColor = Constants.ColorBlue;
		m_guiStyle.fontSize = 200;
		GUI.Label(new Rect(60, 0, Screen.width, Screen.height), Constants.WinGameBlue.ToString(), m_guiStyle);
		m_guiStyle.alignment = TextAnchor.UpperRight;
		m_guiStyle.normal.textColor = Constants.ColorRed;
		GUI.Label(new Rect(0, 0, Screen.width, Screen.height), Constants.WinGameRed.ToString(), m_guiStyle);
		m_guiStyle.alignment = TextAnchor.LowerRight;
		m_guiStyle.normal.textColor = Color.white;
		m_guiStyle.fontSize = 30;
		GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "Lucien COYCAULT & Francois CORBEL ", m_guiStyle);
	}

	void Update ()
	{
		m_timer -= Time.deltaTime;
		if (m_timer < 0f)
			SceneManager.LoadScene("Game");
	}
}
