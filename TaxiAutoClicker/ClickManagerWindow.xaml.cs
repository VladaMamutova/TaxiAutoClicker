using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using TaxiAutoClicker.BoltApplication;
using TaxiAutoClicker.InputActions;
using TaxiAutoClicker.WinAPI;
using Point = System.Drawing.Point;

namespace TaxiAutoClicker
{
    /// <summary>
    /// Логика взаимодействия для ClickManagerWindow.xaml
    /// </summary>
    public partial class ClickManagerWindow : Window
    {
        private readonly ClickManager _clickManagerCopy;
        private Thread _chooseWindowThread;
        private IntPtr _targetWindow;
        private string _windowHeader;
        private Point _leftTop;
        private int _width;
        private int _height;
        public List<ActionItem> Actions { get; set; }

        public ClickManagerWindow()
        {
            InitializeComponent();

            _clickManagerCopy = ClickManager.GetCopyClickManager();
            UpdateActions();
        }

        private void ClickManagerWindow_OnLoaded(object sender,
            RoutedEventArgs e)
        {
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 100),
                IsEnabled = true
            };
            timer.Tick += (o, t) =>
            {
                if (_targetWindow != IntPtr.Zero)
                {
                    WindowService.GetWindowRect(_targetWindow,
                        out Rectangle rect);

                    Point cursorPoint = new Point();
                    VirtualMouse.GetCursorPos(ref cursorPoint);

                    _leftTop = new Point(rect.X, rect.Y);

                    ActiveWindowInfo.Text =
                            $"{_windowHeader}: " +
                            $"{_leftTop}, {_width} x {_height}";

                    XTextBlock.Text = (cursorPoint.X - _leftTop.X).ToString();
                    YTextBlock.Text = (cursorPoint.Y - _leftTop.Y).ToString();

                    if (_width != rect.Width - rect.X || _height !=
                        rect.Height - rect.Y)
                    {
                        _width = rect.Width - rect.X;
                        _height = rect.Height - rect.Y;
                        UpdateActions();
                        ActionsGrid.ItemsSource = Actions;

                        SetBoltIconCoords();
                        SetSettingsIconCoords();
                    }
                }
                else
                {
                   SetElementsState(false);
                }
            };
            timer.Start();
        }

        private void ChooseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            ChooseWindowButton.ContentStringFormat = "Выбор окна...";
            ChooseWindowButton.IsEnabled = false;
            _targetWindow = IntPtr.Zero;
            _leftTop = Point.Empty;
            _width = 0;
            _height = 0;

            _chooseWindowThread = new Thread(() =>
            {
                bool isRightWindow = false;
                do
                {
                    IntPtr handle = WindowService.GetForegroundWindow();
                    string header = WindowService.GetWindowText(handle);
                    if (header.StartsWith("NoxPlayer"))
                    {
                        _targetWindow = handle;
                        _windowHeader = header;

                        isRightWindow = true;
                    }
                    else
                    {
                        Thread.Sleep(500);
                    }
                } while (!isRightWindow);

                Dispatcher?.Invoke(() =>
                {
                    ChooseWindowButton.ContentStringFormat = "Выбрать";
                    ChooseWindowButton.IsEnabled = true;
                    SetElementsState(true);
                    UpdateActions();
                    ActionsGrid.ItemsSource = Actions;
                });
            });
            _chooseWindowThread.Start();
        }

        private void UpdateActions()
        {
            List<ActionItem> actions = new List<ActionItem>();
            for (int i = 0; i < _clickManagerCopy.ActionsCount; i++)
            {
                if (_clickManagerCopy.RegistrationClicks.ContainsKey(i) ||
                    _clickManagerCopy.TaxiOrderingClicks.ContainsKey(i) ||
                    _clickManagerCopy.DataCleaningClicks.ContainsKey(i))
                {
                    ActionItem actionItem;
                    if (_clickManagerCopy.RegistrationClicks.ContainsKey(i))
                    {
                        Click click = _clickManagerCopy.RegistrationClicks[i];
                        Point relativePosition =
                            new Point(
                                (int) (_width * click.Position.X),
                                (int) (_height * click.Position.Y));
                        actionItem = new ActionItem
                        {
                            Number = i + 1,
                            Delay = click.Delay,
                            Description = click.Description,
                            Position = relativePosition.ToString()
                        };
                    }
                    else if (_clickManagerCopy.TaxiOrderingClicks
                        .ContainsKey(i))
                    {
                        Click click = _clickManagerCopy.TaxiOrderingClicks[i];
                        Point relativePosition =
                            new Point(
                                (int)(_width * click.Position.X),
                                (int)(_height * click.Position.Y));
                        actionItem = new ActionItem
                        {
                            Number = i + 1,
                            Delay = click.Delay,
                            Description = click.Description,
                            Position = relativePosition.ToString()
                        };
                    }
                    else
                    {
                        Click click = _clickManagerCopy.DataCleaningClicks[i];
                        Point relativePosition =
                            new Point(
                                (int)(_width * click.Position.X),
                                (int)(_height * click.Position.Y));
                        actionItem = new ActionItem
                        {
                            Number = i + 1,
                            Delay = click.Delay,
                            Description = click.Description,
                            Position = relativePosition.ToString()
                        };
                    }

                    actions.Add(actionItem);
                }
                else if (_clickManagerCopy.KeyboardInputs.ContainsKey(i))
                {
                    actions.Add(new ActionItem
                    {
                        Number = i + 1,
                        Delay = _clickManagerCopy.KeyboardInputs[i].Delay,
                        Description = _clickManagerCopy.KeyboardInputs[i]
                            .Description,
                    });
                }
                else if (_clickManagerCopy.EnterPresses.ContainsKey(i))
                {
                    actions.Add(new ActionItem
                    {
                        Number = i + 1,
                        Delay = _clickManagerCopy.EnterPresses[i].Delay,
                        Description = _clickManagerCopy.EnterPresses[i]
                            .Description,
                    });
                }
            }

            Actions = actions;
        }

        private void SetElementsState(bool state)
        {
            CoordsStackPanel.IsEnabled = state;
            ActionsGrid.IsEnabled = state;
            LoadByDefaultButton.IsEnabled = state;
            SaveButton.IsEnabled = state;
        }

        private void SetBoltIconCoords()
        {
            string[] coords = Actions[_clickManagerCopy.BoltIconKey]
                .Position
                .Split(new[] { '{', '}', ',', '=', 'X', 'Y' },
                    StringSplitOptions.RemoveEmptyEntries);
            BoltX.Text = coords[0];
            BoltY.Text = coords[1];
        }

        private void SetSettingsIconCoords()
        {
            string[] coords = Actions[_clickManagerCopy.SettingsIconKey]
                .Position
                .Split(new[] { '{', '}', ',', '=', 'X', 'Y' },
                    StringSplitOptions.RemoveEmptyEntries);
            SettingsX.Text = coords[0];
            SettingsY.Text = coords[1];
        }

        private void SaveBoltCoordsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Point point =
                    new Point(int.Parse(BoltX.Text), int.Parse(BoltY.Text));
                _clickManagerCopy
                    .TaxiOrderingClicks[_clickManagerCopy.BoltIconKey]
                    .Position = new PointF(1.0f * point.X / _width,
                    1.0f * point.Y / _height);

                UpdateActions();
                ActionsGrid.ItemsSource = Actions;
                ActionsGrid.SelectedIndex = _clickManagerCopy.BoltIconKey;
                ActionsGrid.ScrollIntoView(ActionsGrid.SelectedItem);
                ActionsGrid.Focus();
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Некорректное значение. Пожалуйста, введите целое число.", "TaxiAutoClicker");
                SetBoltIconCoords();
            }
        }

        private void SaveSettingsCoordsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Point point =
                    new Point(int.Parse(SettingsX.Text), int.Parse(SettingsY.Text));
                _clickManagerCopy
                    .DataCleaningClicks[_clickManagerCopy.SettingsIconKey]
                    .Position = new PointF(1.0f * point.X / _width,
                    1.0f * point.Y / _height);

                UpdateActions();
                ActionsGrid.ItemsSource = Actions;
                ActionsGrid.SelectedIndex = _clickManagerCopy.SettingsIconKey;
                ActionsGrid.ScrollIntoView(ActionsGrid.SelectedItem);
                ActionsGrid.Focus();
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Некорректное значение. Пожалуйста, введите целое число.", "TaxiAutoClicker");
                SetSettingsIconCoords();
            }
        }
       
        private void ActionsGrid_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.DisplayIndex == 2)
            {
                try
                {
                    string newValue = ((TextBox)e.EditingElement).Text;
                    ActionItem item = (ActionItem)e.EditingElement.DataContext;
                    int index = item.Number - 1;

                    if (_clickManagerCopy.RegistrationClicks.ContainsKey(index) ||
                    _clickManagerCopy.TaxiOrderingClicks.ContainsKey(index) ||
                    _clickManagerCopy.DataCleaningClicks.ContainsKey(index))
                    {
                        if (string.IsNullOrWhiteSpace(newValue))
                        {
                            throw new Exception("Ячейка с координатами клика не может быть пустой.");
                        }

                        string[] coords = newValue
                            .Split(new[] { '{', '}', ',', '=', 'X', 'Y' },
                                StringSplitOptions.RemoveEmptyEntries);
                        if (coords.Length != 2)
                        {
                            throw new Exception("Неверный формат записи координат.");
                        }

                        if (!int.TryParse(coords[0], out int x) ||
                            !int.TryParse(coords[1], out int y))
                        {
                            throw new Exception(
                                "Координаты клика должны быть целочисленными.");
                        }

                        Point point = new Point(x, y);
                        PointF relativePosition = new PointF(1.0f * point.X / _width,
                            1.0f * point.Y / _height);

                        if (_clickManagerCopy.RegistrationClicks.ContainsKey(index))
                        {
                            _clickManagerCopy.RegistrationClicks[index].Position
                                = relativePosition;
                        }
                        else if (_clickManagerCopy.TaxiOrderingClicks
                            .ContainsKey(index))
                        {
                            _clickManagerCopy.TaxiOrderingClicks[index].Position
                                = relativePosition;
                        }
                        else
                        {
                            _clickManagerCopy.DataCleaningClicks[index].Position
                                = relativePosition;
                        }

                        Actions[index].Position = newValue;
                    }
                    else
                    {
                        throw new Exception(
                            "Ячейка с координатами клика не может быть пустой.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "TaxiAutoClicker");
                    e.Cancel = true;
                    ((TextBox) e.EditingElement).Text =
                        ((ActionItem) e.EditingElement.DataContext).Position;
                }
            }
            else if (e.Column.DisplayIndex == 3)
            {
                string newValue = ((TextBox) e.EditingElement).Text;
                if (!int.TryParse(newValue, out int newIntValue))
                {
                    MessageBox.Show("Значение должно быть целочисленным.");
                    e.Cancel = true;
                    ((TextBox)e.EditingElement).Text =
                        ((ActionItem)e.EditingElement.DataContext).Delay.ToString();
                    return;
                }

                ActionItem item = (ActionItem) e.EditingElement.DataContext;
                int index = item.Number - 1;
                if (_clickManagerCopy.RegistrationClicks.ContainsKey(index))
                {
                    _clickManagerCopy.RegistrationClicks[index].Delay =
                        newIntValue;
                }
                else if (_clickManagerCopy.TaxiOrderingClicks.ContainsKey(index)
                )
                {
                    _clickManagerCopy.TaxiOrderingClicks[index].Delay =
                        newIntValue;
                }
                else if (_clickManagerCopy.DataCleaningClicks.ContainsKey(index)
                )
                {
                    _clickManagerCopy.DataCleaningClicks[index].Delay =
                        newIntValue;
                }
                else if (_clickManagerCopy.KeyboardInputs.ContainsKey(index))
                {
                    _clickManagerCopy.KeyboardInputs[index].Delay = newIntValue;
                }
                else if (_clickManagerCopy.EnterPresses
                    .ContainsKey(index))
                {
                    _clickManagerCopy.EnterPresses[index].Delay = newIntValue;
                }

                Actions[index].Delay = newIntValue;
            }
        }

        private void LoadByDefaultButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _clickManagerCopy.InitializeDefaultClicks();
                UpdateActions();
                ActionsGrid.ItemsSource = Actions;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "TaxiAutoClicker");
            }
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            _clickManagerCopy.SynchronizeOrigin();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (_chooseWindowThread != null && _chooseWindowThread.IsAlive)
            {
                _chooseWindowThread = null;
            }
            _clickManagerCopy.DisposeCopy();
            DialogResult = false;
        }
    }
}
