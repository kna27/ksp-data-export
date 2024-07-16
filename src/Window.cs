using System;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace KSPDataExport
{
    /// <summary>
    /// The mod's GUI
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class Window : MonoBehaviour
    {
        public static bool showGUI;
        public static bool showLoggedVals;
        bool wasLoggingStoppedByIncorrectLogRateValue;

        private Rect windowRect = new Rect(150, 100, 275, 300);
        Rect loggedValsRect = new Rect(150, 100, 225, 760);
        Rect buttonRect = new Rect(50, 25, 175, 25);
        Rect infoRect = new Rect(12.5f, 60, 250, 20);
        Rect valRect = new Rect(10, 30, 175, 20);
        Rect headerMainRect = new Rect(87.5f, 110, 100, 22);
        Rect headerRect = new Rect(62.5f, 30, 100, 20);
        Rect inptRect = new Rect(112.5f, 140, 50, 20);

        public string onText;
        public string autoText;
        public string logRate;
        public string statusText;
        public static string appPath = Application.dataPath;

        GUIStyle valStyle;
        GUIStyle buttonStyle;
        GUIStyle infoStyle;
        GUIStyle closeStyle;

        void Start()
        {
            logRate = DataExport.waitTime.ToString(CultureInfo.InvariantCulture);
            showGUI = false;
            showLoggedVals = false;
            DataExport.isLogging = Config.GetValue(DataExport.cfgPath, "defaultLogState");
            onText = DataExport.isLogging ? "Turn Off" : "Turn On";
        }

        void OnGUI()
        {
            buttonStyle = new GUIStyle("button")
            {
                fontSize = 16
            };
            infoStyle = new GUIStyle("box")
            {
                fontSize = 11,
                alignment = TextAnchor.MiddleLeft
            };
            closeStyle = new GUIStyle("button")
            {
                alignment = TextAnchor.UpperLeft
            };
            valStyle = new GUIStyle("box")
            {
                fontSize = 11,
                alignment = TextAnchor.MiddleLeft
            };

            if (showGUI)
            {
                windowRect = GUI.Window(0, windowRect, MakeWindow, "KSP Data Export");
            }
            if (showLoggedVals)
            {
                loggedValsRect = GUI.Window(1, loggedValsRect, MakeLoggedValsWindow, "Logged Values");
            }
        }

        // The window for selecting which values to log
        void MakeLoggedValsWindow(int windowID)
        {
            // The close button
            if (GUI.Button(new Rect(200, 20, 20, 20), "x", closeStyle))
            {
                showLoggedVals = false;
            }

            int vertHeight = 0;
            int baseValueY = (int)valRect.y;

            for (int i = 0; i < Enum.GetValues(typeof(Category)).Length; i++)
            {
                var categoryValues = DataExport.loggableValues.Where(v => v.Category == (Category)i).ToList();
                if (categoryValues.Count == 0)
                {
                    continue;
                }

                GUI.Box(new Rect(headerRect.x, headerRect.y + vertHeight, headerRect.width, headerRect.height), Enum.GetName(typeof(Category), i));
                vertHeight += 25;

                foreach (var value in categoryValues)
                {
                    GUI.Box(new Rect(valRect.x, baseValueY + vertHeight, valRect.width, valRect.height), value.Name, valStyle);
                    value.Logging = GUI.Toggle(new Rect(200, 30 + vertHeight, 12.5f, 12.5f), value.Logging, "");
                    vertHeight += 25;
                }
            }
            loggedValsRect.height = vertHeight + 50;
            // Make the window draggable
            GUI.DragWindow(new Rect(0, 0, 10000, 50000));
        }

        // The main window
        void MakeWindow(int windowID)
        {
            // Close button
            if (GUI.Button(new Rect(250, 20, 20, 20), "x", closeStyle))
            {
                showGUI = false;
            }
            // Turn on/off button
            if (GUI.Button(buttonRect, onText, buttonStyle))
            {
                DataExport.isLogging = !DataExport.isLogging;
                onText = DataExport.isLogging ? "Turn Off" : "Turn On";
            }
            // Label for CSV name
            if (GUI.Button(infoRect, "CSV Name: " + DataExport.CSVName, infoStyle))
            {
                try
                {
                    System.Diagnostics.Process.Start(DataExport.CSVPath);
                }
                catch
                {
                    ScreenMessages.PostScreenMessage("File does not exist yet. Turn on logging to see file.");
                }
            }
            // Label for file size
            GUI.Box(new Rect(infoRect.x, infoRect.y + 25, infoRect.width, infoRect.height), "File size: " + DataExport.fileSize, infoStyle);
            // Log Rate
            GUI.Box(headerMainRect, "Log Rate (s):");
            logRate = GUI.TextField(inptRect, logRate, 3);
            if (GUI.Button(new Rect(buttonRect.x, buttonRect.y + 150, buttonRect.width, buttonRect.height), "Choose logged vals", buttonStyle))
            {
                showLoggedVals = !showLoggedVals;
            }
            // Opens folder containing .csv files
            if (GUI.Button(new Rect(buttonRect.x, buttonRect.y + 190, buttonRect.width, buttonRect.height), "View graphs", buttonStyle))
            {
                System.Diagnostics.Process.Start(DataExport.dataPath);
                Application.OpenURL(DataExport.dataPath);
            }
            // Opens link to GitHub repo
            if (GUI.Button(new Rect(buttonRect.x, buttonRect.y + 230, buttonRect.width, buttonRect.height), "Help", buttonStyle))
            {
                Application.OpenURL("https://github.com/kna27/ksp-data-export");
            }
            // Check whether log rate does not contain invalid characters, and stop logging if it does
            if (!float.TryParse(logRate, out float f))
            {
                onText = "Turn On";
                ScreenMessages.PostScreenMessage("Not a valid value! Logging paused.");
                wasLoggingStoppedByIncorrectLogRateValue = true;
                DataExport.isLogging = false;
            }
            else
            {
                DataExport.waitTime = float.Parse(logRate);
                if (wasLoggingStoppedByIncorrectLogRateValue)
                {
                    onText = "Turn Off";
                    DataExport.isLogging = true;
                    wasLoggingStoppedByIncorrectLogRateValue = false;
                }
            }
            // Make window draggable
            GUI.DragWindow(new Rect(0, 0, 10000, 50000));
        }
    }
}
