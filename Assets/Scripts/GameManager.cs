using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static bool IsPaused = false;

	[SerializeField]
	private GameObject _pauseMenu;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		} else
			Destroy(gameObject);
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			TogglePause();
		}
	}

	public static void TogglePause()
    {
        Time.timeScale = IsPaused ? 1.0f : 0.0f;
        IsPaused = !IsPaused;
		Instance._pauseMenu.SetActive(IsPaused);
	}

    public static void MainMenu() => LoadScene("MainMenu");

    public static void Quit()
    {
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();   
#endif
	}

	public static void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);
}
