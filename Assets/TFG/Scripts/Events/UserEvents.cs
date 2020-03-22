using UnityEngine.Events;
using TFG.Enum;

namespace TFG.Events
{
    public static class UserEvents
    {
        [System.Serializable] public class EventUserState : UnityEvent<UserState, UserState> { }
    }
}
