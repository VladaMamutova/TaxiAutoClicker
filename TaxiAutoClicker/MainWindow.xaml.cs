using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using TaxiAutoClicker.BoltApplication;
using TaxiAutoClicker.WinAPI;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
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
            if (RegistrationCheckBox.IsChecked == true &&
                (string.IsNullOrEmpty(APIKeyTextBox.Text) ||
                string.IsNullOrEmpty(Mail.Text) ||
                string.IsNullOrEmpty(FirstName.Text) ||
                string.IsNullOrEmpty(LastName.Text)))
            {
                MessageBox.Show("Заполните все поля.", Title);
                return;
            }

            IntPtr[] windows = WindowService
                .FindWindowsWithText("NoxPlayer").ToArray();

            _boltUser = new User(APIKeyTextBox.Text, Mail.Text,
                FirstName.Text, LastName.Text);
            bool needToRegister = RegistrationCheckBox.IsChecked == true;
            bool needToOrderATaxi = OrderingATaxiCheckBox.IsChecked == true;
            int.TryParse(LaunchCount.Text, out int launchCount);
            bool needToClearData = ClearindDataCheckBox.IsChecked == true;
            
            foreach (var window in windows)
            {
                WindowManager windowManager =
                    new WindowManager(window, _boltUser);

                Thread orderThread = new Thread(() =>
                {
                    try
                    {
                        if (needToRegister) windowManager.RegisterInBolt();
                        if (needToOrderATaxi)
                        {
                            for (int i = 0; i < launchCount; i++)
                            {
                                windowManager.OrderATaxi();
                                Thread.Sleep(2500);
                            }
                        }
                        if(needToClearData) windowManager.ClearDataInBolt();
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


        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            RegistrationCheckBox.IsChecked = true;
            OrderingATaxiCheckBox.IsChecked = true;
            ClearindDataCheckBox.IsChecked = true;
            LaunchCount.Text = 10.ToString();

            try
            {
                ClickManager.GetClickManager();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Title);
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

        private void OpenHelp_Click(object sender, RoutedEventArgs e)
        {
            Help.ShowHelp(null, "TaxiAutoClickerHelp.chm");
        }

        private void ToggleButton_OnChanged(object sender, RoutedEventArgs e)
        {
            SetLaunchExpanderText();
        }

        private void SetLaunchExpanderText()
        {
            List<string> result = new List<string>();
            if (RegistrationCheckBox.IsChecked == true)
            {
                result.Add("регистрацию");
            }
            if (OrderingATaxiCheckBox.IsChecked == true)
            {
                if (!string.IsNullOrWhiteSpace(LaunchCount.Text) &&
                    int.TryParse(LaunchCount.Text, out int count))
                {
                    count = Math.Min(count, 10);

                    if (count > 0)
                    {
                        result.Add(count + " вызовов такси");
                    }
                }
            }
            if(ClearindDataCheckBox.IsChecked == true)
            {
                result.Add("очистку данных");
            }

            string resultHeader;
            switch (result.Count)
            {
                case 0:
                {
                    resultHeader = "Ничего не запускать";
                    break;
                }
                case 1:
                {
                    resultHeader = "Запустить " + result[0];
                    break;
                    }
                case 2:
                {
                    resultHeader = $"Запустить {result[0]} и {result[1]}";
                    break;
                }
                default:
                {
                    resultHeader = "Запустить полный цикл заказа такси";
                    break;
                }
            }

            ExpanderText.Text = resultHeader;
        }

        private void LaunchCount_OnKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key < Key.D0 || e.Key > Key.D9) && e.Key != Key.Back &&
                e.Key != Key.Delete)
            {
                e.Handled = true;
            }
        }

        private void LaunchCount_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (OrderingATaxiCheckBox.IsChecked == true)
            {
                SetLaunchExpanderText();
            }
        }
    }
}
