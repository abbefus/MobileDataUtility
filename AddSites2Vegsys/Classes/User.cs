namespace AddSites2Vegsys
{
    public class User
    {
        private string sUsername;

        public string Username
        {
            get { return sUsername; }
            set { sUsername = value; }
        }

        private string sFirstName;

        public string FirstName
        {
            get { return sFirstName; }
            set { sFirstName = value; }
        }

        private string sLastName;

        public string LastName
        {
            get { return sLastName; }
            set { sLastName = value; }
        }

        private bool mblnIsAdmin;
        public bool IsAdmin
        {
            get { return mblnIsAdmin; }
            set { mblnIsAdmin = value; }
        }
		
    }
}
