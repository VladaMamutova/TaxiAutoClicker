using System;
using System.IO;

namespace TaxiAutoClicker.BoltApplication
{
    static class Log
    {
        private const string FileName = "Log.txt";

        public static void Add(string message)
        {
            File.AppendAllText(FileName,
                DateTime.Today.ToString("dd.MM.yyyy") + @"   " +
                DateTime.Now.ToString("HH:mm:ss   ") + message +
                Environment.NewLine);
        }

        public static void CreateNew()
        {
            File.Create(FileName);
        }
    }
}
