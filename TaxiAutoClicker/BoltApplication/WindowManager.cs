using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using TaxiAutoClicker.SMSActivateAPI;
using TaxiAutoClicker.WinAPI;
using Point = System.Drawing.Point;

namespace TaxiAutoClicker.BoltApplication
{
    class WindowManager
    {
        private readonly ClickManager _clickManager;
        private readonly User _user;
        private readonly Profile _profile;
        private readonly Point _leftTop;
        private readonly int _width;
        private readonly int _height;

        private static readonly object DataInputLocker = new object();

        public IntPtr Handle { get; }

        public static string AddressesFilePath { get; } =
            "Script files\\Addresses.txt";

        public static string NumbersFilePath { get; } =
            "Телефоны водителей.txt";

        // default is false, set 1 for true.
        private int _threadSafeBoolBackValue;

        public bool ThreadSafeBool
        {
            get => Interlocked.CompareExchange(ref _threadSafeBoolBackValue, 1, 1) == 1;
            set
            {
                if (value) Interlocked.CompareExchange(ref _threadSafeBoolBackValue, 1, 0);
                else Interlocked.CompareExchange(ref _threadSafeBoolBackValue, 0, 1);
            }
        }

        public WindowManager(IntPtr handle, User user)
        {
            Handle = handle;
            _user = user;
            _profile = new Profile(user.ApiKey);
            WindowService.GetWindowRect(handle, out var rect);
            _leftTop = new Point(rect.Left, rect.Y);
            _width = rect.Width - rect.X;
            _height = rect.Height - rect.Y;
            _clickManager = ClickManager.GetClickManager();
        }

        public void RegisterInBolt()
        {
            for (int i = 0; i < _clickManager.RegistrationActionsCount; i++)
            {
                if (_clickManager.RegistrationClicks.ContainsKey(i))
                {
                    if (_clickManager.RegistrationClicks[i].Description !=
                        "Click 1st field of code")
                    {
                        var x = (int) (_width *
                                       _clickManager.RegistrationClicks[i]
                                           .Position
                                           .X) + _leftTop.X;
                        var y = (int) (_height *
                                       _clickManager.RegistrationClicks[i]
                                           .Position
                                           .Y) + _leftTop.Y;
                        //while (ThreadSafeBool)
                        //{
                        //    Thread.Sleep(100);
                        //}
                        VirtualMouse.SendLeftClick(new Point(x, y));
                        Thread.Sleep(_clickManager.RegistrationClicks[i].Delay);
                    }
                }
                else if (_clickManager.KeyboardInputs.ContainsKey(i))
                {
                    Response response;
                    string description =
                        _clickManager.KeyboardInputs[i].Description;
                    if (description == "Enter phone number")
                    {
                        //while (ThreadSafeBool)
                        //{
                        //    Thread.Sleep(500);
                        //}

                        //ThreadSafeBool = true;
                        //lock (DataInputLocker)
                        {
                            response = _profile.GetNumber();
                            if (response.IsError)
                            {
                                throw new Exception(
                                    "Не удалось получить виртуальный номер: " +
                                    response.Code);
                            }

                            VirtualKeyboard.PrintText(Handle, _profile.Number);
                            Thread.Sleep(_clickManager.KeyboardInputs[i].Delay);

                            i++;
                            var x = (int)(_width * _clickManager
                                              .RegistrationClicks[i]
                                              .Position.X) + _leftTop.X;
                            var y = (int)(_height * _clickManager
                                              .RegistrationClicks[i]
                                              .Position.Y) + _leftTop.Y;

                            VirtualMouse.SendLeftClick(new Point(x, y));
                            Thread.Sleep(_clickManager.RegistrationClicks[i].Delay);
                        }

                        //ThreadSafeBool = false;
                    }
                    else if (description == "Enter code")
                    {
                        response = _profile.GetCode();
                        if (response.IsError)
                        {
                            throw new Exception(
                                "Не удалось получить код на виртуальный номер: " +
                                response.Code);
                        }

                        // Непосредственно перед вводом данным кликаем по полю,
                        // в случае если оно стало неактивным вследствие
                        // действий пользователя или работы нескольких потоков
                        // с несколькими окнами.

                        var x = (int) (_width * _clickManager
                                           .RegistrationClicks[i - 1].Position
                                           .X) + _leftTop.X;
                        var y = (int) (_height * _clickManager
                                           .RegistrationClicks[i - 1].Position
                                           .Y) + _leftTop.Y;
                        VirtualMouse.SendLeftClick(new Point(x, y));
                        Thread.Sleep(300);
                        VirtualKeyboard.PrintNumber(Handle, _profile.Code, 500);
                        Thread.Sleep(_clickManager.KeyboardInputs[i].Delay);
                    }
                    else if (description == "Enter e-mail")
                    {
                        while (ThreadSafeBool)
                        {
                            Thread.Sleep(500);
                        }

                        ThreadSafeBool = true;
                        lock (DataInputLocker)
                        {
                            // Непосредственно перед вводом данным кликаем по полю,
                            // в случае если оно стало неактивным вследствие
                            // действий пользователя или работы нескольких потоков
                            // с несколькими окнами.
                            var x = (int)(_width * _clickManager
                                              .RegistrationClicks[i - 1]
                                              .Position.X) + _leftTop.X;
                            var y = (int)(_height * _clickManager
                                              .RegistrationClicks[i - 1]
                                              .Position.Y) + _leftTop.Y;

                            VirtualMouse.SendLeftClick(new Point(x, y));
                            VirtualKeyboard.PrintText(Handle, _user.Mail);
                            Thread.Sleep(_clickManager.KeyboardInputs[i].Delay);
                            i++;
                            for (int j = 0; j < 5; j++)
                            {
                                if (_clickManager.KeyboardInputs.ContainsKey(i))
                                {
                                    string input = "";
                                    if (_clickManager.KeyboardInputs[i]
                                            .Description == "Enter first name")
                                    {
                                        input = _user.FirstName;
                                    }
                                    else if (_clickManager.KeyboardInputs[i]
                                                 .Description ==
                                             "Enter last name")
                                    {
                                        input = _user.Surname;
                                    }

                                    VirtualKeyboard.PrintText(Handle, input);
                                    Thread.Sleep(_clickManager.KeyboardInputs[i]
                                        .Delay);
                                }
                                else if (_clickManager.EnterPresses.ContainsKey(
                                    i))
                                {
                                    VirtualKeyboard.PressEnter();
                                    Thread.Sleep(_clickManager.EnterPresses[i]
                                        .Delay);
                                }

                                i++;
                            }
                        }

                        ThreadSafeBool = false;
                    }
                }
                else
                {
                    throw new Exception(
                        "Действие не было опознано. Файл кликов \"" +
                        _clickManager.ClicksFilePath + "\" неполный.");
                }
            }

            _profile.CompleteWorkWithNumber();
        }

        public void OrderATaxi()
        {
            string[] addresses;
            try
            {
                addresses =
                    File.ReadAllLines(AddressesFilePath, Encoding.Default);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Невозможно прочесть адреса из файла \"" +
                    AddressesFilePath + "\": " + ex.Message);
            }

            int addressFromIndex = -1;

            int startIndex = _clickManager.RegistrationActionsCount;
            for (int i = startIndex;
                i < _clickManager.TaxiOrderingActionsCount + startIndex;
                i++)
            {
                if (_clickManager.TaxiOrderingClicks.ContainsKey(i))
                {
                    int x = (int) (_width *
                                   _clickManager.TaxiOrderingClicks[i].Position
                                       .X) + _leftTop.X;
                    int y;
                    if (_clickManager.TaxiOrderingClicks[i].Description ==
                        "Click reason for cancellation")
                    {
                        int reasonIndex = new Random().Next(0, 6);
                        int caseAbsolutePosition =
                            450 + 41 *
                            reasonIndex; // (y=450 - первая причина на экране с h=768)
                        double caseRelativePosition =
                            caseAbsolutePosition / 768.0;
                        y = (int) (_height *
                                   _clickManager.TaxiOrderingClicks[i].Position
                                       .Y * caseRelativePosition) + _leftTop.Y;
                    }
                    else
                    {
                        y = (int) (_height * _clickManager.TaxiOrderingClicks[i]
                                       .Position.Y) + _leftTop.Y;
                    }

                    if (_clickManager.TaxiOrderingClicks[i].Description ==
                        "DoubleClick to copy number")
                    {
                        VirtualMouse.SendLeftClick(new Point(x, y));

                        Thread.Sleep(500);

                        // Сохранение номера телефона водителя.
                        IDataObject iData = Clipboard.GetDataObject();
                        if (iData != null &&
                            iData.GetDataPresent(DataFormats.Text))
                        {
                            string phoneNumber =
                                (string) iData.GetData(DataFormats.Text);
                            if (phoneNumber != null &&
                                phoneNumber.StartsWith("+7"))
                            {
                                File.AppendAllLines(NumbersFilePath,
                                    new[] {phoneNumber});
                            }
                        }
                    }
                    else
                    {
                        VirtualMouse.SendLeftClick(new Point(x, y));
                    }

                    Thread.Sleep(_clickManager.TaxiOrderingClicks[i].Delay);
                }
                else if (_clickManager.KeyboardInputs.ContainsKey(i))
                {
                    string input = "";
                    switch (_clickManager.KeyboardInputs[i].Description)
                    {
                        case "Enter address from":
                        {
                            addressFromIndex =
                                new Random().Next(0, addresses.Length);
                            input = addresses[addressFromIndex];
                            break;
                        }
                        case "Enter address to":
                        {
                            int addressToIndex;
                            do
                            {
                                addressToIndex =
                                    new Random().Next(0, addresses.Length);
                            } while (addressToIndex == addressFromIndex);

                            input = addresses[addressToIndex];
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(input))
                    {
                        VirtualKeyboard.PrintText(Handle, input);
                    }

                    Thread.Sleep(_clickManager.KeyboardInputs[i].Delay);
                }
                else
                {
                    throw new Exception(
                        "Действие не было опознано. Файл кликов \"" +
                        _clickManager.ClicksFilePath + "\" неполный.");
                }
            }
        }

        public void ClearDataInBolt()
        {
            int startIndex = _clickManager.RegistrationActionsCount + _clickManager.TaxiOrderingActionsCount;
            for (int i = startIndex;
                i < _clickManager.DataCleaningActionsCount + startIndex;
                i++)
            {
                if (_clickManager.DataCleaningClicks.ContainsKey(i))
                {
                    var x = (int) (_width *
                                   _clickManager.DataCleaningClicks[i].Position
                                       .X) + _leftTop.X;
                    var y = (int) (_height *
                                   _clickManager.DataCleaningClicks[i].Position
                                       .Y) + _leftTop.Y;
                    VirtualMouse.SendLeftClick(new Point(x, y));
                    Thread.Sleep(_clickManager.DataCleaningClicks[i].Delay);
                }
                else
                {
                    throw new Exception(
                        "Действие не было опознано. Файл кликов \"" +
                        _clickManager.ClicksFilePath + "\" неполный.");
                }
            }
        }
    }
}