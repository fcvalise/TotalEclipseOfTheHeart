using UnityEngine;
using Photon;

public class NetworkGame : Photon.MonoBehaviour
{
	private GameManagerScript				m_script;
	private GameManagerScript.PlayerInfos	m_p2Infos;

	void Start()
	{
		m_script = GetComponent<GameManagerScript>();
	}

	void Update()
	{
		if (!photonView.isMine)
		{
			m_script.m_p2Infos = m_p2Infos;
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(m_script.m_p1Infos.m_position);
			stream.SendNext(m_script.m_p1Infos.m_scoreTime);
		}
		else
		{
			// Network player, receive data
			this.m_p2Infos.m_position = (Vector3)stream.ReceiveNext();
			this.m_p2Infos.m_scoreTime = (float)stream.ReceiveNext();
		}
	}
}