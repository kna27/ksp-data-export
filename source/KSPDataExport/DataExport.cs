﻿using System;
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
        public static bool isLogging;
        public static float waitTime = 1f;
        private int lastLoggedTime = 0;
        public static Vessel actVess;
        public static string fileSize;
        public static string CSVName;
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
            CSVName = DateTime.Now.ToString("yyyyMMddHHmm") + ".csv";
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
                fileSize = FileSizeFormatter.FormatSize(fi.Length);
            }
            catch
            {
                fileSize = "0.0 Bytes";
            }
        }
        void FixedUpdate()
        {
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            else
            {
                try
                {
                    string[] arrLine = File.ReadAllLines(CSVpath);
                    arrLine[0] = String.Format("Time,{0}{1}{2}", Vals.everLogVelocity ? "Velocity," : "", Vals.everLogAcceleration ? "Acceleration," : "", Vals.everLogAp ? "Apoapsis," : "");
                    File.WriteAllLines(CSVpath, arrLine);
                }
                catch
                {
                    using (FileStream fs = File.Create(CSVpath)) ;
                    using (StreamWriter file = new StreamWriter(CSVpath, true))
                    {
                        try
                        {
                            file.WriteLine(CSVpath, String.Format("Time,{0}{1}{2}", Vals.everLogVelocity ? "Velocity," : "", Vals.everLogAcceleration ? "Acceleration," : "", Vals.everLogAp ? "Apoapsis," : ""));
                        }
                        catch
                        {
                        }
                    }
                }
            }
            if (!File.Exists(CSVpath) && isLogging)
            {
                File.Create(CSVpath);
            }
            if (isLogging)
            {
                if (Mathf.RoundToInt((float)actVess.missionTime) >= lastLoggedTime + waitTime)
                {
                    elapsedTime = Mathf.RoundToInt((float)actVess.missionTime);

                    srfVel = Vals.logVelocity ? String.Format("{0},", Mathf.RoundToInt((float)actVess.srf_velocity.magnitude).ToString()) : Vals.everLogVelocity ? "," : "";
                    gForce = Vals.logGForce ? String.Format("{0}", Math.Round(actVess.geeForce, 2).ToString()) : Vals.everLogGForce ? "," : "";
                    acceleration = Vals.logAcceleration ? String.Format("{0},", Math.Round(actVess.acceleration.magnitude, 2).ToString()) : Vals.everLogAcceleration ? "," : "";
                    thrust = Vals.logThrust ? String.Format("{0},", 0.ToString()) : Vals.everLogThrust ? "," : ""; //TODO
                    TWR = Vals.logTWR ? String.Format("{0},", 0.ToString()) : Vals.everLogTWR ? "," : ""; //TODO
                    mass = Vals.logMass ? String.Format("{0},", ((float)Math.Round(actVess.GetTotalMass() * 100f) / 100f).ToString()) : Vals.everLogMass ? "," : ""; //TEST
                    pitch = Vals.logPitch ? String.Format("{0},", 0.ToString()) : Vals.everLogPitch ? "," : ""; //TODO

                    altTer = Vals.logAltTer ? String.Format("{0},", Math.Round(FlightGlobals.ship_altitude, 2).ToString()) : Vals.everLogAltTer ? "," : ""; //TEST
                    altSea = Vals.logAltSea ? String.Format("{0},", Math.Round(actVess.terrainAltitude, 2).ToString()) : Vals.everLogAltSea ? "," : ""; //TEST
                    downrangeDist = Vals.logDownrangeDist ? String.Format("{0},", 0.ToString()) : Vals.everLogDownrangeDist ? "," : ""; //TODO
                    lat = Vals.logLat ? String.Format("{0},", 0.ToString()) : Vals.everLogLat ? "," : ""; //TODO
                    lon = Vals.logLon ? String.Format("{0},", 0.ToString()) : Vals.everLogLon ? "," : ""; //TODO

                    ap = Vals.logAp ? String.Format("{0}", Math.Max(0, Mathf.RoundToInt((float)actVess.orbit.ApA)).ToString()) : "";
                    pe = Vals.logPe ? String.Format("{0},", 0.ToString()) : Vals.everLogPe ? "," : ""; //TODO
                    inc = Vals.logInc ? String.Format("{0},", 0.ToString()) : Vals.everLogInc ? "," : ""; //TODO
                    orbVel = Vals.logOrbVel ? String.Format("{0},", 0.ToString()) : Vals.everLogOrbVel ? "," : ""; //TODO
                    gravity = Vals.logGravity ? String.Format("{0},", 0.ToString()) : Vals.everLogGravity ? "," : ""; //TODO

                    targDist = Vals.logTargDist ? String.Format("{0},", 0.ToString()) : Vals.everLogTargDist ? "," : ""; //TODO
                    targVel = Vals.logTargVel ? String.Format("{0},", 0.ToString()) : Vals.everLogTargVel ? "," : ""; //TODO

                    stageDV = Vals.logStageDV ? String.Format("{0},", 0.ToString()) : Vals.everLogStageDV ? "," : ""; //TODO
                    vesselDV = Vals.logVesselDV ? String.Format("{0},", 0.ToString()) : Vals.everLogVesselDV ? "," : ""; //TODO
                    
                    pressure = Vals.logPressure ? String.Format("{0},", 0.ToString()) : Vals.everLogPressure ? "," : ""; //TODO
                    temp = Vals.everLogTemperature ? String.Format("{0},", 0.ToString()) : Vals.everLogTemperature ? "," : ""; //TODO

                    AddData();
                    lastLoggedTime = Mathf.RoundToInt((float)actVess.missionTime);
                }
                try
                {
                    fi = new FileInfo(CSVpath);
                    fileSize = FileSizeFormatter.FormatSize(fi.Length);
                }
                catch
                {
                    fileSize = "0.0 Bytes";
                }
            }
        }

        public static void AddData()
        {
            try
            {
                using (StreamWriter file = new StreamWriter(CSVpath, true))
                {
                    try
                    {
                        file.WriteLine(String.Format("{0},{1}{2}{3}", elapsedTime, srfVel, acceleration, ap));
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error: ", ex);
            }
        }

        static void InitFile()
        {
            File.Create(CSVpath);
            File.Delete(CSVpath);
            using (FileStream fs = File.Create(CSVpath)) ;
            using (StreamWriter file = new StreamWriter(CSVpath, true))
            {
                try
                {
                    file.WriteLine(String.Format("Time,{0}{1}{2}", Vals.everLogVelocity ? "Velocity," : "", Vals.everLogAcceleration ? "Acceleration," : "", Vals.everLogAp ? "Apoapsis," : ""));
                }
                catch
                {
                }
            }
        }
    }
}

public static class FileSizeFormatter
{
    static readonly string[] suffixes =
    { " Bytes", " KB", " MB"};
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