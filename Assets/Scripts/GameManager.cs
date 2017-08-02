using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour {

	public GameObject			m_player1;
	public GameObject			m_player2;
	public GameObject			m_cameraManager;

	public AudioClip			m_music;
	public AudioClip			m_winSound;

	private AudioSource 		m_source;

	private CamerasScript		m_camerasScript;
	private PlayerScript		m_p1Script;
	private PlayerScript		m_p2Script;

	private float				m_timerCollisionPlayer = 0f;
	private float				m_timerCollisionPlayerMax = 2f;

	private PlayerScript.State	m_player1LastState;
	private PlayerScript.State	m_player2LastState;

	void Start ()
	{
		m_camerasScript = m_cameraManager.GetComponent<CamerasScript>();
		m_source = GetComponent<AudioSource>();
		m_source.PlayOneShot(m_music, 1f);

		m_p1Script = m_player1.GetComponent<PlayerScript>();
		m_p2Script = m_player2.GetComponent<PlayerScript>();

		InitPlayers();
	}

	void InitPlayers()
	{
		m_p1Script.SetStartIndex(1);
		m_p2Script.SetStartIndex(3);

		if (Random.value > 0.5f)
		{
			m_p1Script.InitJob(PlayerScript.Job.Cat);
			m_p2Script.InitJob(PlayerScript.Job.Mouse);
		}
		else
		{
			m_p1Script.InitJob(PlayerScript.Job.Mouse);
			m_p2Script.InitJob(PlayerScript.Job.Cat);
		}
	}

	void Update ()
	{
		if (Input.GetKey(KeyCode.Escape))
			SceneManager.LoadScene("MainMenu");
		
		UpdateCollisionPlayer();
		EventEnd();
	}

	void EventEnd()
	{
		if (m_camerasScript.m_isIntroEnded)
		{
			if (m_p1Script.m_scoreTime >= Constants.TimerWin)
			{
				SceneManager.LoadScene("BlueWins");
				m_source.PlayOneShot(m_winSound, 1f);
				Constants.WinGameBlue += 1;
			}
			if (m_p2Script.m_scoreTime >= Constants.TimerWin)
			{
				SceneManager.LoadScene("RedWins");
				m_source.PlayOneShot(m_winSound, 1f);
				Constants.WinGameRed += 1;
			}
		}
	}

	void UpdateCollisionPlayer()
	{
		m_timerCollisionPlayer = Mathf.Max(m_timerCollisionPlayer - Time.deltaTime, 0f);

		if (Vector3.Distance(m_player1.transform.position, m_player2.transform.position) < Constants.DistCollision)
		{
			if (m_timerCollisionPlayer == 0f)
			{
				m_timerCollisionPlayer = m_timerCollisionPlayerMax;
				if (m_p1Script.IsCat())
				{
					m_p2Script.SetStun();
					m_camerasScript.SetWinEvent(m_p1Script.m_side, 30);
				}
				if (m_p2Script.IsCat())
				{
					m_p1Script.SetStun();
					m_camerasScript.SetWinEvent(m_p2Script.m_side, 30);
				}
				m_source.PlayOneShot(m_winSound, 1f);
				m_p1Script.InvertJob();
				m_p2Script.InvertJob();
			}
		}
	}
}
