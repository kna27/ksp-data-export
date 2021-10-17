using System;

namespace KSPDataExport
{
    public static class Utilities
    {
        private static readonly string[] suffixes = { " Bytes", " KB", " MB" };

        /// <summary>
        /// Gets the distance between a lat/lon pair and the vesel's launchsite
        /// </summary>
        /// <param name="lat">Latitude of the vessel</param>
        /// <param name="lon">Longitude of the vessel</param>
        /// <returns>Distance in meters between vessel and it's launchsite</returns>
        internal static double Distance(double lat, double lon)
        {
            if (DataExport.actVess.mainBody.bodyDisplayName == DataExport.launchBody)
            {
                double distance = 600 * Math.Acos((Math.Sin(DataExport.launchLat) * Math.Sin(DegToRad(lat))) + Math.Cos(DataExport.launchLat) * Math.Cos(DegToRad(lat)) *
                    Math.Cos(DegToRad(lon) - DataExport.launchLon));
                return distance;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Converts from degrees to radians
        /// </summary>
        /// <returns>Radians</returns>
        public static double DegToRad(double deg)
        {
            double radians = Math.PI / 180 * deg;
            return radians;
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
