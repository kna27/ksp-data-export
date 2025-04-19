using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace KSPDataExport
{
    /// <summary>
    ///     Loggable values window
    /// </summary>
    public partial class Window : MonoBehaviour
    {
        private void ShowLoggedValuesWindow()
        {
            if (!_loggedValsDialog)
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
            if (_loggedValsDialog && _loggedValsDialog.gameObject.activeSelf)
            {
                _loggedValsDialog.gameObject.SetActive(false);
            }
        }

        // Ensure scrollbar is at top when initialized
        private IEnumerator FixScrollPivotNextFrame(DialogGUIScrollList list)
        {
            yield return null;

            FieldInfo fi = typeof(DialogGUIScrollList)
                .GetField("scrollRect",
                    BindingFlags.Instance | BindingFlags.NonPublic);

            ScrollRect sr = fi?.GetValue(list) as ScrollRect;
            if (!sr || !sr.content) yield break;

            sr.content.pivot = new Vector2(0, 1);
            sr.verticalNormalizedPosition = 1f;
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
            const float windowWidth = 240;
            const float windowHeight = 600f;

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
                    TextAnchor.UpperCenter, new DialogGUIContentSizer(
                        ContentSizeFitter.FitMode.Unconstrained,
                        ContentSizeFitter.FitMode.PreferredSize,
                        true), contentLayout)
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

            StartCoroutine(FixScrollPivotNextFrame(scrollList));

            if (!_loggedValsDialog) return;
            _loggedValsDialog.gameObject.AddComponent<DialogCloseButton>().OnDismiss = () =>
            {
                _showLoggedVals = false;
            };

            _loggedValsDialog.gameObject.SetActive(false);
        }

        private void DismissLoggedValuesWindow()
        {
            if (_loggedValsDialog == null) return;
            _loggedValsDialog.Dismiss();
            _loggedValsDialog = null;
        }
    }
}
