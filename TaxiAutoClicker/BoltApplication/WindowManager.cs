using System;
using System.Drawing;

namespace TaxiAutoClicker.BoltApplication
{
    class WindowManager
    {
        private ClickManager _clickManager;
        private User _user;
        private Point _leftTop;
        private int _width;
        private int _height;

        public IntPtr Handle { get; }
        
        public WindowManager(IntPtr handle, User user)
        {
            Handle = handle;
            _user = user;
            WinAPI.WindowService.GetWindowRect(handle, out var rect);
            _leftTop = new Point(rect.Left, rect.Y);
            _width = rect.Width - rect.X;
            _height = rect.Height - rect.Y;
            _clickManager = ClickManager.GetClickManager();
        }

        public void StartOrderingATaxi()
        {

        }
    }
}
