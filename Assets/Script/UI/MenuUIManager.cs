﻿using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI {
    public class MenuUIManager : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private ThemePanel _prefabThemePanel;
        [SerializeField]
        private HorizontalLayoutGroup _panelThemeList;
        [SerializeField]
        private float _speedMultiple = 4f;
#pragma warning restore 0649

        private float _speedX;

        private float deltaX
        {
            get
            {
                float ret = _prefabThemePanel.GetComponent<RectTransform>().rect.width;
                ret += _panelThemeList.spacing;
                return ret;
            }
        }

        private float getX(int i)
        {
            float ret = _prefabThemePanel.GetComponent<RectTransform>().rect.width / 2;
            ret += i * deltaX;
            return -1 * ret;
        }

        private IEnumerator DelayedEnable()
        {
            while (!GlobalGameContext.initialized)
            {
                yield return null;
            }
            for (int i = 0; i < GlobalGameContext.themes.Count; ++i)
            {
                ThemePanel themePanel = Instantiate(
                    _prefabThemePanel, _panelThemeList.transform);
                themePanel.themeIndex = i;
                themePanel.gameObject.SetActive(true);
            }
        }

        private void OnEnable()
        {
            float x = Screen.safeArea.x / Screen.width;
            float width = Screen.safeArea.width / Screen.width;
            float y = Screen.safeArea.y / Screen.height;
            float height = Screen.safeArea.height / Screen.height;

            _speedX = 0f;
            StartCoroutine(DelayedEnable());
        }

        public void OnNextButton()
        {
            GlobalGameContext.themeIndex++;
            _speedX = deltaX * (GlobalGameContext.themeIndex == 0 ? 1 : -1);
        }

        public void OnPrevButton()
        {
            GlobalGameContext.themeIndex--;
            int count = GlobalGameContext.themes.Count;
            _speedX = deltaX * ((GlobalGameContext.themeIndex == count - 1) ? -1 : 1);
        }

        public void OnPlayButton()
        {
            SceneManager.LoadScene("GameScene");
        }

        private void Update()
        {
            RectTransform rect = _panelThemeList.GetComponent<RectTransform>();
            float x = rect.anchoredPosition.x;
            float targetX = getX(GlobalGameContext.themeIndex);
            if (x != targetX)
            {
                float speed = _speedMultiple * _speedX * Time.deltaTime;
                if (_speedX > 0)
                {
                    x = Mathf.Min(x + speed, targetX);
                }
                else
                {
                    x = Mathf.Max(x + speed, targetX);
                }

                rect.anchoredPosition = new Vector2(x, 0);
            }
        }
    }
}
