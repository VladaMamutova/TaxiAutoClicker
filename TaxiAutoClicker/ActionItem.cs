using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TaxiAutoClicker
{
    public class ActionItem:INotifyPropertyChanged
    {
        public int Number { get; set; }
        public string Description { get; set; }
        public string Position { get; set; }
        public int Delay { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(
            [CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}