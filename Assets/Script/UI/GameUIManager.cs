using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace UI
{
    public class GameUIManager : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private TextMeshProUGUI _creditText;
        [SerializeField]
        private Button _playButton;
        [SerializeField]
        private Button _addCreditButton;
        [SerializeField]
        private OptionsPanel _optionsPanel;
        [SerializeField]
        private CreditMultiplierPanel _multiplierPanel;
        [SerializeField]
        private TextMeshProUGUI _textMultiplier;
#pragma warning restore 0649
        //    public int credit;

        public static GameUIManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
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

        public void SetCredit(int credit)
        {
            _creditText.text = string.Format("Credit:\n{0}", credit);
            _playButton.gameObject.SetActive(credit > 0);
            _addCreditButton.gameObject.SetActive(credit <= 0);
        }

        public void OnOptionsButton()
        {
            _optionsPanel.gameObject.SetActive(true);
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
            _textMultiplier.gameObject.SetActive(multiplier > 0);
            _textMultiplier.text = string.Format("{0}x\nMultiplier", multiplier);
        }
    }
}
