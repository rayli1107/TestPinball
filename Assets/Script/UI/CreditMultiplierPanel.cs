using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class CreditMultiplierPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private VerticalLayoutGroup _parent;
        [SerializeField]
        private Button _prefabMultiplierButton;
#pragma warning restore 0649

        private IEnumerator DelayedInit()
        {
            while (GameController.Instance == null)
            {
                yield return null;
            }

            foreach (Vector2Int setting in GameController.Instance.creditMultiplierList)
            {
                Button button = Instantiate(_prefabMultiplierButton, _parent.transform);
                button.GetComponentInChildren<TextMeshProUGUI>(true).text = string.Format(
                    "{0}X", setting.y);
                button.onClick.AddListener(
                    () => GameController.Instance.SelectCreditMultiplier(setting));
                button.onClick.AddListener(
                    () => gameObject.SetActive(false));
                button.gameObject.SetActive(true);
            }
        }

        private void Awake()
        {
            StartCoroutine(DelayedInit());
        }
    }
}
