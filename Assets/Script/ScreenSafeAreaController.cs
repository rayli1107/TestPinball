using System;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSafeAreaController : MonoBehaviour
{
    public static ScreenSafeAreaController Instance;
    public Action updateCallbacks;
    private Rect _prevSafeArea;

    private void Awake()
    {
        Instance = this;
        _prevSafeArea = Rect.zero;
        Debug.LogFormat("ScreenSafeAreaController.Awake");
        Debug.LogFormat("Display.main {0} {1}", Display.main.systemWidth, Display.main.systemHeight);
        foreach (Resolution resolution in Screen.resolutions)
        {
            Debug.LogFormat("Resolution: {0}", resolution.ToString());
        }
    }

    private void Update()
    {
        if (_prevSafeArea != Screen.safeArea)
        {
            string message = string.Format(
                "w {0} h {1}\nsafearea {2}",
                Screen.width,
                Screen.height,
                Screen.safeArea);
            DebugUtility.LogFormat(message);
            _prevSafeArea = Screen.safeArea;
            updateCallbacks?.Invoke();
        }
    }
}
