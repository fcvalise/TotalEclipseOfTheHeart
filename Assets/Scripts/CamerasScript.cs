using UnityEngine;
using System.Collections;

public class CamerasScript : MonoBehaviour {

	[System.Serializable]
	public struct CameraStruct
	{
		public Camera	m_camera;
		public Vector3	m_initialPosition {get; private set; }
		public Vector3	m_targetPosition  {get; private set; }
		public Vector3	m_initialRotation {get; private set; }
		public Vector3	m_targetRotation {get; private set; }

		public void Init(Vector3 initialPosition, Vector3 targetPosition, Vector3 initialRotation, Vector3 targetRotation, float orthographicSize)
		{
			m_camera.transform.position = initialPosition;
			m_camera.transform.rotation = Quaternion.Euler(initialRotation);
			m_camera.orthographicSize = orthographicSize;

			m_initialPosition = initialPosition;
			m_targetPosition = targetPosition;
			m_initialRotation = initialRotation;
			m_targetRotation = targetRotation;
		}
	};

	public CameraStruct				m_front;
	public CameraStruct				m_back;
	public CameraStruct				m_right;
	public CameraStruct				m_left;

	[HideInInspector]
	public bool						m_isIntroEnded = false;
	[HideInInspector]
	public float					m_speedIntro = 1f;

	private const float				dist = 630f;

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
		m_front.Init(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, -dist), new Vector3(90f, 0f, 0f), new Vector3(0f, 0f, 0f), Constants.CameraOrthographicSizeStart);
		m_back.Init(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, dist), new Vector3(90f, 0f, 180f), new Vector3(180f, 0f, 180f), Constants.CameraOrthographicSizeStart);
		m_right.Init(new Vector3(0f, 0f, 0f), new Vector3(-dist, 0f, 0f), new Vector3(90f, 90f, 0f), new Vector3(0f, 90f, 0f), Constants.CameraOrthographicSizeStart);
		m_left.Init(new Vector3(0f, 0f, 0f), new Vector3(dist, 0f, 0f), new Vector3(90f, -90f, 0f), new Vector3(0f, -90f, 0f), Constants.CameraOrthographicSizeStart);
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
			TransformCamera(ref m_front, coef);
			TransformCamera(ref m_back, coef);
			TransformCamera(ref m_right, coef);
			TransformCamera(ref m_left, coef);
		}

		if (m_timerIntro == m_timerIntroMax)
			m_isIntroEnded = true;
	}

	void TransformCamera(ref CameraStruct cameraStruct, float coef)
	{
		cameraStruct.m_camera.transform.position = Vector3.Slerp(cameraStruct.m_initialPosition, cameraStruct.m_targetPosition, coef);
		cameraStruct.m_camera.transform.rotation = Quaternion.Euler(Vector3.Lerp(cameraStruct.m_initialRotation, cameraStruct.m_targetRotation, coef));
		cameraStruct.m_camera.orthographicSize = Mathf.Lerp(Constants.CameraOrthographicSizeStart, Constants.CameraOrthographicSize, coef);
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
				m_front.m_camera.rect = m_upLeft;
				m_right.m_camera.rect = m_upRight;
				m_back.m_camera.rect = m_downLeft;
				m_left.m_camera.rect = m_downRight;
				break;
			}
			case 1:
			{
				m_front.m_camera.rect = m_downRight;
				m_right.m_camera.rect = m_upLeft;
				m_back.m_camera.rect = m_upRight;
				m_left.m_camera.rect = m_downLeft;
				break;
			}
			case 2:
			{
				m_front.m_camera.rect = m_downLeft;
				m_right.m_camera.rect = m_downRight;
				m_back.m_camera.rect = m_upLeft;
				m_left.m_camera.rect = m_upRight;
				break;
			}
			case 3:
			{
				m_front.m_camera.rect = m_upRight;
				m_right.m_camera.rect = m_downLeft;
				m_back.m_camera.rect = m_downRight;
				m_left.m_camera.rect = m_upLeft;
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
				m_front.m_camera.orthographicSize = Mathf.Lerp(Constants.CameraOrthographicSize, Constants.CameraOrthographicSize - m_amplitude, m_timerWinEvent / m_timerWinEventMax);
				break;
			case PlayerScript.PlayerSide.Back:
				m_back.m_camera.orthographicSize = Mathf.Lerp(Constants.CameraOrthographicSize, Constants.CameraOrthographicSize - m_amplitude, m_timerWinEvent / m_timerWinEventMax);
				break;
			case PlayerScript.PlayerSide.Right:
				m_right.m_camera.orthographicSize = Mathf.Lerp(Constants.CameraOrthographicSize, Constants.CameraOrthographicSize - m_amplitude, m_timerWinEvent / m_timerWinEventMax);
				break;
			case PlayerScript.PlayerSide.Left:
				m_left.m_camera.orthographicSize = Mathf.Lerp(Constants.CameraOrthographicSize, Constants.CameraOrthographicSize - m_amplitude, m_timerWinEvent / m_timerWinEventMax);
				break;
			default:
				break;
		}
	}

	public void SetWinEvent(PlayerScript.PlayerSide side, int amplitude)
	{
		m_sideWinEvent = side;
		m_isZoomWinEvent = true;
		m_amplitude = amplitude;
	}
}
