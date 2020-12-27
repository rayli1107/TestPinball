using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class CurrentTheme
{
    public static ThemeProfile theme;
}

namespace UI {
    public class MenuUIManager : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private ThemeProfile[] _themes;
        [SerializeField]
        private GameObject _prefabThemePanel;
        [SerializeField]
        private HorizontalLayoutGroup _panelThemeList;
        [SerializeField]
        private float _speedMultiple = 4f;
        [SerializeField]
        private OptionsPanel _optionsPanel;
#pragma warning restore 0649

        private int _current;
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

        private void SelectTheme()
        {
            ThemeProfile theme = _themes[_current];
            CurrentTheme.theme = theme;
        }

        private void OnEnable()
        {
            _current = 0;
            _speedX = 0f;
            foreach (ThemeProfile profile in _themes)
            {
                GameObject panel = Instantiate(
                    _prefabThemePanel, _panelThemeList.transform);
                panel.GetComponentInChildren<TextMeshProUGUI>().text = profile.themeName;
                panel.GetComponentInChildren<Image>().sprite = profile.background;
                panel.SetActive(true);
            }

            SelectTheme();
        }

        public void OnNextButton()
        {
            _current = (_current + 1) % _themes.Length;
            _speedX = deltaX * (_current == 0 ? 1 : -1);
            SelectTheme();
        }

        public void OnPrevButton()
        {
            _current = (_current + _themes.Length - 1) % _themes.Length;
            _speedX = deltaX * ((_current == _themes.Length - 1) ? -1 : 1);
            SelectTheme();
        }

        public void OnPlayButton()
        {
            SceneManager.LoadScene("GameScene");
        }

        private void Update()
        {
            RectTransform rect = _panelThemeList.GetComponent<RectTransform>();
            float x = rect.anchoredPosition.x;
            float targetX = getX(_current);
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
        public void OnOptionsButton()
        {
            _optionsPanel.gameObject.SetActive(true);
        }
    }
}
