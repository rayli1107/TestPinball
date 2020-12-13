using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI creditText;
    public Button playButton;
    public Button addCreditButton;
    public Toggle autoZoomToggle;
    public int credit;

    public static UIManager Instance { get; private set; }

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

    public void OnAutoZoomChanged()
    {
        GameController.Instance.autoZoomIn = autoZoomToggle.isOn;
    }

    public void SetCredit(int credit)
    {
        creditText.text = string.Format("Credit:\n{0}", credit);
        playButton.gameObject.SetActive(credit > 0);
        addCreditButton.gameObject.SetActive(credit <= 0);
    }
}
