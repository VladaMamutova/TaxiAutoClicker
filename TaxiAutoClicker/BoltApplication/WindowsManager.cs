using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using TaxiAutoClicker.InputActions;
using TaxiAutoClicker.SMSActivateAPI;
using TaxiAutoClicker.WinAPI;
using Point = System.Drawing.Point;

namespace TaxiAutoClicker.BoltApplication
{
    class WindowsManager
    {
        private readonly User _user;
        private readonly ClickManager _clickManager;
        private readonly Dictionary<IntPtr, WindowRectangle> _windows;
        private readonly Dictionary<IntPtr, Profile> _profiles;
        private readonly Dictionary<IntPtr, bool> _errors;

        public static string AddressesFilePath { get; } =
            "Script files\\Addresses.txt";

        public static string NumbersFilePath { get; } =
            "Телефоны водителей.txt";

        public WindowsManager(User user)
        {
            _user = user;
            _clickManager = ClickManager.GetClickManager();
            _windows = new Dictionary<IntPtr, WindowRectangle>();
            _profiles = new Dictionary<IntPtr, Profile>();
            _errors = new Dictionary<IntPtr, bool>();
        }

        public void AddWindow(IntPtr handle)
        {
            WindowService.GetWindowRect(handle, out var rect);
            WindowRectangle windowRectangle = new WindowRectangle(
                new Point(rect.Left, rect.Y),
                rect.Width - rect.X, rect.Height - rect.Y);

            if (!_windows.ContainsKey(handle))
            {
                _windows.Add(handle, windowRectangle);
            }
            else
            {
                _windows[handle] = windowRectangle;
            }

            if (!_profiles.ContainsKey(handle))
            {
                _profiles.Add(handle, new Profile(_user.ApiKey));
            }
            else
            {
                _profiles[handle] = new Profile(_user.ApiKey);
            }

            if (!_errors.ContainsKey(handle))
            {
                _errors.Add(handle, false);
            }
            else
            {
                _errors[handle] = false;
            }
        }

        private void PerformClick(WindowRectangle window, Click click,
            bool needDelay = true)
        {
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            if (click == null)
            {
                throw new ArgumentNullException(nameof(click));
            }

            Point point = window.GetScreenPoint(click.Position);
            VirtualMouse.SendLeftClick(point);

            if (needDelay)
            {
                Thread.Sleep(click.Delay);
            }
        }

        private TimeSpan EnterPhone()
        {
            Stopwatch startTime = new Stopwatch();

            int lastAction = 0;
            foreach (var window in _windows)
            {
                if (!_errors[window.Key])
                {
                    int i = 0;

                    // Click phone number field
                    PerformClick(window.Value,
                        _clickManager.RegistrationClicks[i++]);

                    // Enter phone number
                    var response = _profiles[window.Key].GetNumber();
                    if (response.IsError)
                    {
                        string error =
                            "Не удалось получить виртуальный номер: " +
                            response.Code;
                        _errors[window.Key] = true;
                        if (_windows.Count - _errors.Count(e => e.Value) < 1)
                        {
                            throw new Exception(error);
                        }

                        Log.Add(WindowService.GetWindowText(window.Key) +
                                ": " + error + ".");
                    }
                    else
                    {
                        VirtualKeyboard.PrintText(window.Key,
                            _profiles[window.Key].Number);
                        Thread.Sleep(_clickManager.KeyboardInputs[i++].Delay);

                        // Click Next
                        PerformClick(window.Value,
                            _clickManager.RegistrationClicks[i],
                            false);
                    }

                    // После окончания выполнения действий в первом окне, запускаем таймер.
                    if (!startTime.IsRunning)
                    {
                        startTime = Stopwatch.StartNew();
                    }

                    if (lastAction == 0)
                    {
                        lastAction = i;
                    }
                }
            }

            TimeSpan requiredDelay = TimeSpan.Zero;
            if (lastAction != 0 && _clickManager
                    .RegistrationClicks.ContainsKey(lastAction))
            {
                requiredDelay =
                    TimeSpan.FromMilliseconds(_clickManager
                        .RegistrationClicks[lastAction].Delay);
            }

            startTime.Stop();

            TimeSpan resultDelay = startTime.Elapsed;

            return requiredDelay - resultDelay > TimeSpan.Zero
                ? requiredDelay - resultDelay
                : TimeSpan.Zero;
        }

        private TimeSpan EnterCode()
        {
            Stopwatch startTime = new Stopwatch();

            int lastAction = 0;
            foreach (var window in _windows)
            {
                if (!_errors[window.Key])
                {
                    int i = 3;

                    // Enter code
                    Response response = _profiles[window.Key].GetCode();
                    if (response.IsError)
                    {
                        string error =
                            "Не удалось получить код на виртуальный номер: " +
                            response.Code;
                        _errors[window.Key] = true;
                        if (_windows.Count - _errors.Count(e => e.Value) < 1)
                        {
                            throw new Exception(error);
                        }

                        Log.Add(WindowService.GetWindowText(window.Key) +
                                ": " + error + ".");
                    }
                    else
                    {
                        // Click 1st field of code
                        PerformClick(window.Value,
                            _clickManager.RegistrationClicks[i++]);

                        Thread.Sleep(300);

                        VirtualKeyboard.PrintNumber(window.Key,
                            _profiles[window.Key].Code, 500);

                        // После окончания выполнения действий в первом окне, запускаем таймер.
                        if (!startTime.IsRunning)
                        {
                            startTime = Stopwatch.StartNew();
                        }

                        if (lastAction == 0)
                        {
                            lastAction = i;
                        }
                    }
                }
            }

            TimeSpan requiredDelay = TimeSpan.Zero;
            if (lastAction != 0 && _clickManager
                    .KeyboardInputs.ContainsKey(lastAction))
            {
                requiredDelay =
                    TimeSpan.FromMilliseconds(_clickManager
                        .KeyboardInputs[lastAction].Delay);
            }

            startTime.Stop();

            TimeSpan resultDelay = startTime.Elapsed;

            return requiredDelay - resultDelay > TimeSpan.Zero
                ? requiredDelay - resultDelay
                : TimeSpan.Zero;
        }

        private TimeSpan EnterUserData()
        {
            Stopwatch startTime = new Stopwatch();

            int lastAction = 0;
            foreach (var window in _windows)
            {
                if (!_errors[window.Key])
                {
                    try
                    {
                        int i = 5;

                        // Click e-mail field
                        PerformClick(window.Value,
                            _clickManager.RegistrationClicks[i++]);

                        // Enter e-mail
                        VirtualKeyboard.PrintText(window.Key, _user.Mail);
                        Thread.Sleep(_clickManager.KeyboardInputs[i++].Delay);

                        // Press Enter
                        VirtualKeyboard.PressEnter();
                        Thread.Sleep(_clickManager.EnterPresses[i++].Delay);

                        // Enter first name
                        VirtualKeyboard.PrintText(window.Key, _user.FirstName);
                        Thread.Sleep(_clickManager.KeyboardInputs[i++].Delay);

                        // Press Enter
                        VirtualKeyboard.PressEnter();
                        Thread.Sleep(_clickManager.EnterPresses[i++].Delay);

                        // Enter last name
                        VirtualKeyboard.PrintText(window.Key, _user.Surname);
                        Thread.Sleep(_clickManager.KeyboardInputs[i++].Delay);

                        // Click Next
                        PerformClick(window.Value,
                            _clickManager.RegistrationClicks[i],
                            false);

                        // После окончания выполнения действий в первом окне, запускаем таймер.
                        if (!startTime.IsRunning)
                        {
                            startTime = Stopwatch.StartNew();
                        }

                        if (lastAction == 0)
                        {
                            lastAction = i;
                        }
                    }
                    catch (Exception ex)
                    {
                        _errors[window.Key] = true;
                        if (_windows.Count - _errors.Count(e => e.Value) < 1)
                        {
                            throw new Exception(ex.Message);
                        }

                        Log.Add(WindowService.GetWindowText(window.Key) +
                                ": " + ex.Message + ".");
                    }
                }
            }

            TimeSpan requiredDelay = TimeSpan.Zero;
            if (lastAction != 0 && _clickManager
                    .RegistrationClicks.ContainsKey(lastAction))
            {
                requiredDelay =
                    TimeSpan.FromMilliseconds(_clickManager
                        .RegistrationClicks[lastAction].Delay);
            }

            startTime.Stop();

            TimeSpan resultDelay = startTime.Elapsed;

            return requiredDelay - resultDelay > TimeSpan.Zero
                ? requiredDelay - resultDelay
                : TimeSpan.Zero;
        }

        public TimeSpan EnterAddresses()
        {
            Stopwatch startTime = new Stopwatch();

            string[] addresses;
            try
            {
                addresses =
                    File.ReadAllLines(AddressesFilePath, Encoding.Default);
            }
            catch (Exception ex)
            {
                throw new Exception("Невозможно прочесть адреса из файла \"" +
                                    AddressesFilePath + "\": " + ex.Message);
            }

            int lastAction = 0;
            foreach (var window in _windows)
            {
                if (!_errors[window.Key])
                {
                    try
                    {
                        // Click Search destination
                        // Click Pickup location
                        // Click x in Pickup location Field
                        // Click Where to?
                        // Click x in Where to? Field
                        // Click Pickup location

                        int index = _clickManager.RegistrationActionsCount;
                        for (int i = index; i < index + 6; i++)
                        {
                            PerformClick(window.Value,
                                _clickManager.TaxiOrderingClicks[i]);
                        }

                        index += 6;

                        // Enter address from
                        var addressFromIndex =
                            new Random().Next(0, addresses.Length);
                        VirtualKeyboard.PrintText(window.Key,
                            addresses[addressFromIndex]);
                        Thread.Sleep(
                            _clickManager.KeyboardInputs[index++].Delay);

                        // Click address from
                        PerformClick(window.Value,
                            _clickManager.TaxiOrderingClicks[index++]);

                        // Click Where to?
                        PerformClick(window.Value,
                            _clickManager.TaxiOrderingClicks[index++]);

                        // Enter address to
                        int addressToIndex;
                        do
                        {
                            addressToIndex =
                                new Random().Next(0, addresses.Length);
                        } while (addressToIndex == addressFromIndex);

                        VirtualKeyboard.PrintText(window.Key,
                            addresses[addressToIndex]);
                        Thread.Sleep(
                            _clickManager.KeyboardInputs[index++].Delay);

                        // Click address to
                        PerformClick(window.Value,
                            _clickManager.TaxiOrderingClicks[index], false);

                        // После окончания выполнения действий в первом окне, запускаем таймер.
                        if (!startTime.IsRunning)
                        {
                            startTime = Stopwatch.StartNew();
                        }

                        if (lastAction == 0)
                        {
                            lastAction = index;
                        }
                    }
                    catch (Exception ex)
                    {
                        _errors[window.Key] = true;
                        if (_windows.Count - _errors.Count(e => e.Value) < 1)
                        {
                            throw new Exception(ex.Message);
                        }

                        Log.Add(WindowService.GetWindowText(window.Key) +
                                ": " + ex.Message + ".");
                    }
                }
            }

            TimeSpan requiredDelay = TimeSpan.Zero;
            if (lastAction != 0 && _clickManager
                    .TaxiOrderingClicks.ContainsKey(lastAction))
            {
                requiredDelay =
                    TimeSpan.FromMilliseconds(_clickManager
                        .TaxiOrderingClicks[lastAction].Delay);
            }

            startTime.Stop();

            TimeSpan resultDelay = startTime.Elapsed;

            return requiredDelay - resultDelay > TimeSpan.Zero
                ? requiredDelay - resultDelay
                : TimeSpan.Zero;
        }

        public void RegisterInBolt()
        {
            var resultDelay = EnterPhone();
            if (resultDelay != TimeSpan.Zero)
            {
                Thread.Sleep(resultDelay);
            }

            resultDelay = EnterCode();
            if (resultDelay != TimeSpan.Zero)
            {
                Thread.Sleep(resultDelay);
            }

            resultDelay = EnterUserData();
            if (resultDelay != TimeSpan.Zero)
            {
                Thread.Sleep(resultDelay);
            }

            foreach (var profile in _profiles)
            {
                profile.Value.CompleteWorkWithNumber();
            }
        }

        public void OrderATaxi()
        {
            var resultDelay = EnterAddresses();
            if (resultDelay != TimeSpan.Zero)
            {
                Thread.Sleep(resultDelay);
            }

            int startIndex = 23;
            KeyValuePair<IntPtr, WindowRectangle>[]
                windows = _windows.ToArray();

            Stopwatch nextDelay = Stopwatch.StartNew();

            for (int i = 0; i < windows.Length; i++)
            {
                int index = startIndex;
                if (nextDelay.IsRunning)
                {
                    nextDelay.Stop();
                    resultDelay = TimeSpan.FromMilliseconds(_clickManager
                                      .TaxiOrderingClicks[index].Delay) -
                                  nextDelay.Elapsed;

                    if (resultDelay > TimeSpan.Zero)
                    {
                        Thread.Sleep(resultDelay);
                    }
                }

                // Запускаем поиск такси в первом окне.
                // Click Select Bolt
                PerformClick(windows[i].Value,
                    _clickManager.TaxiOrderingClicks[index++]);

                // Click Request Bolt
                PerformClick(windows[i].Value,
                    _clickManager.TaxiOrderingClicks[index++]);

                // Click OK in messagebox (only once)
                PerformClick(windows[i].Value,
                    _clickManager.TaxiOrderingClicks[index++]);

                // Click chevron-up button
                PerformClick(windows[i].Value,
                    _clickManager.TaxiOrderingClicks[index++]);

                // Click Call
                PerformClick(windows[i].Value,
                    _clickManager.TaxiOrderingClicks[index++]);

                // DoubleClick to copy number

                VirtualMouse.SendMouseDoubleClick(windows[i].Value
                    .GetScreenPoint(_clickManager.TaxiOrderingClicks[index]
                        .Position));

                // Ожидание для гарантии, что номер успел скопироваться в буфер обмена.
                Thread.Sleep(1000);

                // Сохранение номера телефона водителя.
                SaveNumberFromBuffer();

                Thread.Sleep(_clickManager.TaxiOrderingClicks[index++].Delay);

                // Click Home
                PerformClick(windows[i].Value,
                    _clickManager.TaxiOrderingClicks[index++]);

                // Click Bolt icon
                PerformClick(windows[i].Value,
                    _clickManager.TaxiOrderingClicks[index], false);

                Stopwatch startTime = Stopwatch.StartNew();
                if (i + 1 < windows.Length)
                {
                    // Запускаем поиск такси в следующем окне, если такое есть.
                    // Click Select Bolt
                    PerformClick(windows[i + 1].Value,
                        _clickManager.TaxiOrderingClicks[startIndex]);

                    // Click Request Bolt
                    PerformClick(windows[i + 1].Value,
                        _clickManager.TaxiOrderingClicks[startIndex + 1]);

                    // Click OK in messagebox (only once)
                    PerformClick(windows[i + 1].Value,
                        _clickManager.TaxiOrderingClicks[startIndex + 2],
                        false);

                    // Запускаем отсчёт времени для следующего окна.
                    nextDelay = Stopwatch.StartNew();
                }

                startTime.Stop();
                resultDelay = TimeSpan.FromMilliseconds(_clickManager
                                  .TaxiOrderingClicks[index++].Delay) -
                              startTime.Elapsed;

                if (resultDelay > TimeSpan.Zero)
                {
                    Thread.Sleep(resultDelay);
                }

                // Click Cancel
                PerformClick(windows[i].Value,
                    _clickManager.TaxiOrderingClicks[index++]);

                // Click Cancel ride
                PerformClick(windows[i].Value,
                    _clickManager.TaxiOrderingClicks[index++]);

                // Click reason for cancellation
                int reasonIndex = new Random().Next(0, 6); // всего 6 причин
                int caseAbsolutePosition =
                    450 + 41 *
                    reasonIndex; // (y=450 - первая причина на экране с h=768)
                double caseRelativePosition =
                    caseAbsolutePosition / 768.0;

                Point point = windows[i].Value.GetScreenPoint(new PointF(0.5f,
                    (float) (_clickManager.TaxiOrderingClicks[index].Position
                                 .Y * caseRelativePosition)));

                VirtualMouse.SendLeftClick(point);
                Thread.Sleep(_clickManager.TaxiOrderingClicks[index].Delay);
            }
        }

        public void ClearDataInBolt()
        {
            int startIndex = _clickManager.RegistrationActionsCount +
                             _clickManager.TaxiOrderingActionsCount;

            Stopwatch delay = new Stopwatch();
            
            for (int i = startIndex;
                i < _clickManager.DataCleaningActionsCount + startIndex;
                i+=2)
            {
                if (delay.IsRunning)
                {
                    delay.Stop();
                    TimeSpan resultDelay = TimeSpan.FromMilliseconds(_clickManager
                                               .DataCleaningClicks[i - 1].Delay) -
                                           delay.Elapsed;

                    if (resultDelay > TimeSpan.Zero)
                    {
                        Thread.Sleep(resultDelay);
                    }
                }

                foreach (var window in _windows)
                {
                    if (!_errors[window.Key])
                    {
                        PerformClick(window.Value,
                            _clickManager.DataCleaningClicks[i]);
                        if (i + 1 < _clickManager.DataCleaningActionsCount +
                            startIndex)
                        {
                            PerformClick(window.Value,
                                _clickManager.DataCleaningClicks[i + 1], false);
                        }

                        if (!delay.IsRunning)
                        {
                            delay = Stopwatch.StartNew();
                        }
                    }
                }
            }
        }


        public void SaveNumberFromBuffer()
        {
            IDataObject iData = Clipboard.GetDataObject();
            if (iData != null &&
                iData.GetDataPresent(DataFormats.Text))
            {

                string phoneNumber =
                    (string)iData.GetData(DataFormats.Text);
                if (phoneNumber != null &&
                    phoneNumber.StartsWith("+7"))
                {
                    File.AppendAllLines(NumbersFilePath,
                        new[] { phoneNumber });
                }
            }
        }
    }
}