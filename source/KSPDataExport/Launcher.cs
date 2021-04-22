using UnityEngine;
using KSP.UI.Screens;
using System.IO;

namespace KSPDataExport
{

	using MonoBehavior = MonoBehaviour;

	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class AppLauncher : MonoBehavior
	{
		private static string AppIconPath = @"/GameData/DataExport/icon.png";
		private static Texture2D AppIcon;

		private ApplicationLauncherButton launcher;

		public void Start()
		{
			if (!EventsHolder.alreadyStarted)
			{
				EventsHolder.appPath = Application.dataPath.Substring(0, Application.dataPath.Length - 13);
				AppIconPath = EventsHolder.appPath + AppIconPath;
				AppIcon ??= new Texture2D(36, 36);
				ImageConversion.LoadImage(AppIcon, File.ReadAllBytes(AppIconPath));
			}
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
					ApplicationLauncher.AppScenes.FLIGHT, AppIcon
				);
			}
		}

		private void RemoveLauncher()
		{
			if (launcher != null)
			{
				ApplicationLauncher.Instance.RemoveModApplication(launcher);
				launcher = null;
			}
		}

		private void OnToggleOn()
		{
			Window.showGUI = true;
		}

		private void OnToggleOff()
		{
			Window.showGUI = false;
			Window.showLoggedVals = false;
		}
	}

}
