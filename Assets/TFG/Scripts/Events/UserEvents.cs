using UnityEngine.Events;
using TFG.Enum;

namespace TFG.Events
{
    public static class UserEvents
    {
        public class UserSignIn : UnityEvent { };
        public class UserSignOut : UnityEvent { };

        public static UserSignIn userSignIn = new UserSignIn();
        public static UserSignOut userSignOut = new UserSignOut();
    }
}
