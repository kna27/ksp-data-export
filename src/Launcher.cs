using System.IO;
using KSP.UI.Screens;
using UnityEngine;

namespace KSPDataExport
{
    using MonoBehavior = MonoBehaviour;

    /// <summary>
    ///     The toolbar launcher button
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class AppLauncher : MonoBehavior
    {
        private static string _appIconPath = @"/GameData/DataExport/icon.png";
        private static Texture2D _appIcon;

        private ApplicationLauncherButton _launcher;

        public void Start()
        {
            _appIconPath = Application.platform == RuntimePlatform.OSXPlayer
                ? Directory.GetParent(Directory.GetParent(Application.dataPath)!.ToString())!.ToString()
                : Directory.GetParent(Application.dataPath)!.ToString();
            _appIconPath += @"/GameData/DataExport/icon.png";
            Debug.Log("[Data Export] Launcher Initialized");
            _appIcon ??= new Texture2D(32, 32);
            _appIcon.LoadImage(File.ReadAllBytes(_appIconPath));
            GameEvents.onGUIApplicationLauncherReady.Add(AddLauncher);
            GameEvents.onGUIApplicationLauncherDestroyed.Add(RemoveLauncher);
        }

        public void OnDisable()
        {
            GameEvents.onGUIApplicationLauncherReady.Remove(AddLauncher);
            GameEvents.onGUIApplicationLauncherDestroyed.Remove(RemoveLauncher);
            RemoveLauncher();
        }

        public Vector3 GetAnchor()
        {
            return _launcher?.GetAnchor() ?? Vector3.right;
        }

        private void AddLauncher()
        {
            if (ApplicationLauncher.Ready && _launcher == null)
                _launcher = ApplicationLauncher.Instance.AddModApplication(
                    OnToggleOn, OnToggleOff,
                    null, null,
                    null, null,
                    ApplicationLauncher.AppScenes.FLIGHT, _appIcon
                );
        }

        private void RemoveLauncher()
        {
            if (_launcher == null) return;
            ApplicationLauncher.Instance.RemoveModApplication(_launcher);
            _launcher = null;
        }

        private static void OnToggleOn()
        {
            Window.ShowGUI = true;
        }

        private static void OnToggleOff()
        {
            Window.ShowGUI = false;
            Window.ShowLoggedVals = false;
        }
    }
}
