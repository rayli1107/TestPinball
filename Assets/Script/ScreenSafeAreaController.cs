﻿using System;
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
