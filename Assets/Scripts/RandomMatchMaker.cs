using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class RandomMatchMaker : Photon.PunBehaviour
{
	public GameObject	m_gameManager;
	private GameObject	m_player;

	void Start()
	{
		//PhotonNetwork.logLevel = PhotonLogLevel.Full;
		PhotonNetwork.ConnectUsingSettings("0.1");
	}

	void OnGUI()
	{
		GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString() + "Is Mastrer : " + PhotonNetwork.isMasterClient);
	}

	public override void OnJoinedLobby()
	{
		PhotonNetwork.JoinRandomRoom();
	}

	void OnPhotonRandomJoinFailed()
	{
		Debug.Log("Can't join random room!");
		PhotonNetwork.CreateRoom(null);
	}

	override public void OnCreatedRoom()
	{
		PhotonNetwork.JoinRandomRoom();
	}

	override public void OnJoinedRoom()
	{
		Debug.Log("P" + PhotonNetwork.player.ID);

		if (PhotonNetwork.player.ID == 1)
		{
			m_player = PhotonNetwork.Instantiate("Player1TEOTH", Vector3.zero, Quaternion.identity, 0);
		}
		else
		{
			m_player = PhotonNetwork.Instantiate("Player2TEOTH", Vector3.zero, Quaternion.identity, 0);
			Debug.Log("P2");
		}
		m_gameManager.GetComponent<GameManagerScript>().m_player = m_player;
	}
}
