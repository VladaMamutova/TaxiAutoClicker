using System;
using System.Runtime.InteropServices;

namespace TaxiAutoClicker.WinAPIHelper
{
    class VirtualKeyboard
    {
        private const int WM_CHAR = 0x0102;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern void PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        public static void PrintText(IntPtr hWnd, string text)
        {
            foreach (var symb in text)
            {
                PostMessage(hWnd, WM_CHAR, (IntPtr) symb, (IntPtr) 0);
            }
        }
    }
}
