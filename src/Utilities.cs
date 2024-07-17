using System;
using System.Globalization;
using UnityEngine;

namespace KSPDataExport
{
    public static class Utilities
    {
        private static readonly string[] suffixes = { " Bytes", " KB", " MB" };

        public static string RoundToStr(double value, int decimals)
        {
            return Math.Round(value, decimals).ToString(CultureInfo.InvariantCulture);
        }

        public static double GetSrfAlt()
        {
            if ( DataExport.actVess.terrainAltitude > 0.0 
                 ||  DataExport.actVess.situation == Vessel.Situations.SPLASHED
                 ||  DataExport.actVess.situation == Vessel.Situations.LANDED)
            { 
                return FlightGlobals.ship_altitude - DataExport.actVess.terrainAltitude;
            }
            return FlightGlobals.ship_altitude;
        }
        
        private static Quaternion SurfaceRotation()
        {
            Vessel vessel = DataExport.actVess;
            Vector3 centerOfMass = vessel.CoMD;
            Vector3 up = (centerOfMass - vessel.mainBody.position).normalized;
            Vector3 north = Vector3.ProjectOnPlane((vessel.mainBody.position + vessel.mainBody.transform.up * (float)vessel.mainBody.Radius) - centerOfMass, up).normalized;
            return Quaternion.Inverse(Quaternion.Euler(90.0f, 0.0f, 0.0f) * Quaternion.Inverse(vessel.transform.rotation) * Quaternion.LookRotation(north, up));
        }

        public static double GetPitch()
        {
            Quaternion surfaceRotation = SurfaceRotation();
            return surfaceRotation.eulerAngles.x > 180.0f ? 360.0f - surfaceRotation.eulerAngles.x : -surfaceRotation.eulerAngles.x;
        }
        
        public static double GetThrust()
        {
            double thrust = 0.0;
            foreach (var p in DataExport.actVess.parts)
            {
                foreach (PartModule module in p.Modules)
                {
                    if (!module.isEnabled) continue;
                    var engine = module as ModuleEngines;
                    if (engine != null)
                        thrust += engine.finalThrust;
                }
            }
            return thrust;
        }
        
        public static double DownrangeDistance()
        {
            if (DataExport.actVess.mainBody.bodyDisplayName != DataExport.launchBody) return 0;
            // TODO: Don't use hardcoded radius
            return 600 * Math.Acos((Math.Sin(DataExport.launchLat) * Math.Sin(DegToRad(DataExport.actVess.latitude))) + Math.Cos(DataExport.launchLat) * Math.Cos(DegToRad(DataExport.actVess.latitude)) *
                Math.Cos(DegToRad(DataExport.actVess.longitude) - DataExport.launchLon));
        }

        /// <summary>
        /// Converts from degrees to radians
        /// </summary>
        /// <returns>Radians</returns>
        public static double DegToRad(double deg)
        {
            return Math.PI / 180 * deg;
        }

        /// <summary>
        /// Formats the file size of the CSV file
        /// </summary>
        /// <returns></returns>
        public static string FormatSize(long bytes)
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
