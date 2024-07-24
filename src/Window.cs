using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace KSPDataExport
{
    /// <summary>
    ///     The mod's GUI
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class Window : MonoBehaviour
    {
        public static bool ShowGUI;
        public static bool ShowLoggedVals;
        private bool _wasLoggingStoppedByInvalidRate;

        private Rect _windowRect = new Rect(150, 100, 275, 300);
        private Rect _loggedValsRect = new Rect(150, 100, 225, 760);
        private Rect _buttonRect = new Rect(50, 25, 175, 25);
        private Rect _infoRect = new Rect(12.5f, 60, 250, 20);
        private Rect _valRect = new Rect(10, 30, 175, 20);
        private Rect _headerRect = new Rect(62.5f, 30, 100, 20);
        private readonly Rect _headerMainRect = new Rect(87.5f, 110, 100, 22);
        private readonly Rect _inputRect = new Rect(112.5f, 140, 50, 20);

        public string onText;
        public string logRate;

        private GUIStyle _valStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _infoStyle;
        private GUIStyle _closeStyle;

        private void Start()
        {
            logRate = DataExport.WaitTime.ToString(CultureInfo.InvariantCulture);
            ShowGUI = false;
            ShowLoggedVals = false;
            DataExport.IsLogging = Config.GetValue(DataExport.CfgPath, "defaultLogState");
            onText = DataExport.IsLogging ? "Turn Off" : "Turn On";
        }

        private void OnGUI()
        {
            _buttonStyle = new GUIStyle("button")
            {
                fontSize = 16
            };
            _infoStyle = new GUIStyle("box")
            {
                fontSize = 11,
                alignment = TextAnchor.MiddleLeft
            };
            _closeStyle = new GUIStyle("button")
            {
                alignment = TextAnchor.UpperLeft
            };
            _valStyle = new GUIStyle("box")
            {
                fontSize = 11,
                alignment = TextAnchor.MiddleLeft
            };

            if (ShowGUI) _windowRect = GUI.Window(0, _windowRect, MakeWindow, "KSP Data Export");
            if (ShowLoggedVals) _loggedValsRect = GUI.Window(1, _loggedValsRect, MakeLoggedValsWindow, "Logged Values");
        }

        /// <summary>
        ///     Window for choosing which values to log
        /// </summary>
        private void MakeLoggedValsWindow(int windowID)
        {
            // The close button
            if (GUI.Button(new Rect(200, 20, 20, 20), "x", _closeStyle)) ShowLoggedVals = false;

            int vertHeight = 0;
            int baseValueY = (int)_valRect.y;

            for (int i = 0; i < Enum.GetValues(typeof(Category)).Length; i++)
            {
                var categoryValues = DataExport.LoggableValues.Where(v => v.Category == (Category)i).ToList();
                if (categoryValues.Count == 0) continue;

                GUI.Box(new Rect(_headerRect.x, _headerRect.y + vertHeight, _headerRect.width, _headerRect.height),
                    Enum.GetName(typeof(Category), i));
                vertHeight += 25;

                foreach (LoggableValue value in categoryValues)
                {
                    GUI.Box(new Rect(_valRect.x, baseValueY + vertHeight, _valRect.width, _valRect.height), value.Name,
                        _valStyle);
                    value.Logging = GUI.Toggle(new Rect(200, 30 + vertHeight, 12.5f, 12.5f), value.Logging, "");
                    vertHeight += 25;
                }
            }

            _loggedValsRect.height = vertHeight + 50;
            // Make the window draggable
            GUI.DragWindow(new Rect(0, 0, 10000, 50000));
        }

        // The main window
        private void MakeWindow(int windowID)
        {
            // Close button
            if (GUI.Button(new Rect(250, 20, 20, 20), "x", _closeStyle)) ShowGUI = false;
            // Turn on/off button
            if (GUI.Button(_buttonRect, onText, _buttonStyle))
            {
                DataExport.IsLogging = !DataExport.IsLogging;
                onText = DataExport.IsLogging ? "Turn Off" : "Turn On";
            }

            // Label for CSV name
            if (GUI.Button(_infoRect, "CSV Name: " + DataExport.CsvName, _infoStyle))
                try
                {
                    Process.Start(DataExport.CsvPath);
                }
                catch
                {
                    ScreenMessages.PostScreenMessage("File does not exist yet. Turn on logging to see file.");
                }

            // Label for file size
            GUI.Box(new Rect(_infoRect.x, _infoRect.y + 25, _infoRect.width, _infoRect.height),
                "File size: " + DataExport.FileSize, _infoStyle);
            // Log Rate
            GUI.Box(_headerMainRect, "Log Rate (s):");
            logRate = GUI.TextField(_inputRect, logRate, 3);
            if (GUI.Button(new Rect(_buttonRect.x, _buttonRect.y + 150, _buttonRect.width, _buttonRect.height),
                    "Choose logged vals", _buttonStyle)) ShowLoggedVals = !ShowLoggedVals;
            // Opens folder containing .csv files
            if (GUI.Button(new Rect(_buttonRect.x, _buttonRect.y + 190, _buttonRect.width, _buttonRect.height),
                    "View CSV files", _buttonStyle))
            {
                Process.Start(DataExport.DataPath);
                Application.OpenURL(DataExport.DataPath);
            }

            // Opens link to GitHub repo
            if (GUI.Button(new Rect(_buttonRect.x, _buttonRect.y + 230, _buttonRect.width, _buttonRect.height), "Help",
                    _buttonStyle)) Application.OpenURL("https://github.com/kna27/ksp-data-export");
            // Check whether log rate does not contain invalid characters, and stop logging if it does
            if (!double.TryParse(logRate, out double d) || d < 0.01f)
            {
                onText = "Turn On";
                ScreenMessages.PostScreenMessage("Not a valid value! Logging paused.");
                _wasLoggingStoppedByInvalidRate = true;
                DataExport.IsLogging = false;
            }
            else
            {
                DataExport.WaitTime = d;
                if (_wasLoggingStoppedByInvalidRate)
                {
                    onText = "Turn Off";
                    DataExport.IsLogging = true;
                    _wasLoggingStoppedByInvalidRate = false;
                }
            }

            // Make window draggable
            GUI.DragWindow(new Rect(0, 0, 10000, 50000));
        }
    }
}
