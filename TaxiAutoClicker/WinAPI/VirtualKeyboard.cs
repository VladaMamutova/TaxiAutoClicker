﻿using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace TaxiAutoClicker.WinAPI
{
    class VirtualKeyboard
    {
        private const int WM_CHAR = 0x0102;
        public const int WM_SYSUP = 0x0105;
        private const byte VK_CONTROL = 0x11;
        private const byte VK_RETURN = 0x0D;
        private const int KEYEVENTF_EXTENDEDKEY = 0x1;
        private const int KEYEVENTF_KEYUP = 0x2;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern void PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private static readonly object Locker = new object();

        public static void PrintText(IntPtr hWnd, string text)
        {
            lock (Locker)
            {
                foreach (var symb in text)
                {
                    PostMessage(hWnd, WM_CHAR, (IntPtr) symb, (IntPtr) 0);
                }
            }
        }

        public static void PrintNumber(IntPtr hWnd, string text, int delay)
        {
            lock (Locker)
            {
                foreach (var symb in text)
                {
                    PostMessage(hWnd, WM_SYSUP, (IntPtr)symb, (IntPtr)0);
                    Thread.Sleep(delay);
                }
            }
        }

        public static void Paste()
        {
            // Симулируем нажатие Ctrl.
            keybd_event(VK_CONTROL, 0x45, KEYEVENTF_EXTENDEDKEY | 0, UIntPtr.Zero);

            // Симулируем нажатие клавиши V.
            keybd_event(0x56, 0x45, KEYEVENTF_EXTENDEDKEY | 0, UIntPtr.Zero);

            // Отпускаем V.
            keybd_event(0x56, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, UIntPtr.Zero);

            // Отпускаем Ctrl.
            keybd_event(VK_CONTROL, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        public static void PressEnter()
        {
            keybd_event(VK_RETURN, 0x45, KEYEVENTF_EXTENDEDKEY | 0, UIntPtr.Zero);
            keybd_event(VK_RETURN, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, UIntPtr.Zero);
        }
    }
}
