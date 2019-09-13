namespace TaxiAutoClicker
{
    public struct BoltUser
    {
        public string ApiKey;
        public string Mail;
        public string FirstName;
        public string Surname;

        public BoltUser(string apiKey, string mail, string firstName, string surname)
        {
            ApiKey = apiKey;
            Mail = mail;
            FirstName = firstName;
            Surname = surname;
        }
    }
}