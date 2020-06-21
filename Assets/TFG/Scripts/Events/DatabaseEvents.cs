using UnityEngine.Events;
using TFG.Enum;

namespace TFG.Events
{
    public static class DatabaseEvents
    {
        public class DataRetrieved : UnityEvent<string> { };
        public class CercanasRetrieved : UnityEvent { };
        //public class UserSignOut : UnityEvent { };

        public static DataRetrieved dataRetrieved = new DataRetrieved();
        public static CercanasRetrieved cercanasRetrieved = new CercanasRetrieved();
        //public static UserSignOut userSignOut = new UserSignOut();
    }
}
