using System;

namespace KSPDataExport
{
    public static class Utilities
    {
        private static readonly string[] suffixes = { " Bytes", " KB", " MB" };

        //Gets the distance between a lat/lon pair and the KSC
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

        //Converts degrees to radians (used in Distance)
        public static double DegToRad(double deg)
        {
            double radians = Math.PI / 180 * deg;
            return radians;
        }

        //Formats the file size of the CSV file
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
