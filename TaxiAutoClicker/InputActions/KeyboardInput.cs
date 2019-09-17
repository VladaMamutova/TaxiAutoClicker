using System.Runtime.Serialization;

namespace TaxiAutoClicker.InputActions
{
    [DataContract]
    public class KeyboardInput
    {
        [DataMember]
        public string Description;
        [DataMember]
        public int Delay;

        public KeyboardInput(string description, int delay)
        {
            Description = description;
            Delay = delay;
        }
    }
}