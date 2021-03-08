
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    class ToggleControl : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private Sprite _spriteOn;
        [SerializeField]
        private Sprite _spriteOff;
#pragma warning restore 0649

        private Toggle _toggle;
        private Image _targetImage;

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
            _targetImage = _toggle.targetGraphic.GetComponent<Image>();
        }

        private void OnEnable()
        {
            OnValueChanged();
        }

        public void OnValueChanged()
        {
            if (_targetImage != null && _toggle != null)
            {
                _targetImage.sprite = _toggle.isOn ? _spriteOn : _spriteOff;
            }
        }
    }
}
