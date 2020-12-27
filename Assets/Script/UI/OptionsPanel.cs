using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class OptionsPanel : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private Slider _sliderAudioVolume;
        [SerializeField]
        private Toggle _toggleAutoZoom;
#pragma warning restore 0649

        private void OnEnable()
        {
            _sliderAudioVolume.maxValue = GlobalConfig.audioVolumeMax;
            _sliderAudioVolume.value = GlobalConfig.audioVolume;
            _toggleAutoZoom.isOn = GlobalConfig.autoZoom;
        }

        public void OnAudioVolumeChange()
        {
            GlobalConfig.setAudioVolume(Mathf.FloorToInt(_sliderAudioVolume.value));
        }

        public void OnAutoZoomChange()
        {
            GlobalConfig.setAutoZoom(_toggleAutoZoom.isOn);
        }

        public void OnDoneButton()
        {
            gameObject.SetActive(false);
        }
    }
}
