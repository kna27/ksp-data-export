﻿using UnityEngine;
using KSP.UI.Screens;
using System.IO;

namespace KSPDataExport
{
    using MonoBehavior = MonoBehaviour;
    /// <summary>
    /// The toolbar launcher button
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class AppLauncher : MonoBehavior
    {
        private static string appIconPath = @"/GameData/DataExport/icon.png";
        private static Texture2D appIcon;

        private ApplicationLauncherButton launcher;

        public void Start()
        {
            appIconPath = Application.platform == RuntimePlatform.OSXPlayer ? Directory.GetParent(Directory.GetParent(Application.dataPath).ToString()).ToString() : Directory.GetParent(Application.dataPath).ToString();
            appIconPath += @"/GameData/DataExport/icon.png";
            Debug.Log("[Data Export] Launcher Init");
            appIcon ??= new Texture2D(32, 32);
            appIcon.LoadImage(File.ReadAllBytes(appIconPath));
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
            return launcher?.GetAnchor() ?? Vector3.right;
        }

        private void AddLauncher()
        {
            if (ApplicationLauncher.Ready && launcher == null)
            {
                launcher = ApplicationLauncher.Instance.AddModApplication(
                    OnToggleOn, OnToggleOff,
                    null, null,
                    null, null,
                    ApplicationLauncher.AppScenes.FLIGHT, appIcon
                );
            }
        }

        private void RemoveLauncher()
        {
            if (launcher == null) return;
            ApplicationLauncher.Instance.RemoveModApplication(launcher);
            launcher = null;
        }

        private static void OnToggleOn()
        {
            Window.showGUI = true;
        }

        private static void OnToggleOff()
        {
            Window.showGUI = false;
            Window.showLoggedVals = false;
        }
    }

}
