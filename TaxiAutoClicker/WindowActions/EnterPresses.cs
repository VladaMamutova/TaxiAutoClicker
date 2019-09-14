using System.Runtime.Serialization;

namespace TaxiAutoClicker.WindowActions
{
    [DataContract]
    public struct EnterPresses
    {
        [DataMember]
        public string Description;
        [DataMember]
        public int Delay;

        public EnterPresses(string description, int delay)
        {
            Description = description;
            Delay = delay;
        }
    }
}