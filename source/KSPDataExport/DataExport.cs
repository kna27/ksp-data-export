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
        public static bool isLogging;
        public static float waitTime = 1f;
        private int lastLoggedTime = 0;
        public static Vessel actVess;
        public static string fileSize;
        static int elapsedTime;
        static string srfVel;
        static string acceleration;
        static string apoapsis;
        public static string CSVName;
        FileInfo fi;

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
                    arrLine[0] = String.Format("Time,{0}{1}{2}", Vals.everLogVelocity ? "Velocity," : "", Vals.everLogAcceleration ? "Acceleration," : "", Vals.everLogApoapsis ? "Apoapsis," : "");
                    File.WriteAllLines(CSVpath, arrLine);
                }
                catch
                {
                    using (FileStream fs = File.Create(CSVpath)) ;
                    using (StreamWriter file = new StreamWriter(CSVpath, true))
                    {
                        try
                        {
                            file.WriteLine(CSVpath, String.Format("Time,{0}{1}{2}", Vals.everLogVelocity ? "Velocity," : "", Vals.everLogAcceleration ? "Acceleration," : "", Vals.everLogApoapsis ? "Apoapsis," : ""));
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
                    acceleration = Vals.logAcceleration ? String.Format("{0},", Math.Round(actVess.acceleration.magnitude, 2).ToString()) : Vals.everLogAcceleration ? "," : "";
                    apoapsis = Vals.logApoapsis ? String.Format("{0}", Math.Max(0, Mathf.RoundToInt((float)actVess.orbit.ApA)).ToString()) : "";
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
                        file.WriteLine(String.Format("{0},{1}{2}{3}", elapsedTime, srfVel, acceleration, apoapsis));
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
                    file.WriteLine(String.Format("Time,{0}{1}{2}", Vals.everLogVelocity ? "Velocity," : "", Vals.everLogAcceleration ? "Acceleration," : "", Vals.everLogApoapsis ? "Apoapsis," : ""));
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