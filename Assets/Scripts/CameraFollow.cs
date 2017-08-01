using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public GameObject m_target;

	void Update ()
	{
		transform.position = new Vector3(transform.position.y, m_target.transform.position.y, transform.position.z);
		transform.LookAt(m_target.transform);
	}
}
