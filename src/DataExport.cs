using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace KSPDataExport
{
    /// <summary>
    ///     Writing data to the CSV file
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class DataExport : MonoBehaviour
    {
        public static List<LoggableValue> LoggableValues;
        private static string _appPath;
        public static string CsvPath;
        public static string DataPath;
        public static string CfgPath;
        public static string FileSize;
        public static string CsvName;
        private static string _header;

        public static bool IsLogging;
        public static double WaitTime = 1f;
        private double _lastLoggedTime;

        public static Vessel ActVess;
        private static double _elapsedTime;
        public static string LaunchBody;
        public static double LaunchLat;
        public static double LaunchLon;
        private FileInfo _fi;

        private void Start()
        {
            Debug.Log("[Data Export] Initialized");
            DataPath = @"/GameData/DataExport/graphs/";
            CfgPath = @"/GameData/DataExport/logged.vals";

            ActVess = FlightGlobals.ActiveVessel;
            LaunchBody = ActVess.mainBody.bodyDisplayName;
            LaunchLat = Utilities.DegToRad(ActVess.latitude);
            LaunchLon = Utilities.DegToRad(ActVess.longitude);

            CsvName = ActVess.GetDisplayName() + "_" + DateTime.Now.ToString("MMddHHmm") + ".csv";
            CsvPath = @"/GameData/DataExport/graphs/" + CsvName;
            _appPath = Application.platform == RuntimePlatform.OSXPlayer
                ? Directory.GetParent(Directory.GetParent(Application.dataPath)?.ToString() ?? string.Empty)?.ToString()
                : Directory.GetParent(Application.dataPath)?.ToString();
            DataPath = _appPath + DataPath;
            CfgPath = _appPath + CfgPath;
            CsvPath = _appPath + CsvPath;
            if (!Directory.Exists(DataPath)) Directory.CreateDirectory(DataPath);
            if (!File.Exists(CsvPath) && IsLogging)
                InitFile();
            try
            {
                _fi = new FileInfo(CsvPath);
                FileSize = Utilities.FormatSize(_fi.Length);
            }
            catch
            {
                FileSize = "0.0 Bytes";
            }

            LoggableValues = new List<LoggableValue>
            {
                // Vessel
                new LoggableValue("Surface Speed", Category.Vessel, "logSrfSpeed",
                    () => Utilities.RoundToStr(ActVess.srf_velocity.magnitude, 2)),
                new LoggableValue("GForce", Category.Vessel, "logGForce",
                    () => Utilities.RoundToStr(ActVess.geeForce, 2)),
                new LoggableValue("Acceleration", Category.Vessel, "logAcceleration",
                    () => Utilities.RoundToStr(ActVess.acceleration.magnitude, 2)),
                new LoggableValue("Thrust", Category.Vessel, "logThrust",
                    () => Utilities.RoundToStr(Utilities.GetThrust(), 2)),
                new LoggableValue("TWR", Category.Vessel, "logTWR",
                    () => Utilities.RoundToStr(Utilities.GetThrust() / (ActVess.GetTotalMass() * 10), 2)),
                new LoggableValue("Mass", Category.Vessel, "logMass",
                    () => Utilities.RoundToStr(ActVess.GetTotalMass(), 2)),
                new LoggableValue("Pitch", Category.Vessel, "logPitch",
                    () => Utilities.RoundToStr(Utilities.GetPitch(), 2)),
                // Position
                new LoggableValue("Altitude from Terrain", Category.Position, "logAltTer",
                    () => Utilities.RoundToStr(Utilities.GetSrfAlt(), 2)),
                new LoggableValue("Altitude from the Sea", Category.Position, "logAltSea",
                    () => Utilities.RoundToStr(FlightGlobals.ship_altitude, 2)),
                new LoggableValue("Downrange Distance", Category.Position, "logDownrangeDist",
                    () => Utilities.RoundToStr(Utilities.DownrangeDistance(), 2)),
                new LoggableValue("Latitude", Category.Position, "logLat",
                    () => Utilities.RoundToStr(ActVess.latitude, 5)),
                new LoggableValue("Longitude", Category.Position, "logLon",
                    () => Utilities.RoundToStr(ActVess.longitude, 5)),
                // Orbit
                new LoggableValue("Apoapsis", Category.Orbit, "logAp",
                    () => Utilities.RoundToStr(ActVess.orbit.ApA, 2)),
                new LoggableValue("Periapsis", Category.Orbit, "logPe",
                    () => Utilities.RoundToStr(ActVess.orbit.PeA, 2)),
                new LoggableValue("Inclination", Category.Orbit, "logInc",
                    () => Utilities.RoundToStr(ActVess.orbit.inclination, 2)),
                new LoggableValue("Orbital Velocity", Category.Orbit, "logOrbVel",
                    () => Utilities.RoundToStr(ActVess.obt_velocity.magnitude, 2)),
                new LoggableValue("Gravity", Category.Orbit, "logGravity",
                    () => Utilities.RoundToStr(ActVess.graviticAcceleration.magnitude, 2)),
                // Target
                new LoggableValue("Target Distance", Category.Target, "logTargDist",
                    () => Utilities.RoundToStr(
                        FlightGlobals.fetch.vesselTargetTransform is null
                            ? 0
                            : Vector3.Distance(FlightGlobals.fetch.vesselTargetTransform.position,
                                ActVess.transform.position), 2)),
                new LoggableValue("Target Velocity", Category.Target, "logTargVel",
                    () => Utilities.RoundToStr(
                        FlightGlobals.fetch.vesselTargetTransform is null
                            ? 0
                            : FlightGlobals.ship_tgtVelocity.magnitude, 2)),
                // Resources
                new LoggableValue("Stage DeltaV", Category.Resources, "logStageDV",
                    () => Utilities.RoundToStr(
                        ActVess.VesselDeltaV.GetStage(ActVess.currentStage)
                            .GetSituationDeltaV(DeltaVSituationOptions.Altitude), 0)),
                new LoggableValue("Vessel DeltaV", Category.Resources, "logVesselDV",
                    () => Utilities.RoundToStr(
                        ActVess.VesselDeltaV.GetSituationTotalDeltaV(DeltaVSituationOptions.Altitude),
                        0)), // Uses built-in calculation which is a bit inaccurate, might fix
                // Science
                new LoggableValue("Pressure", Category.Science, "logPressure",
                    () => Utilities.RoundToStr(ActVess.staticPressurekPa, 2)),
                new LoggableValue("External Temperature", Category.Science, "logExternTemp",
                    () => Utilities.RoundToStr(ActVess.externalTemperature, 2))
            };

            _header = LoggableValues.Aggregate("Time,", (current, value) => current + value.Name + ",");
        }

        private void FixedUpdate()
        {
            if (!IsLogging) return;
            // Create the CSV folder if it does not exist
            if (!Directory.Exists(DataPath))
            {
                Debug.Log("[DataExport] Creating directory: " + DataPath);
                Directory.CreateDirectory(DataPath);
            }

            // Create the CSV file if it does not exist
            if (!File.Exists(CsvPath))
            {
                Debug.Log("[DataExport] Creating file: " + CsvPath);
                InitFile();
            }

            // Write a line to the CSV file every WaitTime seconds
            if (Math.Round(ActVess.missionTime, 2) >= _lastLoggedTime + WaitTime)
            {
                _elapsedTime = Math.Round(ActVess.missionTime, 2);
                try
                {
                    using StreamWriter file = new StreamWriter(CsvPath, true);
                    try
                    {
                        string line = _elapsedTime + ",";
                        foreach (LoggableValue l in LoggableValues) line += l.Logging ? l.Value() + "," : ",";
                        file.WriteLine(line);
                    }
                    catch (Exception e)
                    {
                        Debug.Log("[DataExport] Unable to log data to file: " + e);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("[DataExport] Unable to create StreamWriter to log data to file: " + e);
                }

                _lastLoggedTime = Math.Round(ActVess.missionTime, 2);
            }

            try
            {
                _fi.Refresh();
                FileSize = Utilities.FormatSize(_fi.Length);
            }
            catch (Exception e)
            {
                Debug.Log("[DataExport] Unable to get file size: " + e);
                FileSize = "0.0 Bytes";
            }
        }

        /// <summary>
        ///     Initializes the CSV file with the header
        /// </summary>
        private void InitFile()
        {
            using (File.Create(CsvPath))
            {
            }

            _fi = new FileInfo(CsvPath);
            using StreamWriter file = new StreamWriter(CsvPath, true);
            try
            {
                file.WriteLine(_header);
            }
            catch (Exception e)
            {
                Debug.Log("[DataExport] Error initializing file: " + e);
            }
        }
    }
}
