using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Json;
using TaxiAutoClicker.WindowActions;

namespace TaxiAutoClicker.BoltApplication
{
    class ClickManager
    {
        private static ClickManager _clickManager;
        private static readonly object SyncRoot = new object();

        private Dictionary<int, Click> _getNumberClicks;
        private Dictionary<int, Click> _orderATaxiClicks;
        private Dictionary<int, Click> _clearDataClicks;
        private Dictionary<int, Input> _inputs;
        private Dictionary<int, EnterPresses> _enterPresses;

        private ClickManager()
        {
            _getNumberClicks = new Dictionary<int, Click>();
            _orderATaxiClicks = new Dictionary<int, Click>();
            _clearDataClicks = new Dictionary<int, Click>();
            _inputs = new Dictionary<int, Input>();
            _enterPresses = new Dictionary<int, EnterPresses>();
        }

        public static ClickManager GetClickManager()
        {
            if (_clickManager == null)
            {
                lock (SyncRoot)
                {
                    _clickManager = new ClickManager();
                }
            }

            return _clickManager;
        }

        public void SaveClicksToFile()
        {
            DataContractJsonSerializer clickJsonFormatter = new DataContractJsonSerializer(typeof(Click));

            using (FileStream fs = new FileStream("Clicks.json", FileMode.OpenOrCreate))
            {
                //clickJsonFormatter.WriteObject(fs, _clicks);
            }
            //using (FileStream filetream = new FileStream("Clicks", FileMode.OpenOrCreate))
            //{
            //    foreach (var click in _clicks)
            //    {
            //        byte[] array = System.Text.Encoding.UTF8.GetBytes(click.Value.ToString() + Environment.NewLine);
            //        filetream.Write(array, 0, array.Length);
            //    }

            //}
        }

        public void LoadClicksFromFile()
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(Click[]));

            using (FileStream fs = new FileStream("Clicks.json", FileMode.OpenOrCreate))
            {
                Click[] clicks = (Click[])jsonFormatter.ReadObject(fs);

                foreach (Click click in clicks)
                {
                    //AddClick(click);
                }
            }
        }

        public void PerformGettingNumber()
        {

        }

        public void InitializeDefaultClicks()
        {
            int number = 1;
            _getNumberClicks.Add(number++, new Click(new PointF(0.5f, 0.5f), "Click number field", 0));
            _inputs.Add(number++, new Input("Enter Number", 1000));
            _getNumberClicks.Add(number++, new Click(new PointF(0.5f, 0.941f), "Click Next", 0));
            _inputs.Add(number++, new Input("Enter the 1st digit of code", 300));
            _inputs.Add(number++, new Input("Enter the 2nd digit of code", 300));
            _inputs.Add(number++, new Input("Enter the 3rd digit of code", 300));
            _inputs.Add(number++, new Input("Enter the 4th digit of code", 3000));
            _getNumberClicks.Add(number++, new Click(new PointF(0.5f, 0.275f), "Click mail field", 1000));
            _inputs.Add(number++, new Input("Enter mail", 1500));
            _enterPresses.Add(number++, new EnterPresses("Press Enter", 700));
            _inputs.Add(number++, new Input("Enter first name", 1500));
            _enterPresses.Add(number++, new EnterPresses("Press Enter", 700));
            _inputs.Add(number++, new Input("Enter surname", 1500));
            _getNumberClicks.Add(number++, new Click(new PointF(0.5f, 0.945f), "Click Next", 4500));

            _orderATaxiClicks.Add(number++, new Click(new PointF(0.5f, 0.918f), "Click Search destination", 1000));
            _orderATaxiClicks.Add(number++, new Click(new PointF(0.5f, 0.15f), "Click Pickup location", 1000));
            _orderATaxiClicks.Add(number++, new Click(new PointF(0.915f, 0.15f), "Click x in Pickup location Field", 500));
            _orderATaxiClicks.Add(number++, new Click(new PointF(0.5f, 0.2f), "Click Where to?", 1000));
            _orderATaxiClicks.Add(number++, new Click(new PointF(0.915f, 0.2f), "Click x in Where to? Field", 500));
            _orderATaxiClicks.Add(number++, new Click(new PointF(0.5f, 0.15f), "Click Pickup location", 500));
            _inputs.Add(number++, new Input("Enter address trom", 1500));
            _orderATaxiClicks.Add(number++, new Click(new PointF(0.5f, 0.367f), "Click address from", 500));
            _orderATaxiClicks.Add(number++, new Click(new PointF(0.5f, 0.2f), "Click Where to?", 500));
            _inputs.Add(number++, new Input("Enter address to", 1500));
            _orderATaxiClicks.Add(number++, new Click(new PointF(0.5f, 0.255f), "Click address to", 7000));
            _orderATaxiClicks.Add(number++, new Click(new PointF(0.5f, 0.944f), "Click Select Bolt", 3000));
            _orderATaxiClicks.Add(number++, new Click(new PointF(0.5f, 0.944f), "Click Request Bolt", 15000));
            _orderATaxiClicks.Add(number++, new Click(new PointF(0.903f, 0.893f), "Click driver photo", 1500));
            _orderATaxiClicks.Add(number++, new Click(new PointF(0.194f, 0.932f), "Click Contact", 500));
            _orderATaxiClicks.Add(number++, new Click(new PointF(0.5f, 0.842f), "Click Call", 3000));
            _orderATaxiClicks.Add(number++, new Click(new PointF(0.5f, 0.654f), "DoubleClick to copy number", 1500));
            _orderATaxiClicks.Add(number++, new Click(new PointF(1.029f, 0.92f), "Click Home", 1000));
            _orderATaxiClicks.Add(number++, new Click(new PointF(0.374f, 0.384f), "Click Bolt icon", 3000));
            _orderATaxiClicks.Add(number++, new Click(new PointF(0.903f, 0.893f), "Click driver photo", 1500));
            _orderATaxiClicks.Add(number++, new Click(new PointF(0.803f, 0.933f), "Click Cancel", 1000));
            _orderATaxiClicks.Add(number++, new Click(new PointF(0.745f, 0.944f), "Click Cancel Ride", 1000));
            _orderATaxiClicks.Add(number++, new Click(new PointF(0.5f, 1.0f), "Click reason for cancellation", 1000));

            _clearDataClicks.Add(number++, new Click(new PointF(1.029f, 0.92f), "Click Home", 1000));
            _clearDataClicks.Add(number++, new Click(new PointF(0.896f, 0.384f), "Click settings icon", 2000));
            _clearDataClicks.Add(number++, new Click(new PointF(0.5f, 0.759f), "Click applications in settings", 1000));
            _clearDataClicks.Add(number++, new Click(new PointF(0.5f, 0.247f), "Click Bolt in applications", 1000));
            _clearDataClicks.Add(number++, new Click(new PointF(0.735f, 0.434f), "Click Clear Data", 1000));
            _clearDataClicks.Add(number++, new Click(new PointF(0.801f, 0.599f), "Click OK", 1000));
            _clearDataClicks.Add(number, new Click(new PointF(1.029f, 0.92f), "Click Home", 1000));
        }
    }
}
