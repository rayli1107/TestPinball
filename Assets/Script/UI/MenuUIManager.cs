using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI {
    public class MenuUIManager : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private Button _buttonPlay;
        [SerializeField]
        private Button _buttonUnlock;
        [SerializeField]
        private ThemeSelectionManager _themeSelectionManager;
#pragma warning restore 0649

        public void OnPlayButton()
        {
            SceneManager.LoadScene("GameScene");
        }

        public void OnUnlockButton()
        {
            GlobalGameContext.TryUnlock(GlobalGameContext.themeIndex);
            _themeSelectionManager.Refresh();
            EnableAction(true);
        }

        public void EnableAction(bool enable)
        {
            if (enable)
            {
                ThemeState state = GlobalGameContext.GetThemeState(
                    GlobalGameContext.themeIndex);
                _buttonPlay.gameObject.SetActive(state == ThemeState.kCanPlay);
                _buttonUnlock.gameObject.SetActive(state == ThemeState.kCanUnlock);
            }
            else
            {
                _buttonPlay.gameObject.SetActive(false);
                _buttonUnlock.gameObject.SetActive(false);
            }
        }
    }
}
