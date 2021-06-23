//Writing data to the CSV file

using System;
using System.Globalization;
using UnityEngine;
using System.IO;

namespace KSPDataExport
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class DataExport : MonoBehaviour
    {
        private static string appPath;
        public static string CSVPath;
        public static string dataPath;
        public static string cfgPath;
        public static string fileSize;
        public static string CSVName;
        static readonly string[] suffixes = { " Bytes", " KB", " MB" };

        public static bool isLogging;
        public static float waitTime = 1f;
        private int lastLoggedTime;

        private static Vessel actVess;
        static string launchBody;
        static double launchLat;
        static double launchLon;
        FileInfo fi;

        static int elapsedTime;
        static string srfVel;
        static string gForce;
        static string acceleration;
        static string thrust;
        static string TWR;
        static string mass;
        static string pitch;

        static string altTer;
        static string altSea;
        static string downrangeDist;
        static string lat;
        static string lon;

        static string ap;
        static string pe;
        static string inc;
        static string orbVel;
        static string gravity;

        static string targDist;
        static string targVel;

        static string stageDV;
        static string vesselDV;

        static string pressure;
        // ReSharper disable once NotAccessedField.Local
        static string temp;

        void Start()
        {
            Debug.Log("{Data Export} Init");
            dataPath = @"/GameData/DataExport/graphs/";
            cfgPath = @"/GameData/DataExport/logged.vals";

            actVess = FlightGlobals.ActiveVessel;
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
                fileSize = DataExport.FormatSize(fi.Length);
            }
            catch
            {
                fileSize = "0.0 Bytes";
            }
            launchBody = actVess.mainBody.bodyDisplayName;
            launchLat = DegToRad(actVess.latitude);
            launchLon = DegToRad(actVess.longitude);
        }

        void FixedUpdate()
        {

            //Create the CSV folder if it does not exist
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            else if (isLogging)
            {
                try
                {
                    string[] arrLine = File.ReadAllLines(CSVPath);
                    arrLine[0] =
                        $"Time,{(Vals.everLogVelocity ? "Velocity," : "")}{(Vals.everLogGForce ? "GForce," : "")}{(Vals.everLogAcceleration ? "Acceleration," : "")}{(Vals.everLogThrust ? "Thrust," : "")}{(Vals.everLogTWR ? "TWR," : "")}{(Vals.everLogMass ? "Mass," : "")}{(Vals.everLogPitch ? "Pitch," : "")}{(Vals.everLogAltTer ? "AltitudeFromTerrain," : "")}{(Vals.everLogAltSea ? "AltitudeFromSea," : "")}{(Vals.everLogDownrangeDist ? "DownrangeDistance," : "")}{(Vals.everLogLat ? "Latitude," : "")}{(Vals.everLogLon ? "Longitude," : "")}{(Vals.everLogAp ? "Apoapsis," : "")}{(Vals.everLogPe ? "Periapsis," : "")}{(Vals.everLogInc ? "Inclination," : "")}{(Vals.everLogOrbVel ? "OrbitalVelocity," : "")}{(Vals.everLogGravity ? "Gravity," : "")}{(Vals.everLogTargDist ? "TargetDistance," : "")}{(Vals.everLogTargVel ? "TargetVelocity," : "")}{(Vals.everLogStageDV ? "StageDeltaV," : "")}{(Vals.everLogVesselDV ? "VesselDeltaV," : "")}{(Vals.everLogPressure ? "Pressure," : "")}{(Vals.everLogTemperature ? "Temperature," : "")}";
                    File.WriteAllLines(CSVPath, arrLine);
                }
                catch
                {
                    File.Create(CSVPath);
                    using StreamWriter file = new StreamWriter(CSVPath, true);
                    try
                    {
                        file.WriteLine(CSVPath,
                            $"Time,{(Vals.everLogVelocity ? "Velocity," : "")}{(Vals.everLogGForce ? "GForce," : "")}{(Vals.everLogAcceleration ? "Acceleration," : "")}{(Vals.everLogThrust ? "Thrust," : "")}{(Vals.everLogTWR ? "TWR," : "")}{(Vals.everLogMass ? "Mass," : "")}{(Vals.everLogPitch ? "Pitch," : "")}{(Vals.everLogAltTer ? "AltitudeFromTerrain," : "")}{(Vals.everLogAltSea ? "AltitudeFromSea," : "")}{(Vals.everLogDownrangeDist ? "DownrangeDistance," : "")}{(Vals.everLogLat ? "Latitude," : "")}{(Vals.everLogLon ? "Longitude," : "")}{(Vals.everLogAp ? "Apoapsis," : "")}{(Vals.everLogPe ? "Periapsis," : "")}{(Vals.everLogInc ? "Inclination," : "")}{(Vals.everLogOrbVel ? "OrbitalVelocity," : "")}{(Vals.everLogGravity ? "Gravity," : "")}{(Vals.everLogTargDist ? "TargetDistance," : "")}{(Vals.everLogTargVel ? "TargetVelocity," : "")}{(Vals.everLogStageDV ? "StageDeltaV," : "")}{(Vals.everLogVesselDV ? "VesselDeltaV," : "")}{(Vals.everLogPressure ? "Pressure," : "")}{(Vals.everLogTemperature ? "Temperature," : "")}");
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Error writing headers: " + e);
                    }
                }
            }
            //Create the CSV file if we are logging
            if (!File.Exists(CSVPath) && isLogging)
            {
                File.Create(CSVPath);
            }
            if (isLogging)
            {
                if (Mathf.RoundToInt((float)actVess.missionTime) >= lastLoggedTime + waitTime)
                {
                    //Setting the value of all variables
                    elapsedTime = Mathf.RoundToInt((float)actVess.missionTime);

                    srfVel = Vals.logVelocity ?
                        $"{Mathf.RoundToInt((float) actVess.srf_velocity.magnitude).ToString()},"
                        : Vals.everLogVelocity ? "," : "";
                    gForce = Vals.logGForce ?
                        $"{Math.Round(actVess.geeForce, 2).ToString(CultureInfo.InvariantCulture)},"
                        : Vals.everLogGForce ? "," : "";
                    acceleration = Vals.logAcceleration ?
                        $"{Math.Round(actVess.acceleration.magnitude, 2).ToString(CultureInfo.InvariantCulture)},"
                        : Vals.everLogAcceleration ? "," : "";
                    thrust = Vals.logThrust ?
                        $"{Math.Round(actVess.VesselDeltaV.GetStage(actVess.currentStage).GetSituationThrust(DeltaVSituationOptions.Altitude), 2).ToString(CultureInfo.InvariantCulture)},"
                        : Vals.everLogThrust ? "," : "";
                    TWR = Vals.logTWR ?
                        $"{Math.Round(actVess.VesselDeltaV.GetStage(actVess.currentStage).GetSituationTWR(DeltaVSituationOptions.Altitude), 2).ToString(CultureInfo.InvariantCulture)},"
                        : Vals.everLogTWR ? "," : "";
                    mass = Vals.logMass ?
                        $"{((float) Math.Round(actVess.GetTotalMass() * 100f) / 100f).ToString(CultureInfo.InvariantCulture)},"
                        : Vals.everLogMass ? "," : "";
                    pitch = Vals.logPitch ? $"{0.ToString()}," : Vals.everLogPitch ? "," : ""; //TODO

                    altTer = Vals.logAltTer ?
                        $"{Math.Round(FlightGlobals.ship_altitude, 2).ToString(CultureInfo.InvariantCulture)},"
                        : Vals.everLogAltTer ? "," : "";
                    altSea = Vals.logAltSea ?
                        $"{Math.Round(actVess.terrainAltitude, 2).ToString(CultureInfo.InvariantCulture)},"
                        : Vals.everLogAltSea ? "," : "";
                    downrangeDist = Vals.logDownrangeDist ?
                        $"{Math.Round(Distance(actVess.latitude, actVess.longitude), 2).ToString(CultureInfo.InvariantCulture)},"
                        : Vals.everLogDownrangeDist ? "," : "";
                    lat = Vals.logLat ? $"{Math.Round(actVess.latitude, 2).ToString(CultureInfo.InvariantCulture)}," : Vals.everLogLat ? "," : "";
                    lon = Vals.logLon ? $"{Math.Round(actVess.longitude, 2).ToString(CultureInfo.InvariantCulture)}," : Vals.everLogLon ? "," : "";

                    ap = Vals.logAp ? $"{Math.Max(0, Mathf.RoundToInt((float) actVess.orbit.ApA)).ToString()}," : "";
                    pe = Vals.logPe ? $"{Math.Max(0, Mathf.RoundToInt((float) actVess.orbit.PeA)).ToString()}," : Vals.everLogPe ? "," : "";
                    inc = Vals.logInc ?
                        $"{Math.Round(FlightGlobals.ship_orbit.inclination, 2).ToString(CultureInfo.InvariantCulture)},"
                        : Vals.everLogInc ? "," : "";
                    orbVel = Vals.logOrbVel ?
                        $"{Mathf.RoundToInt((float) actVess.obt_velocity.magnitude).ToString()},"
                        : Vals.everLogOrbVel ? "," : "";
                    gravity = Vals.logGravity ? $"{0.ToString()}," : Vals.everLogGravity ? "," : ""; //TODO

                    targDist = Vals.logTargDist ?
                        $"{Math.Round(Vector3.Distance(FlightGlobals.fetch.vesselTargetTransform.position, actVess.transform.position), 2).ToString(CultureInfo.InvariantCulture)},"
                        : Vals.everLogTargDist ? "," : "";
                    targVel = Vals.logTargVel ?
                        $"{Math.Round(FlightGlobals.ship_tgtVelocity.magnitude, 2).ToString(CultureInfo.InvariantCulture)},"
                        : Vals.everLogTargVel ? "," : "";

                    stageDV = Vals.logStageDV ?
                        $"{Mathf.RoundToInt(actVess.VesselDeltaV.GetStage(actVess.currentStage).GetSituationDeltaV(DeltaVSituationOptions.Altitude)).ToString()},"
                        : Vals.everLogStageDV ? "," : "";
                    vesselDV = Vals.logVesselDV ?
                        $"{Mathf.RoundToInt((float) actVess.VesselDeltaV.GetSituationTotalDeltaV(DeltaVSituationOptions.Altitude)).ToString()},"
                        : Vals.everLogVesselDV ? "," : "";

                    pressure = Vals.logPressure ?
                        $"{Math.Round(actVess.staticPressurekPa, 2).ToString(CultureInfo.InvariantCulture)},"
                        : Vals.everLogPressure ? "," : "";
                    temp = Vals.logTemperature ? $"{0.ToString()}," : Vals.everLogTemperature ? "," : ""; //TODO

                    //Write the variables to the file
                    AddData();
                    lastLoggedTime = Mathf.RoundToInt((float)actVess.missionTime);
                }
                try
                {
                    fi = new FileInfo(CSVPath);
                    fileSize = DataExport.FormatSize(fi.Length);
                }
                catch
                {
                    fileSize = "0.0 Bytes";
                }
            }
        }

        //Adds the new line containing the values chosen to be logged to the file
        private static void AddData()
        {
            try
            {
                using StreamWriter file = new StreamWriter(CSVPath, true);
                try
                {
                    //Write a new line to the file
                    file.WriteLine("{0},{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}{20}{21}{22}", elapsedTime, srfVel, gForce, acceleration, thrust, TWR, mass, pitch, altTer, altSea, downrangeDist, lat, lon, ap, pe, inc, orbVel, gravity, targDist, targVel, stageDV, vesselDV, pressure);
                }
                catch (Exception e)
                {
                    Debug.Log("Was unable to log data to file: " + e);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error: ", ex);
            }
        }

        //Initializes the file and writes the headers
        static void InitFile()
        {
            File.Create(CSVPath);
            File.Delete(CSVPath);
            File.Create(CSVPath);
            using StreamWriter file = new StreamWriter(CSVPath, true);
            try
            {
                file.WriteLine(CSVPath,
                    $"Time,{(Vals.everLogVelocity ? "Velocity," : "")}{(Vals.everLogGForce ? "GForce," : "")}{(Vals.everLogAcceleration ? "Acceleration," : "")}{(Vals.everLogThrust ? "Thrust," : "")}{(Vals.everLogTWR ? "TWR," : "")}{(Vals.everLogMass ? "Mass," : "")}{(Vals.everLogPitch ? "Pitch," : "")}{(Vals.everLogAltTer ? "AltitudeFromTerrain," : "")}{(Vals.everLogAltSea ? "AltitudeFromSea," : "")}{(Vals.everLogDownrangeDist ? "DownrangeDistance," : "")}{(Vals.everLogLat ? "Latitude," : "")}{(Vals.everLogLon ? "Longitude," : "")}{(Vals.everLogAp ? "Apoapsis," : "")}{(Vals.everLogPe ? "Periapsis," : "")}{(Vals.everLogInc ? "Inclination," : "")}{(Vals.everLogOrbVel ? "OrbitalVelocity," : "")}{(Vals.everLogGravity ? "Gravity," : "")}{(Vals.everLogTargDist ? "TargetDistance," : "")}{(Vals.everLogTargVel ? "TargetVelocity," : "")}{(Vals.everLogStageDV ? "StageDeltaV," : "")}{(Vals.everLogVesselDV ? "VesselDeltaV," : "")}{(Vals.everLogPressure ? "Pressure," : "")}{(Vals.everLogTemperature ? "Temperature," : "")}");
            }
            catch (Exception e)
            {
                Debug.Log("Exception when writing headers: " + e);
            }
        }

        //Gets the distance between a lat/lon pair and the KSC
        private static double Distance(double _lat, double _lon)
        {
            if (actVess.mainBody.bodyDisplayName == launchBody)
            {
                double distance = ((600 * Math.Acos((Math.Sin(launchLat) * Math.Sin(DegToRad(_lat))) + Math.Cos(launchLat) * Math.Cos(DegToRad(_lat)) * Math.Cos(DegToRad(_lon) - launchLon))));
                return (distance);
            }
            else
            {
                return 0;
            }
        }

        //Converts degrees to radians (used in Distance)
        private static double DegToRad(double deg)
        {
            double radians = (Math.PI / 180) * deg;
            return (radians);
        }

        //Formats the file size of the CSV file
        private static string FormatSize(long bytes)
        {
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return $"{number:n1}{suffixes[counter]}";
        }
    }
}