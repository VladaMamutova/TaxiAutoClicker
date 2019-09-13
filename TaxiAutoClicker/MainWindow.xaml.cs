using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using TaxiAutoClicker.SMSActivateAPI;
using TaxiAutoClicker.WinAPIHelper;
using Point = System.Drawing.Point;

namespace TaxiAutoClicker
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Profile _smsactivateProfile;
        private BoltUser _boltUser;
        private List<Thread> orderingThreads;

        public MainWindow()
        {
            InitializeComponent();
            orderingThreads = new List<Thread>();
        }

        private void LaunchAutoClicker_Click(object sender, RoutedEventArgs e)
        {
            //if (string.IsNullOrEmpty(APIKeyTextBox.Text) ||
            //    string.IsNullOrEmpty(Mail.Text) ||
            //    string.IsNullOrEmpty(FirstName.Text) ||
            //    string.IsNullOrEmpty(LastName.Text))
            //{
            //    MessageBox.Show("Заполните все поля.");
            //    return;
            //}

            IntPtr[] windows = WindowManager
                .FindWindowsWithText("NoxPlayer").ToArray();

            _boltUser = new BoltUser(APIKeyTextBox.Text, Mail.Text,
                FirstName.Text, LastName.Text);
            
            for (int i = 0; i < windows.Length; i++)
            {
                IntPtr window = windows[i];
                Thread orderThread = new Thread(() => StartOrderingATaxi(window, _boltUser));
                orderThread.Start();
                orderingThreads.Add(orderThread);
                Thread.Sleep(200);
            }
        }

        private void StartOrderingATaxi(IntPtr window, BoltUser user)
        {
            WindowManager.GetWindowRect(window, out var rect);
            int x = (rect.Width - rect.X) / 2 + rect.X;
            int y = (rect.Height - rect.Y) / 2 + rect.Y;
            VirtualMouse.SendMouseMovement(new Point(x, y));
            VirtualMouse.SendMouseLeftClick(new Point(x, y));

            _smsactivateProfile = new Profile(user.ApiKey);
            Response resp = _smsactivateProfile.GetNumber();
            if (resp.IsError)
            {
                MessageBox.Show(resp.Code.ToString(), "Ошибка получения номера");
                return;
            }

            Thread.Sleep(1000);
            VirtualKeyboard.PrintText(window, _smsactivateProfile.Number.Substring(1));
            Thread.Sleep(1000);

            // Нажатие кнопки Далее
            x = (rect.Width - rect.X) / 2 + rect.X;
            y = (int)((rect.Height - rect.Y) * 0.941) + rect.Y;
            VirtualMouse.SendMouseMovement(new Point(x, y));
            VirtualMouse.SendMouseLeftClick(new Point(x, y));

            // Получение кода
            resp = _smsactivateProfile.GetCode();
            if (resp.IsError)
            {
                MessageBox.Show(resp.Code.ToString(),
                    "Ошибка получения кода");
                return;
            }

            VirtualKeyboard.PrintText(window, _smsactivateProfile.Code[0].ToString());
            Thread.Sleep(300);
            VirtualKeyboard.PrintText(window, _smsactivateProfile.Code[1].ToString());
            Thread.Sleep(300);
            VirtualKeyboard.PrintText(window, _smsactivateProfile.Code[2].ToString());
            Thread.Sleep(300);
            VirtualKeyboard.PrintText(window, _smsactivateProfile.Code[3].ToString());

            Thread.Sleep(1000);
            _smsactivateProfile.CompleteWorkWithNumber();
            Thread.Sleep(2000);

            x = (rect.Width - rect.X) / 2 + rect.X;
            y = (int)((rect.Height - rect.Y) * 0.275) + rect.Y;
            VirtualMouse.SendMouseMovement(new Point(x, y));
            VirtualMouse.SendMouseLeftClick(new Point(x, y));

            Thread.Sleep(1000);

            // f2142B6fee88086849B527980709109f
            // creativedeveloper0000@gmail.com
            // Артём
            // Ковальский
            VirtualKeyboard.PrintText(window, user.Mail);
            Thread.Sleep(1500);
            VirtualKeyboard.PressEnter();
            Thread.Sleep(700);
            VirtualKeyboard.PrintText(window, user.FirstName);
            Thread.Sleep(1500);
            VirtualKeyboard.PressEnter();
            Thread.Sleep(700);
            VirtualKeyboard.PrintText(window, user.Surname);
            Thread.Sleep(1500);

            // Нажатие кнопки Далее
            x = (rect.Width - rect.X) / 2 + rect.X;
            y = (int)((rect.Height - rect.Y) * 0.945) + rect.Y;
            VirtualMouse.SendMouseMovement(new Point(x, y));
            VirtualMouse.SendMouseLeftClick(new Point(x, y));

            Thread.Sleep(4500);

            // Место нахождения кнопки "Выберите место назначания"
            x = (rect.Width - rect.X) / 2 + rect.X;
            y = (int)((rect.Height - rect.Y) * 0.918) + rect.Y;
            VirtualMouse.SendMouseMovement(new Point(x, y));
            VirtualMouse.SendMouseLeftClick(new Point(x, y));
            Thread.Sleep(1000);

           
            // Место нахождения поля "Откуда"
            x = (rect.Width - rect.X) / 2 + rect.X;
            y = (int)((rect.Height - rect.Y) * 0.15) + rect.Y;
            VirtualMouse.SendMouseMovement(new Point(x, y));
            VirtualMouse.SendMouseLeftClick(new Point(x, y));

            Thread.Sleep(1000);

            // Место нахождения кнопки с крестиком в поле "Откуда"
            x = (int) ((rect.Width - rect.X) * 0.915) + rect.X;
            y = (int) ((rect.Height - rect.Y) * 0.15) + rect.Y;
            VirtualMouse.SendMouseMovement(new Point(x, y));
            VirtualMouse.SendMouseLeftClick(new Point(x, y));

            Thread.Sleep(500);

            // Место нахождения поля "Куда" 
            x = (rect.Width - rect.X) / 2 + rect.X;
            y = (int)((rect.Height - rect.Y) * 0.2) + rect.Y;
            VirtualMouse.SendMouseMovement(new Point(x, y));
            VirtualMouse.SendMouseLeftClick(new Point(x, y));

            Thread.Sleep(1000);

            // Место нахождения кнопки с крестиком в поле "Куда"
            x = (int)((rect.Width - rect.X) * 0.915) + rect.X;
            y = (int)((rect.Height - rect.Y) * 0.2) + rect.Y;
            VirtualMouse.SendMouseMovement(new Point(x, y));
            VirtualMouse.SendMouseLeftClick(new Point(x, y));

            Thread.Sleep(500);

            // Место нахождения поля "Откуда"
            x = (rect.Width - rect.X) / 2 + rect.X;
            y = (int)((rect.Height - rect.Y) * 0.15) + rect.Y;
            VirtualMouse.SendMouseMovement(new Point(x, y));
            VirtualMouse.SendMouseLeftClick(new Point(x, y));

            Thread.Sleep(500);

            string[] addresses = {"Ленинский проспект", "ул. Маршала Говорова",
                    "Невский проспект", "Пискарёвский проспект", "Комендантский проспект"};
            int addressFromIndex = new Random().Next(0, addresses.Length);
            VirtualKeyboard.PrintText(window, addresses[addressFromIndex]);

            Thread.Sleep(1500);

            // Первый, подходящий под описание, адрес
            // (3-я строчка в списке после "Добавить дом" и "Добавить работу")
            x = (rect.Width - rect.X) / 2 + rect.X;
            y = (int)((rect.Height - rect.Y) * 0.367) + rect.Y;
            VirtualMouse.SendMouseMovement(new Point(x, y));
            VirtualMouse.SendMouseLeftClick(new Point(x, y));

            Thread.Sleep(500);

            // Место нахождения поля "Куда"
            x = (rect.Width - rect.X) / 2 + rect.X;
            y = (int)((rect.Height - rect.Y) * 0.2) + rect.Y;
            VirtualMouse.SendMouseMovement(new Point(x, y));
            VirtualMouse.SendMouseLeftClick(new Point(x, y));

            Thread.Sleep(500);

            int addressToIndex;
            do
            {
                addressToIndex = new Random().Next(0, addresses.Length);
            } while (addressToIndex == addressFromIndex);

            VirtualKeyboard.PrintText(window, addresses[addressToIndex]);

            Thread.Sleep(1500);

            // Первый, подходящий под описание, адрес
            // (1-я строчка в списке)
            x = (rect.Width - rect.X) / 2 + rect.X;
            y = (int)((rect.Height - rect.Y) * 0.255) + rect.Y;
            VirtualMouse.SendMouseMovement(new Point(x, y));
            VirtualMouse.SendMouseLeftClick(new Point(x, y));

            Thread.Sleep(7000);

            // Место нахождения кнопки "Заказать Bolt"
            x = (rect.Width - rect.X) / 2 + rect.X;
            y = (int)((rect.Height - rect.Y) * 0.944) + rect.Y;
            VirtualMouse.SendMouseMovement(new Point(x, y));
            VirtualMouse.SendMouseLeftClick(new Point(x, y));

            Thread.Sleep(5000);

            // Место нахождения кнопки подтверждения "Заказать Bolt"
            x = (rect.Width - rect.X) / 2 + rect.X;
            y = (int)((rect.Height - rect.Y) * 0.944) + rect.Y;
            VirtualMouse.SendMouseMovement(new Point(x, y));
            VirtualMouse.SendMouseLeftClick(new Point(x, y));

            Thread.Sleep(15000);

            // Место нахождения кнопки со стрелкой для раскрытия меню водителя
            x = (int)((rect.Width - rect.X) * 0.937) + rect.X;
            y = (int)((rect.Height - rect.Y) * 0.880) + rect.Y;
            VirtualMouse.SendMouseMovement(new Point(x, y));
            VirtualMouse.SendMouseLeftClick(new Point(x, y));

            Thread.Sleep(1500);

            // Место нахождения кнопки "Звонок" в меню водителя
            x = (int)((rect.Height - rect.Y) * 0.12) + rect.X;
            y = (int)((rect.Height - rect.Y) * 0.827) + rect.Y;
            VirtualMouse.SendMouseMovement(new Point(x, y));
            VirtualMouse.SendMouseLeftClick(new Point(x, y));

            Thread.Sleep(3000);

            // Двойное нажатие на телефонный номер для его копирования
            x = (rect.Width - rect.X) / 2 + rect.X;
            y = (int)((rect.Height - rect.Y) * 0.653) + rect.Y;
            VirtualMouse.SendMouseMovement(new Point(x, y));
            VirtualMouse.SendMouseDoubleClick(new Point(x, y));

            Thread.Sleep(1500);

            // Нажатие на диспетчер задач
            x = rect.Width + 12;
            y = (int)((rect.Height - rect.Y) * 0.97) + rect.Y;
            VirtualMouse.SendMouseMovement(new Point(x, y));
            VirtualMouse.SendMouseLeftClick(new Point(x, y));

            Thread.Sleep(1000);

            // Открытие Bolt из диспетчера задач
            x = (rect.Width - rect.X) / 2 + rect.X;
            y = (int)((rect.Height - rect.Y) * 0.35) + rect.Y;
            VirtualMouse.SendMouseMovement(new Point(x, y));
            VirtualMouse.SendMouseLeftClick(new Point(x, y));

            Thread.Sleep(3000);

            // Нажатие на кнопку отмены заказа в меню водителя.
            x = (int)((rect.Width - rect.X) * 0.8616) + rect.Y;
            y = (int)((rect.Height - rect.Y) * 0.8268) + rect.Y;
            VirtualMouse.SendMouseMovement(new Point(x, y));
            VirtualMouse.SendMouseLeftClick(new Point(x, y));

            Thread.Sleep(1000);

            // Нажатие на кнопку отмены заказа во всплывающем уведомлении.
            x = (rect.Width - rect.X) / 2 + rect.Y;
            y = (int)((rect.Height - rect.Y) * 0.9635) + rect.Y;
            VirtualMouse.SendMouseMovement(new Point(x, y));
            VirtualMouse.SendMouseLeftClick(new Point(x, y));

            Thread.Sleep(1000);

            //// Выбор причины
            int caseIndex = 0;
            x = (rect.Width - rect.X) / 2 + rect.Y;
            y = (int)((rect.Height - rect.Y) * 0.716) + rect.Y;
            VirtualMouse.SendMouseMovement(new Point(x, y));
            VirtualMouse.SendMouseLeftClick(new Point(x, y));
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 100),
                IsEnabled = true
            };
            timer.Tick += (o, t) =>
            {
                Point cursorPoint = new Point();
                VirtualMouse.GetCursorPos(ref cursorPoint);

                XTextBlock.Text = cursorPoint.X.ToString();
                YTextBlock.Text = cursorPoint.Y.ToString();
            };
            timer.Start();
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            foreach (var thread in orderingThreads)
            {
                if (thread != null && thread.IsAlive)
                {
                    thread.Abort();
                }
            }
        }

        private void PasteApi_OnClick(object sender, RoutedEventArgs e)
        {
            APIKeyTextBox.Focus();
            if(APIKeyTextBox.Text.Length > 0) APIKeyTextBox.Clear();

            VirtualKeyboard.Paste();
        }
    }
}
