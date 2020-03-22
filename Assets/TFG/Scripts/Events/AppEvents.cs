using UnityEngine.Events;
using TFG.Enum;

namespace TFG.Events
{
    public static class AppEvents
    {
        public class StateChange : UnityEvent<AppState, AppState> { };
        public class SceneLoading : UnityEvent<string> { };
        public class SceneUnloading : UnityEvent<string> { };
        public class SceneLoadComplete : UnityEvent { };
        public class SceneUnloadComplete : UnityEvent { };


        public static StateChange stateChange = new StateChange();
        public static SceneLoading sceneLoading = new SceneLoading();
        public static SceneUnloading sceneUnloading = new SceneUnloading();
        public static SceneLoadComplete sceneLoaded = new SceneLoadComplete();
        public static SceneUnloadComplete sceneUnloaded = new SceneUnloadComplete();
    }
}
