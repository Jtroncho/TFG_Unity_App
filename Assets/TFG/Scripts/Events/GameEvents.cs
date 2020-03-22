using UnityEngine.Events;
using TFG.Enum;

namespace TFG.Events
{
    public static class GameEvens
    {
        [System.Serializable] public class EventGameState : UnityEvent<GameState, GameState> { }
    }
}