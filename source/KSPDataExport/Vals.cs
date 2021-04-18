using UnityEngine;

namespace KSPDataExport
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class Vals : MonoBehaviour
    {
        public static bool logVelocity;
        public static bool logAcceleration;
        public static bool logApoapsis;

        public static bool everLogVelocity;
        public static bool everLogAcceleration;
        public static bool everLogApoapsis;
        void Start()
        {
            everLogVelocity = logVelocity = LoadVals.GetValue(DataExport.cfgPath, "logVelocity");
            everLogAcceleration = logAcceleration = LoadVals.GetValue(DataExport.cfgPath, "logAcceleration");
            everLogApoapsis = logApoapsis = LoadVals.GetValue(DataExport.cfgPath, "logApoapsis");
        }

        void Update()
        {
            everLogVelocity = logVelocity ? logVelocity : everLogVelocity;
            everLogAcceleration = logAcceleration ? logAcceleration : everLogAcceleration;
            everLogApoapsis = logApoapsis ? logApoapsis : everLogApoapsis;
            LoadVals.SetValue(DataExport.cfgPath, "logVelocity", logVelocity);
            LoadVals.SetValue(DataExport.cfgPath, "logAcceleration", logAcceleration);
            LoadVals.SetValue(DataExport.cfgPath, "logApoapsis", logApoapsis);
        }
    }
}
