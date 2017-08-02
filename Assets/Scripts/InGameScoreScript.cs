using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TextMesh))]
[RequireComponent(typeof(MeshRenderer))]
public class InGameScoreScript : MonoBehaviour {

	public GameObject			m_player;
	private TextMesh			m_textMesh;
	private PlayerScript		m_pScript;
	private Material			m_material;

	void Start()
	{
		m_textMesh = GetComponent<TextMesh>();
		m_pScript = m_player.GetComponent<PlayerScript>();
		m_material = GetComponent<MeshRenderer>().material;

		m_textMesh.text = Constants.TimerWin.ToString();
		if (m_player.name == "Dude")
			m_material.color = Constants.ColorBlue;
		else
			m_material.color = Constants.ColorRed;
	}

	void Update()
	{
		if (m_pScript.IsCat())
			m_textMesh.fontSize = 200;
		else
		{
			m_textMesh.text = ((int)Constants.TimerWin - (int)m_pScript.m_scoreTime).ToString();
			m_textMesh.fontSize = 400;
		}
	}
}
