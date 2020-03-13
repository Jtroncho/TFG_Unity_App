using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TFG.Authentification;

public class GameManager : Singleton<GameManager>
{
    //keep track of the game state
    public GameObject[] SystemPrefabs;
    public Events.EventGameState OnGameStateGanged;
    public Events.EventMenuState OnMenuStateGanged;
    public Auth _userSign; //Handle sign ins and outs

    //keep track of instanced prefabs
    List<GameObject> _instancedSystemPrefabs;
    List<AsyncOperation> _loadOperations;

    string _currentLevelName = string.Empty;

    GameState _currentGameState = GameState.LOGIN;
    public GameState CurrentGameState
    {
        get { return _currentGameState; }
        private set { _currentGameState = value; }
    }
    public enum GameState
    {
        LOGIN,
        MAINMENU,
        PREGAME,
        RUNNING,
        PAUSED,
        GAME
    }

    MenuState _currentMenuState = MenuState.LOGIN;
    public MenuState CurrentMenuState
    {
        get { return _currentMenuState; }
        private set { _currentMenuState = value; }
    }
    public enum MenuState
    {        
        LOGIN,
        MAINMENU,
        HIDDEN,
        PAUSE
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        _loadOperations = new List<AsyncOperation>();
        _instancedSystemPrefabs = new List<GameObject>();

        InstantiateSystemPrefabs();

        //UIManager.Instance.OnMainMenuFadeComplete.AddListener(HandleMainMenuFadeComplete);
    }

    private void Update()
    {
        if (_currentGameState == GameManager.GameState.PREGAME)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void OnLoadOperationComplete(AsyncOperation ao)
    {
        if(_loadOperations.Contains(ao))
        {
            _loadOperations.Remove(ao);

            if (_loadOperations.Count == 0)
            {
                UpdateGameState(GameState.RUNNING);
            }
        }
        Debug.Log("<color=#" + ColorUtility.ToHtmlStringRGB(Color.green) + ">" + "Load Complete." + "</color>");
    }

    void OnUnloadOperationComplete(AsyncOperation ao)
    {
        Debug.Log("<color=#" + ColorUtility.ToHtmlStringRGB(Color.green) + ">" + "Unload Complete." + "</color>");
    }

    /*
    void HandleMainMenuFadeComplete (bool fadeOut)
    {
        if (!fadeOut)
        {
            UnloadLevel(_currentLevelName);
        }
    }
    */

    void UpdateGameState(GameState state)
    {
        GameState previousGameState = _currentGameState;
        _currentGameState = state;
        /*
        switch(_currentGameState)
        {
            case GameState.PREGAME:
                Time.timeScale = 1.0f;
                break;

            case GameState.RUNNING:
                Time.timeScale = 1.0f;
                break;

            case GameState.PAUSED:
                Time.timeScale = 0.0f;
                break;

            default:
                break;
        }
        */
        if(_currentGameState == GameState.PAUSED)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }

        Debug.Log("Previous: " + previousGameState + " Current: " + _currentGameState);
        OnGameStateGanged.Invoke(_currentGameState, previousGameState);
    }

    public void UpdateMenuState(MenuState state)
    {
        MenuState previousMenuState = _currentMenuState;
        _currentMenuState = state;
        if (_currentGameState == GameState.PAUSED)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }

        Debug.Log("Previous: " + previousMenuState + " Current: " + _currentMenuState);
        OnMenuStateGanged.Invoke(_currentMenuState, previousMenuState);
    }

    void InstantiateSystemPrefabs()
    {
        GameObject prefabInstance;
        for (int i = 0; i < SystemPrefabs.Length; i++)
        {
            prefabInstance = Instantiate(SystemPrefabs[i]);
            _instancedSystemPrefabs.Add(prefabInstance);
        }
    }

    public void LoadLevel(string levelName)
    {
        Debug.Log("Load level: " + "<color=#" + ColorUtility.ToHtmlStringRGB(Color.black) + ">" + levelName + "</color>");
        AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        if (ao == null)
        {
            Debug.LogError("<color=#" + ColorUtility.ToHtmlStringRGB(Color.red) + ">" + "[GameManager] Unable to load level " + levelName + "</color>");
            return;
        }
        ao.completed += OnLoadOperationComplete;
        _loadOperations.Add(ao);

        _currentLevelName = levelName;
    }

    public void UnloadLevel(string levelName)
    {
        Debug.Log("Unload level: " + "<color=#" + ColorUtility.ToHtmlStringRGB(Color.black) + ">" + levelName + "</color>");
        AsyncOperation ao = SceneManager.UnloadSceneAsync(levelName);
        if (ao == null)
        {
            Debug.LogError("<color=#" + ColorUtility.ToHtmlStringRGB(Color.red) + ">" + "[GameManager] Unable to unload level " + levelName + "</color>");
            return;
        }
        ao.completed += OnUnloadOperationComplete;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();//do whatever singleton does

        for (int i = 0; i < _instancedSystemPrefabs.Count; i++)
        {
            Destroy(_instancedSystemPrefabs[i]);
        }
        _instancedSystemPrefabs.Clear();
    }

    public void StartApp()
    {
        LoadLevel("Login");
    }

    public void TogglePause()
    {
        UpdateGameState(_currentGameState == GameState.RUNNING ? GameState.PAUSED : GameState.RUNNING);
    }

    public void RestartGame()
    {
        UpdateGameState(GameState.PREGAME);
    }

    public void QuitGame()
    {
        //Features for quitting, like Auto Save etc;
        Application.Quit();
    }
}
