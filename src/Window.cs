using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace KSPDataExport
{
    /// <summary>
    ///     The mod's GUI using PopupDialog
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class Window : MonoBehaviour
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
            if (_mainDialog == null)
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
            if (_mainDialog != null && _mainDialog.gameObject.activeSelf)
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

            if (_mainDialog != null)
            {
                _mainDialog.gameObject.AddComponent<DialogCloseButton>().OnDismiss = () =>
                {
                    ShowGUI = false;
                };
                _mainDialog.gameObject.SetActive(false);
            }
        }

        private void DismissMainWindow()
        {
            if (_mainDialog != null)
            {
                _mainDialog.Dismiss();
                _mainDialog = null;
            }
        }

        // --- Logged Values Window Management ---

        private void ShowLoggedValuesWindow()
        {
            if (_loggedValsDialog == null)
            {
                SpawnLoggedValuesWindow();
            }

            if (_loggedValsDialog != null && !_loggedValsDialog.gameObject.activeSelf)
            {
                _loggedValsDialog.gameObject.SetActive(true);
            }
        }

        private void HideLoggedValuesWindow()
        {
            if (_loggedValsDialog != null && _loggedValsDialog.gameObject.activeSelf)
            {
                _loggedValsDialog.gameObject.SetActive(false);
            }
        }


        private void BuildLoggedValuesDialogStructure()
        {
            _loggedValuesDialogContent.Clear();

            for (int i = 0; i < Enum.GetValues(typeof(Category)).Length; i++)
            {
                Category category = (Category)i;
                var categoryValues = DataExport.LoggableValues.Where(v => v.Category == category).OrderBy(v => v.Name)
                    .ToList();
                if (categoryValues.Count == 0) continue;

                // Category Header
                DialogGUILabel header = new DialogGUILabel(Enum.GetName(typeof(Category), i), true);
                _loggedValuesDialogContent.Add(header);
                _loggedValuesDialogContent.Add(new DialogGUISpace(6));

                // Toggles for values in this category
                foreach (LoggableValue value in categoryValues)
                {
                    var toggle = new DialogGUIToggle(
                        value.Logging,
                        value.Name,
                        (state) => { value.Logging = state; },
                        190f
                    );
                    _loggedValuesDialogContent.Add(toggle);
                }

                _loggedValuesDialogContent.Add(new DialogGUISpace(10));
            }
        }

        private void SpawnLoggedValuesWindow()
        {
            float windowWidth = 240;
            float windowHeight = 600f;

            var contentLayout = new DialogGUIVerticalLayout(
                10,
                0,
                4,
                new RectOffset(6, 6, 10, 10),
                TextAnchor.MiddleLeft
            );

            foreach (var item in _loggedValuesDialogContent)
            {
                contentLayout.AddChild(item);
            }

            var scrollList = new DialogGUIScrollList(
                new Vector2(windowWidth - 20, windowHeight - 60),
                false,
                true,
                new DialogGUIVerticalLayout(
                    0, 0, 0, new RectOffset(),
                    TextAnchor.UpperCenter,
                    new DialogGUIBase[]
                    {
                        new DialogGUIContentSizer(
                            ContentSizeFitter.FitMode.Unconstrained,
                            ContentSizeFitter.FitMode.PreferredSize,
                            true),
                        contentLayout
                    }
                )
            );

            _loggedValsDialog = PopupDialog.SpawnPopupDialog(
                new Vector2(0.85f, 0.5f),
                new Vector2(0.85f, 0.5f),
                new MultiOptionDialog(
                    "KSPDataExportLoggedVals",
                    "",
                    "Logged Values",
                    HighLogic.UISkin,
                    new Rect(0.85f, 0.5f, windowWidth, windowHeight),
                    scrollList
                ),
                true,
                HighLogic.UISkin,
                false
            );

            if (_loggedValsDialog != null)
            {
                _loggedValsDialog.gameObject.AddComponent<DialogCloseButton>().OnDismiss = () =>
                {
                    _showLoggedVals = false;
                };

                _loggedValsDialog.gameObject.SetActive(false);
            }
        }

        private void DismissLoggedValuesWindow()
        {
            if (_loggedValsDialog != null)
            {
                _loggedValsDialog.Dismiss();
                _loggedValsDialog = null;
            }
        }

        private void ToggleLogging()
        {
            DataExport.IsLogging = !DataExport.IsLogging;
            _wasLoggingStoppedByInvalidRate = false;
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
                    ScreenMessageStyle.UPPER_CENTER); }
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

                if (_wasLoggingStoppedByInvalidRate) // If it was stopped due to invalid rate, turn it back on
                {
                    DataExport.IsLogging = true;
                    _wasLoggingStoppedByInvalidRate = false;
                }
            }
            else
            {
                // Input is invalid
                if (DataExport.IsLogging) // Only act if it *was* logging
                {
                    ScreenMessages.PostScreenMessage("Invalid log rate! Logging paused.", 3f,
                        ScreenMessageStyle.UPPER_CENTER);
                    DataExport.IsLogging = false;
                    _wasLoggingStoppedByInvalidRate = true;
                    // Button text updates via Func
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
