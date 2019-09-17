using System.Drawing;
using System.Runtime.Serialization;

namespace TaxiAutoClicker.InputActions
{
    [DataContract]
    public class Click
    {
        [DataMember]
        public PointF Position;
        [DataMember]
        public string Description;
        [DataMember]
        public int Delay;

        public Click(PointF position, string description, int delay)
        {
            Position = position;
            Description = description;
            Delay = delay;
        }

        public override string ToString()
        {
            return $"{nameof(Position.X)}={Position.X}, {nameof(Position.Y)}={Position.Y}, " +
                   $"{nameof(Description)}={Description}, {nameof(Delay)}={Delay}";
        }
    }
}