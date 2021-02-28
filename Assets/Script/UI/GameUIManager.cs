using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace UI
{
    public class GameUIManager : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private RectTransform _gameUIPanel;
        [SerializeField]
        private Button _playButton;
        [SerializeField]
        private Button _addCreditButton;
        [SerializeField]
        private CreditMultiplierPanel _multiplierPanel;
        [SerializeField]
        private TextMeshProUGUI _textMultiplier;
        [SerializeField]
        private Image _imageHitAnimation;
        [SerializeField]
        private float _hitAnimationDuration = 2f;
        [SerializeField]
        private Sprite _spriteKey;
#pragma warning restore 0649

        public static GameUIManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            GlobalGameContext.statUpdateAction += RefreshStats;
            RefreshStats();
        }

        private void OnDisable()
        {
            GlobalGameContext.statUpdateAction -= RefreshStats;
        }

        public void OnButtonAddCredit()
        {
            GameController.Instance.StartVideoAd();
        }

        public void OnButtonPlay()
        {
            GameController.Instance.StartPlay();
        }

        public void OnButtonQuit()
        {
            GameController.Instance.Quit();
        }

        private void RefreshStats()
        {
            _playButton.gameObject.SetActive(GlobalGameContext.credits > 0);
            _addCreditButton.gameObject.SetActive(GlobalGameContext.credits <= 0);
        }

        public void ShowCreditMultiplierPanel()
        {
            _multiplierPanel.gameObject.SetActive(true);
        }

        public void EnableButtons(bool enable)
        {
            foreach (Button button in GetComponentsInChildren<Button>(true))
            {
                button.enabled = enable;
            }
        }

        public void SetMultiplierText(int multiplier)
        {
            List<string> text = new List<string>()
            {
                string.Format("{0}x", multiplier),
                "Multiplier",
                "Click to Continue"
            };

            _textMultiplier.gameObject.SetActive(multiplier > 0);
            _textMultiplier.text = string.Join("\n", text);
        }

        public void EnableGameUIPanel(bool enable)
        {
            _gameUIPanel.gameObject.SetActive(enable);
        }

        public void ShowHitAnimation(bool goal, Action callback)
        {
            ThemeProfile theme = GlobalGameContext.currentTheme;
            _imageHitAnimation.sprite = goal ? theme.goal : _spriteKey;
            _imageHitAnimation.rectTransform.localScale = Vector3.one;
            _imageHitAnimation.color = new Color(1f, 1f, 1f, 1f);
            _imageHitAnimation.gameObject.SetActive(true);
            StartCoroutine(HitAnimation(callback));
        }

        private IEnumerator HitAnimation(Action callback)
        {
            float timeStart = Time.time;
            while (true)
            {
                float duration = (Time.time - timeStart) / _hitAnimationDuration;
                if (duration >= 1f)
                {
                    break;
                }

                _imageHitAnimation.color = new Color(1f, 1f, 1f, 1 - duration);
                _imageHitAnimation.rectTransform.localScale = new Vector3(
                    1 + duration, 1 + duration, 1);

                yield return null;
            }
            callback?.Invoke();
            _imageHitAnimation.gameObject.SetActive(false);
        }
    }
}
