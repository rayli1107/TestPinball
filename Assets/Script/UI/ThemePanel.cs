using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ThemePanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _textName;
        [SerializeField]
        private Image _imageTheme;
        [SerializeField]
        private RectTransform _panelLock;
        [SerializeField]
        private Button _buttonUnlock;
        [SerializeField]
        private TextMeshProUGUI _textKeysRequired;
#pragma warning restore 0649

        public int themeIndex;

        public void Refresh()
        {
            ThemeProfile theme = GlobalGameContext.themes[themeIndex].theme;
            _textName.text = theme.themeName;
            _imageTheme.sprite = theme.background;
            _panelLock.gameObject.SetActive(
                GlobalGameContext.themes[themeIndex].locked);
            _buttonUnlock.enabled = GlobalGameContext.keys >= theme.keysToUnlock;
            _textKeysRequired.text = string.Format("Keys Required:\n{0}", theme.keysToUnlock);
        }

        private void OnEnable()
        {
            Refresh();
        }
/*
        public void OnButtonUnlock()
        {
            ThemeProfile theme = GlobalGameContext.themes[themeIndex].theme;
            if (GlobalGameContext.themes[themeIndex].locked &&
                GlobalGameContext.keys >= theme.keysToUnlock)
            {
                GlobalGameContext.keys -= theme.keysToUnlock;
                GlobalGameContext.themes[themeIndex].locked = false;
                Refresh();
            }
        }
        */
    }
}
