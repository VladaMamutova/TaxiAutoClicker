using System.Runtime.Serialization;

namespace TaxiAutoClicker.WindowActions
{
    [DataContract]
    public struct Input
    {
        [DataMember]
        public string Description;
        [DataMember]
        public int Delay;

        public Input(string description, int delay)
        {
            Description = description;
            Delay = delay;
        }
    }
}