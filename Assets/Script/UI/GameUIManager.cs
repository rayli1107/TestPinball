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
        private ImageFadeAnimation _prefabStandardFadeAnimation;
        [SerializeField]
        private ImageFadeAnimation _prefabHeartFadeAnimation;
        [SerializeField]
        private RectTransform _heartFadeTarget;
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

        public void ShowKeyAnimation(Action callback)
        {
            ImageFadeAnimation animator = Instantiate(
                _prefabStandardFadeAnimation, transform);
            animator.callback = callback;
            animator.GetComponent<Image>().sprite = _spriteKey;
            animator.gameObject.SetActive(true);
        }

        public void ShowGoalAnimation(
            Action creditGainCallback,
            Action animationDoneCallback,
            int credits)
        {
            ImageFadeAnimation animator = Instantiate(
                _prefabStandardFadeAnimation, transform);
            animator.callback = () => StartCoroutine(
                ShowCreditGains(creditGainCallback, animationDoneCallback, credits));
            animator.GetComponent<Image>().sprite = GlobalGameContext.currentTheme.completedGoal;
            animator.gameObject.SetActive(true);
        }

        private IEnumerator ShowCreditGains(
            Action creditGainCallback,
            Action animationDoneCallback,
            int credits)
        {
            for (int i = 0; i < credits; ++i)
            {
                ImageFadeAnimation animator = Instantiate(
                    _prefabHeartFadeAnimation, transform);
                if (i == credits - 1)
                {
                    Action callback = creditGainCallback;
                    callback += animationDoneCallback;
                    animator.callback = callback;
                }
                else
                {
                    animator.callback = creditGainCallback;
                }
                animator.move = _heartFadeTarget.transform.position - animator.transform.position;
                animator.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.2f);
            }
        }

/*
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
        */
    }
}
