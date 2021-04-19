using UnityEngine;

namespace KSPDataExport
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class Vals : MonoBehaviour
    {
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

        void Start()
        {
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
            everLogVelocity = logVelocity ? logVelocity : everLogVelocity;
            everLogGForce = logGForce ? logGForce : everLogGForce;
            everLogAcceleration = logAcceleration ? logAcceleration : everLogAcceleration;
            everLogThrust = logThrust ? logThrust : everLogThrust;
            everLogTWR = logTWR ? logTWR : everLogTWR;
            everLogMass = logMass ? logMass : everLogMass;
            everLogPitch = logPitch ? logPitch : everLogPitch;
            everLogAltTer = logAltTer ? logAltTer : everLogAltTer;
            everLogAltSea = logAltSea ? logAltSea : everLogAltSea;
            everLogDownrangeDist = logDownrangeDist ? logDownrangeDist : everLogDownrangeDist;
            everLogLat = logLat ? logLat : everLogLat;
            everLogLon = logLon ? logLon : everLogLon;
            everLogAp = logAp ? logAp : everLogAp;
            everLogPe = logPe ? logPe : everLogPe;
            everLogInc = logInc ? logInc : everLogInc;
            everLogOrbVel = logOrbVel ? logOrbVel : everLogOrbVel;
            everLogGravity = logGravity ? logGravity : everLogGravity;
            everLogTargDist = logTargDist ? logTargDist : everLogTargDist;
            everLogTargVel = logTargVel ? logTargVel : everLogTargVel;
            everLogStageDV = logStageDV ? logStageDV : everLogStageDV;
            everLogVesselDV = logVesselDV ? logVesselDV : everLogVesselDV;
            everLogPressure = logPressure ? logPressure : everLogPressure;
            everLogTemperature = logTemperature ? logTemperature : everLogTemperature;

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
    }
}
