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

        public IntPtr Handle { get; }

        public static string AddressesFilePath { get; } =
            "Script files\\Addresses.txt";

        public static string NumbersFilePath { get; } =
            "Телефоны водителей.txt";

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

        public void StartOrderingATaxi()
        {
            RegisterInBolt();
            for (int i = 0; i < 8; i++)
            {
                OrderATaxi();
                Thread.Sleep(2500);
            }
            ClearDataInBolt();
        }


        public void RegisterInBolt()
        {
            for (int i = 0; i < _clickManager.RegistrationActionsCount; i++)
            {
                if (_clickManager.RegistrationClicks.ContainsKey(i))
                {
                    var x = (int) (_width *
                                   _clickManager.RegistrationClicks[i].Position
                                       .X) + _leftTop.X;
                    var y = (int) (_height *
                                   _clickManager.RegistrationClicks[i].Position
                                       .Y) + _leftTop.Y;
                    VirtualMouse.SendMouseMovement(new Point(x, y));
                    VirtualMouse.SendMouseLeftClick(new Point(x, y));
                    Thread.Sleep(_clickManager.RegistrationClicks[i].Delay);
                }
                else if (_clickManager.KeyboardInputs.ContainsKey(i))
                {
                    Response response;
                    string input = "";
                    switch (_clickManager.KeyboardInputs[i].Description)
                    {
                        case "Enter phone number":
                        {
                            response = _profile.GetNumber();
                            if (response.IsError)
                            {
                                throw new Exception(
                                    "Не удалось получить виртуальный номер:" +
                                    response.Code);
                            }

                            input = _profile.Number;
                            break;
                        }
                        case "Enter code":
                        {
                            response = _profile.GetCode();
                            if (response.IsError)
                            {
                                throw new Exception(
                                    "Не удалось получить код на виртуальный номер:" +
                                    response.Code);
                            }

                            input = _profile.Code;
                            break;
                        }
                        case "Enter e-mail":
                        {
                            input = _user.Mail;
                            break;
                        }
                        case "Enter first name":
                        {
                            input = _user.FirstName;
                            break;
                        }
                        case "Enter last name":
                        {
                            input = _user.Surname;
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(input))
                    {
                        VirtualKeyboard.PrintText(Handle, input);
                    }

                    Thread.Sleep(_clickManager.KeyboardInputs[i].Delay);
                }
                else if (_clickManager.EnterPresses.ContainsKey(i))
                {
                    VirtualKeyboard.PressEnter();
                    Thread.Sleep(_clickManager.EnterPresses[i].Delay);
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
                        VirtualMouse.SendMouseMovement(new Point(x, y));
                        VirtualMouse.SendMouseDoubleClick(new Point(x, y));

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
                        VirtualMouse.SendMouseMovement(new Point(x, y));
                        VirtualMouse.SendMouseLeftClick(new Point(x, y));
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
                    VirtualMouse.SendMouseMovement(new Point(x, y));
                    VirtualMouse.SendMouseLeftClick(new Point(x, y));
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