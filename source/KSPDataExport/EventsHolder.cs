namespace KSPDataExport
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class EventsHolder
    {
        public static bool alreadyStarted = false;
        public static string appPath;
    }
}
