using System;
using System.Linq;
using System.Threading;
using System.Windows;
using TaxiAutoClicker.WinAPIHelper;
using Point = System.Drawing.Point;

namespace TaxiAutoClicker
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LaunchAutoClicker_Click(object sender, RoutedEventArgs e)
        {
            IntPtr[] windows = WindowManager
                .FindWindowsWithText("NoxPlayer").ToArray();
            for (int i = 0; i < windows.Length; i++)
            {
                WindowManager.GetWindowRect(windows[i], out var rect);
                //int x = (rect.Width - rect.X) / 2 + rect.X;
                //int y = (rect.Height - rect.Y) / 2 + rect.Y;
                //MouseOperations.SendMouseMovement(new Point(x, y));
                //MouseOperations.SendMouseLeftClick(new Point(x, y));

                // Место нахождения кнопки "Выберите место назначания"
                int x = (rect.Width - rect.X) / 2 + rect.X;
                int y = (int)((rect.Height - rect.Y) * 0.91) + rect.Y;
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
                x = (int)((rect.Width - rect.X) * 0.915) + rect.Y;
                y = (int)((rect.Height - rect.Y) * 0.15) + rect.Y;
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
                VirtualKeyboard.PrintText(windows[i], addresses[addressFromIndex]);

                Thread.Sleep(1000);

                // Первый, подходящий под описание, адрес
                // (3-я строчка в списке после "Добавить дом" и "Добавить работу")
                x = (rect.Width - rect.X) / 2 + rect.X;
                y = (int)((rect.Height - rect.Y) * 0.367) + rect.Y;
                VirtualMouse.SendMouseMovement(new Point(x, y));
                VirtualMouse.SendMouseLeftClick(new Point(x, y));

                Thread.Sleep(300);

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

                VirtualKeyboard.PrintText(windows[i], addresses[addressToIndex]);

                Thread.Sleep(1000);

                // Первый, подходящий под описание, адрес
                // (1-я строчка в списке)
                x = (rect.Width - rect.X) / 2 + rect.X;
                y = (int)((rect.Height - rect.Y) * 0.255) + rect.Y;
                VirtualMouse.SendMouseMovement(new Point(x, y));
                VirtualMouse.SendMouseLeftClick(new Point(x, y));

                Thread.Sleep(6000);

                // Место нахождения кнопки "Заказать Bolt"
                x = (rect.Width - rect.X) / 2 + rect.X;
                y = (int)((rect.Height - rect.Y) * 0.944) + rect.Y;
                VirtualMouse.SendMouseMovement(new Point(x, y));
                VirtualMouse.SendMouseLeftClick(new Point(x, y));

                Thread.Sleep(3000);

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

                // Выбор причины
                int caseIndex = 0;
                x = (rect.Width - rect.X) / 2 + rect.Y;
                y = (int)((rect.Height - rect.Y) * 0.716) + rect.Y;
                VirtualMouse.SendMouseMovement(new Point(x, y));
                VirtualMouse.SendMouseLeftClick(new Point(x, y));
            }
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
    }
}
