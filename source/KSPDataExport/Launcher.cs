using UnityEngine;
using KSP.UI.Screens;

namespace KSPDataExport
{

	using MonoBehavior = MonoBehaviour;

	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class AppLauncher : MonoBehavior
	{
		private static string AppIconPath = @"C:/Test.png";
		private static Texture2D AppIcon = GameDatabase.Instance.GetTexture(AppIconPath, false);

		private ApplicationLauncherButton launcher;

		public void Start()
		{
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
