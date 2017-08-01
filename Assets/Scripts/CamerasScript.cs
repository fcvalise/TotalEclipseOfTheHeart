using UnityEngine;
using System.Collections;

public class CamerasScript : MonoBehaviour {

	[HideInInspector]
	public bool						m_isIntroEnded = false;
	[HideInInspector]
	public float					m_speedIntro = 1f;

	public Camera					m_cameraFront;
	public Camera					m_cameraBack;
	public Camera					m_cameraRight;
	public Camera					m_cameraLeft;

	private const float				dist = 630f;

	private Vector3					m_cameraInitialPosition = new Vector3(0, 0f, 0f);

	private Vector3					m_cameraFrontPosition = new Vector3(0f, 0f, -dist);
	private Vector3					m_cameraFrontInitialRotation = new Vector3(90f, 0f, 0f);
	private Vector3					m_cameraFrontRotation = new Vector3(0f, 0f, 0f);

	private Vector3					m_cameraBackPosition = new Vector3(0f, 0f, dist);
	private Vector3					m_cameraBackInitialRotation = new Vector3(90f, 0f, 180f);
	private Vector3					m_cameraBackRotation = new Vector3(180f, 0f, 180f);

	private Vector3					m_cameraRightPosition = new Vector3(-dist, 0f, 0f);
	private Vector3					m_cameraRightInitialRotation = new Vector3(90f, 90f, 0f);
	private Vector3					m_cameraRightRotation = new Vector3(0f, 90f, 0f);

	private Vector3					m_cameraLeftPosition = new Vector3(dist, 0f, 0f);
	private Vector3					m_cameraLeftInitialRotation = new Vector3(90f, -90f, 0f);
	private Vector3					m_cameraLeftRotation = new Vector3(0f, -90f, 0f);

	private Rect					m_upLeft = new Rect(0f, 0.5f, 0.5f, 0.5f);
	private Rect					m_upRight = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
	private Rect					m_downLeft = new Rect(0f, 0f, 0.5f, 0.5f);
	private Rect					m_downRight = new Rect(0.5f, 0f, 0.5f, 0.5f);

	private PlayerScript.PlayerSide	m_sideWinEvent;
	private bool					m_isZoomWinEvent = false;
	private int						m_amplitude;

	private float					m_timerIntro = 0f;
	private float					m_timerIntroMax = 16f;
	private float					m_timerWinEvent = 0f;
	private float					m_timerWinEventMax = 0.1f;

	private int						m_indexCamera = 0;

	void Start()
	{
		m_cameraFront.transform.position = m_cameraInitialPosition;
		m_cameraFront.transform.rotation = Quaternion.Euler(m_cameraFrontInitialRotation);
		m_cameraFront.orthographicSize = Constants.CameraOrthographicSizeStart;

		m_cameraBack.transform.position = m_cameraInitialPosition;
		m_cameraBack.transform.rotation = Quaternion.Euler(m_cameraBackInitialRotation);
		m_cameraBack.orthographicSize = Constants.CameraOrthographicSizeStart;

		m_cameraRight.transform.position = m_cameraInitialPosition;
		m_cameraRight.transform.rotation = Quaternion.Euler(m_cameraRightInitialRotation);
		m_cameraRight.orthographicSize = Constants.CameraOrthographicSizeStart;

		m_cameraLeft.transform.position = m_cameraInitialPosition;
		m_cameraLeft.transform.rotation = Quaternion.Euler(m_cameraLeftInitialRotation);
		m_cameraLeft.orthographicSize = Constants.CameraOrthographicSizeStart;
	}

	void Update()
	{
		StartCameraMovement();
		if (m_timerIntro == m_timerIntroMax)
			UpdateWinEvent();
	}

	void StartCameraMovement()
	{
		m_timerIntro = Mathf.Min(m_timerIntro + Time.deltaTime * m_speedIntro, m_timerIntroMax);

		float coef = m_timerIntro / m_timerIntroMax;

		if (m_timerIntro <= m_timerIntroMax)
		{
			m_cameraFront.transform.position = Vector3.Slerp(m_cameraInitialPosition, m_cameraFrontPosition, coef);
			m_cameraFront.transform.rotation = Quaternion.Euler(Vector3.Lerp(m_cameraFrontInitialRotation, m_cameraFrontRotation, coef));
			m_cameraFront.orthographicSize = Mathf.Lerp(Constants.CameraOrthographicSizeStart, Constants.CameraOrthographicSize, coef);

			m_cameraBack.transform.position = Vector3.Slerp(m_cameraInitialPosition, m_cameraBackPosition, coef);
			m_cameraBack.transform.rotation = Quaternion.Euler(Vector3.Lerp(m_cameraBackInitialRotation, m_cameraBackRotation, coef));
			m_cameraBack.orthographicSize = Mathf.Lerp(Constants.CameraOrthographicSizeStart, Constants.CameraOrthographicSize, coef);

			m_cameraLeft.transform.position = Vector3.Slerp(m_cameraInitialPosition, m_cameraLeftPosition, coef);
			m_cameraLeft.transform.rotation = Quaternion.Euler(Vector3.Lerp(m_cameraLeftInitialRotation, m_cameraLeftRotation, coef));
			m_cameraLeft.orthographicSize = Mathf.Lerp(Constants.CameraOrthographicSizeStart, Constants.CameraOrthographicSize, coef);

			m_cameraRight.transform.position = Vector3.Slerp(m_cameraInitialPosition, m_cameraRightPosition, coef);
			m_cameraRight.transform.rotation = Quaternion.Euler(Vector3.Lerp(m_cameraRightInitialRotation, m_cameraRightRotation, coef));
			m_cameraRight.orthographicSize = Mathf.Lerp(Constants.CameraOrthographicSizeStart, Constants.CameraOrthographicSize, coef);
		}

		if (m_timerIntro == m_timerIntroMax)
			m_isIntroEnded = true;
	}

	public void ShuffleCamera(bool isCameraIntro)
	{
		int random = Random.Range(0, 2);

		if (random == m_indexCamera)
			m_indexCamera = random + 1;
		else
			m_indexCamera = random;

		if (m_indexCamera > 3)
			m_indexCamera = 0;

		switch (m_indexCamera)
		{
			case 0:
			{
				m_cameraFront.rect = m_upLeft;
				m_cameraRight.rect = m_upRight;
				m_cameraBack.rect = m_downLeft;
				m_cameraLeft.rect = m_downRight;
				break;
			}
			case 1:
			{
				m_cameraFront.rect = m_downRight;
				m_cameraRight.rect = m_upLeft;
				m_cameraBack.rect = m_upRight;
				m_cameraLeft.rect = m_downLeft;
				break;
			}
			case 2:
			{
				m_cameraFront.rect = m_downLeft;
				m_cameraRight.rect = m_downRight;
				m_cameraBack.rect = m_upLeft;
				m_cameraLeft.rect = m_upRight;
				break;
			}
			case 3:
			{
				m_cameraFront.rect = m_upRight;
				m_cameraRight.rect = m_downLeft;
				m_cameraBack.rect = m_downRight;
				m_cameraLeft.rect = m_upLeft;
				break;
			}
		}
	}

	void UpdateWinEvent()
	{
		if (m_sideWinEvent != PlayerScript.PlayerSide.None)
		{
			if (m_isZoomWinEvent)
			{
				m_timerWinEvent = Mathf.Min(m_timerWinEvent + Time.deltaTime, m_timerWinEventMax);
				if (m_timerWinEvent >= m_timerWinEventMax)
					m_isZoomWinEvent = false;
			}
			else
			{
				m_timerWinEvent = Mathf.Max(m_timerWinEvent - Time.deltaTime, 0f);
				if (m_timerWinEvent <= 0f)
					m_sideWinEvent = PlayerScript.PlayerSide.None;
			}
			ComputeCameraWinEvent();
		}
	}

	void ComputeCameraWinEvent()
	{
		switch (m_sideWinEvent)
		{
			case PlayerScript.PlayerSide.Front:
			{
				m_cameraFront.orthographicSize = Mathf.Lerp(Constants.CameraOrthographicSize, Constants.CameraOrthographicSize - m_amplitude, m_timerWinEvent / m_timerWinEventMax);
				break;
			}
			case PlayerScript.PlayerSide.Back:
			{
				m_cameraBack.orthographicSize = Mathf.Lerp(Constants.CameraOrthographicSize, Constants.CameraOrthographicSize - m_amplitude, m_timerWinEvent / m_timerWinEventMax);
				break;
			}
			case PlayerScript.PlayerSide.Right:
			{
				m_cameraRight.orthographicSize = Mathf.Lerp(Constants.CameraOrthographicSize, Constants.CameraOrthographicSize - m_amplitude, m_timerWinEvent / m_timerWinEventMax);
				break;
			}
			case PlayerScript.PlayerSide.Left:
			{
				m_cameraLeft.orthographicSize = Mathf.Lerp(Constants.CameraOrthographicSize, Constants.CameraOrthographicSize - m_amplitude, m_timerWinEvent / m_timerWinEventMax);
				break;
			}
		}
	}

	public void SetBackGroundColor(Color color)
	{
		m_cameraBack.backgroundColor = color;
		m_cameraFront.backgroundColor = color;
		m_cameraLeft.backgroundColor = color;
		m_cameraRight.backgroundColor = color;
	}

	public void SetWinEvent(PlayerScript.PlayerSide side, int amplitude)
	{
		m_sideWinEvent = side;
		m_isZoomWinEvent = true;
		m_amplitude = amplitude;
	}
}
