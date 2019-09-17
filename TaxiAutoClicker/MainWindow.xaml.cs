using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using TaxiAutoClicker.BoltApplication;
using TaxiAutoClicker.WinAPI;
using WindowService = TaxiAutoClicker.WinAPI.WindowService;

namespace TaxiAutoClicker
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private User _boltUser;
        private readonly List<Thread> _orderingThreads;

        public MainWindow()
        {
            InitializeComponent();
            _orderingThreads = new List<Thread>();
        }

        private void LaunchAutoClicker_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(APIKeyTextBox.Text) ||
                string.IsNullOrEmpty(Mail.Text) ||
                string.IsNullOrEmpty(FirstName.Text) ||
                string.IsNullOrEmpty(LastName.Text))
            {
                MessageBox.Show("Заполните все поля.", Title);
                return;
            }

            try
            {
                ClickManager.GetClickManager();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Title);
                return;
            }

            IntPtr[] windows = WindowService
                .FindWindowsWithText("NoxPlayer").ToArray();

            _boltUser = new User(APIKeyTextBox.Text, Mail.Text,
                FirstName.Text, LastName.Text);

            foreach (var window in windows)
            {
                WindowManager windowManager =
                    new WindowManager(window, _boltUser);

                Thread orderThread = new Thread(() =>
                {
                    try
                    {
                        windowManager.StartOrderingATaxi();
                    }
                    catch (ThreadAbortException) { }
                    catch (Exception ex)
                    {
                        Dispatcher?.Invoke(() =>
                        {
                            MessageBox.Show(ex.Message, Title);
                        });
                    }
                });
                orderThread.SetApartmentState(ApartmentState.STA);
                orderThread.Start();
                _orderingThreads.Add(orderThread);
                Thread.Sleep(200);
            }
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            foreach (var thread in _orderingThreads)
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

        private void ManageClicksWindow_Click(object sender, RoutedEventArgs e)
        {
            ClickManagerWindow window = new ClickManagerWindow {Owner = this};
            if (window.ShowDialog() == true)
            {
                ClickManager.GetClickManager().SaveClicksToFile();
            }
        }
    }
}
