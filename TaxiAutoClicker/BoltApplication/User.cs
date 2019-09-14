namespace TaxiAutoClicker.BoltApplication
{
    public struct User
    {
        public string ApiKey;
        public string Mail;
        public string FirstName;
        public string Surname;

        public User(string apiKey, string mail, string firstName, string surname)
        {
            ApiKey = apiKey;
            Mail = mail;
            FirstName = firstName;
            Surname = surname;
        }
    }
}