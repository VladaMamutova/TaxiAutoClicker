using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using TaxiAutoClicker.SMSActivateAPI;
using TaxiAutoClicker.WinAPI;

namespace TaxiAutoClicker.BoltApplication
{
    class WindowManager
    {
        private ClickManager _clickManager;
        private User _user;
        private Profile _profile;
        private Point _leftTop;
        private int _width;
        private int _height;
        private const string clickFilePath = "Script files\\Clickes.txt";

        public IntPtr Handle { get; }
        
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
        }


        public void RegisterInBolt()
        {
            int lastAction = _clickManager.RegistrationClicks.Keys.Max();
            int x, y;
            for (int i = 0; i <= lastAction; i++)
            {
                if (_clickManager.RegistrationClicks.ContainsKey(i))
                {
                    x = (int)(_width * _clickManager.RegistrationClicks[i].Position.X) + _leftTop.X;
                    y = (int)(_height * _clickManager.RegistrationClicks[i].Position.Y) + _leftTop.Y;
                    VirtualMouse.SendMouseMovement(new Point(x, y));
                    VirtualMouse.SendMouseLeftClick(new Point(x, y));
                    Thread.Sleep(_clickManager.RegistrationClicks[i].Delay);
                }
                else if (_clickManager.Inputs.ContainsKey(i))
                {
                    Response response;
                    string input = "";
                    switch (_clickManager.Inputs[i].Description)
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
                        case "Enter the 1st digit of code":
                        {
                            response = _profile.GetCode();
                            if (response.IsError)
                            {
                                throw new Exception(
                                    "Не удалось получить код на виртуальный номер:" +
                                    response.Code);
                            }

                            input = _profile.Code[0].ToString();
                            break;

                        }
                        case "Enter the 2nd digit of code":
                        {
                            input = _profile.Code[1].ToString();
                            break;
                        }
                        case "Enter the 3rd digit of code":
                        {
                            input = _profile.Code[2].ToString();
                            break;
                        }
                        case "Enter the 4th digit of code":
                        {
                            input = _profile.Code[3].ToString();
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

                    Thread.Sleep(_clickManager.Inputs[i].Delay);
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
                        clickFilePath + "\" неполный.");
                }
            }
            _profile.CompleteWorkWithNumber();
        }
    }
}
