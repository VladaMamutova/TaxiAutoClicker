using System;
using System.Runtime.InteropServices;

namespace TaxiAutoClicker.WinAPI
{
    class KeyHook
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
                LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int VK_ESCAPE = 0x1B;

        static IntPtr _hookID = IntPtr.Zero;
        private static readonly LowLevelKeyboardProc HookProcDelegate;

        public static bool EscPressed;

        static KeyHook()
        {
            HookProcDelegate = HookCallback;
            Register();
        }
        
        public static void Register()
        {
            IntPtr hInstance = LoadLibrary("User32");
            _hookID = SetWindowsHookEx(WH_KEYBOARD_LL, HookProcDelegate, hInstance, 0);
        }

        public static void Unregister()
        {
            EscPressed = false;
            UnhookWindowsHookEx(_hookID);
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if (VK_ESCAPE == vkCode)
                {
                    EscPressed = true;
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
    }
}
