using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

namespace KSPDataExport
{
    /// <summary>
    /// Writing data to the CSV file
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class DataExport : MonoBehaviour
    {
        public static List<LoggableValue> loggableValues;
        private static string appPath;
        public static string CSVPath;
        public static string dataPath;
        public static string cfgPath;
        public static string fileSize;
        public static string CSVName;
        private static string header;
        
        public static bool isLogging;
        public static double waitTime = 1f;
        private double lastLoggedTime;

        public static Vessel actVess;
        public static string launchBody;
        public static double launchLat;
        public static double launchLon;
        FileInfo fi;

        static double elapsedTime;

        void Start()
        {
            Debug.Log("[Data Export] Init");
            dataPath = @"/GameData/DataExport/graphs/";
            cfgPath = @"/GameData/DataExport/logged.vals";

            actVess = FlightGlobals.ActiveVessel;
            launchBody = actVess.mainBody.bodyDisplayName;
            launchLat = Utilities.DegToRad(actVess.latitude);
            launchLon = Utilities.DegToRad(actVess.longitude);

            CSVName = actVess.GetDisplayName() + "_" + DateTime.Now.ToString("MMddHHmm") + ".csv";
            CSVPath = @"/GameData/DataExport/graphs/" + CSVName;
            appPath = Application.platform == RuntimePlatform.OSXPlayer ? Directory.GetParent(Directory.GetParent(Application.dataPath).ToString()).ToString() : Directory.GetParent(Application.dataPath).ToString();
            dataPath = appPath + dataPath;
            cfgPath = appPath + cfgPath;        
            CSVPath = appPath + CSVPath;
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            if (!File.Exists(CSVPath) && isLogging)
            {
                InitFile();
            }
            try
            {
                fi = new FileInfo(CSVPath);
                fileSize = Utilities.FormatSize(fi.Length);
            }
            catch
            {
                fileSize = "0.0 Bytes";
            }
            
            loggableValues = new List<LoggableValue>
            {
                // Vessel
                new LoggableValue("Surface Speed", Category.Vessel, "logSrfSpeed", () => Utilities.RoundToStr(actVess.srf_velocity.magnitude, 2)),
                new LoggableValue("GForce", Category.Vessel, "logGForce", () => Utilities.RoundToStr(actVess.geeForce, 2)),
                new LoggableValue("Acceleration", Category.Vessel, "logAcceleration", () => Utilities.RoundToStr(actVess.acceleration.magnitude, 2)),
                new LoggableValue("Thrust", Category.Vessel, "logThrust", () => Utilities.RoundToStr(Utilities.GetThrust(), 2)),
                new LoggableValue("TWR", Category.Vessel, "logTWR", () => Utilities.RoundToStr(Utilities.GetThrust() / (actVess.GetTotalMass() * 10), 2)),
                new LoggableValue("Mass", Category.Vessel, "logMass", () => Utilities.RoundToStr(actVess.GetTotalMass(), 2)),
                new LoggableValue("Pitch", Category.Vessel, "logPitch", () => Utilities.RoundToStr(Utilities.GetPitch(), 2)),
                // Position
                new LoggableValue("Altitude from Terrain", Category.Position, "logAltTer", () => Utilities.RoundToStr(Utilities.GetSrfAlt(), 2)),
                new LoggableValue("Altitude from the Sea", Category.Position, "logAltSea", () => Utilities.RoundToStr(FlightGlobals.ship_altitude, 2)),
                new LoggableValue("Downrange Distance", Category.Position, "logDownrangeDist", () => Utilities.RoundToStr(Utilities.DownrangeDistance(), 2)),
                new LoggableValue("Latitude", Category.Position, "logLat", () => Utilities.RoundToStr(actVess.latitude, 5)),
                new LoggableValue("Longitude", Category.Position, "logLon", () => Utilities.RoundToStr(actVess.longitude, 5)),
                // Orbit
                new LoggableValue("Apoapsis", Category.Orbit, "logAp", () => Utilities.RoundToStr(actVess.orbit.ApA, 2)),
                new LoggableValue("Periapsis", Category.Orbit, "logPe", () => Utilities.RoundToStr(actVess.orbit.PeA, 2)),
                new LoggableValue("Inclination", Category.Orbit, "logInc", () => Utilities.RoundToStr(actVess.orbit.inclination, 2)),
                new LoggableValue("Orbital Velocity", Category.Orbit, "logOrbVel", () => Utilities.RoundToStr(actVess.obt_velocity.magnitude, 2)),
                new LoggableValue("Gravity", Category.Orbit, "logGravity", () => Utilities.RoundToStr(actVess.graviticAcceleration.magnitude, 2)),
                // Target
                new LoggableValue("Target Distance", Category.Target, "logTargDist", () => Utilities.RoundToStr(FlightGlobals.fetch.vesselTargetTransform is null ? 0 : Vector3.Distance(FlightGlobals.fetch.vesselTargetTransform.position, actVess.transform.position), 2)),
                new LoggableValue("Target Velocity", Category.Target, "logTargVel", () => Utilities.RoundToStr(FlightGlobals.fetch.vesselTargetTransform is null ? 0 : FlightGlobals.ship_tgtVelocity.magnitude, 2)),
                // Resources
                new LoggableValue("Stage DeltaV", Category.Resources, "logStageDV", () => Utilities.RoundToStr(actVess.VesselDeltaV.GetStage(actVess.currentStage).GetSituationDeltaV(DeltaVSituationOptions.Altitude), 0)),
                new LoggableValue("Vessel DeltaV", Category.Resources, "logVesselDV", () => Utilities.RoundToStr(actVess.VesselDeltaV.GetSituationTotalDeltaV(DeltaVSituationOptions.Altitude), 0)), // Uses built-in calculation which is a bit inaccurate, might fix
                // Science
                new LoggableValue("Pressure", Category.Science, "logPressure", () => Utilities.RoundToStr(actVess.staticPressurekPa, 2)),
                new LoggableValue("External Temperature", Category.Science, "logExternTemp", () => Utilities.RoundToStr(actVess.externalTemperature, 2))
            };
            
            header = loggableValues.Aggregate("Time,", (current, value) => current + (value.Name + ","));
        }

        void FixedUpdate()
        {
            // Create the CSV folder if it does not exist
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            else if (isLogging)
            {
                try
                {
                    string[] arrLine = File.ReadAllLines(CSVPath);
                    arrLine[0] = header;
                    File.WriteAllLines(CSVPath, arrLine);
                }
                catch
                {
                    using (FileStream fs = File.Create(CSVPath)) { };
                    using StreamWriter file = new StreamWriter(CSVPath, true);
                    try
                    {
                        file.WriteLine(CSVPath, header);
                    }
                    catch (Exception e)
                    {
                        Debug.Log("[DataExport] Error writing headers: "  + e);
                    }
                }
            }
            // Create the CSV file if we are logging
            if (!File.Exists(CSVPath) && isLogging)
            {
                File.Create(CSVPath);
            }
            if (isLogging)
            {
                if (Math.Round(actVess.missionTime, 2) >= lastLoggedTime + waitTime)
                {
                    // Setting the value of all variables
                    elapsedTime = Math.Round(actVess.missionTime, 2);
                    try
                    {
                        using StreamWriter file = new StreamWriter(CSVPath, true);
                        try
                        {
                            string line = elapsedTime + ",";
                            for (int i = 0; i < loggableValues.Count; i++)
                            {
                                line += loggableValues[i].Logging ? loggableValues[i].Value() + "," : ",";
                            }
                            file.WriteLine(line);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("[DataExport] Was unable to log data to file: " + e);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log("[DataExport] Unable to create StreamWriter: " + e);
                    }
                    lastLoggedTime = Math.Round(actVess.missionTime, 2);
                }
                try
                {
                    fi = new FileInfo(CSVPath);
                    fileSize = Utilities.FormatSize(fi.Length);
                }
                catch
                {
                    fileSize = "0.0 Bytes";
                }
            }
        }

        // Initializes the file and writes the headers
        static void InitFile()
        {
            File.Create(CSVPath);
            File.Delete(CSVPath);
            using (FileStream fs = File.Create(CSVPath)) { };
            using StreamWriter file = new StreamWriter(CSVPath, true);
            try
            {
                file.WriteLine(CSVPath, header);
            }
            catch (Exception e)
            {
                Debug.Log("[DataExport] Error initializing file: " + e);
            }
        }
    }
}
