// Container of whether or not to log values

using UnityEngine;

namespace KSPDataExport
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class Vals : MonoBehaviour
    {
        //The main bools
        public static bool logVelocity;
        public static bool logGForce;
        public static bool logAcceleration;
        public static bool logThrust;
        public static bool logTWR;
        public static bool logMass;
        public static bool logPitch;

        public static bool logAltTer;
        public static bool logAltSea;
        public static bool logDownrangeDist;
        public static bool logLat;
        public static bool logLon;

        public static bool logAp;
        public static bool logPe;
        public static bool logInc;
        public static bool logOrbVel;
        public static bool logGravity;

        public static bool logTargDist;
        public static bool logTargVel;

        public static bool logStageDV;
        public static bool logVesselDV;

        public static bool logPressure;
        public static bool logTemperature;

        //Whether each main bool has ever been set to true
        public static bool everLogVelocity;
        public static bool everLogGForce;
        public static bool everLogAcceleration;
        public static bool everLogThrust;
        public static bool everLogTWR;
        public static bool everLogMass;
        public static bool everLogPitch;

        public static bool everLogAltTer;
        public static bool everLogAltSea;
        public static bool everLogDownrangeDist;
        public static bool everLogLat;
        public static bool everLogLon;

        public static bool everLogAp;
        public static bool everLogPe;
        public static bool everLogInc;
        public static bool everLogOrbVel;
        public static bool everLogGravity;

        public static bool everLogTargDist;
        public static bool everLogTargVel;

        public static bool everLogStageDV;
        public static bool everLogVesselDV;

        public static bool everLogPressure;
        public static bool everLogTemperature;

        //Temporary bools to check for changes
        private static bool tempLogVelocity;
        private static bool tempLogGForce;
        private static bool tempLogAcceleration;
        private static bool tempLogThrust;
        private static bool tempLogTWR;
        private static bool tempLogMass;
        private static bool tempLogPitch;

        private static bool tempLogAltTer;
        private static bool tempLogAltSea;
        private static bool tempLogDownrangeDist;
        private static bool tempLogLat;
        private static bool tempLogLon;

        private static bool tempLogAp;
        private static bool tempLogPe;
        private static bool tempLogInc;
        private static bool tempLogOrbVel;
        private static bool tempLogGravity;

        private static bool tempLogTargDist;
        private static bool tempLogTargVel;

        private static bool tempLogStageDV;
        private static bool tempLogVesselDV;

        private static bool tempLogPressure;
        private static bool tempLogTemperature;

        void Start()
        {
            //Get the value of all of the bools
            everLogVelocity = logVelocity = LoadVals.GetValue(DataExport.cfgPath, "logVelocity");
            everLogGForce = logGForce = LoadVals.GetValue(DataExport.cfgPath, "logGForce");
            everLogAcceleration = logAcceleration = LoadVals.GetValue(DataExport.cfgPath, "logAcceleration");
            everLogThrust = logThrust = LoadVals.GetValue(DataExport.cfgPath, "logThrust");
            everLogTWR = logTWR = LoadVals.GetValue(DataExport.cfgPath, "logTWR");
            everLogMass = logMass = LoadVals.GetValue(DataExport.cfgPath, "logMass");
            everLogPitch = logPitch = LoadVals.GetValue(DataExport.cfgPath, "logPitch");
            everLogAltTer = logAltTer = LoadVals.GetValue(DataExport.cfgPath, "logAltTer");
            everLogAltSea = logAltSea = LoadVals.GetValue(DataExport.cfgPath, "logAltSea");
            everLogDownrangeDist = logDownrangeDist = LoadVals.GetValue(DataExport.cfgPath, "logDownrangeDist");
            everLogLat = logLat = LoadVals.GetValue(DataExport.cfgPath, "logLat");
            everLogLon = logLon = LoadVals.GetValue(DataExport.cfgPath, "logLon");
            everLogAp = logAp = LoadVals.GetValue(DataExport.cfgPath, "logAp");
            everLogPe = logPe = LoadVals.GetValue(DataExport.cfgPath, "logPe");
            everLogInc = logInc = LoadVals.GetValue(DataExport.cfgPath, "logInc");
            everLogOrbVel = logOrbVel = LoadVals.GetValue(DataExport.cfgPath, "logOrbVel");
            everLogGravity = logGravity = LoadVals.GetValue(DataExport.cfgPath, "logGravity");
            everLogTargDist = logTargDist = LoadVals.GetValue(DataExport.cfgPath, "logTargDist");
            everLogTargVel = logTargVel = LoadVals.GetValue(DataExport.cfgPath, "logTargVel");
            everLogStageDV = logStageDV = LoadVals.GetValue(DataExport.cfgPath, "logStageDV");
            everLogVesselDV = logVesselDV = LoadVals.GetValue(DataExport.cfgPath, "logVesselDV");
            everLogPressure = logPressure = LoadVals.GetValue(DataExport.cfgPath, "logPressure");
            everLogTemperature = logTemperature = LoadVals.GetValue(DataExport.cfgPath, "logTemperature");
        }

        void Update()
        {
            // If any of the bools have changed
            if (tempLogVelocity != logVelocity || tempLogGForce != logGForce || tempLogAcceleration != logAcceleration || tempLogThrust != logThrust || tempLogTWR != logTWR || tempLogMass != logMass || tempLogPitch != logPitch || tempLogAltTer != logAltTer || tempLogAltSea != logAltSea || tempLogDownrangeDist != logDownrangeDist || tempLogLat != logLat || tempLogLon != logLon || tempLogAp != logAp || tempLogPe != logPe || tempLogInc != logInc || tempLogOrbVel != logOrbVel || tempLogGravity != logGravity || tempLogTargDist != logTargDist || tempLogTargVel != logTargVel || tempLogStageDV != logStageDV || tempLogVesselDV != logVesselDV || tempLogPressure != logPressure || tempLogTemperature != logTemperature)
            {
                // Change the 'ever' bools if needed
                everLogVelocity = logVelocity || everLogVelocity;
                everLogGForce = logGForce || everLogGForce;
                everLogAcceleration = logAcceleration || everLogAcceleration;
                everLogThrust = logThrust || everLogThrust;
                everLogTWR = logTWR || everLogTWR;
                everLogMass = logMass || everLogMass;
                everLogPitch = logPitch || everLogPitch;
                everLogAltTer = logAltTer || everLogAltTer;
                everLogAltSea = logAltSea || everLogAltSea;
                everLogDownrangeDist = logDownrangeDist || everLogDownrangeDist;
                everLogLat = logLat || everLogLat;
                everLogLon = logLon || everLogLon;
                everLogAp = logAp || everLogAp;
                everLogPe = logPe || everLogPe;
                everLogInc = logInc || everLogInc;
                everLogOrbVel = logOrbVel || everLogOrbVel;
                everLogGravity = logGravity || everLogGravity;
                everLogTargDist = logTargDist || everLogTargDist;
                everLogTargVel = logTargVel || everLogTargVel;
                everLogStageDV = logStageDV || everLogStageDV;
                everLogVesselDV = logVesselDV || everLogVesselDV;
                everLogPressure = logPressure || everLogPressure;
                everLogTemperature = logTemperature || everLogTemperature;

                // Write the new values of the bools to the file
                LoadVals.SetValue(DataExport.cfgPath, "logVelocity", logVelocity);
                LoadVals.SetValue(DataExport.cfgPath, "logGForce", logGForce);
                LoadVals.SetValue(DataExport.cfgPath, "logAcceleration", logAcceleration);
                LoadVals.SetValue(DataExport.cfgPath, "logThrust", logThrust);
                LoadVals.SetValue(DataExport.cfgPath, "logTWR", logTWR);
                LoadVals.SetValue(DataExport.cfgPath, "logMass", logMass);
                LoadVals.SetValue(DataExport.cfgPath, "logPitch", logPitch);
                LoadVals.SetValue(DataExport.cfgPath, "logAltTer", logAltTer);
                LoadVals.SetValue(DataExport.cfgPath, "logAltSea", logAltSea);
                LoadVals.SetValue(DataExport.cfgPath, "logDownrangeDist", logDownrangeDist);
                LoadVals.SetValue(DataExport.cfgPath, "logLat", logLat);
                LoadVals.SetValue(DataExport.cfgPath, "logLon", logLon);
                LoadVals.SetValue(DataExport.cfgPath, "logAp", logAp);
                LoadVals.SetValue(DataExport.cfgPath, "logPe", logPe);
                LoadVals.SetValue(DataExport.cfgPath, "logInc", logInc);
                LoadVals.SetValue(DataExport.cfgPath, "logOrbVel", logOrbVel);
                LoadVals.SetValue(DataExport.cfgPath, "logGravity", logGravity);
                LoadVals.SetValue(DataExport.cfgPath, "logTargDist", logTargDist);
                LoadVals.SetValue(DataExport.cfgPath, "logTargVel", logTargVel);
                LoadVals.SetValue(DataExport.cfgPath, "logStageDV", logStageDV);
                LoadVals.SetValue(DataExport.cfgPath, "logVesselDV", logVesselDV);
                LoadVals.SetValue(DataExport.cfgPath, "logPressure", logPressure);
                LoadVals.SetValue(DataExport.cfgPath, "logTemperature", logTemperature);
            }

            // Set temporary bools to the current bool at the end of the frame
            tempLogVelocity = logVelocity;
            tempLogGForce = logGForce;
            tempLogAcceleration = logAcceleration;
            tempLogThrust = logThrust;
            tempLogTWR = logTWR;
            tempLogMass = logMass;
            tempLogPitch = logPitch;
            tempLogAltTer = logAltTer;
            tempLogAltSea = logAltSea;
            tempLogDownrangeDist = logDownrangeDist;
            tempLogLat = logLat;
            tempLogLon = logLon;
            tempLogAp = logAp;
            tempLogPe = logPe;
            tempLogInc = logInc;
            tempLogOrbVel = logOrbVel;
            tempLogGravity = logGravity;
            tempLogTargDist = logTargDist;
            tempLogTargVel = logTargVel;
            tempLogStageDV = logStageDV;
            tempLogVesselDV = logVesselDV;
            tempLogPressure = logPressure;
            tempLogTemperature = logTemperature;
        }
    }
}
