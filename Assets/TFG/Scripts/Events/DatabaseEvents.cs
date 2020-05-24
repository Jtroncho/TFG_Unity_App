using UnityEngine.Events;
using TFG.Enum;

namespace TFG.Events
{
    public static class DatabaseEvents
    {
        public class DataRetrieved : UnityEvent<string> { };
        //public class UserSignOut : UnityEvent { };

        public static DataRetrieved dataRetrieved = new DataRetrieved();
        //public static UserSignOut userSignOut = new UserSignOut();
    }
}
