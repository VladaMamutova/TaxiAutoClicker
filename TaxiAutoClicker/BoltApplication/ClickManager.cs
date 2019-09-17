using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TaxiAutoClicker.InputActions;

namespace TaxiAutoClicker.BoltApplication
{
    public class ClickManager
    {
        private static ClickManager _clickManager;
        private static ClickManager _clickManagerCopy;
        private static readonly object SyncRoot = new object();

        public Dictionary<int, Click> RegistrationClicks { get; }
        public Dictionary<int, Click> TaxiOrderingClicks { get; }
        public Dictionary<int, Click> DataCleaningClicks { get; }
        public Dictionary<int, KeyboardInput> KeyboardInputs { get; }
        public Dictionary<int, EnterPress> EnterPresses { get; }

        public int RegistrationActionsCount { get; } = 11;
        public int TaxiOrderingActionsCount { get; } = 21;
        public int DataCleaningActionsCount { get; } = 7;

        public int BoltIconKey { get; } = 28;
        public int SettingsIconKey { get; } = 33;

        public int ActionsCount => RegistrationActionsCount +
                                   TaxiOrderingActionsCount +
                                   DataCleaningActionsCount;

        public string ClicksFilePath { get; } =
            "Script files\\Clickes.json";

        private ClickManager()
        {
            RegistrationClicks = new Dictionary<int, Click>();
            TaxiOrderingClicks = new Dictionary<int, Click>();
            DataCleaningClicks = new Dictionary<int, Click>();
            KeyboardInputs = new Dictionary<int, KeyboardInput>();
            EnterPresses = new Dictionary<int, EnterPress>();
        }

        public static ClickManager GetClickManager()
        {
            if (_clickManager == null)
            {
                lock (SyncRoot)
                {
                    _clickManager = new ClickManager();
                    _clickManager.LoadClicksFromFile();
                }
            }

            return _clickManager;
        }

        public static ClickManager GetCopyClickManager()
        {
            lock (SyncRoot)
            {
                _clickManagerCopy = new ClickManager();

                if (_clickManager != null)
                {
                    foreach (var click in _clickManager.RegistrationClicks)
                    {
                        _clickManagerCopy.RegistrationClicks.Add(click.Key,
                            click.Value);
                    }

                    foreach (var click in _clickManager.TaxiOrderingClicks)
                    {
                        _clickManagerCopy.TaxiOrderingClicks.Add(click.Key,
                            click.Value);
                    }

                    foreach (var click in _clickManager.DataCleaningClicks)
                    {
                        _clickManagerCopy.DataCleaningClicks.Add(click.Key,
                            click.Value);
                    }

                    foreach (var keyboardInput in _clickManager.KeyboardInputs)
                    {
                        _clickManagerCopy.KeyboardInputs.Add(keyboardInput.Key,
                            keyboardInput.Value);
                    }

                    foreach (var enterPress in _clickManager.EnterPresses)
                    {
                        _clickManagerCopy.EnterPresses.Add(enterPress.Key,
                            enterPress.Value);
                    }
                }
                else
                {
                    _clickManager = new ClickManager();
                    _clickManager.InitializeDefaultClicks();
                    _clickManagerCopy.InitializeDefaultClicks();
                }
            }

            return _clickManagerCopy;
        }

        public void SynchronizeOrigin()
        {
            lock (SyncRoot)
            {
                _clickManager = new ClickManager();

                if (_clickManagerCopy != null)
                {
                    foreach (var click in _clickManagerCopy.RegistrationClicks)
                    {
                        _clickManager.RegistrationClicks.Add(click.Key,
                            click.Value);
                    }

                    foreach (var click in _clickManagerCopy.TaxiOrderingClicks)
                    {
                        _clickManager.TaxiOrderingClicks.Add(click.Key,
                            click.Value);
                    }

                    foreach (var click in _clickManagerCopy.DataCleaningClicks)
                    {
                        _clickManager.DataCleaningClicks.Add(click.Key,
                            click.Value);
                    }

                    foreach (var keyboardInput in _clickManagerCopy.KeyboardInputs)
                    {
                        _clickManager.KeyboardInputs.Add(keyboardInput.Key,
                            keyboardInput.Value);
                    }

                    foreach (var enterPress in _clickManagerCopy.EnterPresses)
                    {
                        _clickManager.EnterPresses.Add(enterPress.Key,
                            enterPress.Value);
                    }
                }
                else
                {
                    _clickManagerCopy = new ClickManager();
                    _clickManager.InitializeDefaultClicks();
                    _clickManagerCopy.InitializeDefaultClicks();
                }
            }
        }

        public void DisposeCopy()
        {
            _clickManagerCopy = null;
        }

        public void SaveClicksToFile()
        {
            using (FileStream fs = new FileStream(ClicksFilePath, FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
                {
                    for (int i = 0; i < _clickManager.ActionsCount; i++)
                    {
                        JObject jobject;
                        if (_clickManager.RegistrationClicks.ContainsKey(i))
                        {
                            jobject =
                                JObject.FromObject(_clickManager.RegistrationClicks[i]);
                            sw.WriteLine(Actions.Click + Environment.NewLine + jobject);
                        }
                        else if (_clickManager.TaxiOrderingClicks
                            .ContainsKey(i))
                        {
                            jobject =
                                JObject.FromObject(_clickManager.TaxiOrderingClicks[i]);
                            sw.WriteLine(Actions.Click + Environment.NewLine + jobject);
                        }
                        else if (_clickManager.DataCleaningClicks
                            .ContainsKey(i))
                        {

                            jobject =
                                JObject.FromObject(_clickManager.DataCleaningClicks[i]);
                            sw.WriteLine(Actions.Click + Environment.NewLine + jobject);
                        }
                        else if (_clickManager.KeyboardInputs.ContainsKey(i))
                        {
                            jobject =
                                JObject.FromObject(_clickManager.KeyboardInputs[i]);
                            sw.WriteLine(Actions.KeyboardInput + Environment.NewLine + jobject);
                        }
                        else if (_clickManager.EnterPresses.ContainsKey(i))
                        {
                            jobject =
                                JObject.FromObject(_clickManager.EnterPresses[i]);
                            sw.WriteLine(Actions.EnterPress + Environment.NewLine + jobject);
                        }
                    }
                }
            }
        }

        public void LoadClicksFromFile()
        {
            RegistrationClicks.Clear();
            TaxiOrderingClicks.Clear();
            DataCleaningClicks.Clear();
            KeyboardInputs.Clear();
            EnterPresses.Clear();

            using (FileStream fs = new FileStream(ClicksFilePath, FileMode.OpenOrCreate))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                {
                    int actionsCount = 0;
                    while (!sr.EndOfStream)
                    {
                        if (Enum.TryParse(sr.ReadLine(), out Actions action))
                        {
                            string line;
                            string jobjectString = "";
                            do
                            {
                                line = sr.ReadLine();
                                jobjectString += line;
                            } while (line != "}");

                            switch (action)
                            {
                                case Actions.Click:
                                {
                                    if (actionsCount < RegistrationActionsCount)
                                    {
                                        RegistrationClicks.Add(actionsCount,
                                            JsonConvert
                                                .DeserializeObject<Click>(
                                                    jobjectString));
                                    }
                                    else if (actionsCount <
                                             TaxiOrderingActionsCount + RegistrationActionsCount)
                                    {
                                        TaxiOrderingClicks.Add(actionsCount,
                                            JsonConvert
                                                .DeserializeObject<Click>(
                                                    jobjectString));
                                    }
                                    else
                                    {
                                        DataCleaningClicks.Add(actionsCount,
                                            JsonConvert
                                                .DeserializeObject<Click>(
                                                    jobjectString));
                                    }

                                    break;
                                }
                                case Actions.KeyboardInput:
                                {
                                    KeyboardInputs.Add(actionsCount,
                                        JsonConvert
                                            .DeserializeObject<KeyboardInput>(
                                                jobjectString));
                                        break;
                                }
                                case Actions.EnterPress:
                                {
                                    EnterPresses.Add(actionsCount,
                                        JsonConvert
                                            .DeserializeObject<EnterPress>(
                                                jobjectString));
                                    break;
                                    }
                            }
                        }
                        else
                        {
                            throw new Exception(
                                "Действие не было опознано. Файл кликов \"" +
                                ClicksFilePath + "\" был повреждён.");
                        }

                        actionsCount++;
                    }
                }
            }
        }
        
        public void InitializeDefaultClicks()
        {
            RegistrationClicks.Clear();
            TaxiOrderingClicks.Clear();
            DataCleaningClicks.Clear();
            KeyboardInputs.Clear();
            EnterPresses.Clear();

            int number = 0;
            RegistrationClicks.Add(number++, new Click(new PointF(0.5f, 0.5f), "Click phone number field", 0));
            KeyboardInputs.Add(number++, new KeyboardInput("Enter phone number", 1000));
            RegistrationClicks.Add(number++, new Click(new PointF(0.5f, 0.940269768f), "Click Next", 0));
            KeyboardInputs.Add(number++, new KeyboardInput("Enter code", 3000));
            RegistrationClicks.Add(number++, new Click(new PointF(0.5f, 0.274566472f), "Click e-mail field", 1000));
            KeyboardInputs.Add(number++, new KeyboardInput("Enter e-mail", 1500));
            EnterPresses.Add(number++, new EnterPress("Press Enter", 700));
            KeyboardInputs.Add(number++, new KeyboardInput("Enter first name", 1500));
            EnterPresses.Add(number++, new EnterPress("Press Enter", 700));
            KeyboardInputs.Add(number++, new KeyboardInput("Enter last name", 1500));
            RegistrationClicks.Add(number++, new Click(new PointF(0.5f, 0.9441233f), "Click Next", 4500));

            TaxiOrderingClicks.Add(number++, new Click(new PointF(0.5f, 0.917148352f), "Click Search destination", 1000));
            TaxiOrderingClicks.Add(number++, new Click(new PointF(0.5f, 0.153214768f), "Click Pickup location", 1000));
            TaxiOrderingClicks.Add(number++, new Click(new PointF(0.9137324f, 0.153214768f), "Click x in Pickup location Field", 500));
            TaxiOrderingClicks.Add(number++, new Click(new PointF(0.5f, 0.201094389f), "Click Where to?", 1000));
            TaxiOrderingClicks.Add(number++, new Click(new PointF(0.9137324f, 0.201094389f), "Click x in Where to? Field", 500));
            TaxiOrderingClicks.Add(number++, new Click(new PointF(0.5f, 0.153214768f), "Click Pickup location", 500));
            KeyboardInputs.Add(number++, new KeyboardInput("Enter address from", 1500));
            TaxiOrderingClicks.Add(number++, new Click(new PointF(0.5f, 0.366088629f), "Click address from", 500));
            TaxiOrderingClicks.Add(number++, new Click(new PointF(0.5f, 0.201094389f), "Click Where to?", 500));
            KeyboardInputs.Add(number++, new KeyboardInput("Enter address to", 1500));
            TaxiOrderingClicks.Add(number++, new Click(new PointF(0.5f, 0.254335254f), "Click address to", 7000));
            TaxiOrderingClicks.Add(number++, new Click(new PointF(0.5f, 0.943159938f), "Click Select Bolt", 2000));
            TaxiOrderingClicks.Add(number++, new Click(new PointF(0.5f, 0.943159938f), "Click Request Bolt", 15000));
            TaxiOrderingClicks.Add(number++, new Click(new PointF(0.92957747f, 0.8795761f), "Click chevron-up button", 1500));
            TaxiOrderingClicks.Add(number++, new Click(new PointF(0.125f, 0.8265896f), "Click Call", 3000));
            TaxiOrderingClicks.Add(number++, new Click(new PointF(0.5f, 0.653179169f), "DoubleClick to copy number", 1500));
            TaxiOrderingClicks.Add(number++, new Click(new PointF(1.04797983f, 0.896081746f), "Click Home", 1500));
            TaxiOrderingClicks.Add(number++, new Click(new PointF(0.6919014f, 0.383429676f), "Click Bolt icon", 3000));
            TaxiOrderingClicks.Add(number++, new Click(new PointF(0.8556338f, 0.8265896f), "Click Cancel", 1500));
            TaxiOrderingClicks.Add(number++, new Click(new PointF(0.5f, 0.9633911f), "Click Cancel ride", 1000));
            TaxiOrderingClicks.Add(number++, new Click(new PointF(0.5f, 1.0f), "Click reason for cancellation", 1000));
            
            DataCleaningClicks.Add(number++, new Click(new PointF(1.04797983f, 0.896081746f), "Click Home", 1000));
            DataCleaningClicks.Add(number++, new Click(new PointF(0.8863636f, 0.3857729f), "Click Settings icon", 3000));
            DataCleaningClicks.Add(number++, new Click(new PointF(0.5f, 0.760601938f), "Click applications in settings", 1500));
            DataCleaningClicks.Add(number++, new Click(new PointF(0.5f, 0.24897401f), "Click Bolt in applications", 3000));
            DataCleaningClicks.Add(number++, new Click(new PointF(0.7323232f, 0.432284534f), "Click Clear Data", 1000));
            DataCleaningClicks.Add(number++, new Click(new PointF(0.7929293f, 0.6019152f), "Click OK", 1500));
            DataCleaningClicks.Add(number, new Click(new PointF(1.04797983f, 0.896081746f), "Click Home", 1000));
        }
    }
}
