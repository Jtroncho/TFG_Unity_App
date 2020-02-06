namespace TFG.User
{
    public class User
    {
        string _name = string.Empty;
        string _email = string.Empty;
        string _uid = string.Empty;
        string _profileImg = string.Empty;
        string _alias = string.Empty;

        public string Name
        {
            get { return _name; }
            private set { _name = value; }
        }

        public string Alias
        {
            get { return _alias; }
            private set { _alias = value; }
        }

        public string Email
        {
            get { return _email; }
            private set { _email = value; }
        }

        public string UID
        {
            get { return _uid; }
            private set { _uid = value; }
        }

        public string ProfileImage
        {
            get { return _profileImg; }
            private set { _profileImg = value; }
        }
    }
}
