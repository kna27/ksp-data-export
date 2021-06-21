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
        static string temp;

        void Start()
        {
            Debug.Log("{Data Export} Init");
            dataPath = @"/GameData/DataExport/graphs/";
            cfgPath = @"/GameData/DataExport/logged.vals";

            actVess = FlightGlobals.ActiveVessel;
            CSVName = actVess.GetDisplayName() + "_" + DateTime.Now.ToString("MMddHHmm") + ".csv";
            CSVPath = @"/GameData/DataExport/graphs/" + CSVName;
            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                appPath = Directory.GetParent(Directory.GetParent(Application.dataPath).ToString()).ToString();
            }
            else
            {
                appPath = Directory.GetParent(Application.dataPath).ToString();
            }

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

                    srfVel = Vals.logVelocity ? String.Format("{0},", Mathf.RoundToInt((float)actVess.srf_velocity.magnitude).ToString()) : Vals.everLogVelocity ? "," : "";
                    gForce = Vals.logGForce ? String.Format("{0},", Math.Round(actVess.geeForce, 2).ToString(CultureInfo.InvariantCulture)) : Vals.everLogGForce ? "," : "";
                    acceleration = Vals.logAcceleration ? String.Format("{0},", Math.Round(actVess.acceleration.magnitude, 2).ToString(CultureInfo.InvariantCulture)) : Vals.everLogAcceleration ? "," : "";
                    thrust = Vals.logThrust ? String.Format("{0},", Math.Round(actVess.VesselDeltaV.GetStage(actVess.currentStage).GetSituationThrust(DeltaVSituationOptions.Altitude), 2).ToString(CultureInfo.InvariantCulture)) : Vals.everLogThrust ? "," : "";
                    TWR = Vals.logTWR ? String.Format("{0},", Math.Round(actVess.VesselDeltaV.GetStage(actVess.currentStage).GetSituationTWR(DeltaVSituationOptions.Altitude), 2).ToString(CultureInfo.InvariantCulture)) : Vals.everLogTWR ? "," : "";
                    mass = Vals.logMass ? String.Format("{0},", ((float)Math.Round(actVess.GetTotalMass() * 100f) / 100f).ToString(CultureInfo.InvariantCulture)) : Vals.everLogMass ? "," : "";
                    pitch = Vals.logPitch ? String.Format("{0},", 0.ToString()) : Vals.everLogPitch ? "," : ""; //TODO

                    altTer = Vals.logAltTer ? String.Format("{0},", Math.Round(FlightGlobals.ship_altitude, 2).ToString(CultureInfo.InvariantCulture)) : Vals.everLogAltTer ? "," : "";
                    altSea = Vals.logAltSea ? String.Format("{0},", Math.Round(actVess.terrainAltitude, 2).ToString(CultureInfo.InvariantCulture)) : Vals.everLogAltSea ? "," : "";
                    downrangeDist = Vals.logDownrangeDist ? String.Format("{0},", Math.Round(Distance(actVess.latitude, actVess.longitude), 2).ToString(CultureInfo.InvariantCulture)) : Vals.everLogDownrangeDist ? "," : "";
                    lat = Vals.logLat ? String.Format("{0},", Math.Round(actVess.latitude, 2).ToString(CultureInfo.InvariantCulture)) : Vals.everLogLat ? "," : "";
                    lon = Vals.logLon ? String.Format("{0},", Math.Round(actVess.longitude, 2).ToString(CultureInfo.InvariantCulture)) : Vals.everLogLon ? "," : "";

                    ap = Vals.logAp ? String.Format("{0},", Math.Max(0, Mathf.RoundToInt((float)actVess.orbit.ApA)).ToString()) : "";
                    pe = Vals.logPe ? String.Format("{0},", Math.Max(0, Mathf.RoundToInt((float)actVess.orbit.PeA)).ToString()) : Vals.everLogPe ? "," : "";
                    inc = Vals.logInc ? String.Format("{0},", Math.Round(FlightGlobals.ship_orbit.inclination, 2).ToString(CultureInfo.InvariantCulture)) : Vals.everLogInc ? "," : "";
                    orbVel = Vals.logOrbVel ? String.Format("{0},", Mathf.RoundToInt((float)actVess.obt_velocity.magnitude).ToString()) : Vals.everLogOrbVel ? "," : "";
                    gravity = Vals.logGravity ? String.Format("{0},", 0.ToString()) : Vals.everLogGravity ? "," : ""; //TODO

                    targDist = Vals.logTargDist ? String.Format("{0},", Math.Round(Vector3.Distance(FlightGlobals.fetch.vesselTargetTransform.position, actVess.transform.position), 2).ToString(CultureInfo.InvariantCulture)) : Vals.everLogTargDist ? "," : "";
                    targVel = Vals.logTargVel ? String.Format("{0},", Math.Round(FlightGlobals.ship_tgtVelocity.magnitude, 2).ToString(CultureInfo.InvariantCulture)) : Vals.everLogTargVel ? "," : "";

                    stageDV = Vals.logStageDV ? String.Format("{0},", Mathf.RoundToInt((float)actVess.VesselDeltaV.GetStage(actVess.currentStage).GetSituationDeltaV(DeltaVSituationOptions.Altitude)).ToString()) : Vals.everLogStageDV ? "," : "";
                    vesselDV = Vals.logVesselDV ? String.Format("{0},", Mathf.RoundToInt((float)actVess.VesselDeltaV.GetSituationTotalDeltaV(DeltaVSituationOptions.Altitude)).ToString()) : Vals.everLogVesselDV ? "," : "";

                    pressure = Vals.logPressure ? String.Format("{0},", Math.Round(actVess.staticPressurekPa, 2).ToString(CultureInfo.InvariantCulture)) : Vals.everLogPressure ? "," : "";
                    temp = Vals.logTemperature ? String.Format("{0},", 0.ToString()) : Vals.everLogTemperature ? "," : ""; //TODO

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
                    file.WriteLine("{0},{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}{20}{21}{22}", elapsedTime, srfVel, gForce, acceleration, thrust, TWR, mass, pitch, altTer, altSea, downrangeDist, lat, lon, ap, pe, inc, orbVel, gravity, targDist, targVel, stageDV, vesselDV, pressure, temp);
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
            using (FileStream fs = File.Create(CSVPath)) { };
            using StreamWriter file = new StreamWriter(CSVPath, true);
            try
            {
                file.WriteLine(CSVPath, String.Format("Time,{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}{20}{21}{22}", Vals.everLogVelocity ? "Velocity," : "", Vals.everLogGForce ? "GForce," : "", Vals.everLogAcceleration ? "Acceleration," : "", Vals.everLogThrust ? "Thrust," : "", Vals.everLogTWR ? "TWR," : "", Vals.everLogMass ? "Mass," : "", Vals.everLogPitch ? "Pitch," : "", Vals.everLogAltTer ? "AltitudeFromTerrain," : "", Vals.everLogAltSea ? "AltitudeFromSea," : "", Vals.everLogDownrangeDist ? "DownrangeDistance," : "", Vals.everLogLat ? "Latitude," : "", Vals.everLogLon ? "Longitude," : "", Vals.everLogAp ? "Apoapsis," : "", Vals.everLogPe ? "Periapsis," : "", Vals.everLogInc ? "Inclination," : "", Vals.everLogOrbVel ? "OrbitalVelocity," : "", Vals.everLogGravity ? "Gravity," : "", Vals.everLogTargDist ? "TargetDistance," : "", Vals.everLogTargVel ? "TargetVelocity," : "", Vals.everLogStageDV ? "StageDeltaV," : "", Vals.everLogVesselDV ? "VesselDeltaV," : "", Vals.everLogPressure ? "Pressure," : "", Vals.everLogTemperature ? "Temperature," : ""));
            }
            catch
            {
            }
        }

        //Gets the distance between a lat/lon pair and the KSC
        private static double Distance(double lat, double lon)
        {
            if (actVess.mainBody.bodyDisplayName == launchBody)
            {
                double distance = ((600 * Math.Acos((Math.Sin(launchLat) * Math.Sin(DegToRad(lat))) + Math.Cos(launchLat) * Math.Cos(DegToRad(lat)) * Math.Cos(DegToRad(lon) - launchLon))));
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
            decimal number = (decimal)bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return $"{number:n1}{suffixes[counter]}";
        }
    }
}