//Writing data to the CSV file

using System;
using UnityEngine;
using System.IO;

namespace KSPDataExport
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class DataExport : MonoBehaviour
    {
        public static string CSVpath;
        public static string dataPath = @"/GameData/DataExport/graphs/";
        public static string cfgPath = @"/GameData/DataExport/logged.vals";
        public static string fileSize;
        public static string CSVName;
        static readonly string[] suffixes = { " Bytes", " KB", " MB" };

        public static bool isLogging;
        public static float waitTime = 1f;
        private int lastLoggedTime = 0;

        public static Vessel actVess;
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
            actVess = FlightGlobals.ActiveVessel;
            CSVName = actVess.GetDisplayName() + "_" + DateTime.Now.ToString("MMddHHmm") + ".csv";
            CSVpath = @"/GameData/DataExport/graphs/" + CSVName;
            if (!EventsHolder.alreadyStarted)
            {
                EventsHolder.appPath = Application.dataPath.Substring(0, Application.dataPath.Length - 13);
                dataPath = EventsHolder.appPath + dataPath;
                cfgPath = EventsHolder.appPath + cfgPath;
                EventsHolder.alreadyStarted = true;
            }
            CSVpath = EventsHolder.appPath + CSVpath;
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            if (!File.Exists(CSVpath) && isLogging)
            {
                InitFile();
            }
            try
            {
                fi = new FileInfo(CSVpath);
                fileSize = DataExport.FormatSize(fi.Length);
            }
            catch
            {
                fileSize = "0.0 Bytes";
            }
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
                    string[] arrLine = File.ReadAllLines(CSVpath);
                    arrLine[0] = String.Format("Time,{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}{20}{21}{22}", Vals.everLogVelocity ? "Velocity," : "", Vals.everLogGForce ? "GForce," : "", Vals.everLogAcceleration ? "Acceleration," : "", Vals.everLogThrust ? "Thrust," : "", Vals.everLogTWR ? "TWR," : "", Vals.everLogMass ? "Mass," : "", Vals.everLogPitch ? "Pitch," : "", Vals.everLogAltTer ? "AltitudeFromTerrain," : "", Vals.everLogAltSea ? "AltitudeFromSea," : "", Vals.everLogDownrangeDist ? "DownrangeDistance," : "", Vals.everLogLat ? "Latitude," : "", Vals.everLogLon ? "Longitude," : "", Vals.everLogAp ? "Apoapsis," : "", Vals.everLogPe ? "Periapsis," : "", Vals.everLogInc ? "Inclination," : "", Vals.everLogOrbVel ? "OrbitalVelocity," : "", Vals.everLogGravity ? "Gravity," : "", Vals.everLogTargDist ? "TargetDistance," : "", Vals.everLogTargVel ? "TargetVelocity," : "", Vals.everLogStageDV ? "StageDeltaV," : "", Vals.everLogVesselDV ? "VesselDeltaV," : "", Vals.everLogPressure ? "Pressure," : "", Vals.everLogTemperature ? "Temperature," : "");
                    File.WriteAllLines(CSVpath, arrLine);
                }
                catch
                {
                    using (FileStream fs = File.Create(CSVpath)) ;
                    using (StreamWriter file = new StreamWriter(CSVpath, true))
                    {
                        try
                        {
                            file.WriteLine(CSVpath, String.Format("Time,{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}{20}{21}{22}", Vals.everLogVelocity ? "Velocity," : "", Vals.everLogGForce ? "GForce," : "", Vals.everLogAcceleration ? "Acceleration," : "", Vals.everLogThrust ? "Thrust," : "", Vals.everLogTWR ? "TWR," : "", Vals.everLogMass ? "Mass," : "", Vals.everLogPitch ? "Pitch," : "", Vals.everLogAltTer ? "AltitudeFromTerrain," : "", Vals.everLogAltSea ? "AltitudeFromSea," : "", Vals.everLogDownrangeDist ? "DownrangeDistance," : "", Vals.everLogLat ? "Latitude," : "", Vals.everLogLon ? "Longitude," : "", Vals.everLogAp ? "Apoapsis," : "", Vals.everLogPe ? "Periapsis," : "", Vals.everLogInc ? "Inclination," : "", Vals.everLogOrbVel ? "OrbitalVelocity," : "", Vals.everLogGravity ? "Gravity," : "", Vals.everLogTargDist ? "TargetDistance," : "", Vals.everLogTargVel ? "TargetVelocity," : "", Vals.everLogStageDV ? "StageDeltaV," : "", Vals.everLogVesselDV ? "VesselDeltaV," : "", Vals.everLogPressure ? "Pressure," : "", Vals.everLogTemperature ? "Temperature," : ""));
                        }
                        catch
                        {
                        }
                    }
                }
            }
            //Create the CSV file if we are logging
            if (!File.Exists(CSVpath) && isLogging)
            {
                File.Create(CSVpath);
            }
            if (isLogging)
            {
                if (Mathf.RoundToInt((float)actVess.missionTime) >= lastLoggedTime + waitTime)
                {
                    //Setting the value of all variables
                    elapsedTime = Mathf.RoundToInt((float)actVess.missionTime);

                    srfVel = Vals.logVelocity ? String.Format("{0},", Mathf.RoundToInt((float)actVess.srf_velocity.magnitude).ToString()) : Vals.everLogVelocity ? "," : "";
                    gForce = Vals.logGForce ? String.Format("{0},", Math.Round(actVess.geeForce, 2).ToString()) : Vals.everLogGForce ? "," : "";
                    acceleration = Vals.logAcceleration ? String.Format("{0},", Math.Round(actVess.acceleration.magnitude, 2).ToString()) : Vals.everLogAcceleration ? "," : "";
                    thrust = Vals.logThrust ? String.Format("{0},", Math.Round(actVess.VesselDeltaV.GetStage(actVess.currentStage).GetSituationThrust(DeltaVSituationOptions.Altitude), 2).ToString()) : Vals.everLogThrust ? "," : "";
                    TWR = Vals.logTWR ? String.Format("{0},", Math.Round(actVess.VesselDeltaV.GetStage(actVess.currentStage).GetSituationTWR(DeltaVSituationOptions.Altitude), 2).ToString()) : Vals.everLogTWR ? "," : "";
                    mass = Vals.logMass ? String.Format("{0},", ((float)Math.Round(actVess.GetTotalMass() * 100f) / 100f).ToString()) : Vals.everLogMass ? "," : "";
                    pitch = Vals.logPitch ? String.Format("{0},", 0.ToString()) : Vals.everLogPitch ? "," : ""; //TODO

                    altTer = Vals.logAltTer ? String.Format("{0},", Math.Round(FlightGlobals.ship_altitude, 2).ToString()) : Vals.everLogAltTer ? "," : "";
                    altSea = Vals.logAltSea ? String.Format("{0},", Math.Round(actVess.terrainAltitude, 2).ToString()) : Vals.everLogAltSea ? "," : "";
                    downrangeDist = Vals.logDownrangeDist ? String.Format("{0},", Math.Round(Distance(actVess.latitude, actVess.longitude), 2).ToString()) : Vals.everLogDownrangeDist ? "," : "";
                    lat = Vals.logLat ? String.Format("{0},", Math.Round(actVess.latitude, 2).ToString()) : Vals.everLogLat ? "," : "";
                    lon = Vals.logLon ? String.Format("{0},", Math.Round(actVess.longitude, 2).ToString()) : Vals.everLogLon ? "," : "";

                    ap = Vals.logAp ? String.Format("{0},", Math.Max(0, Mathf.RoundToInt((float)actVess.orbit.ApA)).ToString()) : "";
                    pe = Vals.logPe ? String.Format("{0},", Math.Max(0, Mathf.RoundToInt((float)actVess.orbit.PeA)).ToString()) : Vals.everLogPe ? "," : "";
                    inc = Vals.logInc ? String.Format("{0},", Math.Round(FlightGlobals.ship_orbit.inclination, 2).ToString()) : Vals.everLogInc ? "," : "";
                    orbVel = Vals.logOrbVel ? String.Format("{0},", Mathf.RoundToInt((float)actVess.obt_velocity.magnitude).ToString()) : Vals.everLogOrbVel ? "," : "";
                    gravity = Vals.logGravity ? String.Format("{0},", 1.ToString()) : Vals.everLogGravity ? "," : ""; //TODO

                    targDist = Vals.logTargDist ? String.Format("{0},", Math.Round(Vector3.Distance(FlightGlobals.fetch.vesselTargetTransform.position, actVess.transform.position), 2).ToString()) : Vals.everLogTargDist ? "," : "";
                    targVel = Vals.logTargVel ? String.Format("{0},", Math.Round(FlightGlobals.ship_tgtVelocity.magnitude, 2).ToString()) : Vals.everLogTargVel ? "," : "";

                    stageDV = Vals.logStageDV ? String.Format("{0},", Mathf.RoundToInt((float) actVess.VesselDeltaV.GetStage(actVess.currentStage).GetSituationDeltaV(DeltaVSituationOptions.Altitude)).ToString()) : Vals.everLogStageDV ? "," : "";
                    vesselDV = Vals.logVesselDV ? String.Format("{0},", Mathf.RoundToInt((float) actVess.VesselDeltaV.GetSituationTotalDeltaV(DeltaVSituationOptions.Altitude)).ToString()) : Vals.everLogVesselDV ? "," : "";

                    pressure = Vals.logPressure ? String.Format("{0},", 4.ToString()) : Vals.everLogPressure ? "," : ""; //TODO
                    temp = Vals.everLogTemperature ? String.Format("{0},", 5.ToString()) : Vals.everLogTemperature ? "," : ""; //TODO

                    //Write the variables to the file
                    AddData();
                    lastLoggedTime = Mathf.RoundToInt((float)actVess.missionTime);
                }
                try
                {
                    fi = new FileInfo(CSVpath);
                    fileSize = DataExport.FormatSize(fi.Length);
                }
                catch
                {
                    fileSize = "0.0 Bytes";
                }
            }
        }

        //Adds the new line containing the values chosen to be logged to the file
        public static void AddData()
        {
            try
            {
                using (StreamWriter file = new StreamWriter(CSVpath, true))
                {
                    try
                    {
                        //Write a new line to the file
                        file.WriteLine(String.Format("{0},{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}{20}{21}{22}", elapsedTime, srfVel, gForce, acceleration, thrust, TWR, mass, pitch, altTer, altSea, downrangeDist, lat, lon, ap, pe, inc, orbVel, gravity, targDist, targVel, stageDV, vesselDV, pressure, temp));
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Was unable to log data to file: " + e);
                    }
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
            File.Create(CSVpath);
            File.Delete(CSVpath);
            using (FileStream fs = File.Create(CSVpath)) ;
            using (StreamWriter file = new StreamWriter(CSVpath, true))
            {
                try
                {
                    file.WriteLine(CSVpath, String.Format("Time,{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}{20}{21}{22}", Vals.everLogVelocity ? "Velocity," : "", Vals.everLogGForce ? "GForce," : "", Vals.everLogAcceleration ? "Acceleration," : "", Vals.everLogThrust ? "Thrust," : "", Vals.everLogTWR ? "TWR," : "", Vals.everLogMass ? "Mass," : "", Vals.everLogPitch ? "Pitch," : "", Vals.everLogAltTer ? "AltitudeFromTerrain," : "", Vals.everLogAltSea ? "AltitudeFromSea," : "", Vals.everLogDownrangeDist ? "DownrangeDistance," : "", Vals.everLogLat ? "Latitude," : "", Vals.everLogLon ? "Longitude," : "", Vals.everLogAp ? "Apoapsis," : "", Vals.everLogPe ? "Periapsis," : "", Vals.everLogInc ? "Inclination," : "", Vals.everLogOrbVel ? "OrbitalVelocity," : "", Vals.everLogGravity ? "Gravity," : "", Vals.everLogTargDist ? "TargetDistance," : "", Vals.everLogTargVel ? "TargetVelocity," : "", Vals.everLogStageDV ? "StageDeltaV," : "", Vals.everLogVesselDV ? "VesselDeltaV," : "", Vals.everLogPressure ? "Pressure," : "", Vals.everLogTemperature ? "Temperature," : ""));
                }
                catch
                {
                }
            }
        }

        //Gets the distance between a lat/lon pair and the KSC
        private double Distance(double lat, double lon)
        {
            double distance = ((600 * Math.Acos((Math.Sin(-0.0016963029533) * Math.Sin(DegToRad(lat))) + Math.Cos(-0.0016963029533) * Math.Cos(DegToRad(lat)) * Math.Cos(DegToRad(lon) - -1.30127703355))));
            return (distance);
        }

        //Converts degrees to radians (used in Distance)
        public static double DegToRad(double deg)
        {
            double radians = (Math.PI / 180) * deg;
            return (radians);
        }

        //Formats the file size of the CSV file
        public static string FormatSize(Int64 bytes)
        {
            int counter = 0;
            decimal number = (decimal)bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return string.Format("{0:n1}{1}", number, suffixes[counter]);
        }
    }
}