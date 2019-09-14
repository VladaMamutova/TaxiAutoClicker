using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace TaxiAutoClicker.WinAPI
{
    /// <summary>
    /// Эмулирует движения мыши и нажатия кнопки.
    /// </summary>
    class VirtualMouse
    {
        [DllImport("user32.dll", EntryPoint = "mouse_event")]
        private static extern void MouseEvent(MOUSEEVENTF dwFlags, int dx, int dy, int dwData, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref Point lpPoint);

        // Для удобства использования создаем перечисление с необходимыми флагами (константами),
        // которые определяют действия мыши.
        [Flags]
        private enum MOUSEEVENTF
        {
            MOVE = 0x0001,
            LEFTDOWN = 0x0002,
            LEFTUP = 0x0004,
            RIGHTDOWN = 0x0800,
            RIGHTUP = 0x0010,
            ABSOLUTE = 0x8000
        }

        private static readonly Rectangle ScreenSize = Screen.PrimaryScreen.Bounds;

        public static void SendMouseLeftClick(Point p)
        {
            int x = p.X * 65536 / ScreenSize.Width;
            int y = p.Y * 65536 / ScreenSize.Height;
            
            MouseEvent(MOUSEEVENTF.ABSOLUTE | MOUSEEVENTF.LEFTDOWN | MOUSEEVENTF.LEFTUP, x, y, 0, UIntPtr.Zero);
        }

        public static void SendMouseMovement(Point p)
        {
            int x = p.X * 65536 / ScreenSize.Width;
            int y = p.Y * 65536 / ScreenSize.Height;

            MouseEvent(MOUSEEVENTF.ABSOLUTE | MOUSEEVENTF.MOVE, x, y, 0, UIntPtr.Zero);
        }

        void SendMouseRightClick(Point p)
        {
            MouseEvent(MOUSEEVENTF.RIGHTDOWN | MOUSEEVENTF.RIGHTUP, p.X, p.Y, 0, UIntPtr.Zero);
        }

        public static void SendMouseDoubleClick(Point p)
        {
            int x = p.X * 65536 / ScreenSize.Width;
            int y = p.Y * 65536 / ScreenSize.Height;

            MouseEvent(MOUSEEVENTF.LEFTDOWN | MOUSEEVENTF.LEFTUP, x, y, 0, UIntPtr.Zero);
            Thread.Sleep(150);
            MouseEvent(MOUSEEVENTF.LEFTDOWN | MOUSEEVENTF.LEFTUP, x, y, 0, UIntPtr.Zero);
        }

        void SendMouseRightDoubleClick(Point p)
        {
            MouseEvent(MOUSEEVENTF.RIGHTDOWN | MOUSEEVENTF.RIGHTUP, p.X, p.Y, 0, UIntPtr.Zero);

            Thread.Sleep(150);

            MouseEvent(MOUSEEVENTF.RIGHTDOWN | MOUSEEVENTF.RIGHTUP, p.X, p.Y, 0, UIntPtr.Zero);
        }

        void SendMouseDown()
        {
            MouseEvent(MOUSEEVENTF.LEFTDOWN, 50, 50, 0, UIntPtr.Zero);
        }

        void SendMouseUp()
        {
            MouseEvent(MOUSEEVENTF.LEFTUP, 50, 50, 0, UIntPtr.Zero);
        }
    }
}
