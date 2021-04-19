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
            everLogAcceleration = logAcceleration = LoadVals.GetValue(DataExport.cfgPath, "logAcceleration");
            everLogAp = logAp = LoadVals.GetValue(DataExport.cfgPath, "logApoapsis");
        }

        void Update()
        {
            everLogVelocity = logVelocity ? logVelocity : everLogVelocity;
            everLogAcceleration = logAcceleration ? logAcceleration : everLogAcceleration;
            everLogAp = logAp ? logAp : everLogAp;
            LoadVals.SetValue(DataExport.cfgPath, "logVelocity", logVelocity);
            LoadVals.SetValue(DataExport.cfgPath, "logAcceleration", logAcceleration);
            LoadVals.SetValue(DataExport.cfgPath, "logApoapsis", logAp);
        }
    }
}
