using UnityEngine;

namespace KSPDataExport
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class Window : MonoBehaviour
    {
        public bool showGUI = false;
        bool showLoggedVals = false;
        Rect windowRect = new Rect(150, 100, 275, 300);
        Rect loggedValsRect = new Rect(150, 100, 225, 300);
        Rect buttonRect = new Rect(50, 25, 175, 25);
        Rect infoRect = new Rect(12.5f, 60, 250, 20);
        Rect valRect = new Rect(10, 60, 175, 20);
        Rect headerMainRect = new Rect(87.5f, 110, 100, 22);
        Rect headerRect = new Rect(62.5f, 110, 100, 22);
        Rect inptRect = new Rect(112.5f, 140, 50, 20);
        bool wasLoggingStoppedByIncorrectLogRateValue = false;
        public string onText;
        public string autoText;
        public string logRate;
        public string statusText;

        GUIStyle valStyle;
        GUIStyle buttonStyle;
        GUIStyle infoStyle;
        GUIStyle closeStyle;

        public static string appPath = Application.dataPath;
        void Start()
        {
            logRate = DataExport.waitTime.ToString();
            showGUI = false;
            onText = DataExport.isLogging == true ? "Turn Off" : "Turn On";


        }
        void Update()
        {
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.N))
            {
                showGUI = !showGUI;
                showLoggedVals = false;
            }

        }

        void OnGUI()
        {
            buttonStyle = new GUIStyle("button");
            buttonStyle.fontSize = 16;
            infoStyle = new GUIStyle("box");
            infoStyle.fontSize = 11;
            infoStyle.alignment = TextAnchor.MiddleLeft;
            closeStyle = new GUIStyle("button");
            closeStyle.alignment = TextAnchor.UpperLeft;
            valStyle = new GUIStyle("box");
            valStyle.fontSize = 11;
            valStyle.alignment = TextAnchor.MiddleLeft;

            if (showGUI)
            {
                windowRect = GUI.Window(0, windowRect, MakeWindow, "KSP Data Export");
            }
            if (showLoggedVals)
            {
                loggedValsRect = GUI.Window(1, loggedValsRect, MakeLoggedValsWindow, "Logged Values");
            }
        }

        void MakeLoggedValsWindow(int windowID)
        {
            if (GUI.Button(new Rect(200, 20, 20, 20), "x", closeStyle))
            {
                showLoggedVals = false;
            }


            GUI.Box(new Rect(headerRect.x, headerRect.y - 80, headerRect.width, headerRect.height), "Vessel");

            GUI.Box(valRect, "Velocity", valStyle);
            Vals.logVelocity = GUI.Toggle(new Rect(200, 60, 12.5f, 12.5f), Vals.logVelocity, "");
            GUI.Box(new Rect(valRect.x, valRect.y + 25, valRect.width, valRect.height), "Acceleration", valStyle);
            Vals.logAcceleration = GUI.Toggle(new Rect(200, 85, 12.5f, 12.5f), Vals.logAcceleration, "");
            GUI.Box(new Rect(valRect.x, valRect.y + 50, valRect.width, valRect.height), "Apoapsis", valStyle);
            Vals.logAp = GUI.Toggle(new Rect(200, 110, 12.5f, 12.5f), Vals.logAp, "");

            GUI.DragWindow(new Rect(0, 0, 10000, 50000));
        }
        void MakeWindow(int windowID)
        {

            if (GUI.Button(new Rect(250, 20, 20, 20), "x", closeStyle))
            {
                showGUI = false;
            }
            if (GUI.Button(buttonRect, onText, buttonStyle))
            {
                DataExport.isLogging = !DataExport.isLogging;
                onText = DataExport.isLogging == true ? "Turn Off" : "Turn On";
            }
            if (GUI.Button(infoRect, "CSV Name: " + DataExport.CSVName, infoStyle))
            {
                System.Diagnostics.Process.Start(DataExport.CSVpath);
            }
            GUI.Box(new Rect(infoRect.x, infoRect.y + 25, infoRect.width, infoRect.height), "File size: " + DataExport.fileSize, infoStyle);
            GUI.Box(headerMainRect, "Log Rate (s):");
            logRate = GUI.TextField(inptRect, logRate, 3);
            if (GUI.Button(new Rect(buttonRect.x, buttonRect.y + 150, buttonRect.width, buttonRect.height), "Choose logged vals", buttonStyle))
            {
                showLoggedVals = true;
            }
            if (GUI.Button(new Rect(buttonRect.x, buttonRect.y + 190, buttonRect.width, buttonRect.height), "View graphs", buttonStyle))
            {
                Application.OpenURL(DataExport.dataPath);
            }
            if (GUI.Button(new Rect(buttonRect.x, buttonRect.y + 230, buttonRect.width, buttonRect.height), "Help", buttonStyle))
            {
                Application.OpenURL("https://github.com/kna27/ksp-data-export");
            }
            if(!float.TryParse(logRate, out float f))
            {
                onText = "Turn On";
                ScreenMessages.PostScreenMessage("Not a valid value! Logging paused.");
                wasLoggingStoppedByIncorrectLogRateValue = true;
                DataExport.isLogging = false;
            }
            else
            {
                if (wasLoggingStoppedByIncorrectLogRateValue)
                {
                    DataExport.isLogging = true;
                    wasLoggingStoppedByIncorrectLogRateValue = false;
                    DataExport.waitTime = float.Parse(logRate);
                }
            }
            GUI.DragWindow(new Rect(0, 0, 10000, 50000));
        }
    }
}
