using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace UI
{
    public class SafeAreaCanvas : MonoBehaviour
    {
        private void UpdateCanvasSize()
        {
            float x = Screen.safeArea.x / Screen.width;
            float width = Screen.safeArea.width / Screen.width;
            float y = Screen.safeArea.y / Screen.height;
            float height = Screen.safeArea.height / Screen.height;
            float x2 = x + width;
            float y2 = y + height;

            RectTransform rect = GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(x, y);
            rect.anchorMax = new Vector2(x2, y2);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private void OnEnable()
        {
            StartCoroutine(DelayedOnEnable());
        }

        private IEnumerator DelayedOnEnable()
        {
            while (ScreenSafeAreaController.Instance == null)
            {
                yield return null;
            }

            ScreenSafeAreaController.Instance.updateCallbacks += UpdateCanvasSize;
            UpdateCanvasSize();
        }

        private void OnDisable()
        {
            if (ScreenSafeAreaController.Instance != null)
            {
                ScreenSafeAreaController.Instance.updateCallbacks -= UpdateCanvasSize;
            }
        }
    }
}
