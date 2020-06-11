using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TFG.Database
{
    public class LeaderboardEntry
    {
        public string uid = "";
        public string email = "";
        public long score = 0;

        public LeaderboardEntry()
        {
        }

        public LeaderboardEntry(string uid, string email, long score)
        {
            this.uid = uid;
            this.email = email;
            this.score = score;
        }

        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            result["email"] = email;
            result["score"] = score;

            return result;
        }
    }
}

