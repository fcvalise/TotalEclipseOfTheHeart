using UnityEngine;
using Photon;

public class NetworkPlayer : Photon.MonoBehaviour
{
	private PlayerScript		m_pScript;
	private PlayerScript.State	m_pState;
	private bool				m_wait;

	void Start()
	{
		m_pScript = GetComponent<PlayerScript>();
		m_pScript.m_isMine = photonView.isMine;
	}

	void Update()
	{
		if (!photonView.isMine && m_pState != PlayerScript.State.Stun)
		{
			m_pScript.SetState(m_pState, false);
		}
		else
		{
			if (m_pScript.m_state == PlayerScript.State.Idle)
				m_wait = true;
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			if (m_wait)
			{
				stream.SendNext(PlayerScript.State.Idle);
				m_wait = false;
			}
			else
				stream.SendNext(m_pScript.m_state);
		}
		else
		{
			// Network player, receive data
			this.m_pState = (PlayerScript.State)stream.ReceiveNext();
		}
	}
}