using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TFG.Authentification;
using TFG.Enum;
using TFG.Events;

public class AppManager : Singleton<AppManager>
{

    #region Prefab and Scene Management
    //keep track of instanced prefabs
    public GameObject[] SystemPrefabs;
    List<GameObject> _instancedSystemPrefabs;
    List<AsyncOperation> _loadOperations;
    #endregion

    string _currentLevelName = string.Empty;

    [SerializeField] AppState _appStartState = AppState.INIT;
    [SerializeField] AppState _currentAppState;
    [SerializeField] string initScene = "MainMenu";

    #region Main Methods
    /*override protected void Awake()
    {
        //base.Awake();
    }*/

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        _loadOperations = new List<AsyncOperation>();
        _instancedSystemPrefabs = new List<GameObject>();

        InstantiateSystemPrefabs();

        StartApp();
    }

    private void Update()
    {
        /*if (_currentAppState == AppState.LOGIN)
        {
            return;
        }*/

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //TogglePause();
            Debug.Log("Esc key Pressed");
        }
    }

    /*private void OnEnable()
    {
        //AppEvents.stateChange.AddListener();
    }

    private void OnDisable()
    {
        //AppEvents.stateChange.RemoveListener();
    }*/

    #endregion

    #region Prefabs and Scene Management
    void InstantiateSystemPrefabs()
    {
        GameObject prefabInstance;
        for (int i = 0; i < SystemPrefabs.Length; i++)
        {
            prefabInstance = Instantiate(SystemPrefabs[i]);
            _instancedSystemPrefabs.Add(prefabInstance);
        }
    }
    public void LoadScene(string levelName)
    {
        if (AppEvents.sceneLoading != null) 
        { 
            AppEvents.sceneLoading.Invoke(levelName); 
        }
        Debug.Log("Load level: " + "<color=#" + ColorUtility.ToHtmlStringRGB(Color.green) + ">" + levelName + "</color>");
        AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        if (ao == null)
        {
            Debug.LogError("<color=#" + ColorUtility.ToHtmlStringRGB(Color.red) + ">" + "[AppManager] Unable to load level " + levelName + "</color>");
            return;
        }
        ao.completed += OnLoadOperationComplete;
        _loadOperations.Add(ao);

        _currentLevelName = levelName;
    }

    public void UnloadScene(string levelName)
    {
        if (AppEvents.sceneLoading != null)
        {
            AppEvents.sceneUnloading.Invoke(levelName);
        }
        Debug.Log("Unload level: " + "<color=#" + ColorUtility.ToHtmlStringRGB(Color.green) + ">" + levelName + "</color>");
        AsyncOperation ao = SceneManager.UnloadSceneAsync(levelName);
        if (ao == null)
        {
            Debug.LogError("<color=#" + ColorUtility.ToHtmlStringRGB(Color.red) + ">" + "[AppManager] Unable to unload level " + levelName + "</color>");
            return;
        }
        ao.completed += OnUnloadOperationComplete;
    }

    void OnLoadOperationComplete(AsyncOperation ao)
    {
        if (_loadOperations.Contains(ao))
        {
            _loadOperations.Remove(ao);

            if (_loadOperations.Count == 0)
            {
                UpdateAppState(_appStartState);
            }
        }
        Debug.Log("<color=#" + ColorUtility.ToHtmlStringRGB(Color.green) + ">" + "Load Complete." + "</color>");
        if (AppEvents.sceneLoading != null)
        {
            AppEvents.sceneLoaded.Invoke();
        }
    }

    void OnUnloadOperationComplete(AsyncOperation ao)
    {
        Debug.Log("<color=#" + ColorUtility.ToHtmlStringRGB(Color.green) + ">" + "Unload Complete." + "</color>");
        if (AppEvents.sceneLoading != null)
        {
            AppEvents.sceneUnloaded.Invoke();
        }
    }

    #endregion

    void UpdateAppState(AppState state)
    {
        AppState previousAppState = _currentAppState;
        _currentAppState = state;
        Debug.Log("Previous: " + previousAppState + " Current: " + _currentAppState);
        if (AppEvents.sceneLoading != null)
        {
            AppEvents.stateChange.Invoke(_currentAppState, previousAppState);
        }
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
        LoadScene(initScene);
    }

    public void QuitApp()
    {
        //Features for quitting, like Auto Save etc;
        Application.Quit();
    }
}
