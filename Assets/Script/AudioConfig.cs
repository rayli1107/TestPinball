using UnityEngine;

public class AudioConfig : MonoBehaviour
{
    public float audioVolumeMax = 10f;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        GlobalConfig.audioVolumeChangeEvent.AddListener(OnAudioVolumeChange);
        OnAudioVolumeChange();
    }

    private void OnDisable()
    {
        GlobalConfig.audioVolumeChangeEvent.RemoveListener(OnAudioVolumeChange);
    }

    public void OnAudioVolumeChange()
    {
        _audioSource.volume = GlobalConfig.audioVolume / (float)GlobalConfig.audioVolumeMax;
    }
}
