//The GUI

using UnityEngine;

namespace KSPDataExport
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class Window : MonoBehaviour
    {
        public bool showGUI = false;
        bool showLoggedVals = false;
        bool wasLoggingStoppedByIncorrectLogRateValue = false;

        Rect windowRect = new Rect(150, 100, 275, 300);
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

        //The window for selecting which values to log
        void MakeLoggedValsWindow(int windowID)
        {
            //The close button
            if (GUI.Button(new Rect(200, 20, 20, 20), "x", closeStyle))
            {
                showLoggedVals = false;
            }
            //The Headers, Labels, and Buttons for each value
            GUI.Box(new Rect(headerRect.x, headerRect.y + 0, headerRect.width, headerRect.height), "Vessel");
            GUI.Box(new Rect(valRect.x, valRect.y + 25, valRect.width, valRect.height), "Velocity", valStyle);
            Vals.logVelocity = GUI.Toggle(new Rect(200, 55, 12.5f, 12.5f), Vals.logVelocity, "");
            GUI.Box(new Rect(valRect.x, valRect.y + 50, valRect.width, valRect.height), "G-Force", valStyle);
            Vals.logGForce = GUI.Toggle(new Rect(200, 80, 12.5f, 12.5f), Vals.logGForce, "");
            GUI.Box(new Rect(valRect.x, valRect.y + 75, valRect.width, valRect.height), "Acceleration", valStyle);
            Vals.logAcceleration = GUI.Toggle(new Rect(200, 105, 12.5f, 12.5f), Vals.logAcceleration, "");
            GUI.Box(new Rect(valRect.x, valRect.y + 100, valRect.width, valRect.height), "Thrust", valStyle);
            Vals.logThrust = GUI.Toggle(new Rect(200, 130, 12.5f, 12.5f), Vals.logThrust, "");
            GUI.Box(new Rect(valRect.x, valRect.y + 125, valRect.width, valRect.height), "TWR", valStyle);
            Vals.logTWR = GUI.Toggle(new Rect(200, 155, 12.5f, 12.5f), Vals.logTWR, "");
            GUI.Box(new Rect(valRect.x, valRect.y + 150, valRect.width, valRect.height), "Mass", valStyle);
            Vals.logMass = GUI.Toggle(new Rect(200, 180, 12.5f, 12.5f), Vals.logMass, "");
            GUI.Box(new Rect(valRect.x, valRect.y + 175, valRect.width, valRect.height), "Pitch (N/A WIP)", valStyle);
            Vals.logPitch = GUI.Toggle(new Rect(200, 205, 12.5f, 12.5f), Vals.logPitch, "");
            GUI.Box(new Rect(headerRect.x, headerRect.y + 200, headerRect.width, headerRect.height), "Position");
            GUI.Box(new Rect(valRect.x, valRect.y + 225, valRect.width, valRect.height), "Altitude From Terrain", valStyle);
            Vals.logAltTer = GUI.Toggle(new Rect(200, 255, 12.5f, 12.5f), Vals.logAltTer, "");
            GUI.Box(new Rect(valRect.x, valRect.y + 250, valRect.width, valRect.height), "Altitude From Sea", valStyle);
            Vals.logAltSea = GUI.Toggle(new Rect(200, 280, 12.5f, 12.5f), Vals.logAltSea, "");
            GUI.Box(new Rect(valRect.x, valRect.y + 275, valRect.width, valRect.height), "Downrange Distance", valStyle);
            Vals.logDownrangeDist = GUI.Toggle(new Rect(200, 305, 12.5f, 12.5f), Vals.logDownrangeDist, "");
            GUI.Box(new Rect(valRect.x, valRect.y + 300, valRect.width, valRect.height), "Latitude", valStyle);
            Vals.logLat = GUI.Toggle(new Rect(200, 330, 12.5f, 12.5f), Vals.logLat, "");
            GUI.Box(new Rect(valRect.x, valRect.y + 325, valRect.width, valRect.height), "Longitude", valStyle);
            Vals.logLon = GUI.Toggle(new Rect(200, 355, 12.5f, 12.5f), Vals.logLon, "");
            GUI.Box(new Rect(headerRect.x, headerRect.y + 350, headerRect.width, headerRect.height), "Orbit");
            GUI.Box(new Rect(valRect.x, valRect.y + 375, valRect.width, valRect.height), "Apoapsis", valStyle);
            Vals.logAp = GUI.Toggle(new Rect(200, 405, 12.5f, 12.5f), Vals.logAp, "");
            GUI.Box(new Rect(valRect.x, valRect.y + 400, valRect.width, valRect.height), "Periapsis", valStyle);
            Vals.logPe = GUI.Toggle(new Rect(200, 430, 12.5f, 12.5f), Vals.logPe, "");
            GUI.Box(new Rect(valRect.x, valRect.y + 425, valRect.width, valRect.height), "Inclination", valStyle);
            Vals.logInc = GUI.Toggle(new Rect(200, 455, 12.5f, 12.5f), Vals.logInc, "");
            GUI.Box(new Rect(valRect.x, valRect.y + 450, valRect.width, valRect.height), "Orbital Velocity", valStyle);
            Vals.logOrbVel = GUI.Toggle(new Rect(200, 480, 12.5f, 12.5f), Vals.logOrbVel, "");
            GUI.Box(new Rect(valRect.x, valRect.y + 475, valRect.width, valRect.height), "Gravity (N/A WIP)", valStyle);
            Vals.logGravity = GUI.Toggle(new Rect(200, 505, 12.5f, 12.5f), Vals.logGravity, "");
            GUI.Box(new Rect(headerRect.x, headerRect.y + 500, headerRect.width, headerRect.height), "Target");
            GUI.Box(new Rect(valRect.x, valRect.y + 525, valRect.width, valRect.height), "Target Distance (N/A WIP)", valStyle);
            Vals.logTargDist = GUI.Toggle(new Rect(200, 555, 12.5f, 12.5f), Vals.logTargDist, "");
            GUI.Box(new Rect(valRect.x, valRect.y + 550, valRect.width, valRect.height), "Target Velocity (N/A WIP)", valStyle);
            Vals.logTargVel = GUI.Toggle(new Rect(200, 580, 12.5f, 12.5f), Vals.logTargVel, "");
            GUI.Box(new Rect(headerRect.x, headerRect.y + 575, headerRect.width, headerRect.height), "Resources");
            GUI.Box(new Rect(valRect.x, valRect.y + 600, valRect.width, valRect.height), "Stage DeltaV", valStyle);
            Vals.logStageDV = GUI.Toggle(new Rect(200, 630, 12.5f, 12.5f), Vals.logStageDV, "");
            GUI.Box(new Rect(valRect.x, valRect.y + 625, valRect.width, valRect.height), "Vessel DeltaV", valStyle);
            Vals.logVesselDV = GUI.Toggle(new Rect(200, 655, 12.5f, 12.5f), Vals.logVesselDV, "");
            GUI.Box(new Rect(headerRect.x, headerRect.y + 650, headerRect.width, headerRect.height), "Science");
            GUI.Box(new Rect(valRect.x, valRect.y + 675, valRect.width, valRect.height), "Pressure (N/A WIP)", valStyle);
            Vals.logPressure = GUI.Toggle(new Rect(200, 705, 12.5f, 12.5f), Vals.logPressure, "");
            GUI.Box(new Rect(valRect.x, valRect.y + 700, valRect.width, valRect.height), "Temperature (N/A WIP)", valStyle);
            Vals.logTemperature = GUI.Toggle(new Rect(200, 730, 12.5f, 12.5f), Vals.logTemperature, "");

            //Make the window dragable
            GUI.DragWindow(new Rect(0, 0, 10000, 50000));
        }

        //The main window
        void MakeWindow(int windowID)
        {
            //Close button
            if (GUI.Button(new Rect(250, 20, 20, 20), "x", closeStyle))
            {
                showGUI = false;
            }
            //Turn on/off button
            if (GUI.Button(buttonRect, onText, buttonStyle))
            {
                DataExport.isLogging = !DataExport.isLogging;
                onText = DataExport.isLogging == true ? "Turn Off" : "Turn On";
            }
            //Label for CSV name
            if (GUI.Button(infoRect, "CSV Name: " + DataExport.CSVName, infoStyle))
            {
                try
                {
                    System.Diagnostics.Process.Start(DataExport.CSVpath);
                }
                catch
                {
                    ScreenMessages.PostScreenMessage("File does not exist yet. Turn on logging to see file.");
                }
            }
            //Label for file size
            GUI.Box(new Rect(infoRect.x, infoRect.y + 25, infoRect.width, infoRect.height), "File size: " + DataExport.fileSize, infoStyle);
            //Log Rate
            GUI.Box(headerMainRect, "Log Rate (s):");
            logRate = GUI.TextField(inptRect, logRate, 3);
            if (GUI.Button(new Rect(buttonRect.x, buttonRect.y + 150, buttonRect.width, buttonRect.height), "Choose logged vals", buttonStyle))
            {
                showLoggedVals = true;
            }
            //Opens folder containing .csv files
            if (GUI.Button(new Rect(buttonRect.x, buttonRect.y + 190, buttonRect.width, buttonRect.height), "View graphs", buttonStyle))
            {
                Application.OpenURL(DataExport.dataPath);
            }
            //Opens link to GitHub repo
            if (GUI.Button(new Rect(buttonRect.x, buttonRect.y + 230, buttonRect.width, buttonRect.height), "Help", buttonStyle))
            {
                Application.OpenURL("https://github.com/kna27/ksp-data-export");
            }
            //Check whether log rate does not contain invalid characters, and stop logging if it does
            if (!float.TryParse(logRate, out float f))
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
            //Make window draggable
            GUI.DragWindow(new Rect(0, 0, 10000, 50000));
        }
    }
}
