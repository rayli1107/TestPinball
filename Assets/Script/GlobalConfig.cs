using System;
using UnityEngine;
using UnityEngine.Events;

public static class GlobalConfig
{
    // Audio Volume Config
    public static int audioVolumeMax { get; } = 10;

    public static int audioVolume { get; private set; } = audioVolumeMax;

    public static UnityEvent audioVolumeChangeEvent { get; } = new UnityEvent();
    public static void setAudioVolume(int volume)
    {
        audioVolume = Mathf.Clamp(volume, 0, audioVolumeMax);
        audioVolumeChangeEvent.Invoke();
    }

    // Auto Zoom Config
    public static bool autoZoom { get; private set; } = true;
    public static UnityEvent autoZoomChangeEvent { get; } = new UnityEvent();
    public static void setAutoZoom (bool value)
    {
        autoZoom = value;
        autoZoomChangeEvent.Invoke();
    }
}
