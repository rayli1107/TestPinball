using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text creditText;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void OnButtonAddCredit()
    {
        GameController.Instance.AddCredit();
    }

    public void OnButtonPlay()
    {
        GameController.Instance.StartPlay();
    }

    public void SetCreditText(int credit)
    {
        creditText.text = string.Format("Credit: {0}", credit);
    }

    void Start()
    {
    }

    void LateUpdate()
    {
    }
}
