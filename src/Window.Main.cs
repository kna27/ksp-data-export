using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using UnityEngine;

namespace KSPDataExport
{
    /// <summary>
    ///     The mod's main GUI
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public partial class Window
    {
        // --- State Variables ---
        public static bool ShowGUI;
        private static bool _showLoggedVals;
        private bool _wasLoggingStoppedByInvalidRate;
        private string _logRateInput;

        // --- PopupDialog References ---
        private PopupDialog _mainDialog;
        private PopupDialog _loggedValsDialog;

        // --- Dialog GUI Element References (for dynamic updates) ---
        private DialogGUIButton _onOffButton;
        private DialogGUILabel _fileSizeLabel;
        private readonly List<DialogGUIBase> _loggedValuesDialogContent = new List<DialogGUIBase>();

        private void Start()
        {
            _logRateInput = DataExport.WaitTime.ToString(CultureInfo.InvariantCulture);
            ShowGUI = false;
            _showLoggedVals = false;
            DataExport.IsLogging = Config.GetValue(DataExport.CfgPath, "defaultLogState");

            BuildLoggedValuesDialogStructure();
        }

        private void OnDestroy()
        {
            DismissMainWindow();
            DismissLoggedValuesWindow();
        }

        private void Update()
        {
            if (ShowGUI)
            {
                ShowWindow();
            }
            else
            {
                HideWindow();
                if (_showLoggedVals)
                {
                    _showLoggedVals = false;
                }
            }

            if (_showLoggedVals)
            {
                ShowLoggedValuesWindow();
            }
            else
            {
                HideLoggedValuesWindow();
            }

            HandleLogRateInput();
        }

        // --- Main Window Management ---

        private void ShowWindow()
        {
            if (!_mainDialog)
            {
                SpawnMainWindow();
            }

            if (_mainDialog != null && !_mainDialog.gameObject.activeSelf)
            {
                _mainDialog.gameObject.SetActive(true);
            }
        }

        private void HideWindow()
        {
            if (_mainDialog && _mainDialog.gameObject.activeSelf)
            {
                _mainDialog.gameObject.SetActive(false);
            }
        }

        private void SpawnMainWindow()
        {
            var dialogElements = new List<DialogGUIBase>();

            _onOffButton = new DialogGUIButton(
                () => DataExport.IsLogging ? "Turn Off" : "Turn On",
                ToggleLogging,
                250.0f, 30.0f, false
            );
            dialogElements.Add(_onOffButton);

            var csvButton = new DialogGUIButton(
                () => DataExport.CsvName,
                TryOpenCsvFile,
                250f, 30f, false);
            dialogElements.Add(csvButton);

            _fileSizeLabel = new DialogGUILabel(() => "File size: " + DataExport.FileSize, 250f);
            dialogElements.Add(_fileSizeLabel);

            dialogElements.Add(new DialogGUISpace(5));
            dialogElements.Add(new DialogGUIHorizontalLayout(
                TextAnchor.MiddleLeft,
                new DialogGUILabel("Log Rate (s):", 100f),
                new DialogGUITextInput(_logRateInput, false, 3, OnLogRateChanged, 50f, 30f)
            ));
            dialogElements.Add(new DialogGUISpace(5));

            dialogElements.Add(new DialogGUIButton(
                () => _showLoggedVals ? "Close logged values" : "Choose logged values",
                ToggleLoggedValsWindow,
                250.0f, 30.0f, false
            ));

            dialogElements.Add(new DialogGUIButton(
                "View CSV files",
                ViewCsvFolder,
                250.0f, 30.0f, false
            ));

            dialogElements.Add(new DialogGUIButton(
                "Help",
                ShowHelp,
                250.0f, 30.0f, false
            ));

            // Use default position and size
            Rect windowRect = new Rect(0.65f, 0.5f, 275, 290);

            _mainDialog = PopupDialog.SpawnPopupDialog(
                new Vector2(0.65f, 0.5f),
                new Vector2(0.65f, 0.5f),
                new MultiOptionDialog(
                    "KSPDataExportMain",
                    "",
                    "KSP Data Export",
                    HighLogic.UISkin,
                    windowRect,
                    dialogElements.ToArray()
                ),
                true,
                HighLogic.UISkin,
                false
            );

            if (!_mainDialog) return;
            _mainDialog.gameObject.AddComponent<DialogCloseButton>().OnDismiss = () => { ShowGUI = false; };
            _mainDialog.gameObject.SetActive(false);
        }

        private void DismissMainWindow()
        {
            if (_mainDialog == null) return;
            _mainDialog.Dismiss();
            _mainDialog = null;
        }

        private void ToggleLogging()
        {
            DataExport.IsLogging = !DataExport.IsLogging;
            _wasLoggingStoppedByInvalidRate = false;

            AppLauncher launcher = FindObjectOfType<AppLauncher>();
            if (launcher)
            {
                launcher.UpdateIconState();
            }
        }

        private void TryOpenCsvFile()
        {
            try
            {
                if (System.IO.File.Exists(DataExport.CsvPath))
                {
                    Process.Start(DataExport.CsvPath);
                }
                else
                {
                    ScreenMessages.PostScreenMessage("File does not exist yet. Turn on logging to create file.", 3f,
                        ScreenMessageStyle.UPPER_CENTER);
                }
            }
            catch (Exception ex)
            {
                ScreenMessages.PostScreenMessage($"Error opening file: {ex.Message}", 5f,
                    ScreenMessageStyle.UPPER_CENTER);
            }
        }

        private string OnLogRateChanged(string newRate)
        {
            _logRateInput = newRate;
            return newRate;
        }

        private void HandleLogRateInput()
        {
            if (double.TryParse(_logRateInput, NumberStyles.Any, CultureInfo.InvariantCulture, out double d) &&
                d >= 0.01)
            {
                // Input is valid
                if (Math.Abs(DataExport.WaitTime - d) > 0.001)
                {
                    DataExport.WaitTime = d;
                }

                if (!_wasLoggingStoppedByInvalidRate) return;
                DataExport.IsLogging = true;
                _wasLoggingStoppedByInvalidRate = false;

                // Update icon
                AppLauncher launcher = FindObjectOfType<AppLauncher>();
                if (launcher)
                {
                    launcher.UpdateIconState();
                }
            }
            else
            {
                // Input is invalid
                if (!DataExport.IsLogging) return; // Only act if it was logging
                ScreenMessages.PostScreenMessage("Invalid log rate! Logging paused.", 3f,
                    ScreenMessageStyle.UPPER_CENTER);
                DataExport.IsLogging = false;
                _wasLoggingStoppedByInvalidRate = true;

                AppLauncher launcher = FindObjectOfType<AppLauncher>();
                if (launcher)
                {
                    launcher.UpdateIconState();
                }
                // If not logging, do nothing about invalid input until user tries to turn logging on
            }
        }

        private void ToggleLoggedValsWindow()
        {
            _showLoggedVals = !_showLoggedVals;
        }

        private void ViewCsvFolder()
        {
            try
            {
                System.IO.Directory.CreateDirectory(DataExport.DataPath);
                Process.Start(DataExport.DataPath);
            }
            catch (Exception ex)
            {
                ScreenMessages.PostScreenMessage($"Error opening folder: {ex.Message}", 5f,
                    ScreenMessageStyle.UPPER_CENTER);
            }
        }

        private void ShowHelp()
        {
            Application.OpenURL("https://github.com/kna27/ksp-data-export");
        }

        private class DialogCloseButton : MonoBehaviour
        {
            public Action OnDismiss { get; set; }

            public void OnDestroy()
            {
                OnDismiss?.Invoke();
            }
        }
    }
}
