using System;
using UnityEngine;

public static class DebugUtility
{
    public static Action logUpdateAction;
    public static string currentMessage { get; private set; }
    public static void LogFormat(string format, params object[] args)
    {
        currentMessage = string.Format(format, args);
        Debug.Log(currentMessage);
        logUpdateAction?.Invoke();
    }
}
