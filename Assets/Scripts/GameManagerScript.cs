using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class GameManagerScript : MonoBehaviour {

	public struct PlayerInfos
	{
		public Vector3					m_position;
		public float					m_scoreTime;
	};

	private static PhotonView ScenePhotonView;

	public GameObject			m_player;

	public AudioClip			m_music;
	public AudioClip			m_winSound;

	private AudioSource 		m_source;

	public PlayerInfos			m_p1Infos;
	public PlayerInfos			m_p2Infos;

	private PlayerScript		m_pScript;

	private float				m_timerCollisionPlayer = 0f;
	private float				m_timerCollisionPlayerMax = 2f;

	private PlayerScript.State	m_player1LastState;
	private PlayerScript.State	m_player2LastState;

	void Start ()
	{
		m_source = GetComponent<AudioSource>();
		m_source.PlayOneShot(m_music, 1f);

		ScenePhotonView = this.GetComponent<PhotonView>();
	}

	void Update ()
	{
		if (Input.GetKey(KeyCode.Escape))
			SceneManager.LoadScene("MainMenu");

		if (PhotonNetwork.playerList.Length == 2)
		{
			if (!m_pScript)
				m_pScript = m_player.GetComponent<PlayerScript>();

			UpdateCollisionPlayer();
			UpdateInfos();
			ScenePhotonView.RPC("EventEnd", PhotonTargets.All);
		}
	}

	[PunRPC]
	void EventEnd()
	{
		if (m_p1Infos.m_scoreTime >= Constants.TimerWin)
		{
			SceneManager.LoadScene("BlueWins");
			m_source.PlayOneShot(m_winSound, 1f);
			Constants.WinGameBlue += 1;
		}
		if (m_p2Infos.m_scoreTime >= Constants.TimerWin)
		{
			SceneManager.LoadScene("RedWins");
			m_source.PlayOneShot(m_winSound, 1f);
			Constants.WinGameRed += 1;
		}
	}

	void UpdateCollisionPlayer()
	{
		m_timerCollisionPlayer = Mathf.Max(m_timerCollisionPlayer - Time.deltaTime, 0f);

		if (Vector3.Distance(m_p1Infos.m_position, m_p2Infos.m_position) < Constants.DistCollision)
		{
			if (m_timerCollisionPlayer == 0f)
			{
				m_timerCollisionPlayer = m_timerCollisionPlayerMax;
				ScenePhotonView.RPC("Collide", PhotonTargets.All);
				m_source.PlayOneShot(m_winSound, 1f);
			}
		}
	}

	[PunRPC]
	void Collide()
	{
		PlayerScript.Collide();
	}

	void UpdateInfos()
	{
		m_p1Infos.m_position = m_player.transform.position;
		m_p1Infos.m_scoreTime = m_pScript.m_scoreTime;
	}
}
