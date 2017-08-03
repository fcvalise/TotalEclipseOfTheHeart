using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuScript : MonoBehaviour {

	bool m_inputPressed = false;

	void Start()
	{
		//Screen.SetResolution(126 * 6, 68 * 6, Screen.fullScreen);
	}

	void Update()
	{
		if (Input.anyKeyDown)
		{
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(GameObject.Find("PlayButton").gameObject, pointer, ExecuteEvents.pointerDownHandler);
			m_inputPressed = true;
		}
		else if (m_inputPressed && !Input.anyKey)
		{
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(GameObject.Find("PlayButton").gameObject, pointer, ExecuteEvents.pointerClickHandler);
		}

	}

	public void ShowControls()
	{
		SceneManager.LoadScene("Controls");
	}

	public void ShowRules()
	{
		SceneManager.LoadScene("Rules");
	}


	public void PlayGame()
	{
		SceneManager.LoadScene("Game");
	}

	public void ExitGame()
	{
		Application.Quit();
	}
}
