using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace UI
{
    public class CommonUIPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _creditText;
        [SerializeField]
        private TextMeshProUGUI _keysText;
        [SerializeField]
        private OptionsPanel _optionsPanel;
#pragma warning restore 0649

        private DateTime _lastUpdateTime;

        private void OnEnable()
        {
            GlobalGameContext.statUpdateAction += RefreshStats;
            RefreshStats();
        }

        private void OnDisable()
        {
            GlobalGameContext.statUpdateAction -= RefreshStats;
        }

        private void RefreshStats()
        {
            _lastUpdateTime = DateTime.Now;

            List<string> creditText = new List<string>() {
                GlobalGameContext.credits.ToString()
            };

            if (GlobalGameContext.credits < GlobalGameContext.maxCredits &&
                _lastUpdateTime < GlobalGameContext.creditReloadTime)
            {
                TimeSpan diff = GlobalGameContext.creditReloadTime - _lastUpdateTime;
                creditText.Add(diff.ToString(@"mm\:ss"));
            }

            _creditText.text = string.Join("\n", creditText);
            _keysText.text = GlobalGameContext.keys.ToString();
        }

        public void OnOptionsButton()
        {
            _optionsPanel.gameObject.SetActive(true);
        }

        private void Update()
        {
            TimeSpan diff = DateTime.Now - _lastUpdateTime;
            if (diff.TotalSeconds >= 1)
            {
                RefreshStats();
            }
        }
    }
}
