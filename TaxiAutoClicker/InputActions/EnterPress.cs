using System.Runtime.Serialization;

namespace TaxiAutoClicker.InputActions
{
    [DataContract]
    public class EnterPress
    {
        [DataMember]
        public string Description;
        [DataMember]
        public int Delay;

        public EnterPress(string description, int delay)
        {
            Description = description;
            Delay = delay;
        }
    }
}