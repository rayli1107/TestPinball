using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text creditText;
    public Button addCreditButton;

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

    public void SetCreditText(int credit)
    {
        creditText.text = string.Format("Credit: {0}", credit);
        addCreditButton.gameObject.SetActive(credit == 0);
    }

    void Start()
    {
    }

    void LateUpdate()
    {
    }
}
