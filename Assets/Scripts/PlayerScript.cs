using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	public enum State
	{
		Idle,
		Right,
		Left,
		Jump,
		Stun
	};

	public enum PlayerSide
	{
		Front,
		Right,
		Back,
		Left,
		None
	};

	public enum Job
	{
		Cat,
		Mouse
	}

	private static PhotonView ScenePhotonView;

	public GameObject			m_spriteManager;
	public KeyCode				m_keyLeft;
	public KeyCode				m_keyRight;
	public KeyCode				m_keyUp;
	public KeyCode				m_keyDown;
	public GameObject			m_camerasManager;


	public Job					m_job;
	public bool					m_isMine;
	public int					m_startIndex;

	public State				m_state { get; private set; }
	public PlayerSide			m_side { get; private set; }
	public float				m_scoreTime { get; private set; }
	public float				m_speedBoost { get; private set; }
	public int					m_moveCount { get; private set; }

	private SpriteManagerScript m_spriteManagerScript;
	private CamerasScript		m_camerasScript;

	private int					m_indexMiddle = 0;
	private int					m_nextIndexMiddle = 0;
	private int					m_indexCorner = 0;
	private float				m_timerMove2 = 0f;
	private float				m_timerMove1 = 0f;
	private float				m_timerMoveMax = 1f;
	private float				m_timerJump = 0f;
	private float				m_timerJumpMax = 1.3f;
	private float				m_timerStun = 0f;
	private float				m_timerStunMax = 1f;
	private State				m_stateSaveStun = State.Idle;
	private int					m_moveCountMax = 4;

	void Start()
	{
		ScenePhotonView = this.GetComponent<PhotonView>();

		m_spriteManager = GameObject.Instantiate(m_spriteManager, this.transform);
		m_spriteManagerScript = m_spriteManager.GetComponent<SpriteManagerScript>();
		m_spriteManagerScript.m_player = this.gameObject;

		m_state = State.Idle;

		m_camerasManager = GameObject.Find("CamerasManager");
		if (m_camerasManager)
			m_camerasScript = m_camerasManager.GetComponent<CamerasScript>();
		InitAI();
		SetStartIndex(m_startIndex);
		transform.position = Constants.BottomPosition[m_indexMiddle];
		UpdateRotation();
	}

	void Update()
	{
		if (PhotonNetwork.playerList.Length == 2)
		{
			if (m_isMine)
			{
				UpdateState();
				UpdateStateMobile();
			}
			UpdateMove();
			UpdateScore();

			transform.position = new Vector3((int)(transform.position.x / 10) * 10, (int)(transform.position.y / 10) * 10, (int)(transform.position.z / 10) * 10);
		}
	}

	void UpdateState()
	{
		if (Input.GetKeyDown(m_keyRight))
		{
			SetState(State.Right, false);
		}
		else if (Input.GetKeyDown(m_keyLeft))
		{
			SetState(State.Left, false);
		}
		else if (Input.GetKeyDown(m_keyUp) || Input.GetKeyDown(m_keyDown))
		{
			SetState(State.Jump, false);
		}
	}

	public void SetState(State state, bool isAI)
	{

		if (m_state == State.Idle)
		{
			m_state = state;
		}
		m_isAIActive = isAI;
	}

	void UpdateStateMobile()
	{
		if (m_keyLeft != KeyCode.LeftArrow)
			return;

		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);

			if (touch.phase == TouchPhase.Began)
			{
				PlayerSide side = PlayerSide.None;
				Vector2 position = touch.position;

				if (position.x < (Screen.width / 2) && position.y > (Screen.height / 2))
					side = PlayerSide.Front;
				if (position.x > (Screen.width / 2) && position.y > (Screen.height / 2))
					side = PlayerSide.Right;
				if (position.x < (Screen.width / 2) && position.y < (Screen.height / 2))
					side = PlayerSide.Back;
				if (position.x > (Screen.width / 2) && position.y < (Screen.height / 2))
					side = PlayerSide.Left;

				if (m_state == State.Idle)
					MoveMobile(side);

				//m_camerasScript.SetWinEvent(side, 10);
				m_isAIActive = false;
			}
		}
		UpdateStateMobileSimulate();
	}

	void UpdateStateMobileSimulate()
	{
		if (Input.GetMouseButtonDown(0))
		{
			PlayerSide side = PlayerSide.None;
			Vector2 position = Input.mousePosition;

			if (position.x < (Screen.width / 2) && position.y > (Screen.height / 2))
				side = PlayerSide.Front;
			if (position.x > (Screen.width / 2) && position.y > (Screen.height / 2))
				side = PlayerSide.Right;
			if (position.x < (Screen.width / 2) && position.y < (Screen.height / 2))
				side = PlayerSide.Back;
			if (position.x > (Screen.width / 2) && position.y < (Screen.height / 2))
				side = PlayerSide.Left;
			
			if (m_state == State.Idle)
				MoveMobile(side);

			//m_camerasScript.SetWinEvent(side, 10);
			m_isAIActive = false;
		}
	}

	void MoveMobile(PlayerSide side)
	{
		int index = (int)side;
		int playerIndex = m_nextIndexMiddle;
		//m_camerasScript.SetWinEvent(side, 30);

		while (index != 2)
		{
			index = IncrementIndex(index, 1);
			playerIndex = IncrementIndex(playerIndex, 1);
		}

		bool ret = JumpTowardEnemy(playerIndex, index);
		if (!ret)
			MoveTowardEnemy(playerIndex, index);
	}

	void UpdateMove()
	{
		switch (m_state)
		{
			case State.Right:
			{
				Move(1);
				break;
			}
			case State.Left:
			{
				Move(-1);
				break;
			}
			case State.Jump:
			{
				Jump();
				break;
			}
			case State.Stun:
			{
				Stun();
				break;
			}
			default:
				break;
		}
	}

	void Move(int index)
	{
		if (m_timerMove1 < m_timerMoveMax)
		{
			m_indexCorner = m_indexMiddle;
			if (index < 0)
				m_indexCorner = IncrementIndex(m_indexCorner, index);

			m_timerMove1 = Mathf.Min(m_timerMove1 + Time.deltaTime * (1f + 1f * m_speedBoost), m_timerMoveMax);
			transform.position = Vector3.Lerp(Constants.BottomPosition[m_indexMiddle], Constants.CornerPosition[m_indexCorner], m_timerMove1 / m_timerMoveMax);
		}
		else
		{

			if (m_timerMove2 == 0f)
			{
				m_nextIndexMiddle = IncrementIndex(m_nextIndexMiddle, index);
				m_indexMiddle = IncrementIndex(m_indexMiddle, index);
				UpdateRotation();
			}
			m_timerMove2 = Mathf.Min(m_timerMove2 + Time.deltaTime * (1f + 1f * m_speedBoost), m_timerMoveMax);
			transform.position = Vector3.Lerp(Constants.CornerPosition[m_indexCorner], Constants.BottomPosition[m_nextIndexMiddle], m_timerMove2 / m_timerMoveMax);
		}

		m_spriteManagerScript.SetMoveOpacity(m_timerMove1 / m_timerMoveMax, m_timerMove2 / m_timerMoveMax);

		if (m_timerMove2 >= m_timerMoveMax)
		{
			m_state = State.Idle;
			m_timerMove2 = 0f;
			m_timerMove1 = 0f;
			UpdateMoveCount();
		}
	}

	void Jump()
	{
		if (m_timerJump == 0f)
		{
			m_nextIndexMiddle = IncrementIndex(m_nextIndexMiddle, 2);
		}
		m_timerJump = Mathf.Min(m_timerJump + Time.deltaTime, m_timerJumpMax);
		transform.position = Vector3.Lerp(Constants.BottomPosition[m_indexMiddle], Constants.BottomPosition[m_nextIndexMiddle], m_timerJump / m_timerJumpMax);

		m_spriteManagerScript.SetJumpOpacity(m_timerJump / m_timerJumpMax);

		if (m_timerJump >= m_timerJumpMax)
		{
			m_state = State.Idle;
			m_timerJump = 0f;
			m_indexMiddle = IncrementIndex(m_indexMiddle, 2);
			UpdateRotation();
		}
	}

	void Stun()
	{
		m_timerStun -= Time.deltaTime;
		if (m_timerStun <= 0)
			m_state = m_stateSaveStun;
	}

	void UpdateRotation()
	{
		switch (m_indexMiddle)
		{
			case 0:
				transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
				m_side = PlayerSide.Front;
				break;
			case 1:
				transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
				m_side = PlayerSide.Right;
				break;
			case 2:
				transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
				m_side = PlayerSide.Back;
				break;
			case 3:
				transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
				m_side = PlayerSide.Left;
				break;
			default:
				break;
		}
	}

	void UpdateMoveCount()
	{
		/*
		if (m_camerasScript.m_isIntroEnded)
		{
			if (m_job == Job.Cat)
				m_moveCount++;
			else
				m_moveCount = 0;
			
			if (m_moveCount >= m_moveCountMax)
			{
				m_moveCount = 0;
				m_speedBoost = 1f;
			}
			else
				m_speedBoost = 0f;
		}
		*/
	}

	void UpdateScore()
	{
		if (m_job == Job.Mouse)
			m_scoreTime += Time.deltaTime;
	}

	int IncrementIndex(int index, int value)
	{
		const int maxCorner = 4;

		index += value;
		if (index >= maxCorner)
			index = index - maxCorner;
		else if (index < 0)
			index = maxCorner + index;
		return index;
	}

	public int GetNextIndexMiddle()
	{
		return m_nextIndexMiddle;
	}

	public void SetStartIndex(int index)
	{
		m_indexCorner = index;
		m_indexMiddle = index;
		m_nextIndexMiddle = index;
		transform.position = Constants.BottomPosition[m_indexMiddle];
	}

	[PunRPC]
	public void Collided()
	{
		if (m_job == Job.Mouse)
		{
			SetStun();
			if (m_camerasManager)
				m_camerasScript.SetWinEvent(m_side, 30);
		}
		InvertJob();
	}

	public static void Collide()
	{
		ScenePhotonView.RPC("Collided", PhotonTargets.All);
	}

	public void SetStun()
	{
		if (m_state != State.Stun)
		{
			m_timerStun = m_timerStunMax;
			m_stateSaveStun = m_state;
			m_state = State.Stun;
		}
	}

	public void InitJob(Job job)
	{
		m_job = job;
	}

	public void InvertJob()
	{
		if (m_job == Job.Cat)
			m_job = Job.Mouse;
		else
			m_job = Job.Cat;
	}

	public bool IsCat()
	{
		return m_job == Job.Cat;
	}

	public bool IsMouse()
	{
		return m_job == Job.Mouse;
	}

	/*
	/* AI
	*/

	enum AIDifficulty
	{
		Wait,
		TurnAround,
		RandomDirection,
		ChooseDirection,
		RandomJump,
		ChooseJump
	};

	public GameObject		m_enemy;
	public bool				m_isAIActive = true;
	private AIDifficulty 	m_AIDifficulty = AIDifficulty.TurnAround;
	private float			m_timerDecision = 0f;
	private float			m_timerRandomizeBehavior = 0f;

	void InitAI()
	{
		ChooseAIDifficulty();
	}

	void UpdateAI()
	{
		if (m_state == State.Idle)
		{
			m_timerDecision -= Time.deltaTime;
			if (m_timerDecision <= 0f)
			{
				UpdateAIBehavior();
				m_timerDecision = Random.Range(0.05f, 0.5f);
			}
		}
		m_timerRandomizeBehavior -= Time.deltaTime;
	}

	public bool IsAIActive()
	{
		return m_isAIActive;
	}

	public int GetDifficulty()
	{
		return (int)m_AIDifficulty;
	}

	public void SetAIDifficulty(int difficulty)
	{
		m_AIDifficulty = (AIDifficulty)difficulty;
	}

	void ChooseAIDifficulty()
	{
		m_AIDifficulty = (AIDifficulty)(Constants.WinGameBlue + 1);
	}

	void UpdateAIBehavior()
	{
		int playerIndex = m_nextIndexMiddle;
		int enemyIndex = m_enemy.GetComponent<PlayerScript>().GetNextIndexMiddle();

		while (enemyIndex != 2)
		{
			enemyIndex = IncrementIndex(enemyIndex, 1);
			playerIndex = IncrementIndex(playerIndex, 1);
		}

		switch (m_AIDifficulty)
		{
			case AIDifficulty.TurnAround:
				m_state = State.Right;
				break;
			case AIDifficulty.RandomDirection:
				MoveRandom();
				break;
			case AIDifficulty.ChooseDirection:
				MovePlayer(playerIndex, enemyIndex);
				break;
			case AIDifficulty.RandomJump:
				if (!JumpRandom())
					MovePlayer(playerIndex, enemyIndex);
				break;
			case AIDifficulty.ChooseJump:
			{
				if (m_timerRandomizeBehavior <= 0f)
				{
					m_timerRandomizeBehavior = Random.Range(7, 10);
					if (!JumpRandom())
						MoveRandom();
				}
				else if (!JumpPlayer(playerIndex, enemyIndex))
					MovePlayer(playerIndex, enemyIndex);
				break;
			}
			case AIDifficulty.Wait:
			default:
				break;
		}
	}

	void MoveRandom()
	{
		if (Random.value > 0.5f)
			m_state = State.Right;
		else
			m_state = State.Left;
	}

	void MovePlayer(int playerIndex, int enemyIndex)
	{
		if (Mathf.Abs(enemyIndex - playerIndex) == 2)
			MoveRandom();
		else if (m_job == Job.Cat)
			MoveTowardEnemy(playerIndex, enemyIndex);
		else
			MoveAwayEnemy(playerIndex, enemyIndex);
	}

	void MoveAwayEnemy(int playerIndex, int enemyIndex)
	{
		if (enemyIndex < playerIndex)
			m_state = State.Right;
		else
			m_state = State.Left;
	}

	void MoveTowardEnemy(int playerIndex, int enemyIndex)
	{
		if (enemyIndex > playerIndex)
			m_state = State.Right;
		else if (enemyIndex < playerIndex)
			m_state = State.Left;
	}

	bool JumpRandom()
	{
		if (Random.value < 0.3f)
		{
			m_state = State.Jump;
			return true;
		}
		return false;
	}

	bool JumpPlayer(int playerIndex, int enemyIndex)
	{
		if (m_job == Job.Cat)
			return JumpTowardEnemy(playerIndex, enemyIndex);
		return JumpAwayEnemy(playerIndex, enemyIndex);
	}

	bool JumpAwayEnemy(int playerIndex, int enemyIndex)
	{
		if (Mathf.Abs(enemyIndex - playerIndex) == 1)
		{
			m_state = State.Jump;
			return true;
		}
		return false;
	}

	bool JumpTowardEnemy(int playerIndex, int enemyIndex)
	{
		if (Mathf.Abs(enemyIndex - playerIndex) == 2)
		{
			m_state = State.Jump;
			return true;
		}
		return false;
	}
}