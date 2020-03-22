using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TFG.Events;
using TFG.Enum;

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
        AppEvents.stateChange.AddListener(HandleGameStateChanged);
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

    void HandleGameStateChanged(AppState currentState, AppState previousState)
    {
        _useDefaultCursor = currentState == AppState.PAUSE;
    }
}

[System.Serializable]
public class EventVector3 : UnityEvent<Vector3> { }
