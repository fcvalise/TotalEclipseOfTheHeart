using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TextMesh))]
[RequireComponent(typeof(MeshRenderer))]
public class InGameScoreScript : MonoBehaviour {

	public GameObject			m_player;
	private TextMesh			m_textMesh;
	private PlayerScript		m_pScript;
	private Material			m_material;
	private PlayerScript.Job	m_lastJob;
	private float				m_timerSay;
	private float				m_timerSayMax = 2f;

	void Start()
	{
		m_textMesh = GetComponent<TextMesh>();
		m_pScript = m_player.GetComponent<PlayerScript>();
		m_material = GetComponent<MeshRenderer>().material;
	}

	void Update()
	{
		if (m_pScript.m_job != m_lastJob)
			m_timerSay = m_timerSayMax;

		if (m_pScript.m_job == PlayerScript.Job.Cat)
			m_material.color = new Color(0, 0, 0, 0);
		else if (m_timerSay > 0f)
		{
			if (m_pScript.m_job == PlayerScript.Job.Mouse)
				m_textMesh.text = "Tag, you're it !";
			m_timerSay -= Time.deltaTime;
			m_textMesh.fontSize = 180;
			if (m_player.name == "Dude")
				m_material.color = Constants.ColorBlue;
			else
				m_material.color = Constants.ColorRed;
		}
		else if (m_pScript.m_job == PlayerScript.Job.Mouse)
		{
			m_textMesh.text = ((int)Constants.TimerWin - (int)m_pScript.m_scoreTime).ToString();
			m_textMesh.fontSize = 400;
			if (m_player.name == "Dude")
				m_material.color = Constants.ColorBlue;
			else
				m_material.color = Constants.ColorRed;
		}

		m_lastJob = m_pScript.m_job;
	}
}
