using System;
using System.Globalization;
using UnityEngine;

namespace KSPDataExport
{
    /// <summary>
    ///     Utility methods for formatting, calculating value points, etc.
    /// </summary>
    public static class Utilities
    {
        private static readonly string[] Suffixes = { " Bytes", " KB", " MB" };

        /// <summary>
        ///     Rounds a double to a string with a specified number of decimal places
        /// </summary>
        public static string RoundToStr(double value, int decimals)
        {
            return Math.Round(value, decimals).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Converts from degrees to radians
        /// </summary>
        public static double DegToRad(double deg)
        {
            return Math.PI / 180 * deg;
        }

        /// <summary>
        ///     Formats a file size in bytes to a human-readable string
        /// </summary>
        public static string FormatSize(long bytes)
        {
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }

            return $"{number:n1}{Suffixes[counter]}";
        }

        // Methods below are for calculating data points

        /// <summary>
        ///     Returns the altitude above terrain
        /// </summary>
        public static double GetSrfAlt()
        {
            if (DataExport.ActVess.terrainAltitude > 0.0
                || DataExport.ActVess.situation == Vessel.Situations.SPLASHED
                || DataExport.ActVess.situation == Vessel.Situations.LANDED)
                return FlightGlobals.ship_altitude - DataExport.ActVess.terrainAltitude;
            return FlightGlobals.ship_altitude;
        }

        /// <summary>
        ///     Returns a Quaternion representing the rotation of the vessel relative to the surface
        ///     Adapted from MechJeb2
        /// </summary>
        private static Quaternion SurfaceRotation()
        {
            Vessel vessel = DataExport.ActVess;
            Vector3 centerOfMass = vessel.CoMD;
            Vector3 up = (centerOfMass - vessel.mainBody.position).normalized;
            Vector3 north = Vector3
                .ProjectOnPlane(
                    vessel.mainBody.position + vessel.mainBody.transform.up * (float)vessel.mainBody.Radius -
                    centerOfMass, up).normalized;
            return Quaternion.Inverse(Quaternion.Euler(90.0f, 0.0f, 0.0f) *
                                      Quaternion.Inverse(vessel.transform.rotation) *
                                      Quaternion.LookRotation(north, up));
        }

        /// <summary>
        ///     Returns the pitch of the vessel
        /// </summary>
        public static double GetPitch()
        {
            Quaternion surfaceRotation = SurfaceRotation();
            return surfaceRotation.eulerAngles.x > 180.0f
                ? 360.0f - surfaceRotation.eulerAngles.x
                : -surfaceRotation.eulerAngles.x;
        }

        /// <summary>
        ///     Returns the thrust of the vessel
        /// </summary>
        public static double GetThrust()
        {
            double thrust = 0.0;
            foreach (Part p in DataExport.ActVess.parts)
            foreach (PartModule module in p.Modules)
            {
                if (!module.isEnabled) continue;
                ModuleEngines engine = module as ModuleEngines;
                if (engine)
                    thrust += engine.finalThrust;
            }

            return thrust;
        }

        /// <summary>
        ///     Returns the downrange distance from the launch site
        /// </summary>
        public static double DownrangeDistance()
        {
            if (!DataExport.ActVess.mainBody.Equals(DataExport.LaunchBody)) return 0;
            return DataExport.LaunchBody.Radius * Math.Acos(
                Math.Sin(DataExport.LaunchLat) * Math.Sin(DegToRad(DataExport.ActVess.latitude)) +
                Math.Cos(DataExport.LaunchLat) * Math.Cos(DegToRad(DataExport.ActVess.latitude)) *
                Math.Cos(DegToRad(DataExport.ActVess.longitude) - DataExport.LaunchLon));
        }
    }
}
