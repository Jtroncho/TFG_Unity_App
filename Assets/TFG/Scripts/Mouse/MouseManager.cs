using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MouseManager : MonoBehaviour
{
    public LayerMask clickableLayer;

    public Texture2D pointer;
    public Texture2D target;
    public Texture2D doorway;

    public EventVector3 OnClickEnvironment;

    private bool _useDefaultCursor = false;

    private void Start()
    {
        GameManager.Instance.OnGameStateGanged.AddListener(HandleGameStateChanged);
    }

    void Update()
    {
        if(_useDefaultCursor)
        {
            Cursor.SetCursor(pointer, new Vector2(16, 16), CursorMode.Auto);
            return;
        }

        //Here to use another type of cursor outside pause Menu
    }

    void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        _useDefaultCursor = currentState == GameManager.GameState.PAUSED;
    }
}

[System.Serializable]
public class EventVector3 : UnityEvent<Vector3> { }
