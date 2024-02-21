using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool IsPaused = false;

    public static GameState _state = GameState.Playing;

    public Scenes previousScene = Scenes.MainMenu;
    public Scenes currentScene = Scenes.MainMenu;

	[SerializeField]
	private GameObject _pauseMenu;

    public static UnityEvent OnGamePaused;
    public static UnityEvent OnGameUnpaused;
    public UnityEvent OnMainMenuLoaded;
    public UnityEvent OnLevel1Loaded;
    public UnityEvent OnLevel2Loaded;
    public UnityEvent OnLevel3Loaded;
    public UnityEvent OnLevel4Loaded;
    public UnityEvent OnGameOverLoaded;
    public UnityEvent OnEndLoaded;

    private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		} else
			Destroy(gameObject);

        if(currentScene == Scenes.MainMenu)
        OnMainMenuLoaded?.Invoke();
	}

    private void OnEnable()
    {
        Goal.ActionOnGoalReached += LoadSceneByEnum;
    }

    private void OnDisable()
    {
        Goal.ActionOnGoalReached -= LoadSceneByEnum;
    }

    private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			TogglePause();
		}
	}

	public void TogglePause()
    {
        if(_state == GameState.Menu)
        Time.timeScale = IsPaused ? 1.0f : 0.0f;
        IsPaused = !IsPaused;
        if (IsPaused)
        {
            OnGamePaused?.Invoke();
            _state = GameState.Paused;
        }
        else
        {
            _state = GameState.Playing;
            OnGameUnpaused?.Invoke();
        }
		Instance._pauseMenu.SetActive(IsPaused);
	}

    public void MainMenu()
    {
        LoadSceneByEnum(Scenes.MainMenu);
    }
    public void Level1()
    {
        LoadSceneByEnum(Scenes.Level1);
    }
    public void Level2()
    {
        LoadSceneByEnum(Scenes.Level2);
    }
    public void Level3()
    {
        LoadSceneByEnum(Scenes.Level3);
    }
    public void Level4()
    {
        LoadSceneByEnum(Scenes.Level4);
    }
    public void GameOver()
    {
        LoadSceneByEnum(Scenes.GameOver);
    }
    public void End()
    {
        LoadSceneByEnum(Scenes.End);
    }
    public void Quit()
    {
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();   
#endif
	}

	public static void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);

    public void LoadSceneByEnum(Scenes scene)
    {
        Debug.Log("Loading new scene");
        ResetUI();

        switch (scene)
        {
            case Scenes.MainMenu:
                UpdateScene(Scenes.MainMenu);
                LoadScene("MainMenu");
                OnMainMenuLoaded?.Invoke();
                break;
            case Scenes.Level1:
                UpdateScene(Scenes.Level1);
                LoadScene("Level1");
                OnLevel1Loaded?.Invoke();
                break;
            case Scenes.Level2:
                UpdateScene(Scenes.Level2);
                LoadScene("Level2");
                OnLevel2Loaded?.Invoke();
                break;
            case Scenes.Level3:
                Debug.Log("scene does not currently exist");
                //UpdateScene(Scenes.Level3);
                //LoadScene("Level3");
                //OnLevel3Loaded?.Invoke();
                break;
            case Scenes.Level4:
                Debug.Log("scene does not currently exist");
                //UpdateScene(Scenes.Level4);
                //LoadScene("Level4");
                //OnLevel4Loaded?.Invoke();
                break;
            case Scenes.GameOver:
                UpdateScene(Scenes.GameOver);
                LoadScene("GameOver");
                OnGameOverLoaded?.Invoke();
                break;
            case Scenes.End:
                UpdateScene(Scenes.End);
                LoadScene("End");
                OnEndLoaded?.Invoke();
                break;
            default:
                break;
        }
    }

    public void LoadPreviousScene()
    {
        LoadSceneByEnum(previousScene);
    }

    private void UpdateScene(Scenes newScene)
    {
        previousScene = currentScene;
        currentScene = newScene;
    }

    public void ResetUI()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}

public enum GameState
{
    Paused,
    Playing,
    Menu
}

public enum Scenes
{
    MainMenu,
    Level1,
    Level2,
    Level3,
    Level4,
    GameOver,
    End
}
