using UnityEngine.Events;
using TFG.Enum;
using TFG.UI;

namespace TFG.Events
{
    public static class MenuEvents
    {
        /*[System.Serializable] public class StateChange : UnityEvent<MenuState, MenuState> { }

        public static StateChange stateChange = new StateChange();*/

        [System.Serializable] public class GroupChange : UnityEvent<UI_System> { }
        [System.Serializable] public class ScreenChange : UnityEvent<UI_Screen> { }

        public static GroupChange groupChange = new GroupChange();
        public static ScreenChange screenChange = new ScreenChange();
    }

}

